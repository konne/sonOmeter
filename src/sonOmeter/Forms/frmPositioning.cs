using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.MathEx;
using UKLib.Survey.Math;
using sonOmeter.Classes.Sonar2D;
using UKLib.Debug;

namespace sonOmeter
{
    public partial class frmPositioning : sonOmeter.frmSmallToolWindow
    {
        private SonarPanelType type = SonarPanelType.HF;
        private SonarLine lastLine = null;

        public frmPositioning(SonarProject prj)
        {
            InitializeComponent();

            InitializeToolWindow(prj);

            GlobalNotifier.SignIn(globalEventHandler, GlobalNotifier.MsgTypes.NewCoordinate);

            positioningControl.DepthType = type.ToString();
        }

        protected override void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            if (state == GlobalNotifier.MsgTypes.NewCoordinate)
            {
                try
                {
                    if (!(args is Coordinate) || (project.SelectedCorridor == null))
                        return;

                    PointD pt = ((Coordinate)args).Point + project.LastLockOffset;

                    positioningControl.Distance = project.SelectedCorridor.GetCorridorDistance(pt);
                    positioningControl.Station = project.SelectedCorridor.GetStation(pt);
                }
                catch (Exception ex)
                {
                    DebugClass.SendDebugLine(this, DebugLevel.Red, "NewCoordinate @ frmPositioning : " + ex.Message);
                }
            }
            else
                base.OnGlobalEvent(sender, args, state);
        }

        protected override void SelectSonarLine(SonarLine line, GlobalNotifier.MsgTypes state)
        {
            if (line == null)
                return;

            // Set depth value depending on selected panel type.
            if (type == SonarPanelType.HF)
                positioningControl.Depth = line.HF.Depth;
            else
                positioningControl.Depth = line.NF.Depth;

            // Check for GlobalNotifier state.
            if (state == GlobalNotifier.MsgTypes.WorkLineChanged)
            {
                if (line.CoordRvHv.Type != CoordinateType.Empty)
                {
                    double dNearest = 0;
                    BuoyConnection cNearest = null;
                    PointD pt = line.CoordRvHv.Point + line.LockOffset;

                    if (project.GetNearestConnection(pt, ref cNearest, ref dNearest, false))
                    {
                        positioningControl.Distance = cNearest.GetCorridorDistance(pt);
                        positioningControl.Station = cNearest.GetStation(pt);
                    }
                }
            }

            lastLine = line;
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            positioningControl.InstrumentColor = GSC.Settings.CS.BackColor;
            positioningControl.BackColor = GSC.Settings.CS.BackColor;
            positioningControl.ForeColor = GSC.Settings.CS.ForeColor;
            positioningControl.TextColor = GSC.Settings.CS.ForeColor;
            positioningControl.NFI = GSC.Settings.NFI;
        }

        private void positioningControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (type == SonarPanelType.HF)
                type = SonarPanelType.NF;
            else
                type = SonarPanelType.HF;

            positioningControl.DepthType = type.ToString();

            SelectSonarLine(lastLine, GlobalNotifier.MsgTypes.None);
        }
    }
}
