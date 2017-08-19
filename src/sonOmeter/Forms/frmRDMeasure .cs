using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.Survey.Parse;
using UKLib.Hex;
using UKLib.Survey.Math;
using UKLib.MathEx;
using sonOmeter.Classes.Sonar2D;
using UKLib.Debug;

namespace sonOmeter
{
    public partial class frmRDMeasure : DockDotNET.DockWindow
    {
        GlobalEventHandler globalEventHandler;
        SonarProject project = null;

        public frmRDMeasure()
        {
            InitializeComponent();

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);

            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.NewCoordinate);
            filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
            filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);

            GlobalNotifier.SignIn(globalEventHandler, filterlist);

            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);
            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));
        }

        public frmRDMeasure(SonarProject project)
            : this()
        {
            this.project = project;
        }

        protected virtual void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("CS.") || (e.PropertyName == "All"))
            {
                //this.BackColor = GSC.Settings.CS.BackColor;
                //this.ForeColor = GSC.Settings.CS.ForeColor;

                //Invalidate();
            }
        }

        private struct PosListItem
        {
            public DateTime Time;
            public Coordinate Coord;
            public Double Angle;
            public Double Distance;
            public Double Station;
        }

        List<PosListItem> PosList = new List<PosListItem>();

        protected virtual void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    SearchRDMessage(args as SonarLine);
                    SearchDLSBMessage(args as SonarLine);
                    break;
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    SonarLine line = (args as RecordEventArgs).Tag as SonarLine;

                    if (GSC.Settings.Lic[Module.Modules.RadioDetection] && (line != null))
                    {
                        if (double.IsNaN(line.AntDepth))
                            SearchDLSBMessage(line);
                    }
                    break;
                case GlobalNotifier.MsgTypes.NewCoordinate:
                    try
                    {
                        if (!(args is Coordinate) || (project.SelectedCorridor == null))
                            return;

                        Coordinate coord = (Coordinate)args;

                        // Verschiebung und Drehung
                        //PointD ptLockOffset = new PointD(-0.504, -3.807).Rotate(project.LastAngle);
                        PointD pt = coord.Point + project.LastLockOffset;
                        coord.RV = pt.X;
                        coord.HV = pt.Y;
                        coord.AL = coord.AL - MADepth;

                        DateTime now = DateTime.Now;
                        PosListItem itm = new PosListItem()
                        {
                            Coord = coord,
                            Distance = project.SelectedCorridor.GetCorridorDistance(pt),
                            Station = project.SelectedCorridor.GetStation(pt),
                            // TODO: Use pitch and roll?
                            Angle = project.LastRotation.Yaw,
                            Time = now
                        };
                        for (int i = 0; i < PosList.Count; i++)
                        {
                            if ((now - PosList[i].Time).TotalSeconds > 30)
                            {
                                PosList.RemoveAt(i);
                                i--;
                            }
                        }
                        PosList.Add(itm);
                    }
                    catch (Exception ex)
                    {
                        DebugClass.SendDebugLine(this, DebugLevel.Red, "NewCoordinate @ frmPositioning : " + ex.Message);
                    }
                    break;
            }
        }

        Double MPDepth = 0;
        Double MADepth = 0;
        PosListItem MPPosItem = new PosListItem() { Distance = 0, Station = 0 };
        Double MPDiameter = 0;

        private void SearchRDMessage(SonarLine line)
        {
            foreach (SonarPos pos in line.PosList)
            {
                if ((pos.type == PosType.RD4000) | (pos.type == PosType.RD8000))
                {
                    #region Parse Parameters
                    if (Double.TryParse(tbPipelineDiameter.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out MPDiameter))
                    {
                        tbPipelineDiameter.BackColor = Color.White;
                    }
                    else
                    {
                        tbPipelineDiameter.BackColor = Color.Red;
                    }

                    Single Seconds = 0;
                    if (Single.TryParse(tbSeconds.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out Seconds))
                    {
                        tbSeconds.BackColor = Color.White;
                    }
                    else
                    {
                        tbSeconds.BackColor = Color.Red;
                    }
                    #endregion

                    Double Depth = 0;
                    try
                    {
                        if (pos.type == PosType.RD4000)
                        {

                            RD4000Message msg = new RD4000Message(pos.strPos);
                            Depth = msg.Depth;
                        }
                        if (pos.type == PosType.RD8000)
                        {
                            RD8000Message msg = new RD8000Message(HexStrings.ToByteArray(pos.strPos));
                            Depth = msg.Depth;
                            CompDepth(msg.Depth);
                        }
                    }
                    catch (Exception ex)
                    {
                        UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
                    }

                    Depth = -(Depth - MPDiameter / 2F);

                    tbDepth.Text = Depth.ToString("0.00");

                    PosListItem pitm = new PosListItem { Distance = 0, Station = 0 };
                    double secs = 1000000;
                    foreach (PosListItem itm in PosList)
                    {
                        double lsecs = (itm.Time - pos.time.AddSeconds(-Seconds)).TotalSeconds;
                        if (Math.Abs(secs) > Math.Abs(lsecs))
                        {
                            secs = lsecs;
                            pitm = itm;
                        }
                    }
                    MPPosItem = pitm;
                    MPDepth = Depth;
                    tbDistance.Text = pitm.Distance.ToString("0.0");
                    tbStation.Text = pitm.Station.ToString("0.0");
                    tbTimeDiff.Text = secs.ToString("0.00") + " s";
                    tbAltitude.Text = (pitm.Coord.AL + Depth).ToString("0.0");
                    btnAdd.Enabled = true;
                }
            }
        }

        private double CompDepth(double p)
        {
            return
                -0.0061 * p * p * p * p * p * p
                - 0.0932 * p * p * p * p * p
                - 0.4956 * p * p * p * p
                - 1.1556 * p * p * p
                - 1.1758 * p * p
                - 0.4210 * p
                - 0.018;
        }

        private void SearchDLSBMessage(SonarLine line)
        {
            if (line == null)
                return;

            double antLength = 0;
            Double.TryParse(tbAntLength.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out antLength);

            foreach (SonarPos pos in line.PosList)
            {
                if (pos.type == PosType.DLSB30)
                {
                    DLSB30 dls = new DLSB30(pos.strPos);
                    MADepth = (antLength - dls.Distance - 1.48);
                    tbAntDepth.Text = MADepth.ToString("0.00", GSC.Settings.NFI);
                    line.AntDepth = -MADepth;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SonarRecord rec = project.NewRecord;

            string desc = "ST" + MPPosItem.Station.ToString("0000.0") + " DT" + MPPosItem.Distance.ToString("000.0") + " DM" + MPDiameter.ToString("00.00");

            rec.AddManualPoint(new ManualPoint(MPPosItem.Coord, (float)MPDepth, 908, desc));
            btnDiscard_Click(null, null);
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            tbDistance.Text = "";
            tbStation.Text = "";
            tbTimeDiff.Text = "";
            tbDepth.Text = "";
        }

    }
}
