using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using UKLib.Survey.Math;
using UKLib.MathEx;
using sonOmeter.Classes;
using sonOmeter.Classes.Sonar2D;
using System.Collections.ObjectModel;
using UKLib.Debug;
using DockDotNET;
using UKLib.Survey.Parse;
using System.Collections.Generic;
using UKLib.DXF;
using UKLib;
using UKLib.ExtendedAttributes;
using sonOmeter.resx;
using System.IO;

namespace sonOmeter
{
    #region TraceRectangleList
    public class TraceRectangleList
    {
        private Hashtable table = new Hashtable();

        public RectangleF[] GetDrawArray(RectangleD rcRegion, PointD ptOffset)
        {
            RectangleD[] arD;

            lock (table.SyncRoot)
            {
                arD = new RectangleD[table.Count];
                table.Values.CopyTo(arD, 0);
            }

            int max = arD.Length;
            
            RectangleF[] arF = new RectangleF[max];
            RectangleD rc;
            PointF pt;

            for (int i = 0; i < max; i++)
            {
                rc = arD[i];

                if (!rcRegion.Contains(rc))
                    continue;

                pt = (rc.BL + ptOffset).PointF;
                arF[i] = new RectangleF(pt.X, pt.Y, (float)(rc.Width), (float)(rc.Height));
            }

            return arF;
        }

        public bool Add(Coordinate c)
        {
            if (c.Type != CoordinateType.TransverseMercator)
                return false;

            double size = GSC.Settings.TraceSize;

            int nX = (int)Math.Floor(c.RV / size);
            int nY = (int)Math.Floor(c.HV / size);

            string hash = nX.ToString() + ";" + nY.ToString();

            lock (table.SyncRoot)
            {
                if (table.Contains(hash))
                    return false;

                table.Add(hash, new RectangleD(nX * size, nY * size, (nX + 1.0) * size, (nY + 1.0) * size));
            }

            return true;
        }

        public void Add(List<SonarLine> lines)
        {
            int max = lines.Count;

            for (int i = 0; i < max; i++)
                Add(lines[i].CoordRvHv);
        }

        public void Clear()
        {
            if (table != null)
                table.Clear();
        }

        public void Draw(Graphics g, RectangleD rcRegion)
        {
            if (table.Count > 0)
                g.FillRectangles(new SolidBrush(GSC.Settings.CS.TraceColor), GetDrawArray(rcRegion, PointD.Origin - rcRegion.Center));
        }
    }
    #endregion

    /// <summary>
    /// Summary description for Sonar2DView.
    /// </summary>
    public class Sonar2DView : System.Windows.Forms.UserControl
    {
        public enum InteractionModes
        {
            None,
            Find,
            PlaceBuoy,
            MeasureStart,
            MeasureEnd,
            EditBlankLine
        }

        #region Construct and dispose
        public Sonar2DView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Initialize trace mode array - this is buffered all the time.
            RebuildTraceArray();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));

            RedrawSonarBmp(); // fast redraw.
            RedrawDXFBmp();   // may take a while - triggers RedrawSonarBmp() after completion.

            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GetFilterList());
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private List<GlobalNotifier.MsgTypes> GetFilterList()
        {
            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceBuoy);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceManualPoint);
            filterlist.Add(GlobalNotifier.MsgTypes.ToggleDXFFile);
            filterlist.Add(GlobalNotifier.MsgTypes.ToggleBlankLine);
            filterlist.Add(GlobalNotifier.MsgTypes.CutEvent);
            filterlist.Add(GlobalNotifier.MsgTypes.NewCoordinate);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateCoordinates);

            return filterlist;
        }
        #endregion

        #region Component Designer generated code
        private System.ComponentModel.IContainer components;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.changeTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // changeTimer
            // 
            this.changeTimer.Enabled = true;
            this.changeTimer.Interval = 250;
            this.changeTimer.Tick += new System.EventHandler(this.changeTimer_Tick);
            // 
            // Sonar2DView
            // 
            this.BackColor = System.Drawing.Color.Blue;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Sonar2DView";
            this.ResumeLayout(false);

        }
        #endregion

        #region Enum types
        public enum Mode2D
        {
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode2D_None", true)]
            None = 0,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode2D_Trace", true)]
            Trace = 1,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode2D_Surface", true)]
            Surface = 2,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode2D_Both", true)]
            Both = 3
        }

        public enum Mode3D
        {
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode3D_None", true)]
            None = 0,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode3D_TwoD", true)]
            TwoD = 1,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode3D_ThreeD", true)]
            ThreeD = 2,
            [LocalizedEnumDisplayName(typeof(frm2DStrings), "Mode3D_Both", true)]
            Both = 3
        }
        #endregion

        private class BWDXFResult
        {
            public Bitmap bmp;
            public RectangleD viewPort;
            public double viewAngle;
        }

        #region Variables
        // Tooltip, timer, borders
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Timer changeTimer;

        private BorderStyle borderStyle = BorderStyle.None;
        private int borderX = 20;
        private int borderY = 5;

        // Grid settings
        private bool showGrid = true;
        public double gridSpacing = 0;
        private const double minGridSpacing = 100.0;

        public event EventHandler OnGridChanged;

        // Input device settings and variables
        private double wheelSpeed = 10.0;

        private bool selectMode = false;

        private InteractionModes interactionMode = InteractionModes.None;

        private Point ptStart = new Point(0, 0);
        private Point ptEnd = new Point(0, 0);

        // Draw settings
        private SonarPanelType panelType = SonarPanelType.HF;
        private TopColorMode topColorMode = TopColorMode.Top;
        private Mode2D viewMode2D = Mode2D.Surface;
        private Mode3D viewMode3D = Mode3D.TwoD;
        private bool showBlankLines = true;

        // Sonar background worker variables
        BackgroundWorker<bool, Bitmap> bwDrawSonarBmp = null;
        Bitmap bmpSonarBW = null;
        Bitmap bmpSonar = null;
        bool bwSonarNeedStartAgain = false;

        // DXF background worker variables
        BackgroundWorker<bool, BWDXFResult> bwDrawDXFBmp = null;
        Bitmap bmpDXFBW = null;
        Bitmap bmpDXF = null;
        bool bwDXFNeedStartAgain = false;

        private RectangleD viewPortDXF = new RectangleD();
        private double viewAngleDXF = 0.0;

        private RectangleD viewPortDXFbw = new RectangleD();
        private double viewAngleDXFbw = 0.0;

        // Blank line
        private bool ptBLGrabbed = false;
        private int blankLinePointIndex = -1;
        private int blankLineSelectIndex = -1;

        // Workline
        private SonarLine workLine = null;
        private SonarRecord workLineRec = null;
        private SonarDevice workLineDev = null;

        // Coordinates and viewport
        private RectangleD coordLimits = new RectangleD();
        private RectangleD viewPort = new RectangleD(0, 0, 1, 1);
        private RectangleD viewPortExt = new RectangleD(0, 0, 1, 1);
        private double viewAngle = 0.0;

        private Coordinate lastCoord = new Coordinate();

        // Misc
        private SonarProject project = null;
        private BuoyConnection lockedConnection = null;
        private TraceRectangleList trace = new TraceRectangleList();
        #endregion

        #region Events
        [Description("This event occurs on a changed viewport."), Category("Misc")]
        public event EventHandler ViewPortChanged;

        [Description("This event occurs on Mousemove and CoordChange."), Category("Misc")]
        public event CoordEventHandler CoordChange;
        #endregion

        #region Properties
        [Browsable(false)]
        public InteractionModes InteractionMode
        {
            get { return interactionMode; }
            set
            {
                interactionMode = value;

                if (interactionMode == InteractionModes.EditBlankLine)
                {
                    Invalidate();
                }
            }
        }

        [Description("Toggles the SonarLine, volume or depth display of the sonar data."), Category("Behavior")]
        public TopColorMode TopColorMode
        {
            get { return topColorMode; }
            set { topColorMode = value; ResetChangeTimer(); }
        }

        [Description("The style of the surrounding border."), Category("Appearance")]
        public new BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set { borderStyle = value; Invalidate(); }
        }

        [Description("The function of the panel."), Category("Data")]
        public SonarPanelType PanelType
        {
            get { return panelType; }
            set { panelType = value; ResetChangeTimer(); }
        }

        [Description("The mode of the 2D view."), Category("Data")]
        public Mode2D ViewMode2D
        {
            get { return viewMode2D; }
            set { viewMode2D = value; ResetChangeTimer(); }
        }

        [Description("The mode of the 2D view."), Category("Data")]
        public Mode3D ViewMode3D
        {
            get { return viewMode3D; }
            set { viewMode3D = value; ResetChangeTimer(); }
        }

        [Description("Toggles the Grid."), Category("Grid")]
        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value; ResetChangeTimer(); }
        }

        [Description("Toggles the BlankLines."), Category("Data")]
        public bool ShowBlankLines
        {
            get { return showBlankLines; }
            set { showBlankLines = value; ResetChangeTimer(); }
        }

        [Browsable(false)]
        public double GridSpacing
        {
            get
            {
                return gridSpacing;
            }
            private set
            {
                gridSpacing = value;
            }
        }

        [Description("The size of the surrounding border."), Category("Appearance")]
        public int BorderX
        {
            get { return borderX; }
            set
            {
                borderX = value;
                ResetChangeTimer();
            }
        }

        [Description("The size of the surrounding border."), Category("Appearance")]
        public int BorderY
        {
            get { return borderY; }
            set
            {
                borderY = value;
                ResetChangeTimer();
            }
        }

        [Description("The zoom speed when using the mouse wheel."), Category("Behavior")]
        public double WheelSpeed
        {
            get { return wheelSpeed; }
            set { wheelSpeed = value; }
        }

        [Description("The visible region."), Category("Layout")]
        public RectangleD ViewPort
        {
            get { return viewPort; }
            set
            {
                viewPort = value;
                AdjustViewPortAspectRatio();

                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);

                RedrawSonarBmp(); // fast redraw.
                RedrawDXFBmp();   // may take a while - triggers RedrawSonarBmp() after completion.
            }
        }

        [Description("The angle of the visible region."), Category("Layout")]
        public double ViewAngle
        {
            get { return viewAngle; }
            set
            {
                if (!GSC.Settings.Lic[Module.Modules.V22])
                    value = 0;

                if (value < 0)
                    viewAngle = value + 360.0;
                else if (value >= 360.0)
                    viewAngle = value - 360.0;
                else
                    viewAngle = value;

                AdjustViewPortAspectRatio();

                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);

                RedrawSonarBmp(); // fast redraw.
                RedrawDXFBmp();   // may take a while - triggers RedrawSonarBmp() after completion.
            }
        }

        [Description("The bounding rectangle of the record coordinates."), Category("Data")]
        public RectangleD CoordLimits
        {
            get { return coordLimits; }
            set { coordLimits = value; }
        }

        [Browsable(false)]
        public SonarProject Project
        {
            set
            {
                project = value;
                ResetChangeTimer();
            }
        }

        [Browsable(false)]
        public Coordinate LastCoord
        {
            get { return lastCoord; }
        }

        [Browsable(false)]
        public BuoyConnection LockedConnection
        {
            get { return lockedConnection; }
        }
        #endregion

        #region ViewPort
        private void AdjustViewPortAspectRatio()
        {
            if ((this.Width == 0) ||
                (this.Height == 0))
                return;

            double ratioRC = viewPort.Width / viewPort.Height;
            double ratio2D = (double)this.Width / (double)this.Height;
            double dw = viewPort.Height * ratio2D - viewPort.Width;
            double dh = viewPort.Width / ratio2D - viewPort.Height;
            double viewAngleRad = Math.PI * viewAngle / 180.0;

            if (ratioRC != ratio2D)
            {
                if (viewPort.Width < viewPort.Height)
                {
                    viewPort.Left -= dw / 2.0;
                    viewPort.Right += dw / 2.0;
                }
                else
                {
                    viewPort.Top += dh / 2.0;
                    viewPort.Bottom -= dh / 2.0;
                }
            }

            viewPortExt = new RectangleD(viewPort);

            dw = Math.Abs(viewPortExt.Width * Math.Cos(viewAngleRad)) + Math.Abs(viewPortExt.Height * Math.Sin(viewAngleRad)) - viewPortExt.Width;
            dh = Math.Abs(viewPortExt.Height * Math.Cos(viewAngleRad)) + Math.Abs(viewPortExt.Width * Math.Sin(viewAngleRad)) - viewPortExt.Height;

            viewPortExt.Left -= dw / 2.0;
            viewPortExt.Right += dw / 2.0;
            viewPortExt.Bottom -= dh / 2.0;
            viewPortExt.Top += dh / 2.0;
        }
        #endregion

        #region ChangeTimer
        private void changeTimer_Tick(object sender, System.EventArgs e)
        {
            DockPanel panel = this.Parent as DockPanel;

            changeTimer.Stop();

            if ((panel == null) || (!panel.Form.Visible))
                return;

            RedrawSonarBmp();
        }

        public void ResetChangeTimer()
        {
            changeTimer.Start();
        }
        #endregion

        #region Drawing
        #region Trace
        public void RebuildTraceArray()
        {
            if (trace != null)
                trace.Clear();
            else
                trace = new TraceRectangleList();

            if (project == null)
                return;

            int nRec = project.Records.Count;
            int r;

            // Look into each record of the current project.
            for (r = 0; r < nRec; r++)
                RebuildTraceArray(project.Records[r]);

            // Look into each record of the current project.
            nRec = project.Profiles.Count;

            for (r = 0; r < nRec; r++)
                RebuildTraceArray(project.Profiles[r]);
        }

        private void RebuildTraceArray(SonarRecord rec)
        {
            SonarDevice dev;
            List<SonarLine> lineList;
            List<ManualPoint> pointList;
            SonarLine line;
            LineData data;
            int nDev = 0;
            int nLin = 0;
            int nMPt = 0;
            int d, l, m;
            
            nDev = rec.Devices.Count;

            // Look into each device of the current record.
            for (d = 0; d < nDev; d++)
            {
                dev = rec.Devices[d];
                
                // Skip, if not enabled.
                if (!dev.ShowInTrace)
                    continue;

                lineList = dev.SonarLines;
                nLin = lineList.Count;

                // Look into each line of the current device.
                for (l = 0; l < nLin; l++)
                {
                    line = lineList[l];

                    if (panelType == SonarPanelType.HF)
                        data = line.HF;
                    else
                        data = line.NF;

                    // Filter out points that were completely cut.
                    if (data.TCut <= data.BCut)
                        continue;

                    if (data.isNullEntries)
                        continue;

                    trace.Add(line.CoordRvHv);
                }
            }

            // Look into each manual point of the current record.
            if (rec.ShowManualPoints)
            {
                pointList = rec.ManualPoints;
                nMPt = pointList.Count;

                for (m = 0; m < nMPt; m++)
                    trace.Add(pointList[m].Coord);
            }
        }
        #endregion

        #region RectArray filling
        private RectArrayF[] InitRectArray(double scaleFactor)
        {
            RectArrayF[] array;
            int i, colorCount = GSC.Settings.SECL.Count;
            float oneOverScale = (float)(1.0 / scaleFactor);

            array = new RectArrayF[colorCount + 2];

            for (i = 0; i < colorCount; i++)
                array[i] = new RectArrayF(GSC.Settings.SECL[i].SonarColor, oneOverScale);

            array[colorCount] = new RectArrayF(GSC.Settings.CS.ArchNoDataColor, oneOverScale);
            array[colorCount + 1] = new RectArrayF(GSC.Settings.CS.ManualPointColor, oneOverScale);

            return array;
        }

        private void FillArrayFromDevice(List<SonarLine> sonarLines, RectArrayF[] rcArray, Graphics graphics, RectangleD rcRegion, float w, float h, double scaleFactor, ArchBounds bounds)
        {
            RectangleF rcLast = RectangleF.Empty;
            RectangleF rc = RectangleF.Empty;
            SonarLine line = null;
            LineData data = null;
            PointD pt;
            int col = 0;
            int i = 0;
            int sum;
            int count = sonarLines.Count;
            float depthFieldMin, depthFieldMax;

            int skip = 1; // (int)((7.0 / scaleFactor - (double)w) / 2.0 * scaleFactor);
            //if (skip <= 0)
            //    skip = 1;

            // Iterate through all lines with previously calculated step size (skip).
            for (i = 0; i < count; i += skip)
            {
                try
                {
                    line = sonarLines[i];

                    // Not inside of drawing region? Skip it!
                    if (!rcRegion.Contains(line.CoordRvHv.Point))
                        continue;

                    if (panelType == SonarPanelType.HF)
                    {
                        data = line.HF;
                        depthFieldMin = bounds.MinHF;
                        depthFieldMax = bounds.MaxHF;
                    }
                    else
                    {
                        data = line.NF;
                        depthFieldMin = bounds.MinNF;
                        depthFieldMax = bounds.MaxNF;
                    }

                    col = data.TopColor;

                    // Filter out not usable top colors.
                    if ((col < -1) || (col > 6))
                        continue;

                    // Filter out points that were completely cut.
                    if (data.TCut <= data.BCut)
                        continue;

                    // Prepare data and create the rectangle.
                    pt = line.CoordRvHv.Point;
                    if (col == -1)
                        col = GSC.Settings.SECL.Count;

                    rc = GetRect(rcRegion, w, h, pt, ref col);

                    if (topColorMode == TopColorMode.Vol)
                    {
                        sum = (int)data.GetVolume(false);
                        if (sum < 0)
                            sum = 0;
                        if (sum > 255)
                            sum = 255;
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(sum, sum, sum)), rc);
                    }
                    else if (topColorMode == TopColorMode.Dep)
                    {
                        sum = (int)(255.0 * (depthFieldMax - data.Depth) / (depthFieldMax - depthFieldMin));
                        if (sum < 0)
                            sum = 0;
                        if (sum > 255)
                            sum = 255;
                        sum = 255 - sum;
                        graphics.FillRectangle(new SolidBrush(Color.FromArgb(sum, sum, sum)), rc);
                    }
                    else if (rc != rcLast)
                    {
                        rcArray[col].List.Add(rc);
                        rcLast = rc;
                    }
                }
                catch
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "PositionView::FillArray failed.");
                }
            }
        }

        private RectangleF GetRect(RectangleD rcRegion, float w, float h, PointD pt, ref int col)
        {
            pt = new PointD(pt.X - GSC.Settings.DotSize / 2.0, pt.Y - GSC.Settings.DotSize / 2.0);

            return new RectangleF((float)(pt.X - rcRegion.Center.X), (float)(pt.Y - rcRegion.Center.Y), w, h);
        }

        private void FillArrayFromRecord(SonarRecord rec, RectArrayF[] rcArray, Graphics graphics, RectangleD rcRegion, float w, float h, double scaleFactor)
        {
            int nDev = rec.DeviceCount;
            int i;

            // Skip if record bounding box is not present in our drawing region.
            if (!rcRegion.Contains(rec.CoordLimits))
                return;

            // Iterate through all devices.
            for (i = 0; i < nDev; i++)
            {
                SonarDevice device = rec.Devices[i];

                // Not selected? Skip this one!
                if (!device.ShowInTrace)
                    continue;

                // Fill the array.
                FillArrayFromDevice(device.SonarLines, rcArray, graphics, rcRegion, w, h, scaleFactor, rec.DepthFieldBounds);
            }

            // Skip if manual points are disabled.
            if (!rec.ShowManualPoints)
                return;

            // Iterate through all manual points.
            nDev = rec.ManualPoints.Count;

            for (i = 0; i < nDev; i++)
            {
                // This entry is not inside of the region? Skip it!
                if (!rcRegion.Contains(rec.ManualPoints[i].GetPointRVHV()))
                    continue;

                // Add the manual point.
                int col = GSC.Settings.SECL.Count + 1;
                rcArray[col].List.Add(GetRect(rcRegion, w, h, rec.ManualPoints[i].GetPointRVHV(), ref col));
            }
        }

        private bool FillArray(RectArrayF[] rcArray, Graphics graphics, RectangleD rcRegion, double scaleFactor)
        {
            bool restart = false;

            if (rcRegion.IsEmpty)
                return restart;

            // Retrieve dot sizes.
            float w = (float)GSC.Settings.DotSize;
            float h = (float)GSC.Settings.DotSize;

            if (w * (float)scaleFactor < 5.0F)
            {
                h *= ((7.0F / (float)scaleFactor - w) / 2.0F * (float)scaleFactor);
                w *= ((7.0F / (float)scaleFactor - w) / 2.0F * (float)scaleFactor);
            }

            // Iterate through all records.
            int i;
            int nRec = ((viewMode3D & Mode3D.TwoD) != Mode3D.None) ? project.RecordCount : 0;

            for (i = 0; i < nRec; i++)
            {
                if (project.Record(i).ApplyingArchAndVolume)
                    restart = true;
                FillArrayFromRecord(project.Record(i), rcArray, graphics, rcRegion, w, h, scaleFactor);
            }
              
            // Iterate through all profiles.
            nRec = ((viewMode3D & Mode3D.TwoD) != Mode3D.None) ? project.ProfileCount : 0;

            for (i = 0; i < nRec; i++)
            {
                if (project.Profile(i).ApplyingArchAndVolume)
                    restart = true;
                FillArrayFromRecord(project.Profile(i), rcArray, graphics, rcRegion, w, h, scaleFactor);
            }

            // Iterate through all 3D records.
            nRec = ((viewMode3D & Mode3D.ThreeD) != Mode3D.None) ? project.Record3DCount : 0;

            for (i = 0; i < nRec; i++)
            {
                Sonar3DRecord rec3D = project.Record3D(i);

                if (rec3D.ApplyingArchAndVolume)
                    restart = true;

                // Not selected? Skip this one!
                if (!rec3D.ShowInTrace)
                    continue;

                // Adjust w and h scale factors.
                w = (float)rec3D.GridX;
                h = (float)rec3D.GridY;

                // Fill the array.
                FillArrayFromDevice(rec3D.SonarLines(), rcArray, graphics, rcRegion, w, h, scaleFactor, rec3D.DepthFieldBounds);
            }

            return restart;
        }

        private void DrawArray(RectArrayF[] rcArray, Graphics g)
        {
            for (int i = 0; i < rcArray.Length; i++)
            {
                RectArrayF rcAr = rcArray[i];

                try
                {
                    if ((GSC.Settings.DotShape == DotShapes.Rectangle) || (rcArray.Length == 1))
                    {
                        if (rcAr.List.Count > 0)
                            g.FillRectangles(rcAr.Brush, rcAr.Array);
                    }
                    else if (GSC.Settings.DotShape == DotShapes.Circle)
                    {
                        for (int j = 0; j < rcAr.List.Count; j++)
                            g.FillEllipse(rcAr.Brush, (RectangleF)rcAr.List[j]);
                    }
                }
                catch (Exception ex)
                {
                    // Filter out all unwanted events. :-)
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
                }
            }
        }
        #endregion

        #region Blank line
        private void DrawBlankLine(Graphics graphics, RectangleD rcRegion, double scaleFactor, BlankLine blankLine)
        {
            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            try
            {
                if (blankLine.Poly.Count < 2)
                    return;

                if (rcRegion.IsEmpty)
                    return;

                GraphicsPath path = new GraphicsPath();
                PointD pt1, pt2;
                int i, max = blankLine.Poly.Count;
                Color color = blankLine.Color;
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(color, (float)(2.0 / scaleFactor));
                float sizeBig = (float)(10.0 / scaleFactor);
                float sizeSmall = (float)(7.0 / scaleFactor);

                for (i = 1; i <= max; i++)
                {
                    pt1 = blankLine.Poly.Points[i - 1] - rcRegion.Center;
                    pt2 = blankLine.Poly.Points[(i == max) ? 0 : i] - rcRegion.Center;

                    path.AddLine(pt1.PointF, pt2.PointF);

                    if ((i == max) & (interactionMode == InteractionModes.EditBlankLine) & (project.EditBlankLine == blankLine))
                        graphics.FillEllipse(brush, pt1.BuildRectF(sizeBig, sizeBig));
                    else
                        graphics.FillEllipse(brush, pt1.BuildRectF(sizeSmall, sizeSmall));

                    if ((interactionMode != InteractionModes.EditBlankLine) || (project.EditBlankLine != blankLine))
                        continue;

                    if (ctrlHold | altHold)
                        continue;

                    if (i == blankLineSelectIndex)
                    {
                        graphics.DrawLine(pen, pt1.PointF, pt2.PointF);
                        graphics.FillEllipse(brush, ((pt1 + pt2) / 2.0).BuildRectF(sizeSmall, sizeSmall));
                    }
                }

                brush.Color = Color.FromArgb(GSC.Settings.CS.AlphaChannel, color);
                pen.Width = pen.Width / 2.0F;

                graphics.DrawPath(pen, path);
                graphics.FillPath(brush, path);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.DrawBlankLine: " + e.Message);
            }
        }
        #endregion

        #region Markers
        private void DrawMarkers(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            if (project == null)
                return;

            g.RotateTransform((float)viewAngle);

            MatrixD mat = GetTransform();
            Pointer ptr = new Pointer();

            // Draw the buoy connections.
            foreach (BuoyConnection c in project.BuoyConnectionList)
                c.Paint(g, rcRegion, scaleFactor);

            // Draw the buoys.
            foreach (Buoy b in project.BuoyList)
                b.Paint(g, rcRegion, scaleFactor);

            // Draw the record markers.
            for (int i = 0; i < project.RecordCount; i++)
            {
                SonarRecord record = project.Record(i);
                RecordMarker marker = new RecordMarker();
                Coordinate c1, c2;

                if (record.ShowInTrace)
                {
                    record.RecalcRecordMarkers();

                    c1 = record.StartCoord;
                    c2 = record.StartCoord2;
                    if ((c1.Type == CoordinateType.TransverseMercator) &&
                            (c2.Type == CoordinateType.TransverseMercator))
                    {
                        marker.Coord = c1;
                        marker.NextCoord = c2;
                        marker.Paint(g, rcRegion, scaleFactor);
                    }

                    c1 = record.EndCoord;
                    c2 = record.EndCoord2;
                    if ((c1.Type == CoordinateType.TransverseMercator) &&
                        (c2.Type == CoordinateType.TransverseMercator))
                    {
                        marker.Coord = c1;
                        marker.NextCoord = c2;
                        marker.Paint(g, rcRegion, scaleFactor);
                    }
                }

                // Draw the movement vector.
                if (record.Recording || project.Tracking)
                {
                    ptr.Angle = 90 - project.LastRotation.Yaw;
                    ptr.Coord = project.LastCoord;
                }
                else
                    continue;

                RectangleD rcRegOff = new RectangleD(rcRegion);
                rcRegOff.Offset(-project.LastLockOffset); // Negative sign because of later subtraction of the region center.
                lastCoord = ptr.Coord;
                ptr.Velocity = 60;
                ptr.Type = GSC.Settings.BoatVector;
                ptr.Paint(g, rcRegOff, scaleFactor);
            }

            // Draw the work line marker.
            if (workLine != null)
            {
                if (workLine.CoordRvHv.Type == CoordinateType.TransverseMercator)
                {
                    WorkLineMarker wlm = new WorkLineMarker();
                    wlm.Coord = workLine.CoordRvHv;
                    wlm.Paint(g, rcRegion, scaleFactor);
                }
            }
        }
        #endregion

        #region Redraw and background worker
        public void RedrawSonarBmp()
        {
            if (bwDrawSonarBmp == null)
            {
                bwDrawSonarBmp = new BackgroundWorker<bool, Bitmap>();
                bwDrawSonarBmp.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<Bitmap>>(bwDrawSonarBmp_RunWorkerCompleted);
                bwDrawSonarBmp.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, Bitmap>>(bwDrawSonarBmp_DoWork);
            }
            if (!bwDrawSonarBmp.IsBusy)
            {
                bwDrawSonarBmp.RunWorkerAsync();
            }
            else
            {
                bwSonarNeedStartAgain = true;
            }
        }

        void bwDrawSonarBmp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs<Bitmap> e)
        {
            try
            {
                if (bmpSonarBW == null)
                    return;

                bmpSonar = bmpSonarBW.Clone() as Bitmap;

                Invalidate();
                if (bwSonarNeedStartAgain)
                {
                    bwSonarNeedStartAgain = false;
                    RedrawSonarBmp();
                }
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        }

        void bwDrawSonarBmp_DoWork(object sender, DoWorkEventArgs<bool, Bitmap> e)
        {
            try
            {
                if ((this.Width <= 0) || (this.Height <= 0) || (viewPort.Width <= 0) || double.IsNaN(viewPort.Width))
                    return;

                if (GSC.Settings == null)
                    return;

                bool clear = true;

                if ((bmpDXF != null) && !bwDrawDXFBmp.IsBusy)
                {
                    // Copy DXF bitmap directly.
                    bmpSonarBW = bmpDXF.Clone() as Bitmap;
                    clear = false;
                }
                else if ((bmpSonarBW == null) || (Width != bmpSonarBW.Width) || (Height != bmpSonarBW.Height))
                {
                    // Rebuild bitmap structure if needed.
                    if (bmpSonarBW != null)
                        bmpSonarBW.Dispose();
                    bmpSonarBW = new Bitmap(this.Width, this.Height);
                }

                // Get graphics object.
                Graphics g = Graphics.FromImage(bmpSonarBW);
                g.SmoothingMode = SmoothingMode.HighSpeed;

                // Flush old bitmap.
                // Edit: ...with transparent color...
                if (clear)
                    g.Clear(Color.Transparent);
                    //g.Clear(this.BackColor);

                // Set the drawing transformation.
                double scaleFactor = (double)(bmpSonarBW.Width - 2 * borderX) / viewPort.Width;
                g.ScaleTransform((float)scaleFactor, -(float)scaleFactor);
                g.TranslateTransform((float)(viewPort.Width / 2.0), -(float)(viewPort.Height / 2.0));

                // Draw DXF indirectly (if needed)
                if ((bmpDXF != null) && clear && !viewPortDXF.IsEmpty)
                {
                    Matrix t = g.Transform;

                    g.ScaleTransform(1.0F, -1.0F);
                    g.RotateTransform((float)(viewAngleDXF - viewAngle));

                    RectangleD rc = new RectangleD(viewPortDXF);
                    rc.Offset(-viewPort.Center.X, -2.0 * viewPortDXF.Center.Y + viewPort.Center.Y);

                    g.DrawImage(bmpDXF, rc.RectF);

                    g.Transform = t;
                }

                g.RotateTransform((float)viewAngle);

                // Draw grid.
                DrawGrid(g, viewPortExt, scaleFactor);

                // Prepare and draw.
                if ((project != null) && (panelType != SonarPanelType.Void))
                {
                    RectArrayF[] rcArray;
                    switch (viewMode2D & Mode2D.Both)
                    {
                        case Mode2D.Both:
                            trace.Draw(g, viewPortExt);

                            rcArray = InitRectArray(scaleFactor);
                            if (FillArray(rcArray, g, viewPortExt, scaleFactor))
                                bwSonarNeedStartAgain = true;
                            DrawArray(rcArray, g);
                            break;

                        case Mode2D.Trace:
                            trace.Draw(g, viewPortExt);
                            break;

                        default:
                            rcArray = InitRectArray(scaleFactor);
                            if (FillArray(rcArray, g, viewPortExt, scaleFactor))
                                bwSonarNeedStartAgain = true;
                            DrawArray(rcArray, g);
                            break;
                    }

                    rcArray = null;
                }

                // Invalidate
                g.Dispose();

                // Create result.
                e.Result = (bmpSonarBW != null) ? (Bitmap)bmpSonarBW.Clone() : null;
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        }
        #endregion

        #region DXF background worker
        public void RedrawDXFBmp()
        {
            if ((project == null) || (project.DXFFiles.Count == 0) || (GSC.Settings == null) || !GSC.Settings.Lic[Module.Modules.DXF])
            {
                bmpDXF = null;
                bmpDXFBW = null;

                return;
            }

            if (bwDrawDXFBmp == null)
            {
                bwDrawDXFBmp = new BackgroundWorker<bool, BWDXFResult>();
                bwDrawDXFBmp.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<BWDXFResult>>(bwDrawDXFBmp_RunWorkerCompleted);
                bwDrawDXFBmp.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, BWDXFResult>>(bwDrawDXFBmp_DoWork);
            }
            if (!bwDrawDXFBmp.IsBusy)
            {
                bwDrawDXFBmp.RunWorkerAsync();
            }
            else
            {
                bwDXFNeedStartAgain = true;
            }
        }

        void bwDrawDXFBmp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs<BWDXFResult> e)
        {
            if (e.Result != null)
            {
                if (e.Result.viewPort != null)
                    viewPortDXF = e.Result.viewPort;
                viewAngleDXF = e.Result.viewAngle;

                if (e.Result.bmp != null)
                    bmpDXF = e.Result.bmp;

                // Force the redraw of the complete sonar bitmap.
                RedrawSonarBmp();
            }

            if (bwDXFNeedStartAgain)
            {
                bwDXFNeedStartAgain = false;
                RedrawDXFBmp();
            }
        }

        void bwDrawDXFBmp_DoWork(object sender, DoWorkEventArgs<bool, BWDXFResult> e)
        {
            if ((this.Width <= 0) || (this.Height <= 0))
                return;

            if (GSC.Settings == null)
                return;

            RectangleD viewPortExtTemp = new RectangleD(viewPortExt);
            viewPortDXFbw = new RectangleD(viewPort);
            viewAngleDXFbw = viewAngle;

            if (viewPortDXFbw.Width == 0)
                return;

            // Rebuild bitmap structure if needed.
            if ((bmpDXFBW == null) || (Width != bmpDXFBW.Width) || (Height != bmpDXFBW.Height))
            {
                if (bmpDXFBW != null)
                    bmpDXFBW.Dispose();
                bmpDXFBW = new Bitmap(this.Width, this.Height);
            }

            // Get graphics object.
            Graphics graphics = Graphics.FromImage(bmpDXFBW);
            graphics.SmoothingMode = SmoothingMode.HighSpeed;

            // Flush old bitmap.
            graphics.Clear(this.BackColor);

            // Set the drawing transformation.
            double scaleFactor = (double)(bmpDXFBW.Width - 2 * borderX) / viewPortDXFbw.Width;
            graphics.ScaleTransform((float)scaleFactor, -(float)scaleFactor);
            graphics.TranslateTransform((float)(viewPortDXFbw.Width / 2.0), -(float)(viewPortDXFbw.Height / 2.0));
            graphics.RotateTransform((float)viewAngleDXFbw);

            // Draw all DXF files included in the project.
            if (GSC.Settings.Lic[Module.Modules.DXF] && project != null)
            {
                foreach (DXFFile file in project.DXFFiles)
                    if (file.ShowInTrace)
                    {
                        file.DXF.OnDraw(graphics, viewPortExtTemp, scaleFactor, GSC.Settings.SkipDetail / scaleFactor);
                        //var g = new ukmaLib.Drawing.GDI.Graphics(graphics);
                        //file.DXF.OnDraw(g, new ukmaLib.Drawing.RectangleD(viewPortExtTemp.Left, viewPortExtTemp.Bottom, viewPortExtTemp.Right, viewPortExtTemp.Top), scaleFactor, GSC.Settings.SkipDetail / scaleFactor);
                        //Console.WriteLine("Points: {0}, Arcs: {1}", g.counter, g.arccounter);
                    }
            }

            // Invalidate
            graphics.Dispose();

            // Create result.
            e.Result = new BWDXFResult() { bmp = (bmpDXFBW != null) ? (Bitmap)bmpDXFBW.Clone() : null, viewAngle = viewAngleDXFbw, viewPort = (viewPortDXFbw != null) ? new RectangleD(viewPortDXFbw) : null };
        }
        #endregion

        #region GeoTagged Files
        public void DrawGeoTaggedImage(string fileName, CoordinateType type, bool transparentBG, bool drawOverlays)
        {
            if ((viewPort.Width == 0) || (viewPort.Height == 0) || (bmpSonar == null))
                return;

            // Save the current viewport.
            RectangleD rc = new RectangleD(viewPort);

            // Create a new bitmap with control size.
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(bmp);
            
            // Flush the Background, if needed.
            if (!transparentBG)
                g.Clear(GSC.Settings.CS.BackColor);
            else
                g.Clear(Color.Transparent);

            // Decide what to draw.
            if (drawOverlays)
            {
                // Create a bypass to the OnPaint function of this control.
                // Let the control paint on the bitmap instead on its own surface.
                OnPaint(new PaintEventArgs(g, new Rectangle(0, 0, bmp.Width, bmp.Height)));
            }
            else
            {
                // Draw the source image directly.
                if ((this.Width != bmpSonar.Width) || (this.Height != bmpSonar.Height))
                    g.DrawImage(bmpSonar, 0, 0, this.Width, this.Height);
                else
                    g.DrawImageUnscaled(bmpSonar, 0, 0, bmpSonar.Width, bmpSonar.Height);
            }

            // Create coordinates.
            Coordinate cTL = new Coordinate(rc.TL.X, rc.TL.Y, 0, CoordinateType.TransverseMercator);
            Coordinate cBR = new Coordinate(rc.BR.X, rc.BR.Y, 0, CoordinateType.TransverseMercator);

            // Convert them, if needed.
            double x = 0;
            double y = 0;
            Vector3D vDelta = new Vector3D();

            if (type == CoordinateType.Elliptic)
            {
                cTL = GSC.Settings.ForwardTransform.Run(cTL, type);
                cBR = GSC.Settings.ForwardTransform.Run(cBR, type);

                x = Trigonometry.Rad2Grad(cTL.LO);
                y = Trigonometry.Rad2Grad(cTL.LA);

                // ...convert rad to grad...
                // ...and switch LA/LO in order to get the right "delta vector"...
                double d;
                d = Trigonometry.Rad2Grad(cTL.LA);
                cTL.LA = Trigonometry.Rad2Grad(cTL.LO);
                cTL.LO = d;
                d = Trigonometry.Rad2Grad(cBR.LA);
                cBR.LA = Trigonometry.Rad2Grad(cBR.LO);
                cBR.LO = d;

                // Get the delta.
                vDelta = cBR.DeltaVector(cTL);

                // korrektur gedehte Bilder im Elliptic
                vDelta.X = -vDelta.X;
                vDelta.Y = -vDelta.Y;
            }
            else if (type == CoordinateType.TransverseMercator)
            {
                x = cTL.RV;
                y = cTL.HV;

                // Get the delta.
                vDelta = cTL.DeltaVector(cBR);
            }

            // Write data.
            string worldFileName = fileName + "w";
            StreamWriter writer = new StreamWriter(worldFileName);
            writer.WriteLine((vDelta.X / bmp.Width).ToString(GSC.Settings.NFI));
            writer.WriteLine("0");
            writer.WriteLine("0");
            writer.WriteLine((vDelta.Y / bmp.Height).ToString(GSC.Settings.NFI));
            writer.WriteLine(x.ToString(GSC.Settings.NFI));
            writer.WriteLine(y.ToString(GSC.Settings.NFI));
            writer.Close();

            // Dispose.
            bmp.Save(fileName);
            g.Dispose();
            bmp.Dispose();
        }
        #endregion
        #endregion

        #region Transformation
        private MatrixD GetTransform()
        {
            MatrixD matD = new MatrixD();

            matD.Scale(1, -1, MatrixOrder.Append);
            matD.Translate(-viewPort.Left, viewPort.Top, MatrixOrder.Append);
            matD.Scale((double)(this.Width) / viewPort.Width, (double)(this.Height) / viewPort.Height, MatrixOrder.Append);

            return matD;
        }

        private PointD ScreenToRegion(Point pt)
        {
            double x = (pt.X - borderX) * viewPort.Width / (this.Width - 2 * borderX) + viewPort.Left;
            double y = viewPort.Top - (pt.Y - borderY) * viewPort.Height / (this.Height - 2 * borderY);

            return ((new PointD(x, y) - viewPort.Center).Rotate(-viewAngle) + viewPort.Center);
        }

        private PointD RegionToScreen(PointD pt)
        {
            pt = (pt - viewPort.Center).Rotate(viewAngle) + viewPort.Center;

            double x = (pt.X - viewPort.Left) * (this.Width - 2 * borderX) / viewPort.Width + borderX;
            double y = (viewPort.Top - pt.Y) * (this.Height - 2 * borderY) / viewPort.Height + borderY;

            return new PointD(x, y);
        }
        #endregion

        #region Grid
        private double GetGridSpacing(double ratio)
        {
            double d = 0;

            double dBase = Math.Floor(Math.Log10(ratio));

            d = ratio / Math.Pow(10, dBase);

            if (d > 5.0)
                d = 10.0;
            else if (d > 2.0)
                d = 5.0;
            else if (d > 1.0)
                d = 2.0;
            else
                d = 1.0;

            return d * Math.Pow(10, dBase);
        }

        private void DrawGrid(Graphics graphics, RectangleD rcRegion, double scaleFactor)
        {
            if (!showGrid || (GSC.Settings.GridSize <= 0))
                return;

            if (rcRegion.IsEmpty)
                return;

            gridSpacing = GetGridSpacing(rcRegion.Width / (double)this.Width * minGridSpacing);

            // Prepare region rectangle.
            RectangleD rc = new RectangleD(rcRegion);

            // Draw grid.
            PointD pt, pt2;
            float oneOverScale = (float)(1.0 / scaleFactor);
            Pen penBig = new Pen(GSC.Settings.CS.BigGridColor, oneOverScale);
            Pen penSmall = new Pen(GSC.Settings.CS.SmallGridColor, oneOverScale);
            Pen penBack = new Pen(GSC.Settings.CS.BackColor, oneOverScale);
            double factor = gridSpacing / GSC.Settings.SmallGridDiv;
            double d, x2, y2;
            double x = (int)(rc.Left / gridSpacing);
            double y = (int)(rc.Bottom / gridSpacing);
            x *= gridSpacing;
            y *= gridSpacing;

            pt = new PointD(x, y);
            pt2 = new PointD(x + factor, y + factor);

            d = (((pt2.X - pt.X) > (pt.Y - pt2.Y)) ? (pt2.X - pt.X) : (pt.Y - pt2.Y)) * scaleFactor;

            if ((GSC.Settings.SmallGridDiv > 0) && (d > 20.0))
            {
                for (x2 = x; x2 < rc.Right; x2 += factor)
                {
                    for (y2 = y; y2 < rc.Top; y2 += factor)
                    {
                        pt = new PointD(x2 - rcRegion.Center.X, y2 - rcRegion.Center.Y);
                        graphics.DrawLine(penSmall, (float)pt.X, (float)pt.Y, (float)pt.X + oneOverScale, (float)pt.Y + oneOverScale);
                        graphics.DrawLine(penBack, (float)pt.X + oneOverScale, (float)pt.Y + oneOverScale, (float)pt.X + 2.0F * oneOverScale, (float)pt.Y + 2.0F * oneOverScale);
                    }
                }
            }

            rc.Offset(-rcRegion.Center.X, -rcRegion.Center.Y);
            x -= rcRegion.Center.X;
            y -= rcRegion.Center.Y;

            while (x < rc.Right)
            {
                graphics.DrawLine(penBig, (float)x, (float)rc.Bottom, (float)x, (float)rc.Top);
                x += gridSpacing;
            }

            while (y < rc.Top)
            {
                graphics.DrawLine(penBig, (float)rc.Left, (float)y, (float)rc.Right, (float)y);
                y += gridSpacing;
            }
        }
        #endregion

        #region Paint events
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                if ((viewPort.Width == 0) || (viewPort.Height == 0) || (bmpSonar == null))
                    return;

                // Get graphics object.
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw bitmap.
                if ((Width != bmpSonar.Width) || (Height != bmpSonar.Height))
                    g.DrawImage(bmpSonar, 0, 0, Width, Height);
                else
                    g.DrawImageUnscaled(bmpSonar, 0, 0, bmpSonar.Width, bmpSonar.Height);

                // Set the drawing transformation.
                Matrix t = g.Transform;
                double scaleFactor = (double)this.Width / viewPort.Width;
                g.ScaleTransform((float)scaleFactor, -(float)scaleFactor);
                g.TranslateTransform((float)(viewPort.Width / 2.0), -(float)(viewPort.Height / 2.0));

                // Draw markers.
                DrawMarkers(g, viewPort, scaleFactor);

                // Draw blanklines.
                if (showBlankLines)
                    foreach (BlankLine blankline in project.BlankLines)
                        if (blankline.ShowInTrace)
                            DrawBlankLine(g, viewPort, scaleFactor, blankline);

                // Revert to the standard transformation matrix.
                g.Transform = t;

                // Draw measurement line.
                if (interactionMode == InteractionModes.MeasureEnd)
                    g.DrawLine(new Pen(this.ForeColor, 2), ptStart, PointToClient(MousePosition));

                // Draw selection.
                DrawSelection(g);

                // Draw border.
                switch (borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        g.DrawRectangle(SystemPens.WindowFrame, 0, 0, Width - 1, Height - 1);
                        break;
                    case BorderStyle.Fixed3D:
                        g.DrawLine(SystemPens.ControlDark, 0, 0, Width - 1, 0);
                        g.DrawLine(SystemPens.ControlDark, 0, 0, 0, Height - 1);
                        g.DrawLine(SystemPens.ControlLightLight, Width - 1, 0, Width - 1, Height - 1);
                        g.DrawLine(SystemPens.ControlLightLight, 0, Height - 1, Width - 1, Height - 1);
                        break;
                }

                if (OnGridChanged != null)
                    OnGridChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.OnPaint: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustViewPortAspectRatio();
            ResetChangeTimer();
            Invalidate();
        }
        #endregion

        #region Keyboard events
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }
        #endregion

        #region Selection
        public void SelectRegion()
        {
            this.ViewPort = new RectangleD(ScreenToRegion(ptStart), ScreenToRegion(ptEnd));
        }

        public void DrawSelection(Graphics graphics)
        {
            if (!selectMode)
                return;

            Region reg = new Region(new Rectangle(0, 0, this.Size.Width, this.Size.Height));
            int x, y;

            if (ptStart.X < ptEnd.X)
                x = ptStart.X;
            else
                x = ptEnd.X;

            if (ptStart.Y < ptEnd.Y)
                y = ptStart.Y;
            else
                y = ptEnd.Y;

            Rectangle rc = new Rectangle(x, y, Math.Abs(ptEnd.X - ptStart.X), Math.Abs(ptEnd.Y - ptStart.Y));
            reg.Xor(rc);
            graphics.FillRegion(new SolidBrush(Color.FromArgb(GSC.Settings.CS.AlphaChannel, Color.Black)), reg);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.DrawRectangle(new Pen(Color.Black), rc);
        }

        public void FindLine(double rv, double hv)
        {
            int max;
            SonarDevice device;
            SonarLine line;
            double d;

            double min = double.MaxValue;
            int rec = -1;
            int dev = -1;
            int lin = -1;

            try
            {
                for (int i = 0; i < project.RecordCount; i++)
                {
                    for (int j = 0; j < project.Record(i).DeviceCount; j++)
                    {
                        device = project.Record(i).Devices[j];

                        if (!device.ShowInTrace)
                            continue;

                        max = device.SonarLines.Count;
                        for (int k = 0; k < max; k++)
                        {
                            line = device.SonarLine(k);
                            if (line.CoordRvHv.Type != CoordinateType.TransverseMercator)
                                continue;
                            if (line.CoordRvHv.Point.IsZero)
                                continue;
                            d = line.CoordRvHv.Point.Distance(rv, hv);
                            if (d > min)
                                continue;
                            min = d;
                            rec = i;
                            dev = j;
                            lin = k;
                        }
                    }
                }

                if ((rec > -1) && (dev > -1) && (lin > -1))
                {
                    SonarRecord record = project.Record(rec);
                    device = record.Devices[dev];
                    GlobalNotifier.Invoke(this, new RecordEventArgs(record, device, device.SonarLine(lin)), GlobalNotifier.MsgTypes.LineFound);
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.FindLine: " + e.Message);
            }
        }
        #endregion

        #region Mouse events
        protected void OnLMButton(MouseEventArgs e)
        {
            bool shiftHold = (ModifierKeys & Keys.Shift) != Keys.None;
            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            // Coordinate calculation
            PointD pt = ScreenToRegion(new Point(e.X, e.Y));

            switch (interactionMode)
            {
                case InteractionModes.EditBlankLine:
                    #region EditBlankLine
                    if (project.EditBlankLine == null)
                        break;

                    if (this.Cursor == Cursors.Hand)
                    {
                        // A node is selected.
                        if (ctrlHold)
                        {
                            // Delete the selected node.
                            if (blankLinePointIndex != -1)
                                project.EditBlankLine.Poly.Points.RemoveAt(blankLinePointIndex);
                        }
                        else if (altHold)
                        {
                            // "Take" the selected node.
                            if (blankLinePointIndex != -1)
                            {
                                project.EditBlankLine.Poly.Points[blankLinePointIndex] = pt;
                                ptBLGrabbed = true;
                            }
                        }
                        else if (blankLineSelectIndex != -1)
                        {
                            // Insert a new point and "take" it.
                            project.EditBlankLine.Poly.Points.Insert(blankLineSelectIndex, pt);
                            blankLinePointIndex = blankLineSelectIndex;
                            ptBLGrabbed = true;
                        }
                    }
                    else
                    {
                        if (ctrlHold)
                        {
                            // Zoom box...
                            ptEnd = ptStart = new Point(e.X, e.Y);
                            selectMode = true;
                        }
                        else
                            project.EditBlankLine.Poly.Points.Add(pt);
                    }
                    Invalidate();
                    #endregion
                    break;

                case InteractionModes.None:
                    #region None
                    if (ctrlHold)
                    {
                        // Zoom box...
                        ptEnd = ptStart = new Point(e.X, e.Y);
                        selectMode = true;
                    }
                    else if (altHold || shiftHold)
                    {
                        ptStart = new Point(e.X, e.Y);
                        this.Cursor = Cursors.Hand;
                    }
                    #endregion
                    break;

                case InteractionModes.Find:
                    #region Find
                    this.Cursor = Cursors.WaitCursor;
                    FindLine(pt.X, pt.Y);
                    this.Cursor = Cursors.Default;
                    interactionMode = InteractionModes.None;
                    #endregion
                    break;

                case InteractionModes.MeasureStart:
                    #region MeasureStart
                    ptStart = new Point(e.X, e.Y);
                    interactionMode = InteractionModes.MeasureEnd;
                    this.Cursor = Cursors.Cross;
                    #endregion
                    break;

                case InteractionModes.MeasureEnd:
                    #region MeasureEnd
                    interactionMode = InteractionModes.None;

                    PointD pt1 = ScreenToRegion(ptStart);
                    PointD pt2 = ScreenToRegion(new Point(e.X, e.Y));
                    int backup = GSC.Settings.NFI.NumberDecimalDigits;
                    GSC.Settings.NFI.NumberDecimalDigits = 3;

                    string msg = "";
                    msg += "?RV: " + (pt2.X - pt1.X).ToString("f", GSC.Settings.NFI) + "m\n";
                    msg += "?HV: " + (pt2.Y - pt1.Y).ToString("f", GSC.Settings.NFI) + "m\n\n";
                    msg += "Distance: " + pt2.Distance(pt1).ToString("f", GSC.Settings.NFI) + "m";

                    GSC.Settings.NFI.NumberDecimalDigits = backup;

                    MessageBox.Show(msg, "Measure result:");

                    this.Cursor = Cursors.Default;
                    toolTip.Active = false;
                    Invalidate();
                    #endregion
                    break;

                case InteractionModes.PlaceBuoy:
                    #region PlaceBuoy
                    Buoy buoy = new Buoy();

                    buoy.RV = pt.X;
                    buoy.HV = pt.Y;

                    project.BuoyList.Add(buoy);
                    Invalidate();

                    this.Cursor = Cursors.Default;
                    GSC.Settings.Changed = true;
                    interactionMode = InteractionModes.None;
                    #endregion
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Coordinate calculation
            PointD pt = ScreenToRegion(new Point(e.X, e.Y));

            switch (e.Button)
            {
                case MouseButtons.Left:
                    OnLMButton(e);
                    break;

                case MouseButtons.Middle:
                    ptStart = new Point(e.X, e.Y);
                    this.Cursor = Cursors.Hand;
                    break;

                case MouseButtons.Right:
                    interactionMode = InteractionModes.None;
                    this.Cursor = Cursors.Default;
                    toolTip.Active = false;

                    int max = project.BuoyConnectionList.Count;
                    lockedConnection = null;

                    for (int i = 0; i < max; i++)
                    {
                        BuoyConnection c = project.BuoyConnectionList[i] as BuoyConnection;

                        c.Highlighted = false;

                        if (c.IsInCorridor(pt))
                        {
                            lockedConnection = c;
                            c.Highlighted = true;
                            project.SelectedCorridor = c;
                        }
                    }

                    Invalidate();
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool shiftHold = (ModifierKeys & Keys.Shift) != Keys.None;
            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            // Coordinate calculation
            PointD pt = ScreenToRegion(new Point(e.X, e.Y));

            if (interactionMode == InteractionModes.MeasureEnd)
            {
                #region Measure and update tool tip.
                PointD pt1 = ScreenToRegion(ptStart);
                PointD pt2 = ScreenToRegion(new Point(e.X, e.Y));

                toolTip.SetToolTip(this, "Distance: " + pt1.Distance(pt2).ToString("f", GSC.Settings.NFI));
                toolTip.Active = true;

                Invalidate();
                #endregion
            }
            else if (selectMode)
            {
                #region Update frame extension in select mode.
                ptEnd = new Point(e.X, e.Y);
                Invalidate();
                #endregion
            }
            else if ((e.Button == MouseButtons.Middle) || ((e.Button == MouseButtons.Left) && shiftHold))
            {
                #region Update viewable region in drag mode.
                double dx, dy;

                dx = (e.X - ptStart.X) * viewPort.Width / (this.Width - 2 * borderX);
                dy = (ptStart.Y - e.Y) * viewPort.Height / (this.Height - 2 * borderY);

                viewPort.Offset((new PointD(-dx, -dy)).Rotate(-viewAngle));

                this.ViewPort = viewPort;

                ptStart = new Point(e.X, e.Y);
                #endregion
            }
            else if ((e.Button == MouseButtons.Left) && altHold && (interactionMode != InteractionModes.EditBlankLine))
            {
                #region Update viewable region in rotation mode.
                PointD pt1 = new PointD(e.X, e.Y);
                PointD pt2 = new PointD(ptStart);
                PointD ptO = new PointD((double)this.Width / 2.0, (double)this.Height / 2.0);

                ptStart = new Point(e.X, e.Y);

                if (pt1.Equals(ptO) || pt2.Equals(ptO))
                    return;

                double a1 = pt1.AngleDeg(ptO);
                double a2 = pt2.AngleDeg(ptO);

                this.ViewAngle += (a2 - a1);
                #endregion
            }
            else if ((interactionMode == InteractionModes.EditBlankLine) && (project.EditBlankLine != null))
            {
                if (ptBLGrabbed)
                {
                    #region Move the selected point.
                    project.EditBlankLine.Poly.Points[blankLinePointIndex] = pt;
                    Invalidate();
                    #endregion
                }
                else
                {
                    #region Select blank line connection.
                    int i, max = project.EditBlankLine.Poly.Count;

                    PointD pt1, pt2, ptM = new PointD(PointToClient(new Point(MousePosition.X, MousePosition.Y)));
                    LineD line;
                    double d, dNearest = double.MaxValue;
                    double w = this.Width - 2 * borderX;
                    double h = this.Height - 2 * borderY;

                    int oldIndex = blankLineSelectIndex;
                    blankLineSelectIndex = -1;

                    System.Windows.Forms.Cursor cur = Cursors.Default;

                    for (i = 1; i <= max; i++)
                    {
                        pt1 = project.EditBlankLine.Poly.Points[i - 1];
                        pt2 = project.EditBlankLine.Poly.Points[(i == max) ? 0 : i];

                        if (altHold | ctrlHold)
                        {
                            d = pt.Distance(pt1);

                            if ((d < dNearest) && (d < 5.0))
                            {
                                dNearest = d;
                                blankLinePointIndex = i - 1;
                                cur = Cursors.Hand;
                            }
                        }
                        else
                        {
                            line = new LineD(pt1, pt2);
                            d = Math.Abs(line.PointDistance(pt));

                            if ((d < dNearest) && (d < 25.0) && line.Inside(pt))
                            {
                                dNearest = d;
                                blankLineSelectIndex = i;

                                ptM = (pt1 + pt2) / 2.0;

                                if (pt.Distance(ptM) < 5.0)
                                    cur = Cursors.Hand;
                            }
                        }
                    }

                    if (this.Cursor != cur)
                        this.Cursor = cur;

                    if (oldIndex != blankLineSelectIndex)
                        Invalidate();
                    #endregion
                }
            }
            else
            {
                #region Update tool tip.
                double min = double.MaxValue;
                double d, d2;
                string s = "";
                int i, max = project.BuoyList.Count;

                for (i = 0; i < max; i++)
                {
                    Buoy b = project.BuoyList[i] as Buoy;

                    Coordinate c = b.Coord;

                    if (c.Type == CoordinateType.Elliptic)
                        c = GSC.Settings.InversTransform.Run(c, CoordinateType.TransverseMercator);

                    d = c.Point.Distance(pt);
                    d2 = RegionToScreen(c.Point).Distance((double)e.X, (double)e.Y);

                    if ((d < min) && (d2 < 20))
                    {
                        min = d;
                        s = "Buoy [" + b.ID + "]";
                        if (b.Description.Length > 0)
                            s += " - " + b.Description;
                    }
                }

                if (s.Length > 0)
                    max = 0;
                else
                    max = project.RecordCount;

                for (i = 0; i < max; i++)
                {
                    SonarRecord record = project.Record(i);

                    if (record.Devices.Count == 0)
                        continue;

                    SonarDevice device = record.Devices[0];

                    if (!record.ShowInTrace || (device.SonarLines.Count < 2))
                        continue;

                    SonarLine line = device.GetFirstLineWithCoord();
                    if (line != null)
                    {
                        Coordinate c = line.CoordRvHv;
                        d = c.Point.Distance(pt);
                        d2 = RegionToScreen(c.Point).Distance((double)e.X, (double)e.Y);

                        if ((d < min) && (d2 < 10))
                        {
                            min = d;
                            s = "Start of: " + record.Description;
                        }
                    }

                    line = device.GetLastLineWithCoord();
                    if (line != null)
                    {
                        Coordinate c = line.CoordRvHv;
                        d = c.Point.Distance(pt);
                        d2 = RegionToScreen(c.Point).Distance((double)e.X, (double)e.Y);

                        if ((d < min) && (d2 < 10))
                        {
                            min = d;
                            s = "End of: " + record.Description;
                        }
                    }
                }

                if (s.Length > 0)
                    max = 0;
                else
                    max = project.BuoyConnectionList.Count;

                for (i = 0; i < max; i++)
                {
                    BuoyConnection c = project.BuoyConnectionList[i] as BuoyConnection;

                    if (c.IsInCorridor(pt))
                    {
                        s = "Connection [" + c.ID + "]";
                        if (c.Description.Length > 0)
                            s += " - " + c.Description;
                    }
                }

                if (s.Length > 0)
                {
                    toolTip.SetToolTip(this, s);
                    toolTip.Active = true;
                }
                else
                    toolTip.Active = false;
                #endregion
            }

            Coordinate coord = new Coordinate(pt.X, pt.Y, 0, CoordinateType.TransverseMercator);
            CoordEventArgs e2 = new CoordEventArgs(coord);
            if (CoordChange != null)
                CoordChange(this, e2);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (selectMode)
            {
                ptEnd = new Point(e.X, e.Y);
                SelectRegion();
                selectMode = !selectMode;
                Invalidate();
            }

            ptBLGrabbed = false;

            if (interactionMode != InteractionModes.MeasureEnd)
                this.Cursor = Cursors.Default;
        }

        public void InvokeOnMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            if (ctrlHold)
            {
                double x, y, w, h, dx, dy;

                if ((viewPort.Width == 0) ||
                    (viewPort.Height == 0))
                    return;

                x = (e.X - borderX) * viewPort.Width / (this.Width - 2 * borderX);
                y = (e.Y - borderY) * viewPort.Height / (this.Height - 2 * borderY);
                w = viewPort.Width;
                h = viewPort.Height;

                dx = w / ((double)e.Delta / 120.0 * wheelSpeed);
                dy = dx * viewPort.Height / viewPort.Width;

                viewPort.Left += x * (dx / w);
                viewPort.Right = viewPort.Left + (w - dx);

                viewPort.Top -= y * (dy / h);
                viewPort.Bottom = viewPort.Top - (h - dy);

                this.ViewPort = viewPort;
            }
            else if (altHold)
            {
                this.ViewAngle = (double)((int)(this.ViewAngle / 5.0) + Math.Sign(e.Delta)) * 5.0;
            }
        }
        #endregion

        #region Work line
        public void MoveWorkLine(bool back, bool fast, bool sendEvent)
        {
            try
            {
                #region Old work line validation
                if (project.RecordCount == 0)
                    return;

                if (workLineRec == null)
                    workLineRec = project.Record(0);

                if (workLineRec.DeviceCount == 0)
                    return;

                if (workLineDev == null)
                    workLineDev = workLineRec.Devices[0];

                if (workLineDev.SonarLines.Count == 0)
                    return;

                if (workLine == null)
                    workLine = workLineDev.SonarLine(0);
                #endregion

                if (fast)
                {
                    MoveWorkLine(back, false, false);
                    MoveWorkLine(back, false, false);
                    MoveWorkLine(back, false, false);
                    MoveWorkLine(back, false, false);
                    MoveWorkLine(back, false, sendEvent);
                    return;
                }

                int i = workLineDev.IndexOf(workLine);
                int j = workLineRec.IndexOf(workLineDev);
                int k = project.IndexOf(workLineRec);

                if (back)
                {
                    if (i == 0)
                    {
                        if (k == 0)
                            k = project.RecordCount - 1;
                        else
                            k--;

                        workLineRec = project.Record(k);
                        workLineDev = workLineRec.Devices[j];
                        i = workLineDev.SonarLines.Count - 1;
                    }
                    else
                        i--;
                }
                else
                {
                    if (i == workLineDev.SonarLines.Count - 1)
                    {
                        if (k == project.RecordCount - 1)
                            k = 0;
                        else
                            k++;

                        workLineRec = project.Record(k);
                        workLineDev = workLineRec.Devices[j];
                        i = 0;
                    }
                    else
                        i++;
                }

                workLine = workLineDev.SonarLine(i);
                SonarLine line = workLine;

                if (sendEvent)
                {
                    SonarRecord record = project.Record(k);
                    SonarDevice device = record.Devices[j];
                    GlobalNotifier.Invoke(this, new RecordEventArgs(record, device, line), GlobalNotifier.MsgTypes.WorkLineChanged);
                }

                Invalidate();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.MoveWorkLine: " + e.Message);
            }
        }

        public void SwitchWorkLineDev(bool back)
        {
            try
            {
                #region Old work line validation
                if (project.RecordCount == 0)
                    return;

                if (workLineRec == null)
                    workLineRec = project.Record(0);

                if (workLineRec.DeviceCount == 0)
                    return;

                if (workLineDev == null)
                    workLineDev = workLineRec.Devices[0];

                if (workLineDev.SonarLines.Count == 0)
                    return;

                if (workLine == null)
                    workLine = workLineDev.SonarLine(0);
                #endregion

                int i = workLineDev.IndexOf(workLine);
                int n = workLineRec.SonarLines().IndexOf(workLine);
                int j = workLineRec.IndexOf(workLineDev);
                int k = project.IndexOf(workLineRec);

                int prev = n;
                int next = n;
                int m;

                if (back)
                {
                    if (j == 0)
                        j = workLineRec.DeviceCount - 1;
                    else
                        j--;
                }
                else
                {
                    if (j == workLineRec.DeviceCount - 1)
                        j = 0;
                    else
                        j++;
                }

                workLineDev = workLineRec.Devices[j];

                for (m = n; m >= 0; m--)
                {
                    if (workLineRec.SonarLine(m).SonID != j)
                        continue;

                    prev = m;
                    break;
                }

                for (m = n; m < workLineRec.SonarLines().Count; m++)
                {
                    if (workLineRec.SonarLine(m).SonID != j)
                        continue;

                    next = m;
                    break;
                }

                if ((prev == n) || (m - prev > next - m))
                    i = workLineDev.IndexOf(workLineRec.SonarLine(next));
                else
                    i = workLineDev.IndexOf(workLineRec.SonarLine(prev));

                workLine = workLineDev.SonarLine(i);
                SonarLine line = workLine;

                SonarRecord record = project.Record(k);
                SonarDevice device = record.Devices[j];
                GlobalNotifier.Invoke(this, new RecordEventArgs(record, device, line), GlobalNotifier.MsgTypes.WorkLineChanged);

                Invalidate();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.SwitchWorkLineDev: " + e.Message);
            }
        }

        private void OnWorkLineChanged(object sender, object args)
        {
            try
            {
                if (sender == this)
                    return;

                RecordEventArgs e = args as RecordEventArgs;

                if ((e.Rec == null) || (e.Dev == null))
                    return;

                workLineRec = e.Rec;
                workLineDev = e.Dev;

                if (workLineDev.ShowInTrace)
                    workLine = e.Tag as SonarLine;
                else
                    workLine = null;

                ResetChangeTimer();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar2DView.OnWorkLineChanged: " + e.Message);
            }
        }
        #endregion

        #region Global event handler
        private void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    OnWorkLineChanged(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.ToggleDXFFile:
                    RedrawDXFBmp();
                    break;
                case GlobalNotifier.MsgTypes.ToggleBlankLine:
                    ResetChangeTimer();
                    break;
                case GlobalNotifier.MsgTypes.PlaceBuoy:
                case GlobalNotifier.MsgTypes.CutEvent:
                    ResetChangeTimer();
                    break;
                case GlobalNotifier.MsgTypes.PlaceManualPoint:
                    trace.Add((args as ManualPoint).Coord);
                    ResetChangeTimer();
                    break;
                case GlobalNotifier.MsgTypes.NewCoordinate:
                    try
                    {
                        if (project.Tracking)
                            ResetChangeTimer();
                    }
                    catch (Exception ex)
                    {
                        DebugClass.SendDebugLine(this, DebugLevel.Red, "NewCoordinate @ Sonar2DView : " + ex.Message);
                    }
                    break;
                case GlobalNotifier.MsgTypes.UpdateCoordinates:
                    if (!project.Recording)
                        return;
                    trace.Add((List<SonarLine>)args);
                    ResetChangeTimer();
                    break;
            }
        }

        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            bool all = e.PropertyName == "All";

            if (e.PropertyName.StartsWith("CS.") || all)
            {
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;
            }

            if ((e.PropertyName == "WheelSpeed") || all)
                wheelSpeed = GSC.Settings.WheelSpeed;

            if ((e.PropertyName == "TraceSize") || all)
                RebuildTraceArray();

            // Unclear: A lot of global settings properties affect 2D drawing behaviour - too much overhead to test each one?
            ResetChangeTimer();
        }
        #endregion
    }
}
