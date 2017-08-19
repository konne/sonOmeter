using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UKLib.Survey;
using UKLib.Survey.Parse;
using sonOmeter.Classes;
using UKLib.MathEx;

namespace sonOmeter
{
    /// <summary>
    /// Compass visualization window.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmCompass : frmSmallToolWindow
    {
        public frmCompass(SonarProject prj)
        {
            InitializeComponent();

            compassControl.Azimuth = Double.NaN;
            compassControl.ShowSpeed = true;
            
            InitializeToolWindow(prj);
        }

        protected override void SelectSonarLine(SonarLine line, GlobalNotifier.MsgTypes state)
        {
            if (line == null)
                return;

            double azimuth = Double.NaN;
            foreach (SonarPos pos in line.PosList)
            {
                switch (pos.type)
                {
                    case PosType.Compass:
                        var cmp = new Compass(pos.strPos) - GSC.Settings.CompassZero;
                        azimuth = cmp.Yaw;
                        break;
                    case PosType.HMR3300:
                        var cmp2 = new HMR3300(pos.strPos) - GSC.Settings.CompassZero;
                        azimuth = cmp2.Yaw;
                        break;

                    case PosType.NMEA:
                        NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);
                        if (nmea.Type == "$GPVTG")
                            compassControl.Speed = nmea.Speed;
                        
                        if (nmea.Type == "$SBG01")
                        {
                            var rotation = new RotD(nmea.Yaw, nmea.Pitch, nmea.Roll) - GSC.Settings.CompassZero;
                            azimuth = rotation.Yaw;                            
                        }                 
                        break;
                    default:
                        break;
                }
            }

            if (!Double.IsNaN(azimuth)) compassControl.Azimuth = azimuth;
        }

        protected override void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            compassControl.InstrumentColor = GSC.Settings.CS.BackColor;
            compassControl.BackColor = GSC.Settings.CS.BackColor;
            compassControl.ForeColor = GSC.Settings.CS.ForeColor;
            compassControl.TextColor = GSC.Settings.CS.ForeColor;
            compassControl.NFI = GSC.Settings.NFI;

            base.OnSettingsChanged(sender, e);
        }
    }
}
