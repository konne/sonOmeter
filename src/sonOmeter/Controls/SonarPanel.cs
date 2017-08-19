using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Globalization;
using UKLib.MathEx;
using UKLib.Survey.Math;
using sonOmeter.Classes;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;
using UKLib.Debug;
using System.Collections.Generic;
using UKLib.Arrays;
using UKLib;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for SonarPanel.
    /// </summary>
    public class SonarPanel : System.Windows.Forms.UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #region Construction and dispose
        public SonarPanel()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();                                   

            this.ForeColor = GSC.Settings.CS.ForeColor;
            this.BackColor = GSC.Settings.CS.BackColor;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            RedrawSonarBmp();
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
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // SonarPanel
            // 
            this.BackColor = System.Drawing.Color.Blue;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SonarPanel";
            this.Size = new System.Drawing.Size(232, 184);

        }
        #endregion

        private class BWResult
        {
            public Bitmap bmp;
            public bool tryKeepAppend;
        }

        #region Variables
        SonarDepthMeter depthMeter = null;
        SonarPanelType panelType = SonarPanelType.Void;
        BorderStyle borderStyle = BorderStyle.None;
        List<SonarLine> sonarLines = null;
        List<Marker> markers = new List<Marker>();
        SlideBar depthSlideBar = null;
        SlideBar fileSlideBar = null;
        IDList<Sonar2DElement> buoyList = null;
        IDList<ManualPoint> pointList = null;

        float depthZoomHi = 0.0F;
        float depthZoomLo = -100.0F;

        SonarLine workLine = null;
        int workLineWidth = 48;
        int workLinePos = 48;
        int bottomWidth = 0;
        int selIndex = -1;
        int startPtr = 0;
        int startLine = 0;

        bool isCut = true;
        bool showPos = false;
        bool showVol = false;
        bool showRul = false;
        bool showDepthLines = false;
        bool ctrlHold = false;
        bool altHold = false;
        bool shiftHold = false;
        bool selectMode = false;
        bool recording = false;
        PlaceMode placeSomething = PlaceMode.Nothing;

        bool showSurfLHF = false;
        bool showCalcLHF = false;
        bool showSurfLNF = false;
        bool showCalcLNF = false;

        Bitmap bmpSonar = null;

        CutLineSet clSetHF = null;
        CutLineSet clSetNF = null;

        Point ptStart = new Point(0, 0);
        Point ptEnd = new Point(0, 0);

        CutMode cutMode = CutMode.Nothing;
        CutMouseMode cutMouseMode = CutMouseMode.Normal;

        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        // Draw variables
        BackgroundWorker<bool, BWResult> bwDrawSonarBmp = null;
        Bitmap bwSonarBmp = null;
        bool bwNeedStartAgain = false;
        #endregion

        #region Properties
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
            set { panelType = value; }
        }

        [Description("The associated depth meter."), Category("Data")]
        public SonarDepthMeter DepthMeter
        {
            get { return depthMeter; }
            set { depthMeter = value; Invalidate(); }
        }

        [Description("The width of the work line in the associated depth meter."), Category("Data")]
        public int WorkLineWidth
        {
            get { return workLineWidth; }
            set { workLineWidth = value; Invalidate(); }
        }

        [Description("The position of the work line in the associated depth meter."), Category("Data")]
        public int WorkLinePos
        {
            get { return workLinePos; }
            set { workLinePos = value; Invalidate(); }
        }

        [Browsable(false)]
        public SonarLine WorkLine
        {
            get { return workLine; }
            set
            {
                workLine = value;

                int pos = RefToScreen();

                if (pos > this.Width)
                    workLine = ScreenToRef(this.Width - 1);
                else if (pos < 0)
                    workLine = null;
            }
        }

        public int BottomWidth
        {
            set { bottomWidth = value; Invalidate(); }
        }

        public bool Recording
        {
            set { recording = value; Invalidate(); }
        }

        [Browsable(false)]
        public PlaceMode PlaceSomething
        {
            get { return placeSomething; }
            set
            {
                placeSomething = value;

                if (placeSomething != PlaceMode.Nothing)
                    this.Cursor = Cursors.Cross;
                else
                    this.Cursor = Cursors.Default;
            }
        }

        public bool ShowSurfLHF
        {
            set
            {
                showSurfLHF = value;
                showDepthLines = showDepthLines | showSurfLHF;
                Invalidate();
            }
            get { return showSurfLHF; }
        }

        public bool ShowCalcLHF
        {
            set
            {
                showCalcLHF = value;
                showDepthLines = showDepthLines | showCalcLHF;
                Invalidate();
            }
            get { return showCalcLHF; }
        }


        public bool ShowSurfLNF
        {
            set
            {
                showSurfLNF = value;
                showDepthLines = showDepthLines | showSurfLNF;
                Invalidate();
            }
            get { return showSurfLNF; }
        }

        public bool ShowCalcLNF
        {
            set
            {
                showCalcLNF = value;
                showDepthLines = showDepthLines | showCalcLNF;
                Invalidate();
            }
            get { return showCalcLNF; }
        }

        [Browsable(false), DefaultValue(CutMode.Nothing)]
        public CutMode CutMode
        {
            set { cutMode = value; }
            get { return cutMode; }
        }

        [Browsable(false)]
        public CutLineSet ClSetHF
        {
            set { clSetHF = value; }
        }

        [Browsable(false)]
        public CutLineSet ClSetNF
        {
            set { clSetNF = value; }
        }

        public SlideBar FileSlideBar
        {
            get { return fileSlideBar; }
            set { fileSlideBar = value; }
        }

        public SlideBar DepthSlideBar
        {
            get { return depthSlideBar; }
            set { depthSlideBar = value; }
        }

        public float DepthZoomHi
        {
            set { depthZoomHi = value; }
        }

        public float DepthZoomLo
        {
            set { depthZoomLo = value; }
        }

        public bool IsCut
        {
            set { isCut = value; }
            get { return isCut; }
        }

        public bool ShowPos
        {
            set { showPos = value; }
            get { return showPos; }
        }

        public bool ShowVol
        {
            set { showVol = value; }
            get { return showVol; }
        }

        public bool ShowRul
        {
            set { showRul = value; }
            get { return showRul; }
        }

        [Browsable(false)]
        public bool CtrlHold
        {
            get { return ctrlHold; }
            set
            {
                if (ctrlHold != value)
                    selIndex = -1;

                ctrlHold = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public bool AltHold
        {
            get { return altHold; }
            set
            {
                if (altHold != value)
                    selIndex = -1;

                altHold = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public bool ShiftHold
        {
            get { return shiftHold; }
            set
            {
                if (shiftHold != value)
                    selIndex = -1;

                shiftHold = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public CutMouseMode CutMouseMode
        {
            get { return cutMouseMode; }
            set { cutMouseMode = value; }
        }

        public void ToggleDepthLines()
        {
            showDepthLines = !showDepthLines;
            Invalidate();
        }
        #endregion

        #region Paint structs
        struct PanelProp
        {
            public float w;
            public float h;
            public float high;
            public float low;
            public int begin;
            public int end;
            public int mid;
            public int pos;
        }
        #endregion

        #region Init and helper functions
        private PanelProp GetProp()
        {
            PanelProp pp;

            pp.w = (float)(Width / fileSlideBar.SectionWidth);
            pp.h = (float)((Height - bottomWidth) / (depthZoomHi - depthZoomLo));
            pp.high = depthZoomHi;
            pp.low = depthZoomLo;
            pp.begin = (int)fileSlideBar.SectionStart;
            pp.end = (int)fileSlideBar.SectionEnd;
            pp.mid = (pp.end + pp.begin) >> 1;
            pp.pos = 0;

            if (pp.begin < 0)
                pp.begin = 0;

            if (sonarLines == null)
                return pp;

            if (pp.end >= sonarLines.Count)
                pp.end = sonarLines.Count - 1;

            pp.mid = (pp.end + pp.begin) >> 1;

            return pp;
        }

        private RectArrayF[] InitRectArray()
        {
            int count = GSC.Settings.SECL.Count;
            RectArrayF[] array = new RectArrayF[2 * count + 4];

            for (int i = 0; i < count; i++)
            {
                array[i] = new RectArrayF(GSC.Settings.SECL[i].SonarColor);
                array[i + count] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.SECL[i].SonarColor));
            }

            array[2 * count] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.CS.PosLineColor));
            array[2 * count + 1] = new RectArrayF(GSC.Settings.CS.ArchNoDataColor);
            array[2 * count + 2] = new RectArrayF(GSC.Settings.CS.ManualPointColor);
            array[2 * count + 3] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.CS.PosLineColorUnused));
            
            return array;
        }

        private LineData GetData(int i)
        {
            if ((i < 0) && (i >= sonarLines.Count))
                return null;

            SonarLine line = sonarLines[i];
            LineData data;

            if (panelType == SonarPanelType.HF)
                data = line.HF;
            else
                data = line.NF;

            return data;
        }

        public CutLine GetCutLine()
        {
            return GetCutLine(false, cutMode);
        }

        public CutLine GetCutLine(bool invPanelType, CutMode mode)
        {
            CutLineSet clSet = null;

            switch (panelType)
            {
                case SonarPanelType.HF:
                    if (invPanelType)
                        clSet = clSetNF;
                    else
                        clSet = clSetHF;
                    break;
                case SonarPanelType.NF:
                    if (invPanelType)
                        clSet = clSetHF;
                    else
                        clSet = clSetNF;
                    break;
                default:
                    return null;
            }

            switch (mode)
            {
                case CutMode.Top:
                    return clSet.CutTop;
                case CutMode.Bottom:
                    return clSet.CutBottom;
                case CutMode.Surface:
                    return clSet.CutSurface;
                case CutMode.CDepth:
                    return clSet.CutCalcDepth;
                default:
                    return null;
            }
        }

        public void RefreshLists(List<SonarLine> lines, IDList<Sonar2DElement> buoys, IDList<ManualPoint> points)
        {
            this.buoyList = buoys;
            this.sonarLines = lines;
            this.pointList = points;

            markers.Clear();

            Marker m;
            int i, max;

            max = points.Count;

            for (i = 0; i < max; i++)
            {
                m = points[i] as ManualPoint;

                if ((m.Tag is SonarLine) && (sonarLines.Contains(m.Tag as SonarLine)))
                    markers.Add(m);
            }

            max = buoyList.Count;

            for (i = 0; i < max; i++)
            {
                m = buoyList[i] as Buoy;

                if ((m.Tag is SonarLine) && (sonarLines.Contains(m.Tag as SonarLine)))
                    markers.Add(m);
            }
        }

        public void AddMarker(Marker m)
        {
            markers.Add(m);
        }
        #endregion

        #region Workline transformation
        public int RefToScreen()
        {
            if (sonarLines == null)
                return -1;

            PanelProp pp = GetProp();
            int pos = sonarLines.IndexOf(workLine);

            if (pos == -1)
                return -1;

            return Convert.ToInt32((pos - pp.begin) * this.Width / fileSlideBar.SectionWidth);
        }

        public SonarLine ScreenToRef(int pos)
        {
            if ((this.Width <= 0) || (pos < 0) || (sonarLines == null))
                return null;

            PanelProp pp = GetProp();

            int n = Convert.ToInt32(fileSlideBar.SectionWidth * pos / this.Width) + pp.begin;

            if (n > sonarLines.Count - 1)
                n = sonarLines.Count - 1;

            return sonarLines[n];
        }
        #endregion

        #region Drawing functions
        #region General drawing
        private void AddLine(bool active, int ctrlHeight, int i, RectArrayF[] rcArray, PanelProp pp, bool ignorePos, bool showSubmarine)
        {
            SonarLine line = (SonarLine)sonarLines[i];
            line.IsCut = isCut;
            LineData data = GetData(i);
            int colorCount = GSC.Settings.SECL.Count;
            int j;

            if (data.Entries == null)
                return;

            if (showPos && !ignorePos)
            {
                bool added = false;
                bool unused = false;

                foreach (SonarPos pos in line.PosList)
                {
                    if (!pos.RealPos)
                        continue;

                    if ((line.PosList.Count > 0) && !added)
                    {
                        if (!pos.Used || pos.Disabled)
                            unused = true;
                        else
                        {
                            added = true;
                            rcArray[2 * colorCount].List.Add(new RectangleF(pp.pos * pp.w, 1, pp.w, Height - 2 - bottomWidth));
                            break;
                        }
                    }
                }

                if (unused && !added)
                    rcArray[2 * colorCount + 3].List.Add(new RectangleF(pp.pos * pp.w, 1, pp.w, Height - 2 - bottomWidth));
            }

            float y;
            float w;
            float h;
            int max = data.Entries.Length;

            // UBOOT
            float depth = sonarLines[i].SubDepth;
            w = Math.Min(pp.w * GSC.Settings.SubmarineDepthMarker / 3.0F, 20.0F);
            h = w * (0.5F);
            y = -pp.h * (-depth - depthZoomHi) - h / 2.0F;
            if ((i % GSC.Settings.SubmarineDepthMarker == 0) && GSC.Settings.Lic[Module.Modules.Submarine] && showSubmarine)
                rcArray[2 * colorCount + 2].List.Add(new RectangleF(pp.pos * pp.w, y, w, h));
            // UBOOT
            
            if (isCut)
            {
                if (depthZoomHi > data.TCut)
                    pp.high = data.TCut;
                if (depthZoomLo < data.BCut)
                    pp.low = data.BCut;
            }

            //if (active && (data.LastCol > -1))
            if (active && (data.LastCol > -2))
            {
                y = -pp.h * (data.Depth - depthZoomHi);
                h = pp.h * (0.1F);

                if (data.LastCol == -1)
                    j = 2 * colorCount + 1;
                else
                    j = data.LastCol;

                rcArray[j].List.Add(new RectangleF(pp.pos * pp.w, y, pp.w, h));
            }

            for (j = 0; j < max; j++)
            {
                DataEntry entry = data.Entries[j];

                // UBOOT
                entry.high -= depth;
                entry.low -= depth;
                entry.uncutHigh -= depth;
                // UBOOT

                float high = isCut ? entry.high : entry.uncutHigh;

                if ((!entry.visible && isCut) || (entry.low > pp.high) || (high < pp.low))
                    continue;

                if (active && (entry.low > data.Depth))
                    DrawEntry(active, ctrlHeight, rcArray, pp, entry, data.Depth, colorCount);
                else
                {
                    if (active && (high > data.Depth))
                        DrawEntry(active, ctrlHeight, rcArray, pp, entry, data.Depth, colorCount);

                    DrawEntry(active, ctrlHeight, rcArray, pp, entry, data.Depth, 0);
                }
            }
        }

        private void DrawEntry(bool active, int ctrlHeight, RectArrayF[] rcArray, PanelProp pp, DataEntry entry, float depth, int offset)
        {
            float y;
            float h;
            float high, low;
            RectangleF rc;

            try
            {
                high = isCut ? entry.high : entry.uncutHigh;

                if (active && (high > depth) && (entry.low <= depth) && (offset == 0))
                    high = depth;

                if (high > pp.high)
                    high = pp.high;

                if (entry.low < pp.low)
                    low = pp.low;
                else
                    low = entry.low;

                y = -pp.h * (high - depthZoomHi);
                h = pp.h * (high - low);

                if (y + h > ctrlHeight - 1 - bottomWidth)
                    h = ctrlHeight - 1 - y - bottomWidth;
                if (y < 1)
                    y = 1;

                rc = new RectangleF(pp.pos * pp.w, y, pp.w, h);

                rcArray[entry.colorID + offset].List.Add(rc);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawEntry: " + ex.Message);
            }
        }

        private void DrawArray(RectArrayF[] rcArray, Graphics graphics)
        {
            RectangleF rc = new RectangleF(0.0F, 0.0F, 1.0F, 1.0F);
            int colorCount = GSC.Settings.SECL.Count;

            for (int i = 0; i < rcArray.Length; i++)
            {
                if (rcArray[i].List.Count == 0)
                    continue;

                try
                {
                    RectangleF[] ar = rcArray[i].Array;

                    if (i == colorCount * 2 + 2)
                        for (int j = 0; j < ar.Length; j++)
                            graphics.FillEllipse(rcArray[i].Brush, ar[j]);
                    else if (i >= colorCount * 2)
                        graphics.FillRectangles(rcArray[i].Brush, ar);
                    else if (i >= colorCount)
                    {
                        if (GSC.Settings.SECL[i - colorCount].SonarColorVisible && GSC.Settings.ArchActive)
                            graphics.FillRectangles(rcArray[i].Brush, ar);
                    }
                    else if (GSC.Settings.SECL[i].SonarColorVisible)
                        graphics.FillRectangles(rcArray[i].Brush, ar);
                }
                catch (Exception ex)
                {
                    DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawArray: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        #endregion

        #region Misc drawing
        private void BuildSurfaceLines()
        {
            clSetNF.CutSurface.Clear();
            clSetHF.CutSurface.Clear();

            // Prepare and draw.
            if (sonarLines != null)
            {
                // Add each line to buf.
                int count = sonarLines.Count;
                float y;

                for (int i = 0; i < count; i++)
                {
                    y = sonarLines[i].NF.GetSurfaceHeight();
                    if (!float.IsNaN(y))
                        clSetNF.CutSurface.Add(i, y);
                    y = sonarLines[i].HF.GetSurfaceHeight();
                    if (!float.IsNaN(y))
                        clSetHF.CutSurface.Add(i, y);
                }
            }
        }

        private void DrawBottom(int i, PanelProp pp, Graphics graphics)
        {
            LineData data = GetData(i);
            int sum = (int)data.GetVolume(false);

            if (showVol)
            {
                if (sum < 0)
                    sum = 0;
                if (sum > 255)
                    sum = 255;

                graphics.FillRectangle(new SolidBrush(Color.FromArgb(sum, sum, sum)), (i - pp.begin) * pp.w, Height - bottomWidth, pp.w, bottomWidth - 1);
            }

            if (showRul && ((SonarLine)sonarLines[i]).IsMarked)
            {
                PointF[] pt = new PointF[2];
                pt[0] = new PointF((i - pp.begin) * pp.w, Height);
                pt[1] = new PointF((i - pp.begin) * pp.w, Height - bottomWidth / 2);
                graphics.DrawLines(new Pen(GSC.Settings.CS.PosTickColor), pt);
            }
        }
        #endregion

        #region Draw selection
        public void SelectRegion()
        {
            PanelProp pp = GetProp();

            int xStart, xEnd;
            double yHigh, yLow;

            if (ptStart.X < ptEnd.X)
            {
                xStart = (int)(ptStart.X / pp.w + pp.begin);
                xEnd = (int)(ptEnd.X / pp.w + pp.begin);
            }
            else
            {
                xStart = (int)(ptEnd.X / pp.w + pp.begin);
                xEnd = (int)(ptStart.X / pp.w + pp.begin);
            }

            if (ptStart.Y < ptEnd.Y)
            {
                yHigh = ptStart.Y / pp.h;
                yLow = ptEnd.Y / pp.h;
            }
            else
            {
                yHigh = ptEnd.Y / pp.h;
                yLow = ptStart.Y / pp.h;
            }

            fileSlideBar.SlideAnchor = SlideBar.SlideAnchors.End;
            fileSlideBar.SectionStart = xStart;
            fileSlideBar.SlideAnchor = SlideBar.SlideAnchors.Width;
            fileSlideBar.SectionEnd = xEnd;

            depthSlideBar.SlideAnchor = SlideBar.SlideAnchors.End;
            depthSlideBar.SectionStart = yHigh;
            depthSlideBar.SlideAnchor = SlideBar.SlideAnchors.Width;
            depthSlideBar.SectionEnd = yLow;

            depthSlideBar.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
            depthSlideBar.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
        }

        private void DrawSelected(Graphics graphics)
        {
            CutLine linePtr;
            PanelProp pp = GetProp();

            try { linePtr = GetCutLine(); }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawSelected: " + ex.Message + "\n" + ex.StackTrace);
                return;
            }

            if ((selIndex < 0) || (selIndex > linePtr.Count - 1))
                return;

            PointF[] ptList = linePtr.GetNeighbours(selIndex);
            if (ptList == null)
                return;

            for (int i = 0; i < ptList.Length; i++)
            {
                ptList[i] = new PointF((ptList[i].X - pp.begin + 0.5F) * pp.w, -pp.h * (ptList[i].Y - depthZoomHi));
            }

            graphics.DrawLines(new Pen(GSC.Settings.CS.CutLineColor, 4), ptList);
        }

        private void DrawSelectRegion(Graphics graphics)
        {
            Region reg = new Region(new Rectangle(0, 0, Size.Width, Size.Height));
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
        #endregion

        #region Draw work line
        public void DrawWorkLine(bool remove, Graphics g)
        {
            try
            {
                // Filter out invisible lines.
                int pos = sonarLines.IndexOf(workLine);

                if (pos == -1)
                    return;

                // Draw line.
                PanelProp pp = GetProp();
                if (g == null)
                    g = Graphics.FromHwnd(this.Handle);

                // Draw background line and then the line rectangles
                RectangleF rc = new RectangleF((pos - pp.begin) * pp.w, 1, pp.w, Height - 2 - bottomWidth);

                if (!remove)
                {
                    g.FillRectangle(new SolidBrush(GSC.Settings.CS.WorkLineColor), rc);
                    DrawWorkLine(pos);
                    return;
                }

                g.DrawImage(bmpSonar, rc, rc, GraphicsUnit.Pixel);
                pp.pos = pp.begin - pos;

                DrawSingleLine(pos, pp, g, false, false);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawWorkLine: " + ex.Message);
            }
        }

        public void DrawWorkLine(int pos)
        {
            try
            {
                PanelProp pp = GetProp();

                // Route drawing to depth meter, if available.
                if (depthMeter == null)
                    return;

                Graphics g = depthMeter.WorkLinePaintObj;
                g.TranslateTransform(0, Location.Y);
                pp.begin = pos;
                pp.w = depthMeter.WorkLinePaintSize.Width;
                g.FillRectangle(new SolidBrush(GSC.Settings.CS.BackColor), new Rectangle(0, 1, (int)pp.w, this.Height - 2 - bottomWidth));

                DrawSingleLine(pos, pp, g, true, false);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawWorkLine:" + e.Message);
            }
        }

        private void DrawSingleLine(int pos, PanelProp pp, Graphics g, bool ignorePos, bool showSubmarine)
        {
            // Prepare brushes.
            RectArrayF[] rcArray = InitRectArray();

            // Add line to buf and draw it.
            AddLine(GSC.Settings.ArchActive, this.Height, pos, rcArray, pp, ignorePos, showSubmarine);
            DrawArray(rcArray, g);
            DrawBottom(pos, pp, g);
        }
        #endregion

        #region Draw cut line
        private void DrawCutLine(Graphics graphics, CutLine cutLine, Color color, bool fromTop, bool dim)
        {
            PanelProp pp = GetProp();
            GraphicsPath path = new GraphicsPath();
            PointF pt1, pt2;
            int count = cutLine.Count;

            try
            {
                for (int i = 1; i < count; i++)
                {
                    pt1 = new PointF((cutLine[i - 1].X - pp.begin + 0.5f) * pp.w, -pp.h * (cutLine[i - 1].Y - depthZoomHi));
                    pt2 = new PointF((cutLine[i].X - pp.begin + 0.5f) * pp.w, -pp.h * (cutLine[i].Y - depthZoomHi));

                    path.AddLine(pt1, pt2);
                }

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawPath(new Pen(color, 2), path);

                if (dim)
                {
                    if (fromTop)
                    {
                        pt1 = new PointF((cutLine[count - 1].X - pp.begin) * pp.w, -pp.h * (cutLine[count - 1].Y - depthZoomHi));
                        pt2 = new PointF(pt1.X, -pp.h * (0 - depthZoomHi));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, -pp.h * (0 - depthZoomHi));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, -pp.h * (cutLine[0].Y - depthZoomHi));
                        path.AddLine(pt1, pt2);
                    }
                    else
                    {
                        pt1 = new PointF((cutLine[count - 1].X - pp.begin) * pp.w, -pp.h * (cutLine[count - 1].Y - depthZoomHi));
                        pt2 = new PointF(pt1.X, -pp.h * ((float)GSC.Settings.DepthBottom - depthZoomHi));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, -pp.h * ((float)GSC.Settings.DepthBottom - depthZoomHi));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, -pp.h * (cutLine[0].Y - depthZoomHi));
                        path.AddLine(pt1, pt2);
                    }

                    graphics.FillPath(new SolidBrush(Color.FromArgb(GSC.Settings.CS.AlphaChannel, Color.White)), path);
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.DrawCutLine:" + e.Message);
            }
        }
        #endregion
        #endregion

        #region Paint event and bitmap refresh
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            try
            {
                PanelProp pp = GetProp();

                // Get graphics object.
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;

                // Draw bitmap.
                if (bmpSonar != null)
                {
                    if ((Width != bmpSonar.Width) || (Height != bmpSonar.Height))
                        g.DrawImage(bmpSonar, 0, 0, bmpSonar.Width, Height);
                    else
                        g.DrawImageUnscaled(bmpSonar, 0, 0, bmpSonar.Width, bmpSonar.Height);
                }
                else
                    g.Clear(this.BackColor);


                // Draw work line.
                DrawWorkLine(false, g);

                // Draw additional lines.
                if (cutMode != CutMode.Nothing)
                {
                    // Draw cut lines.
                    CutLineSet clSet;
                    CutLine linePtr;

                    if (panelType == SonarPanelType.HF)
                        clSet = clSetHF;
                    else
                        clSet = clSetNF;

                    // Draw begin cut line.
                    linePtr = clSet.CutTop;
                    if (linePtr.Count == 0)
                    {
                        linePtr.Insert(0, 0);
                        linePtr.Insert(sonarLines.Count, 0);
                    }

                    DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, true, true);

                    // Draw end cut line.
                    linePtr = clSet.CutBottom;
                    if (linePtr.Count == 0)
                    {
                        linePtr.Insert(0, (float)GSC.Settings.DepthBottom);
                        linePtr.Insert(sonarLines.Count, (float)GSC.Settings.DepthBottom);
                    }

                    DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, false, true);

                    // Draw calculated depth line.
                    if (clSet.CalcDepthAv)
                    {
                        linePtr = clSet.CutCalcDepth;
                        DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, true, true);
                    }

                    // Draw selected lines bold.
                    DrawSelected(g);
                }
                else if (showDepthLines)
                {
                    // Draw depth lines.
                    if (showSurfLHF)
                    {
                        BuildSurfaceLines();
                        DrawCutLine(g, clSetHF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false);
                    }

                    if (showSurfLNF)
                    {
                        BuildSurfaceLines();
                        DrawCutLine(g, clSetNF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false);
                    }

                    if (showCalcLHF)
                    {
                        if (clSetHF.CutCalcDepth.Count != 0)
                            DrawCutLine(g, clSetHF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false);
                    }

                    if (showCalcLNF)
                    {
                        if (clSetNF.CutCalcDepth.Count != 0)
                            DrawCutLine(g, clSetNF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false);
                    }
                }

                // Draw Buoys & Manual Points
                int max = markers.Count;
                float buoySize = (float)GSC.Settings.BuoySize;

                if (GSC.Settings.BuoySizeUnit == Buoy.BuoySizeUnit.Meters)
                {
                    buoySize *= (float)((double)this.Height / (depthZoomHi - depthZoomLo));
                }

                for (int i = 0; i < max; i++)
                {
                    Marker m = markers[i];

                    // Draw it.
                    int pos = sonarLines.IndexOf(m.Tag as SonarLine);
                    float y = -pp.h * (m.Depth - depthZoomHi);

                    if ((pos < pp.begin) || (pos > pp.end))
                        continue;

                    m.Paint1D(g, (float)(pos - pp.begin) * pp.w, y, buoySize, this.Width, this.Height);
                }

                // Draw selected region.
                if (selectMode)
                {
                    DrawSelectRegion(g);
                }

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
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.OnPaint: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void RedrawSonarBmp()
        {
            RedrawSonarBmp(false);
        }

        public void RedrawSonarBmp(bool tryKeepAppend)
        {
            if (bwDrawSonarBmp == null)
            {
                bwDrawSonarBmp = new BackgroundWorker<bool, BWResult>();
                bwDrawSonarBmp.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<BWResult>>(bwDrawSonarBmp_RunWorkerCompleted);
                bwDrawSonarBmp.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, BWResult>>(bwDrawSonarBmp_DoWork);
            }
            if (!bwDrawSonarBmp.IsBusy)
            {
                bwDrawSonarBmp.RunWorkerAsync(tryKeepAppend);                
            }
            else
            {
                bwNeedStartAgain = true;
            }         
        }

        void bwDrawSonarBmp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs<BWResult> e)
        {
            if (e.Result != null)
            {
                if (e.Result.bmp != null)
                    bmpSonar = e.Result.bmp;

                Invalidate();
            }

            if (bwNeedStartAgain)
            {
                bwNeedStartAgain = false;
                if (e.Result != null)
                    RedrawSonarBmp(e.Result.tryKeepAppend);
                else
                    RedrawSonarBmp(false);
            }
        }

        private int bwLastDrawLine = -1;
        private float bwLastMoveError = 0;
        private int bwLastppBegin = -1;
        private int bwLastppEnde = -1;
        void bwDrawSonarBmp_DoWork(object sender, DoWorkEventArgs<bool, BWResult> e)
        {
            try
            {
                if ((Width <= 0) || (Height <= 0))
                    return;
                
                // rebuild if tryAppend false
                bool rebuild = !(bool)e.Argument;
            
                // Set up view mode.
                SonarViewMode viewMode = GSC.Settings.ViewMode;
                if (!recording)
                    viewMode = SonarViewMode.LeftFixed;

                // Rebuild bitmap structure if needed.
                if (bwSonarBmp == null)
                    bwSonarBmp = new Bitmap(Width, Height);
                else if ((Width != bwSonarBmp.Width) || (Height != bwSonarBmp.Height))
                {
                    rebuild = true;
                    if (bwSonarBmp != null)
                    {
                        bwSonarBmp.Dispose();
                    }
                    bwSonarBmp = new Bitmap(Width, Height);
                }

                // Get graphics object.
                Graphics g = Graphics.FromImage(bwSonarBmp);
                g.SmoothingMode = SmoothingMode.HighSpeed;

                // Set colors.
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;

                // Flush old bitmap.
                if (rebuild)
                {
                    SolidBrush brush = new SolidBrush(BackColor);
                    g.FillRectangle(brush, 0, 0, Width, Height);
                    brush.Dispose();
                }

                // Prepare and draw.
                if ((sonarLines != null) && (panelType != SonarPanelType.Void))
                {
                    PanelProp pp = GetProp();
                    int ctrlHeight = this.Height;
                    bool active = GSC.Settings.ArchActive;
                    int i;

                    // Prepare brushes.
                    RectArrayF[] rcArray = InitRectArray();

                    startPtr += pp.begin - startLine;
                    if (startPtr < 0)
                        startPtr = 0;
                    if (pp.end - pp.begin > 0)
                        startPtr %= pp.end - pp.begin;

                    int start = pp.begin;
                    bool drawLines = true;

                    if (recording)
                    {
                        if (viewMode == SonarViewMode.RadarLike)
                        {
                            if (!rebuild)
                            {
                                start = bwLastDrawLine + 1;
                                SolidBrush sbrush = new SolidBrush(BackColor);
                                g.FillRectangle(sbrush, new RectangleF(startPtr * pp.w, 1, pp.w, Height - 2 - bottomWidth));
                            }
                            else
                            {
                                startPtr = 0;
                            }
                        }
                      
                        if (viewMode == SonarViewMode.RightFixed)
                        {
                            if ((!rebuild) & (pp.begin > 0))
                            {
                                start = bwLastDrawLine;
                                Bitmap temp = (Bitmap)bwSonarBmp.Clone();

                                float move = pp.w + bwLastMoveError;
                                bwLastMoveError = move - (int)move;
                                if (move >= 1)
                                {
                                    g.DrawImageUnscaled(temp, -(int)(move), 0);
                                    SolidBrush sbrush = new SolidBrush(BackColor);

                                    g.FillRectangle(sbrush, new RectangleF(Width - (int)move, 1, pp.w, Height - 2 - bottomWidth));
                                }
                                else
                                {
                                    drawLines = false;
                                }
                                //g.DrawImage(temp, -pp.w, 0);
                                temp.Dispose();
                            }
                            else
                            {
                                startPtr = 0;
                            }
                        }
                        if (viewMode == SonarViewMode.LeftFixed)
                        {
                            if (!rebuild)
                            {
                                if (bwLastppEnde == pp.end)
                                {
                                    drawLines = false;
                                }
                                else
                                {
                                    start = bwLastDrawLine;
                                }
                            }                          

                            bwLastppBegin = pp.begin;
                            bwLastppEnde = pp.end;
                        }

                    }
                   
                    
                    
                    bwLastDrawLine = pp.end;

                    if (drawLines)
                    {
                        // Add each line to buf.
                        for (i = start; i <= pp.end; i++)
                        {
                            pp.pos = i - pp.begin;

                            if (viewMode == SonarViewMode.RadarLike)
                            {
                                pp.pos += startPtr;
                                if (pp.begin > 0)
                                    pp.pos %= pp.end - pp.begin;
                            }

                            AddLine(active, ctrlHeight, i, rcArray, pp, false, true);
                            DrawBottom(i, pp, g);
                        }

                        startLine = pp.begin;

                        // Draw all lines.
                        DrawArray(rcArray, g);

                        #region Radargradient
                        // Draw radar gradient.
                        if ((fileSlideBar != null) && (viewMode == SonarViewMode.RadarLike) && (fileSlideBar.SectionWidth <= fileSlideBar.RegionWidth))
                        {
                            Color color = GSC.Settings.CS.BackColor;

                            int radarGradient = this.Width >> 3;

                            RectangleF rect = new RectangleF((startPtr + 1) * pp.w, 1, radarGradient, Height - 2 - bottomWidth);

                            LinearGradientBrush brush = new LinearGradientBrush(
                                rect,
                                Color.FromArgb(255, color),
                                Color.FromArgb(0, color),
                                LinearGradientMode.Horizontal);

                            ColorBlend blend = new ColorBlend(radarGradient);

                            for (i = 0; i < radarGradient; i++)
                            {
                                blend.Colors[i] = Color.FromArgb(255 - (int)((double)(i * 256) / (double)radarGradient), color);
                                if (i == 0)
                                    blend.Positions[0] = 0;
                                else
                                    blend.Positions[i] = (float)(Math.Asin((double)i / (double)(radarGradient - 1) * 2.0 - 1.0) / Math.PI + 0.5);

                                if (blend.Positions[i] < 0)
                                    blend.Positions[i] = 0;
                                else if (blend.Positions[i] > 1)
                                    blend.Positions[i] = 1;
                            }

                            brush.InterpolationColors = blend;
                            g.FillRectangle(brush, rect);

                            brush.Dispose();
                        }
                        #endregion
                    }

                    rcArray = null;
                }

                // Invalidate
                g.Dispose();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.RedrawSonarBmp: " + ex.Message);
            }
            finally
            {
                // Create result.
                e.Result = new BWResult() { tryKeepAppend = (bool)e.Argument, bmp = (bwSonarBmp != null) ? (Bitmap)bwSonarBmp.Clone() : null };
            }
        }
        #endregion

        #region Moving and resizing
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        #endregion

        #region Mouse events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            PanelProp pp = GetProp();
            float y = -e.Y / pp.h + depthZoomHi;
            int pos = (int)(fileSlideBar.SectionWidth * e.X / this.Width) + pp.begin;

            if ((shiftHold) & (placeSomething == PlaceMode.Nothing))
                pos = (pos < pp.mid) ? pp.begin : pp.end;

            if (cutMode == CutMode.Nothing)
            {
                if (ctrlHold)
                {
                    ptEnd = ptStart = new Point(e.X, e.Y);
                    selectMode = true;
                }
                else if ((placeSomething != PlaceMode.Nothing) && (e.Button == MouseButtons.Left) && (pos > -1) && (pos < sonarLines.Count))
                {
                    SonarLine line = sonarLines[pos];

                    PlaceMarker(y, line, !shiftHold);
                }
                return;
            }

            // Left mouse button sets new point or deletes old one.
            if (e.Button == MouseButtons.Left)
            {
                if (cutMouseMode == CutMouseMode.Paint)
                    GetCutLine().SafeInsert(new PointF(pos, y));
                if (cutMouseMode == CutMouseMode.Rubber)
                    GetCutLine().Remove(GetCutLine().GetNearestAt(pos));
                else if (altHold)
                    return;
                else if (ctrlHold && !shiftHold)
                {
                    if (selIndex >= 0)
                    {
                        try
                        {
                            GetCutLine().Remove(selIndex);
                        }
                        catch (Exception ex)
                        {
                            DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.OnMouseDown: " + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    CutLineSet clSet;

                    if (panelType == SonarPanelType.HF)
                        clSet = clSetHF;
                    else
                        clSet = clSetNF;

                    switch (cutMode)
                    {
                        case CutMode.Top:
                            if (clSet.CutBottom.InterpolateAt(pos) < y)
                                clSet.CutTop.SafeInsert(new PointF(pos, y));
                            break;
                        case CutMode.Bottom:
                            if (clSet.CutTop.InterpolateAt(pos) > y)
                                clSet.CutBottom.SafeInsert(new PointF(pos, y));
                            break;
                    }
                }

                Invalidate();
            }
        }

        public void PlaceMarker(float y, SonarLine line, bool showDialog)
        {
            if ((line == null) || (line.CoordRvHv.Type != CoordinateType.TransverseMercator))
            {
                MessageBox.Show("No coordinate available at this line.\nNothing added.", "Command aborted.");
                return;
            }

            Marker marker = null;

            if (placeSomething == PlaceMode.Buoy)
                marker = new Buoy();
            else if (placeSomething == PlaceMode.ManualPoint)
                marker = new ManualPoint();
            else
                return;

            marker.Coord = line.CoordRvHv;
            marker.Depth = y;
            marker.Tag = line;
            marker.Type = Marker.LastType;

            if (showDialog)
            {
                if (frmPropertyViewer.ShowDialog(marker) != DialogResult.OK)
                    return;

                Marker.LastType = marker.Type;
            }

            if (placeSomething == PlaceMode.ManualPoint)
                GlobalNotifier.Invoke(this, marker, GlobalNotifier.MsgTypes.PlaceManualPoint);
            else if ((buoyList != null) && (placeSomething == PlaceMode.Buoy))
                GlobalNotifier.Invoke(this, marker, GlobalNotifier.MsgTypes.PlaceBuoy);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (cutMode == CutMode.Nothing)
            {
                if (selectMode)
                {
                    ptEnd = new Point(e.X, e.Y);
                    Invalidate();
                }

                return;
            }

            PanelProp pp = GetProp();
            int pos = (int)(fileSlideBar.SectionWidth * e.X / this.Width) + pp.begin;
            float x = Math.Max(0, (float)(fileSlideBar.SectionWidth * e.X / Width) + pp.begin);
            float y = -e.Y / pp.h + depthZoomHi;

            if (e.Button == MouseButtons.Left)
            {
                if (cutMouseMode == CutMouseMode.Paint)
                {
                    GetCutLine().SafeInsert(new PointF(pos, y));
                    Invalidate();
                    return;
                }
                else if (cutMouseMode == CutMouseMode.Rubber)
                {
                    GetCutLine().Remove(GetCutLine().GetNearestAt(pos));
                    Invalidate();
                    return;
                }
                else if (altHold)
                {
                    if (x >= fileSlideBar.RegionWidth)
                        x = (float)fileSlideBar.RegionWidth - 1;

                    try
                    {
                        GetCutLine().Move(selIndex, new PointF(x, y));
                    }
                    catch (Exception ex)
                    {
                        DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.OnMouseMove: " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }

            if (ctrlHold || altHold)
            {
                SelectNode(e.X);
            }
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
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            selIndex = -1;
            Invalidate();
        }
        #endregion

        #region Cut line
        public void CutNow()
        {
            // Prepare and cut.
            if ((sonarLines == null) || (panelType == SonarPanelType.Void))
                return;

            CutLineSet clSet;
            DataEntry entry;
            LineData data;
            int max = sonarLines.Count;

            if (panelType == SonarPanelType.HF)
                clSet = clSetHF;
            else
                clSet = clSetNF;

            for (int i = 0; i < max; i++)
            {
                data = GetData(i);

                if ((data == null) || (data.Entries == null))
                    continue;

                data.TCut = clSet.CutTop.InterpolateAt(i);
                data.BCut = clSet.CutBottom.InterpolateAt(i);

                // UBOOT
                float depth = sonarLines[i].SubDepth;
                // UBOOT                    

                int maxEntries = data.Entries.Length;

                bool setTopCol = true;
                for (int j = 0; j < maxEntries; j++)
                {
                    entry = data.Entries[j];

                    // UBOOT
                    entry.high -= depth;
                    entry.low -= depth;
                    entry.uncutHigh -= depth;
                    // UBOOT

                    if (entry.uncutHigh > data.TCut)
                        entry.high = Math.Max(entry.low, data.TCut);
                    else if (entry.uncutHigh < data.BCut)
                        entry.high = entry.low;
                    else
                        entry.high = entry.uncutHigh;

                    // data.Entries[j].high = entry.high;
                    data.Entries[j].high = entry.high + depth;
                    data.Entries[j].visible = (entry.high > entry.low) & (entry.high > data.BCut);

                    if (setTopCol & entry.visible)
                    {
                        data.TopColor = entry.colorID;
                        setTopCol = false;
                    }
                }
            }

            GSC.Settings.Changed = true;

            // Invalidate
            RedrawSonarBmp();
        }

        public void SelectNode(int x)
        {
            PanelProp pp = GetProp();
            int pos = (int)(fileSlideBar.SectionWidth * x / Width) + pp.begin;

            if (shiftHold)
                pos = (pos < pp.mid) ? pp.begin : pp.end;

            try
            {
                selIndex = GetCutLine().GetNearestAt(pos);
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.SelectNode: " + ex.Message + "\n" + ex.StackTrace);
            }

            Invalidate();
        }
        #endregion

        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            // Unclear: Several global settings properties affect record view
            RedrawSonarBmp();
        }
    }
}