using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UKLib.Survey;
using UKLib.Survey.Parse;
using sonOmeter.Classes;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for frmCompass.
	/// </summary>
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public partial class frmSat : frmSmallToolWindow
	{
		#region Variables
		SatSNR[] satList = new SatSNR[128];
		#endregion

		public frmSat(SonarProject prj)
		{
			InitializeComponent();

            InitializeToolWindow(prj);           
		}

        protected override void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            satControl.SatNumMin = GSC.Settings.MinSatellites;
            satControl.SatQualityMin = GSC.Settings.MinQuality;
            satControl.SatQualityMeterMin = GSC.Settings.MinQualityMeter;
            satControl.InstrumentColor = GSC.Settings.CS.BackColor;
            satControl.BackColor = GSC.Settings.CS.BackColor;
            satControl.ForeColor = GSC.Settings.CS.ForeColor;
            satControl.TextColor = GSC.Settings.CS.ForeColor;
            satControl.NFI = GSC.Settings.NFI;
            satControl.ColorInUsedGlonas = GSC.Settings.CS.ColorInUsedGlonas;
            satControl.ColorInUsedGPS = GSC.Settings.CS.ColorInUsedGPS;
            satControl.ColorNotUsedGlonas = GSC.Settings.CS.ColorNotUsedGlonas;
            satControl.ColorNotUsedGPS = GSC.Settings.CS.ColorNotUsedGPS;
            satControl.ColorTryUsedGlonas = GSC.Settings.CS.ColorTryUsedGlonas;
            satControl.ColorTryUsedGPS = GSC.Settings.CS.ColorTryUsedGPS;

            base.OnSettingsChanged(sender, e);
        }

        protected override void SelectSonarLine(SonarLine line, GlobalNotifier.MsgTypes state)
        {
            if (line == null)
                return;
        
            foreach (SonarPos pos in line.PosList)
			{
				switch (pos.type)
				{
					case PosType.NMEA:
						NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);

                        string nmeaType = nmea.Type;
                        if (nmeaType.StartsWith("$GN"))
                            nmeaType = nmeaType.Replace("$GN", "$GP");

						if (nmeaType == "$GPGGA")
						{
							satControl.SatNum = nmea.Satellites;
							satControl.SatQuality = nmea.Quality;
						}
                        if (nmeaType == "$PTNL")
                        {
                            satControl.SatNum = nmea.Satellites;
                            satControl.SatQuality = nmea.Quality;
                        }
                        if (nmeaType == "$GPLLK" && nmea.Latitude != null)
                        {
                            satControl.SatNum = nmea.Satellites;
                            satControl.SatQuality = nmea.Quality;
                        }                      
                        if (nmeaType == "$GPLLQ" && nmea.Altitude != 0)
                        {                            
                            satControl.SatNum = nmea.Satellites;
                            satControl.SatQuality = nmea.Quality;
                            satControl.SatQualityMeter = nmea.QualityMeter;
                        }               
						if (nmeaType.Substring(3) == "GSV")
						{
							try
							{
								for (int i= 0; i < 4; i++)
								{
									int satnr=0;
									SatSNR sat =  nmea.SatSNRs(i,ref satnr);
									if ((satnr > 0) & (satnr < 129)) 
										satList[satnr-1] = sat;
								}
							}
							catch (Exception ex)
							{
								UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmSat.prj_NewSonarLine: " + ex.Message);
							}
						}
						if (nmeaType.Substring(3) == "GSA")
						{
                            int SatNrArea = 0;

                            bool validfound = false;
							for (int i= 1; i <= 12; i++)
							{
								int satnr = nmea.SatNrInUse(i)-1;
                                if ((satnr > 0) & (satnr < 128))
                                {
                                    if (satnr > 32 && !validfound)
                                        SatNrArea = 32;
                                    if (satnr > 64 && !validfound)
                                        SatNrArea = 64;

                                    validfound = true;
                                    satList[satnr].Status = SatStatus.InUse;
                                }

							}
                            if (validfound)
                            {
                                for (int i = SatNrArea; i < SatNrArea+32; i++)
                                {
                                    if ((satList[i].Status == SatStatus.NotUsed) && (satList[i].SNR > 0))
                                        satList[i].Status = SatStatus.TryUse;

                                    satControl.SatList[i] = satList[i];

                                    satList[i].SNR = 0;
                                    satList[i].Elevation = -1;
                                    satList[i].Status = SatStatus.NotUsed;
                                }
                                Invalidate();
                            }
						}
						break;
					default:
						break;
				}
			}
		}       
	}
}
