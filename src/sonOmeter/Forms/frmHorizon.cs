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
    /// Summary description for frmCompass.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmHorizon : frmSmallToolWindow
    {
        public frmHorizon(SonarProject prj)
        {
            InitializeComponent();

            InitializeToolWindow(prj);
        }

        protected override void SelectSonarLine(SonarLine line, GlobalNotifier.MsgTypes state)
        {
            if (line == null)
                return;

            double bank = Double.NaN;
            double elevation = Double.NaN;

            foreach (SonarPos pos in line.PosList)
            {
                switch (pos.type)
                {
                    case PosType.Compass:
                        var cmp = new Compass(pos.strPos) - GSC.Settings.CompassZero;
                        bank = cmp.Roll;
                        elevation = cmp.Pitch;
                        break;
                    case PosType.HMR3300:
                        var cmp2 = new HMR3300(pos.strPos) - GSC.Settings.CompassZero;
                        bank = cmp2.Roll;
                        elevation = cmp2.Pitch;
                        break;

                    case PosType.NMEA:
                        NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);
                     
                        if (nmea.Type == "$SBG01")
                        {
                            var rotation = new RotD(nmea.Yaw, nmea.Pitch, nmea.Roll) - GSC.Settings.CompassZero;
                            bank = rotation.Roll;
                            elevation = rotation.Pitch;
                        }            
                        break;
                    default:
                        break;
                }
            }

            if (!Double.IsNaN(bank)) horizonControl.Bank = bank;
            if (!Double.IsNaN(elevation)) horizonControl.Elevation = elevation;
        }

        protected override void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {            
            horizonControl.InstrumentColor = GSC.Settings.CS.BackColor;
            horizonControl.BackColor = GSC.Settings.CS.BackColor;
            horizonControl.ForeColor = GSC.Settings.CS.ForeColor;
            horizonControl.TextColor = GSC.Settings.CS.ForeColor;
            horizonControl.NFI = GSC.Settings.NFI;

            base.OnSettingsChanged(sender, e);
        }
    }
}
