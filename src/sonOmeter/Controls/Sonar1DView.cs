using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.MathEx;
using System.Drawing.Drawing2D;
using UKLib.Debug;
using sonOmeter.Classes.Sonar2D;
using UKLib.Arrays;
using UKLib.Survey.Math;
using UKLib;

namespace sonOmeter
{
    #region Enum types
    public enum CutMouseMode
    {
        Normal,
        Paint,
        Rubber,
        Disable,
        Remove
    }
    
    public enum PlaceMode
    {
        Nothing,
        Buoy,
        ManualPoint
    }
    #endregion

    public partial class Sonar1DView : UserControl
    {
        private class BWResult
        {
            public Bitmap bmpHF;
            public Bitmap bmpNF;
            public bool tryKeepAppend;
        }

        #region Variables
        // Draw data
        List<SonarLine> sonarLines = null;
        IDList<ManualPoint> pointList = null;
        IDList<Sonar2DElement> buoyList = null;
        double panelYRatio = 0.5;
        int borderWidth = 1;
        bool recording = false;
        bool showPos = false;
        bool showHF = true;
        bool showNF = true;
        bool isCut = true;
        bool useAbsoluteHeights = false;
        double startAL = double.NaN;

        // Work line.
        SonarDepthMeter depthMeter = null;
        SonarLine workLine = null;

        // Info bar at bottom
        bool showVol = false;
        bool showRul = false;
        int infoBarHeight = 16;

        // Cut lines
        CutMode cutMode = CutMode.Nothing;
        CutMouseMode cutMouseMode = CutMouseMode.Normal;
        CutLineSet clSetHF = null;
        CutLineSet clSetNF = null;

        bool showSurf = false;
        bool showCalc = false;
        bool showSurfOp = false;
        bool showCalcOp = false;
        bool showDepthLines = false;
        
        // Draw variables (progress state)
        int startPtr = 0;
        int startLine = 0;

        int bwLastDrawLine = -1;
        float bwLastMoveError = 0;
        int bwLastppBegin = -1;
        int bwLastppEnde = -1;

        int disableSelStart = -1;
        int disableSelStop = -1;

        // Draw variables (viewport)
        RectangleD viewPort = new RectangleD();

        // Draw variables (selection and mouse interaction)
        PlaceMode placeSomething = PlaceMode.Nothing;
        PanelProp ppStart = new PanelProp();
        Point ptStart = new Point(0, 0);
        Point ptEnd = new Point(0, 0);
        bool selectMode = false;
        int selIndex = -1;

        // Draw variables (background worker)
        BackgroundWorker<bool, BWResult> bwDrawSonarBmp = null;
        Bitmap sonarBmpHFbw = null;
        Bitmap sonarBmpHF = null;
        Bitmap sonarBmpNFbw = null;
        Bitmap sonarBmpNF = null;
        bool bwNeedStartAgain = false;

        // Draw variables (color codes)
        int colorArchHF = 4 * GSC.Settings.SECL.Count + 0;
        int colorArchNF = 4 * GSC.Settings.SECL.Count + 1;
        int colorPosLineUsed = 4 * GSC.Settings.SECL.Count + 2;
        int colorPosLineUnused = 4 * GSC.Settings.SECL.Count + 3;
        int colorManualPoint = 4 * GSC.Settings.SECL.Count + 4;

        // Draw variables (RD8000 antenna distance)
        SonarLine antDepthLine = null;
        #endregion

        #region Properties
        public bool UseAbsoluteHeights
        {
            get { return useAbsoluteHeights; }
            set
            {
                useAbsoluteHeights = value;

                if (value && double.IsNaN(startAL))
                {
                    // Iterate through the first sonar lines until the first coordinate was found. Set the start altitude.
                    startAL = 0.0;

                    int max = sonarLines.Count;

                    for (int i = 0; i < max; i++)
                    {
                        Coordinate coord = sonarLines[i].CoordRvHv;
                        if (coord.Type == CoordinateType.TransverseMercator)
                        {
                            startAL = coord.AL;
                            break;
                        }
                    }
                }
            }
        }

        public int InfoBarHeight
        {
            set { infoBarHeight = value; RedrawSonarBmp(); }
            get { return infoBarHeight; }
        }

        public bool Recording
        {
            set { recording = value; RedrawSonarBmp(); }
        }

        public bool IsCut
        {
            set { isCut = value; RedrawSonarBmp(); }
            get { return isCut; }
        }

        public bool ShowPos
        {
            set { showPos = value; RedrawSonarBmp(); }
            get { return showPos; }
        }

        public bool ShowVol
        {
            set { showVol = value; RedrawSonarBmp(); }
            get { return showVol; }
        }

        public bool ShowRul
        {
            set { showRul = value; RedrawSonarBmp(); }
            get { return showRul; }
        }

        public bool ShowHF
        {
            set
            {
                showHF = value;
                if (!showHF)
                    showNF = true;
                RedrawSonarBmp();
            }
            get { return showHF; }
        }

        public bool ShowNF
        {
            set
            {
                showNF = value;
                if (!showNF)
                    showHF = true;
                RedrawSonarBmp();
            }
            get { return showNF; }
        }

        public bool ShowSurf
        {
            set
            {
                showSurf = value;
                showDepthLines = showDepthLines | showSurf;
                Invalidate();
            }
            get { return showSurf; }
        }

        public bool ShowCalc
        {
            set
            {
                showCalc = value;
                showDepthLines = showDepthLines | showCalc;
                Invalidate();
            }
            get { return showCalc; }
        }


        public bool ShowSurfOp
        {
            set
            {
                showSurfOp = value;
                showDepthLines = showDepthLines | showSurfOp;
                Invalidate();
            }
            get { return showSurfOp; }
        }

        public bool ShowCalcOp
        {
            set
            {
                showCalcOp = value;
                showDepthLines = showDepthLines | showCalcOp;
                Invalidate();
            }
            get { return showCalcOp; }
        }

        [Description("The visible region."), Category("Layout")]
        public RectangleD ViewPort
        {
            get { return viewPort; }
            set
            {
                viewPort = value;

                if (ViewPortChanged != null)
                    ViewPortChanged(this, EventArgs.Empty);
                
                RedrawSonarBmp();
            }
        }

        [Description("The associated depth meter."), Category("Data")]
        public SonarDepthMeter DepthMeter
        {
            get { return depthMeter; }
            set { depthMeter = value; Invalidate(); }
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

        [Browsable(false), DefaultValue(CutMode.Nothing)]
        public CutMode CutMode
        {
            set { cutMode = value; }
            get { return cutMode; }
        }

        [Browsable(false)]
        public CutMouseMode CutMouseMode
        {
            get { return cutMouseMode; }
            set { cutMouseMode = value; }
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

        public void ToggleDepthLines()
        {
            showDepthLines = !showDepthLines;
            Invalidate();
        }

        [Browsable(false)]
        public SonarLine AntDepthLine
        {
            get { return antDepthLine; }
            set { antDepthLine = value; }
        }
        #endregion

        #region Events
        [Description("This event occurs on a changed viewport."), Category("Misc")]
        public event EventHandler ViewPortChanged;
        #endregion

        #region Constructor
        public Sonar1DView()
        {
            InitializeComponent();

            this.ForeColor = GSC.Settings.CS.ForeColor;
            this.BackColor = GSC.Settings.CS.BackColor;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            RedrawSonarBmp();
        }
        #endregion

        #region List management
        public void RefreshLists(List<SonarLine> lines, IDList<Sonar2DElement> buoys, IDList<ManualPoint> points)
        {
            this.buoyList = buoys;
            this.pointList = points;
            this.sonarLines = lines;
        }
        #endregion

        #region RectArray
        private RectArrayF[] InitRectArray(double scaleFactor)
        {
            int count = GSC.Settings.SECL.Count;
            RectArrayF[] array = new RectArrayF[4 * count + 5];
            float oneOverScale = (float)(1.0 / scaleFactor);

            for (int i = 0; i < count; i++)
            {
                array[i + 0 * count] = new RectArrayF(GSC.Settings.SECL[i].SonarColor, oneOverScale); // HF
                array[i + 1 * count] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.SECL[i].SonarColor), oneOverScale);
                array[i + 2 * count] = new RectArrayF(GSC.Settings.SECL[i].SonarColor, oneOverScale); // NF
                array[i + 3 * count] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.SECL[i].SonarColor), oneOverScale);
            }

            array[colorArchHF] = new RectArrayF(GSC.Settings.CS.ArchNoDataColor, oneOverScale); // HF
            array[colorArchNF] = new RectArrayF(GSC.Settings.CS.ArchNoDataColor, oneOverScale); // NF
            array[colorPosLineUsed] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.CS.PosLineColor), oneOverScale);
            array[colorPosLineUnused] = new RectArrayF(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.CS.PosLineColorUnused), oneOverScale);
            array[colorManualPoint] = new RectArrayF(GSC.Settings.CS.ManualPointColor, oneOverScale);

            return array;
        }

        private void DrawArray(RectArrayF[] rcArray, Graphics gHF, Graphics gNF)
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

                    if (i == colorManualPoint)
                        for (int j = 0; j < ar.Length; j++)
                        {
                            if (showHF)
                                gHF.FillEllipse(rcArray[i].Brush, ar[j]);
                            if (showNF)
                                gNF.FillEllipse(rcArray[i].Brush, ar[j]);
                        }
                    else if ((i == colorPosLineUsed) || (i == colorPosLineUnused))
                    {
                        if (showHF)
                            gHF.FillRectangles(rcArray[i].Brush, ar);
                        if (showNF)
                            gNF.FillRectangles(rcArray[i].Brush, ar);
                    }
                    else if (i == colorArchNF)
                        gNF.FillRectangles(rcArray[i].Brush, ar);
                    else if (i == colorArchHF)
                        gHF.FillRectangles(rcArray[i].Brush, ar);
                    else if (i >= 3 * colorCount)
                    {
                        if (GSC.Settings.SECL[i - 3 * colorCount].SonarColorVisible && GSC.Settings.ArchActive)
                            gNF.FillRectangles(rcArray[i].Brush, ar);
                    }
                    else if (i >= 2 * colorCount)
                    {
                        if (GSC.Settings.SECL[i - 2 * colorCount].SonarColorVisible)
                            gNF.FillRectangles(rcArray[i].Brush, ar);
                    }
                    else if (i >= colorCount)
                    {
                        if (GSC.Settings.SECL[i - colorCount].SonarColorVisible && GSC.Settings.ArchActive)
                            gHF.FillRectangles(rcArray[i].Brush, ar);
                    }
                    else if (GSC.Settings.SECL[i].SonarColorVisible)
                        gHF.FillRectangles(rcArray[i].Brush, ar);
                }
                catch (Exception ex)
                {
                    DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar1DView.DrawArray: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        #endregion

        #region Paint structs
        public struct PanelProp
        {
            public float w;
            public float h;
            public float hHF;
            public float hNF;
            public float subw;
            public float subh;
            public float high;
            public float low;
            public int begin;
            public int end;
            public int mid;
            public int pos;
            public float y;
            public SonarPanelType panelType;
        }

        public PanelProp GetProp(int mouseX, int mouseY)
        {
            PanelProp pp = GetProp();

            pp.pos = (int)((mouseX - borderWidth) / pp.w) + pp.begin;

            int h = this.Height - (showHF & showNF ? 4 : 2) * borderWidth;

            int hHF = (showHF ? (showNF ? (int)((double)h * panelYRatio) : h) : 0);
            int hNF = h - hHF;

            if (h <= 0)
                pp.panelType = SonarPanelType.Void;
            else if (showHF && (mouseY >= borderWidth) && (mouseY < borderWidth + hHF))
                pp.panelType = SonarPanelType.HF;
            else if (showNF && (mouseY >= this.Height - borderWidth - hNF) && (mouseY < this.Height - borderWidth))
                pp.panelType = SonarPanelType.NF;
            else if (showHF && showNF && (mouseY >= borderWidth + hHF) && (mouseY < this.Height - borderWidth - hNF))
                pp.panelType = SonarPanelType.Sep;
            else
                pp.panelType = SonarPanelType.Void;

            if (showHF && showNF && (pp.panelType == SonarPanelType.NF))
                mouseY -= (hHF + 2 * borderWidth);

            float dy = 0;

            if (useAbsoluteHeights && (pp.pos > -1) && (pp.pos < sonarLines.Count))
            {
                SonarLine line = sonarLines[pp.pos];
                dy = (float)((line.CoordRvHv.Type == CoordinateType.TransverseMercator) ? line.CoordRvHv.AL : 0);
            }

            pp.y = -(float)(mouseY - borderWidth) / pp.h + (float)viewPort.Top - dy;

            return pp;
        }

        public PanelProp GetProp()
        {
            PanelProp pp;

            int h = this.Height - (showHF & showNF ? 4 : 2) * borderWidth;

            int hHF = (showHF ? (showNF ? (int)((double)h * panelYRatio) : h) : 0);
            int hNF = h - hHF;

            pp.w = (float)((double)(this.Width - 2 * borderWidth) / viewPort.Width);
            pp.h = (float)((double)h / ((showHF & showNF ? 2 : 1) * viewPort.Height));
            pp.hHF = (float)((double)hHF / viewPort.Height);
            pp.hNF = (float)((double)hNF / viewPort.Height);
            pp.subw = Math.Min(pp.w * GSC.Settings.SubmarineDepthMarker / 3.0F, 20.0F);
            pp.subh = pp.subw / (2.0F * pp.h);
            pp.subw /= pp.w;
            pp.high = (float)viewPort.Top;
            pp.low = (float)viewPort.Bottom;
            pp.begin = (int)viewPort.Left;
            pp.end = (int)viewPort.Right;
            pp.mid = (pp.end + pp.begin) >> 1;
            pp.pos = 0;
            pp.y = 0;
            pp.panelType = SonarPanelType.Void;

            if (pp.begin < 0)
                pp.begin = 0;

            if (sonarLines == null)
                return pp;

            if (pp.end >= sonarLines.Count)
                pp.end = sonarLines.Count - 1;

            pp.mid = (pp.end + pp.begin) >> 1;

            return pp;
        }

        public int GetPos(int pos, PanelProp pp, SonarViewMode viewMode)
        {
            pos -= pp.begin;

            if ((viewMode == SonarViewMode.RadarLike) && recording)
            {
                pos += startPtr;
                if (pp.begin > 0)
                    pos %= pp.end - pp.begin;
            }

            return pos;
        }
        #endregion

        #region Work line
        public int RefToScreen()
        {
            if (sonarLines == null)
                return -1;

            PanelProp pp = GetProp();
            int pos = sonarLines.IndexOf(workLine);

            if (pos == -1)
                return -1;

            return Convert.ToInt32((float)(pos - pp.begin) * pp.w);
        }

        public SonarLine ScreenToRef(int pos)
        {
            if ((this.Width <= 0) || (pos < 0) || (sonarLines == null))
                return null;

            PanelProp pp = GetProp();

            int n = Convert.ToInt32((float)pos / pp.w) + pp.begin;

            if (n > sonarLines.Count - 1)
                n = sonarLines.Count - 1;

            return sonarLines[n];
        }
        #endregion

        #region Drawing functions
        #region General drawing
        private void AddLine(bool active, int i, RectArrayF[] rcArray, PanelProp pp, bool ignorePos, bool showSubmarine)
        {
            if (i < 0)
                return;

            SonarLine line = (SonarLine)sonarLines[i];
            float al = (float)(useAbsoluteHeights ? (line.CoordRvHv.Type == CoordinateType.TransverseMercator) ? line.CoordRvHv.AL : startAL : 0);

            line.IsCut = isCut;

            if ((line.HF.Entries == null) || (line.NF.Entries == null))
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
                            rcArray[colorPosLineUsed].List.Add(new RectangleF((float)pp.pos, 0, 1.0F, (float)viewPort.Height));
                            break;
                        }
                    }
                }

                if (unused && !added)
                    rcArray[colorPosLineUnused].List.Add(new RectangleF((float)pp.pos, 0, 1.0F, (float)viewPort.Height));
            }

            float sDepth = GSC.Settings.Lic[Module.Modules.Submarine] ? sonarLines[i].SubDepth : 0.0F;
            float y;

            // Submarine stuff
            y = sDepth + pp.high - pp.subh / 2.0F - al;
            if ((i % GSC.Settings.SubmarineDepthMarker == 0) && GSC.Settings.Lic[Module.Modules.Submarine] && showSubmarine)
                rcArray[colorManualPoint].List.Add(new RectangleF(pp.pos, y, pp.subw, pp.subh));

            if (showHF)
                AddEntries(line.HF, rcArray, pp, sDepth, i, active, false, al);

            if (showNF)
                AddEntries(line.NF, rcArray, pp, sDepth, i, active, true, al);
        }

        private void AddEntries(LineData data, RectArrayF[] rcArray, PanelProp pp, float sDepth, int i, bool active, bool NF, float al)
        {
            RectangleF rc;
            float dDepth = data.Depth + al;
            float yMax = pp.high;
            float yMin = pp.low;
            float high;
            float low;
            int colorCount = GSC.Settings.SECL.Count;
            int max = data.Entries.Length;
            int j;

            // Adjust Y limits due to cut mode.
            if (isCut)
            {
                if (yMax > data.TCut + al)
                    yMax = data.TCut + al;
                if (yMin < data.BCut + al)
                    yMin = data.BCut + al;
            }

            // Draw last color in arch mode.
            if (active && (data.LastCol > -2) && (dDepth > pp.low))
            {
                low = dDepth - 0.1F;
                if (low < pp.low)
                    low = pp.low;

                if (data.LastCol == -1)
                    j = NF ? colorArchNF : colorArchHF;
                else
                    j = data.LastCol + (NF ? 2 * colorCount : 0);

                rcArray[j].List.Add(new RectangleF((float)pp.pos, -dDepth + pp.high, 1.0F, dDepth - low));
            }

            // Process each data entry.
            for (j = 0; j < max; j++)
            {
                // Get the entry.
                DataEntry entry = data.Entries[j];

                // Subtract submarine depth (if available).
                high = (isCut ? entry.high : entry.uncutHigh) - sDepth + al;
                low = entry.low - sDepth + al;

                if (low < pp.low)
                    low = pp.low;

                // Check visibility state.
                if ((!entry.visible && isCut) || (low > yMax) || (high < yMin))
                    continue;

                if (low < yMin)
                    low = yMin;
                if (high > yMax)
                    high = yMax;

                // Draw the entry.
                try
                {
                    // Build the rectangle.
                    rc = new RectangleF((float)pp.pos, -high + pp.high, 1.0F, high - low);

                    if (active && (low > dDepth))
                        rcArray[entry.colorID + colorCount * (NF ? 3 : 1)].List.Add(rc);
                    else
                    {
                        if (active && (high > dDepth))
                        {
                            // Change top for arch mode (unfortunately, RectangleF.Top is readonly).
                            rc = new RectangleF((float)pp.pos, -high + pp.high, 1.0F, high - dDepth);
                            rcArray[entry.colorID + colorCount * (NF ? 3 : 1)].List.Add(rc);

                            rc = new RectangleF((float)pp.pos, -dDepth + pp.high, 1.0F, dDepth - low);
                            rcArray[entry.colorID + (NF ? 2 * colorCount : 0)].List.Add(rc);
                        }
                        else
                            rcArray[entry.colorID + (NF ? 2 * colorCount : 0)].List.Add(rc);
                    }
                }
                catch (Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.AddLine: " + ex.Message+":"+ex.StackTrace);
                }
            }
        }

        private void DrawBottom(int i, PanelProp pp, Graphics gHF, Graphics gNF)
        {
            if (i < 0)
                return;

            SonarLine line = sonarLines[i];
            int sumHF = (int)line.HF.GetVolume(false);
            int sumNF = (int)line.NF.GetVolume(false);

            if (showVol)
            {
                if (sumHF < 0)
                    sumHF = 0;
                if (sumNF < 0)
                    sumNF = 0;
                if (sumHF > 255)
                    sumHF = 255;
                if (sumNF > 255)
                    sumNF = 255;

                if (showHF)
                    gHF.FillRectangle(new SolidBrush(Color.FromArgb(sumHF, sumHF, sumHF)), pp.pos, -pp.low + pp.high, 1.0F, infoBarHeight / pp.hHF);
                if (showNF)
                    gNF.FillRectangle(new SolidBrush(Color.FromArgb(sumNF, sumNF, sumNF)), pp.pos, -pp.low + pp.high, 1.0F, infoBarHeight / pp.hNF);
            }

            if (showRul && line.IsMarked)
            {
                PointF[] pt = new PointF[2];
                pt[0] = new PointF(pp.pos, Height);
                pt[1] = new PointF(pp.pos, Height - infoBarHeight / 2);
                if (showHF)
                    gHF.DrawLine(new Pen(GSC.Settings.CS.PosTickColor), pp.pos, -pp.low + pp.high, pp.pos, -pp.low + pp.high + infoBarHeight / (2.0F * pp.hHF));
                if (showNF)
                    gNF.DrawLine(new Pen(GSC.Settings.CS.PosTickColor), pp.pos, -pp.low + pp.high, pp.pos, -pp.low + pp.high + infoBarHeight / (2.0F * pp.hNF));
            }
        }
        #endregion

        #region Draw selection
        public void SelectRegion()
        {
            PanelProp ppEnd = GetProp(ptEnd.X, ptEnd.Y);

            int xStart, xEnd;
            double yHigh, yLow;

            if (ptStart.X < ptEnd.X)
            {
                xStart = ppStart.pos;
                xEnd = ppEnd.pos;
            }
            else
            {
                xStart = ppEnd.pos;
                xEnd = ppStart.pos;
            }

            if (ptStart.Y < ptEnd.Y)
            {
                yHigh = ppStart.y;
                yLow = ppEnd.y;
            }
            else
            {
                yHigh = ppEnd.y;
                yLow = ppStart.y;
            }

            this.ViewPort = new RectangleD(xStart, -yLow, xEnd, -yHigh);
        }

        private void DrawSelected(Graphics graphics, SonarPanelType panelType)
        {
            CutLine linePtr;
            PanelProp pp = GetProp();

            float h = -((panelType == SonarPanelType.HF) ? pp.hHF : pp.hNF);

            try { linePtr = GetCutLine(panelType); }
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

            float pos, al;
            Coordinate coord;

            for (int i = 0; i < ptList.Length; i++)
            {
                pos = ptList[i].X;
                coord = sonarLines[(int)pos].CoordRvHv;
                al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;

                ptList[i] = new PointF((ptList[i].X - pp.begin + 0.5F) * pp.w, h * (ptList[i].Y - (float)viewPort.Top + al));
            }

            graphics.DrawLines(new Pen(GSC.Settings.CS.CutLineColor, 4), ptList);
        }

        private void DrawSelectRegions(Graphics graphics, PanelProp pp)
        {
            if (selectMode)
            {
                int x, y;
                Region reg = new Region(new Rectangle(0, 0, this.Size.Width, this.Size.Height));

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

            if (cutMouseMode == sonOmeter.CutMouseMode.Disable || cutMouseMode == sonOmeter.CutMouseMode.Remove)
            {
                RectangleF rcF = new RectangleF((float)((Math.Min(disableSelStart, disableSelStop) - pp.begin) * this.Size.Width) / (float)viewPort.Width, 0, (float)(Math.Abs(disableSelStop - disableSelStart) * this.Size.Width) / (float)viewPort.Width, this.Size.Height);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(GSC.Settings.CS.AlphaChannel, GSC.Settings.CS.DisableSelColor)), rcF);
                graphics.DrawRectangle(new Pen(GSC.Settings.CS.DisableSelColor), rcF.X, rcF.Y, rcF.Width, rcF.Height);
            }
        }
        #endregion

        #region Draw cut line
        private void DrawCutLine(Graphics graphics, CutLine cutLine, Color color, bool fromTop, bool dim, SonarPanelType panelType)
        {
            if (sonarLines.Count < 1)
                return;

            PanelProp pp = GetProp();
            GraphicsPath path = new GraphicsPath();
            PointF pt1, pt2;
            int count = cutLine.Count;
            float pos, al;
            Coordinate coord;

            float h = -((panelType == SonarPanelType.HF) ? pp.hHF : pp.hNF);
            float yMin = depthMeter.DepthBottom;

            try
            {
                for (int i = 1; i < count; i++)
                {
                    pos = cutLine[i - 1].X;
                    coord = sonarLines[(int)pos].CoordRvHv;
                    al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                    pt1 = new PointF((pos - pp.begin + 0.5f) * pp.w, h * (Math.Max(yMin, cutLine[i - 1].Y + al) - (float)viewPort.Top));
                    pos = cutLine[i].X;
                    coord = sonarLines[(int)pos].CoordRvHv;
                    al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                    pt2 = new PointF((pos - pp.begin + 0.5f) * pp.w, h * (Math.Max(yMin, cutLine[i].Y + al) - (float)viewPort.Top));

                    path.AddLine(pt1, pt2);
                }

                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawPath(new Pen(color, 2), path);

                if (dim)
                {
                    if (fromTop)
                    {
                        pos = cutLine[count - 1].X;
                        coord = sonarLines[(int)pos].CoordRvHv;
                        al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                        pt1 = new PointF((pos - pp.begin) * pp.w, h * (Math.Max(yMin, cutLine[count - 1].Y + al) - (float)viewPort.Top));
                        pt2 = new PointF(pt1.X, h * (0 - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, h * (0 - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);

                        pos = cutLine[0].X;
                        coord = sonarLines[(int)pos].CoordRvHv;
                        al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, h * (Math.Max(yMin, cutLine[0].Y + al) - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);
                    }
                    else
                    {
                        pos = cutLine[count - 1].X;
                        coord = sonarLines[(int)pos].CoordRvHv;
                        al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                        pt1 = new PointF((pos - pp.begin) * pp.w, h * (Math.Max(yMin, cutLine[count - 1].Y + al) - (float)viewPort.Top));
                        pt2 = new PointF(pt1.X, h * (yMin - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);

                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, h * (yMin - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);

                        pos = cutLine[0].X;
                        coord = sonarLines[(int)pos].CoordRvHv;
                        al = (useAbsoluteHeights && (coord.Type == CoordinateType.TransverseMercator)) ? (float)coord.AL : 0;
                        pt1 = pt2;
                        pt2 = new PointF((0 - pp.begin) * pp.w, h * (Math.Max(yMin, cutLine[0].Y + al) - (float)viewPort.Top));
                        path.AddLine(pt1, pt2);
                    }

                    graphics.FillPath(new SolidBrush(Color.FromArgb(GSC.Settings.CS.AlphaChannel, Color.White)), path);
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar1DView.DrawCutLine:" + e.Message);
            }
        }

        private void DrawCutLineSet(Graphics g, CutLineSet clSet)
        {
            CutLine linePtr;

            // Draw begin cut line.
            linePtr = clSet.CutTop;
            if (linePtr.Count == 0)
            {
                linePtr.Insert(0, 0);
                linePtr.Insert(sonarLines.Count, 0);
            }

            DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, true, true, clSet.PanelType);

            // Draw end cut line.
            linePtr = clSet.CutBottom;
            if (linePtr.Count == 0)
            {
                linePtr.Insert(0, (float)GSC.Settings.DepthBottom);
                linePtr.Insert(sonarLines.Count, (float)GSC.Settings.DepthBottom);
            }

            DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, false, true, clSet.PanelType);

            // Draw calculated depth line.
            if (clSet.CalcDepthAv)
            {
                linePtr = clSet.CutCalcDepth;
                DrawCutLine(g, linePtr, GSC.Settings.CS.CutLineColor, true, true, clSet.PanelType);
            }

            // Draw selected lines bold.
            DrawSelected(g, clSet.PanelType);
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

        public void DrawWorkLine(int pos)
        {
            PanelProp pp = GetProp();
            int h = this.Height - (showHF & showNF ? 4 : 2) * borderWidth;
            int hHF = (showHF ? (showNF ? (int)((double)h * panelYRatio) : h) : 0);
            int hNF = h - hHF;
            int hInfoBar = (showRul || showVol) ? infoBarHeight : 0;

            Graphics gHF = null;
            Graphics gNF = null;

            // Prepare brushes.
            RectArrayF[] rcArray = InitRectArray(1.0);

            // Prepare graphics objects for depth meter.
            if (showHF)
            {
                gHF = depthMeter.WorkLinePaintObj;
                gHF.TranslateTransform(0, borderWidth);
                gHF.ScaleTransform(depthMeter.WorkLinePaintSize.Width, (float)((double)(hHF - hInfoBar) / viewPort.Height));
                gHF.Clear(this.BackColor);
            }

            if (showNF)
            {
                gNF = depthMeter.WorkLinePaintObj;
                gNF.TranslateTransform(0, this.Height - borderWidth - hNF);
                gNF.ScaleTransform(depthMeter.WorkLinePaintSize.Width, (float)((double)(hNF - hInfoBar) / viewPort.Height));
                if (!showHF)
                    gNF.Clear(this.BackColor);
            }

            // Add line to buf and draw it.
            AddLine(GSC.Settings.ArchActive, pos, rcArray, pp, true, false);
            DrawArray(rcArray, gHF, gNF);
            DrawBottom(pos, pp, gHF, gNF);

            if (showHF)
                gHF.Dispose();
            if (showNF)
                gNF.Dispose();
        } 
        #endregion
        #endregion

        #region Redraw background worker
        #region Start
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
        #endregion

        #region Completed
        void bwDrawSonarBmp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs<BWResult> e)
        {
            try
            {
                if (e.Result != null)
                {
                    if (e.Result.bmpNF != null)
                        sonarBmpNF = e.Result.bmpNF;
                    if (e.Result.bmpHF != null)
                        sonarBmpHF = e.Result.bmpHF;

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
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        } 
        #endregion

        #region Do work
        void bwDrawSonarBmp_DoWork(object sender, DoWorkEventArgs<bool, BWResult> e)
        {
            try
            {
                int w = this.Width - 2 * borderWidth;
                int h = this.Height - (showHF & showNF ? 4 : 2) * borderWidth;

                if ((viewPort.Width <= 0) || (viewPort.Height == 0))
                    return;

                if ((w <= 0) || (h <= 0))
                    return;

                int hHF = (showHF ? (showNF ? (int)((double)h * panelYRatio) : h) : 0);
                int hNF = h - hHF;
                int hInfoBar = (showRul || showVol) ? infoBarHeight : 0;

                // rebuild if tryAppend false
                bool rebuild = !(bool)e.Argument;

                // Set up view mode.
                SonarViewMode viewMode = GSC.Settings.ViewMode;
                if (!recording)
                    viewMode = SonarViewMode.LeftFixed;

                // Set colors.
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;
                SolidBrush backBrush = new SolidBrush(this.BackColor);

                Graphics gHF = null;
                Graphics gNF = null;

                #region Init HF
                // Rebuild bitmap structure if needed.
                if (showHF)
                {
                    if ((sonarBmpHFbw == null) || (w != sonarBmpHFbw.Width) || (hHF != sonarBmpHFbw.Height))
                    {
                        rebuild = true;
                        if (sonarBmpHFbw != null)
                            sonarBmpHFbw.Dispose();
                        sonarBmpHFbw = new Bitmap(w, hHF);
                    }

                    // Get graphics object.
                    gHF = Graphics.FromImage(sonarBmpHFbw);
                    gHF.SmoothingMode = SmoothingMode.HighSpeed;

                    // Flush old bitmap.
                    if (rebuild)
                        gHF.Clear(this.BackColor);

                    gHF.ScaleTransform((float)(w / viewPort.Width), (float)((double)(hHF - hInfoBar) / viewPort.Height));
                }
                else
                {
                    if (sonarBmpHFbw != null)
                        sonarBmpHFbw.Dispose();
                    sonarBmpHFbw = null;
                }
                #endregion

                #region Init NF
                rebuild = !(bool)e.Argument;

                // Rebuild bitmap structure if needed.
                if (showNF)
                {
                    if ((sonarBmpNFbw == null) || (w != sonarBmpNFbw.Width) || (hNF != sonarBmpNFbw.Height))
                    {
                        rebuild = true;
                        if (sonarBmpNFbw != null)
                            sonarBmpNFbw.Dispose();
                        sonarBmpNFbw = new Bitmap(w, hNF);
                    }

                    // Get graphics object.
                    gNF = Graphics.FromImage(sonarBmpNFbw);
                    gNF.SmoothingMode = SmoothingMode.HighSpeed;

                    // Flush old bitmap.
                    if (rebuild)
                        gNF.Clear(this.BackColor);

                    gNF.ScaleTransform((float)(w / viewPort.Width), (float)((double)(hNF - hInfoBar) / viewPort.Height));
                }
                else
                {
                    if (sonarBmpNFbw != null)
                        sonarBmpNFbw.Dispose();
                    sonarBmpNFbw = null;
                }
                #endregion

                // Prepare and draw.
                if ((sonarLines != null) && (showNF | showHF))
                {
                    PanelProp pp = GetProp();
                    bool archActive = GSC.Settings.ArchActive;
                    int i;

                    // Prepare brushes - TODO: Scale factor?
                    RectArrayF[] rcArray = InitRectArray(1.0);

                    // Move start pointer.
                    startPtr += pp.begin - startLine;
                    if (startPtr < 0)
                        startPtr = 0;
                    if (pp.end - pp.begin > 0)
                        startPtr %= pp.end - pp.begin;

                    int start = pp.begin;
                    bool drawLines = true;

                    // Update pointers during recording and clear regions that will be refreshed.
                    if (recording)
                    {
                        switch (viewMode)
                        {
                            case SonarViewMode.RadarLike:
                                if (!rebuild)
                                {
                                    start = bwLastDrawLine + 1;
                                    if (showHF)
                                        gHF.FillRectangle(backBrush, new RectangleF(startPtr, 0, 1, (float)viewPort.Height));
                                    if (showNF)
                                        gNF.FillRectangle(backBrush, new RectangleF(startPtr, 0, 1, (float)viewPort.Height));
                                }
                                else
                                    startPtr = 0;
                                break;

                            case SonarViewMode.RightFixed:
                                if ((!rebuild) & (pp.begin > 0))
                                {
                                    start = bwLastDrawLine;

                                    float move = pp.w + bwLastMoveError;
                                    bwLastMoveError = move - (int)move;

                                    if (move >= 1)
                                    {
                                        Matrix t;

                                        if (showHF)
                                        {
                                            t = gHF.Transform;
                                            gHF.Transform = new Matrix();
                                            gHF.DrawImageUnscaled(sonarBmpHF, -(int)move, 0);
                                            gHF.FillRectangle(backBrush, new RectangleF(w - (int)move, 0, (int)move, (float)hHF));
                                            gHF.Transform = t;
                                        }
                                        if (showNF)
                                        {
                                            t = gNF.Transform;
                                            gNF.Transform = new Matrix();
                                            gNF.DrawImageUnscaled(sonarBmpNF, -(int)move, 0);
                                            gNF.FillRectangle(backBrush, new RectangleF(w - (int)move, 0, (int)move, (float)hNF));
                                            gNF.Transform = t;
                                        }
                                    }
                                    else
                                        drawLines = false;

                                    //temp.Dispose();
                                }
                                else
                                    startPtr = 0;
                                break;

                            case SonarViewMode.LeftFixed:
                                if (!rebuild)
                                {
                                    if (bwLastppEnde == pp.end)
                                        drawLines = false;
                                    else
                                        start = bwLastDrawLine;
                                }

                                bwLastppBegin = pp.begin;
                                bwLastppEnde = pp.end;
                                break;
                        }
                    }

                    bwLastDrawLine = pp.end;

                    if (drawLines)
                    {
                        // Add each line to buf.
                        for (i = start; i <= pp.end; i++)
                        {
                            pp.pos = GetPos(i, pp, viewMode);

                            AddLine(archActive, i, rcArray, pp, false, true);
                            DrawBottom(i, pp, gHF, gNF);
                        }

                        startLine = pp.begin;

                        // Draw all lines.
                        DrawArray(rcArray, gHF, gNF);

                        #region Radargradient
                        // Draw radar gradient.
                        if ((viewMode == SonarViewMode.RadarLike) && (viewPort.Width <= sonarLines.Count))
                        {
                            Color color = GSC.Settings.CS.BackColor;

                            int radarGradient = Math.Max((int)viewPort.Width >> 3, 2);

                            RectangleF rect = new RectangleF(startPtr + 1, 0, radarGradient, (float)viewPort.Height);

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
                            
                            if (showHF)
                                gHF.FillRectangle(brush, rect);
                            if (showNF)
                                gNF.FillRectangle(brush, rect);

                            brush.Dispose();
                        }
                        #endregion
                    }

                    rcArray = null;
                }

                // Invalidate
                if (showHF)
                    gHF.Dispose();
                if (showNF)
                    gNF.Dispose();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar1DView.RedrawSonarBmp: " + ex.Message);
            }
            finally
            {
                // Create result.
                e.Result = new BWResult() { tryKeepAppend = (bool)e.Argument, bmpNF = (sonarBmpNFbw != null) ? (Bitmap)sonarBmpNFbw.Clone() : null, bmpHF = (sonarBmpHFbw != null) ? (Bitmap)sonarBmpHFbw.Clone() : null };
            }
        }  
        #endregion
        #endregion

        #region Paint event
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                int w = this.Width - 2 * borderWidth;
                int h = this.Height - (showHF & showNF ? 4 : 2) * borderWidth;

                if ((w <= 0) || (h <= 0))
                    return;

                int hHF = (showHF ? (showNF ? (int)((double)h * panelYRatio) : h) : 0);
                int hNF = h - hHF;
                int hInfoBar = (showRul || showVol) ? infoBarHeight + 1 : 0;

                SonarViewMode viewMode = GSC.Settings.ViewMode;
                
                // Get graphics object.
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.Clear(this.BackColor);

                // Prepare the drawing rectangles.
                Rectangle rcHF = new Rectangle(borderWidth, borderWidth, w, hHF);
                Rectangle rcNF = new Rectangle(borderWidth, this.Height - borderWidth - hNF, w, hNF);

                // Draw bitmap(s).
                if (showHF && (sonarBmpHF != null))
                {
                    if ((w != sonarBmpHF.Width) || (hHF != sonarBmpHF.Height))
                        g.DrawImage(sonarBmpHF, rcHF);
                    else
                        g.DrawImageUnscaled(sonarBmpHF, rcHF);
                }
                if (showNF && (sonarBmpNF != null))
                {
                    if ((w != sonarBmpNF.Width) || (hNF != sonarBmpNF.Height))
                        g.DrawImage(sonarBmpNF, rcNF);
                    else
                        g.DrawImageUnscaled(sonarBmpNF, rcNF);
                }

                // Prepare the clipping regions and adjust NF rectangle to transformed matrix.
                rcHF.Height -= hInfoBar;
                Region regHF = new Region(rcHF);
                rcNF = new Rectangle(borderWidth, borderWidth, w, hNF - hInfoBar);
                Region regNF = new Region(rcNF);
                Region regOld = g.Clip.Clone();

                // Draw work line.
                PanelProp pp = GetProp();
                int pos = sonarLines.IndexOf(workLine);

                if ((pos != -1) && (depthMeter != null))
                {
                    DrawWorkLine(pos);

                    if (showHF)
                        g.FillRectangle(new SolidBrush(GSC.Settings.CS.WorkLineColor), new RectangleF((float)(pos - pp.begin) * pp.w + (float)borderWidth, (float)borderWidth, pp.w, hHF));
                    if (showNF)
                        g.FillRectangle(new SolidBrush(GSC.Settings.CS.WorkLineColor), new RectangleF((float)(pos - pp.begin) * pp.w + (float)borderWidth, (float)(this.Height - borderWidth - hNF), pp.w, hNF));
                }

                // Prepare transform matrices for overlay objects.
                Matrix T = g.Transform;
                Matrix tHF = g.Transform;
                Matrix tNF = g.Transform;

                tHF.Translate(borderWidth, borderWidth);
                tNF.Translate(borderWidth, this.Height - borderWidth - hNF);

                // Draw additional lines.
                if (cutMode != CutMode.Nothing)
                {
                    // Draw cut lines.
                    if (showHF)
                    {
                        g.Transform = tHF;
                        g.Clip = regHF;
                        DrawCutLineSet(g, clSetHF);
                    }

                    if (showNF)
                    {
                        g.Transform = tNF;
                        g.Clip = regNF;
                        DrawCutLineSet(g, clSetNF);
                    }
                }
                else if (showDepthLines)
                {
                    // Draw depth lines.
                    if ((showSurf) || (showSurfOp))
                        BuildSurfaceLines();

                    if (showHF)
                    {
                        g.Transform = tHF;
                        g.Clip = regHF;
                        
                        if (showSurf)
                            DrawCutLine(g, clSetHF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false, SonarPanelType.HF);
                        if (showSurfOp)
                            DrawCutLine(g, clSetNF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false, SonarPanelType.HF);
                        if (showCalc && (clSetHF.CutCalcDepth.Count != 0))
                            DrawCutLine(g, clSetHF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false, SonarPanelType.HF);
                        if (showCalcOp && (clSetNF.CutCalcDepth.Count != 0))
                            DrawCutLine(g, clSetNF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false, SonarPanelType.HF);
                    }

                    if (showNF)
                    {
                        g.Transform = tNF;
                        g.Clip = regNF;

                        if (showSurf)
                            DrawCutLine(g, clSetNF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false, SonarPanelType.NF);
                        if (showSurfOp)
                            DrawCutLine(g, clSetHF.CutSurface, GSC.Settings.CS.SurfLineColor, true, false, SonarPanelType.NF);
                        if (showCalc && (clSetNF.CutCalcDepth.Count != 0))
                            DrawCutLine(g, clSetNF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false, SonarPanelType.NF);
                        if (showCalcOp && (clSetHF.CutCalcDepth.Count != 0))
                            DrawCutLine(g, clSetHF.CutCalcDepth, GSC.Settings.CS.CutLineColor, true, false, SonarPanelType.NF);
                    }
                }

                // Draw Buoys & Manual Points
                Dictionary<int, MPConnStyle> styles = GSC.Settings.MPConnStyles;
                Dictionary<int, IDList<ManualPoint>> connectMP = new Dictionary<int, IDList<ManualPoint>>();
                IDList<Marker> markers = new IDList<Marker>();
                Marker m, m2;
                int i, max;

                #region Fill marker list
                max = pointList.Count;

                for (i = 0; i < max; i++)
                {
                    ManualPoint pt = pointList[i] as ManualPoint;

                    if ((pt.Tag is SonarLine) && (sonarLines.Contains(pt.Tag as SonarLine)))
                    {
                        // Divert the manual points that shall be connected.
                        if (styles.ContainsKey(pt.Type))
                        {
                            if (!connectMP.ContainsKey(pt.Type))
                                connectMP.Add(pt.Type, new IDList<ManualPoint>());

                            connectMP[pt.Type].Add(pt);

                            MPConnStyle style = styles[pt.Type];

                            if (style.ShowPoints)
                            {
                                if (style.UseTemplate)
                                    markers.Add(new ManualPoint(pt, style.Template));
                                else
                                    markers.Add(pt);
                            }
                        }
                        else
                            markers.Add(pt);
                    }
                }

                max = buoyList.Count;

                for (i = 0; i < max; i++)
                {
                    m = buoyList[i] as Buoy;

                    if ((m.Tag is SonarLine) && (sonarLines.Contains(m.Tag as SonarLine)))
                        markers.Add(m);
                } 
                #endregion

                #region Draw all valid markers
                max = markers.Count;
                float buoySizeHF = (float)GSC.Settings.BuoySize;
                float buoySizeNF = (float)GSC.Settings.BuoySize;
                
                if (GSC.Settings.BuoySizeUnit == Buoy.BuoySizeUnit.Meters)
                {
                    buoySizeHF *= (float)((double)hHF / viewPort.Height);
                    buoySizeNF *= (float)((double)hNF / viewPort.Height);
                }

                for (i = 0; i < max; i++)
                {
                    m = markers[i];

                    // Draw it.
                    pos = sonarLines.IndexOf(m.Tag as SonarLine);
                    
                    if ((pos < pp.begin) || (pos > pp.end))
                        continue;

                    pos = GetPos(pos, pp, viewMode);

                    float y = m.Depth - (float)viewPort.Top + (float)(useAbsoluteHeights ? m.AL : m.DepthCorrection);

                    if (showHF)
                    {
                        g.Transform = tHF;
                        g.Clip = regHF;
                        
                        m.Paint1D(g, (float)pos * pp.w + (float)borderWidth, -pp.hHF * y + (float)borderWidth, buoySizeHF, w, hHF);
                    }

                    if (showNF)
                    {
                        g.Transform = tNF;
                        g.Clip = regNF;
                        
                        m.Paint1D(g, (float)pos * pp.w + (float)borderWidth, -pp.hNF * y + (float)borderWidth, buoySizeNF, w, hNF);
                    }
                }

                foreach (KeyValuePair<int,  IDList<ManualPoint>> kvp in connectMP)
                {
                    IDList<ManualPoint> list = kvp.Value;
                    max = list.Count;
                    Pen connectPen = styles[kvp.Key].CreatePen();

                    for (i = 0; i < max - 1; i++)
                    {
                        m = list[i];
                        m2 = list[i + 1];

                        // Draw it.
                        int pos1 = sonarLines.IndexOf(m.Tag as SonarLine);
                        int pos2 = sonarLines.IndexOf(m2.Tag as SonarLine);

                        if (((pos1 < pp.begin) && (pos2 < pp.begin)) || ((pos1 > pp.end) && (pos2 > pp.end)))
                            continue;

                        pos1 = GetPos(pos1, pp, viewMode);
                        pos2 = GetPos(pos2, pp, viewMode);
                        
                        float y1 = m.Depth - (float)viewPort.Top + (float)(useAbsoluteHeights ? m.AL : m.DepthCorrection);
                        float y2 = m2.Depth - (float)viewPort.Top + (float)(useAbsoluteHeights ? m2.AL : m2.DepthCorrection);

                        if (showHF)
                        {
                            g.Transform = tHF;
                            g.Clip = regHF;

                            g.DrawLine(connectPen, (float)pos1 * pp.w + (float)borderWidth, -pp.hHF * y1 + (float)borderWidth, (float)pos2 * pp.w + (float)borderWidth, -pp.hHF * y2 + (float)borderWidth);
                        }

                        if (showNF)
                        {
                            g.Transform = tNF;
                            g.Clip = regNF;

                            g.DrawLine(connectPen, (float)pos1 * pp.w + (float)borderWidth, -pp.hNF * y1 + (float)borderWidth, (float)pos2 * pp.w + (float)borderWidth, -pp.hNF * y2 + (float)borderWidth);
                        }
                    }
                }
                #endregion

                #region Draw the antenna depth marker
                if (GSC.Settings.Lic[Module.Modules.RadioDetection] && GSC.Settings.ShowAntDepth && (antDepthLine != null) && !double.IsNaN(antDepthLine.AntDepth))
                {
                    double antDepthValue = antDepthLine.AntDepth;

                    ManualPoint ptAnt = new ManualPoint(antDepthLine.CoordRvHv, (float)antDepthValue, -1);
                    ptAnt.ShowTextDepth = false;
                    ptAnt.ShowTextDesc = false;
                    ptAnt.ShowTextType = false;
                    ptAnt.Color = GSC.Settings.CS.AntennaColor;

                    Pen antennaPen = new Pen(GSC.Settings.CS.AntennaColor, 5.0F);
                    
                    pos = GetPos(sonarLines.IndexOf(antDepthLine), pp, viewMode);
                    float yAnt = (float)(antDepthValue - viewPort.Top + (useAbsoluteHeights ? ptAnt.AL : ptAnt.DepthCorrection));

                    if (showHF)
                    {
                        g.Transform = tHF;
                        g.Clip = regHF;

                        g.DrawLine(antennaPen, (float)pos * pp.w + (float)borderWidth, 0, (float)pos * pp.w + (float)borderWidth, -pp.hHF * yAnt + (float)borderWidth);
                    }

                    if (showNF)
                    {
                        g.Transform = tNF;
                        g.Clip = regNF;

                        g.DrawLine(antennaPen, (float)pos * pp.w + (float)borderWidth, 0, (float)pos * pp.w + (float)borderWidth, -pp.hNF * yAnt + (float)borderWidth);
                    }
                }
                #endregion

                // Reset the transformation matrix.
                g.Transform = T;

                // Clear clipping region.
                g.Clip = regOld;

                // Draw selected region.
                DrawSelectRegions(g, pp);            

                // Draw border.
                g.DrawLine(SystemPens.ControlDark, 0, 0, this.Width - 1, 0);
                g.DrawLine(SystemPens.ControlDark, 0, 0, 0, this.Height - 1);

                if (showHF && showNF)
                {
                    g.DrawLine(SystemPens.ControlLightLight, 0, hHF + borderWidth, this.Width - 1, hHF + borderWidth);
                    g.DrawLine(SystemPens.ControlDark, 0, hHF + 1 + borderWidth, this.Width - 1, hHF + 1 + borderWidth);
                }
                g.DrawLine(SystemPens.ControlLightLight, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                g.DrawLine(SystemPens.ControlLightLight, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar1DView.OnPaint: " + ex.Message + "\n" + ex.StackTrace);
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            bool shiftHold = (ModifierKeys & Keys.Shift) != Keys.None;
            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            PanelProp pp = GetProp(e.X, e.Y);

            // Reset cursor if not over seperator.
            if ((pp.panelType != SonarPanelType.Sep) && (Cursor == Cursors.HSplit))
                Cursor = Cursors.Default;

            // Seperator hit - begin drag movement.
            if ((pp.panelType == SonarPanelType.Sep) && (e.Button == MouseButtons.Left))
            {
                ppStart = pp;
                return;
            }

            if (shiftHold && (placeSomething == PlaceMode.Nothing))
                pp.pos = (pp.pos < pp.mid) ? pp.begin : pp.end;

            if (cutMode == CutMode.Nothing)
            {
                if (ctrlHold)
                {
                    ppStart = pp;
                    ptEnd = ptStart = new Point(e.X, e.Y);
                    selectMode = true;
                }
                else if ((placeSomething != PlaceMode.Nothing) && (e.Button == MouseButtons.Left) && (pp.pos > -1) && (pp.pos < sonarLines.Count))
                {
                    SonarLine line = sonarLines[pp.pos];

                    PlaceMarker(pp.y, line, !shiftHold);
                }
                return;
            }

            CutLine cl = GetCutLine(pp.panelType);

            // Left mouse button sets new point or deletes old one.
            if ((cl != null) && (e.Button == MouseButtons.Left))
            {
                if (cutMouseMode == CutMouseMode.Paint)
                    cl.SafeInsert(new PointF(pp.pos, pp.y));
                else if (cutMouseMode == CutMouseMode.Rubber)
                    cl.Remove(GetCutLine(pp.panelType).GetNearestAt(pp.pos, 2));
                else if (cutMouseMode == CutMouseMode.Disable || cutMouseMode == CutMouseMode.Remove)
                    disableSelStart = disableSelStop = pp.pos;
                else if (altHold)
                    return;
                else if (ctrlHold && !shiftHold)
                {
                    if (selIndex >= 0)
                    {
                        try
                        {
                            cl.Remove(selIndex);
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

                    if (pp.panelType == SonarPanelType.HF)
                        clSet = clSetHF;
                    else
                        clSet = clSetNF;

                    switch (cutMode)
                    {
                        case CutMode.Top:
                            if (clSet.CutBottom.InterpolateAt(pp.pos) < pp.y)
                                clSet.CutTop.SafeInsert(new PointF(pp.pos, pp.y));
                            break;
                        case CutMode.Bottom:
                            if (clSet.CutTop.InterpolateAt(pp.pos) > pp.y)
                                clSet.CutBottom.SafeInsert(new PointF(pp.pos, pp.y));
                            break;
                    }
                }

                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool ctrlHold = (ModifierKeys & Keys.Control) != Keys.None;
            bool altHold = (ModifierKeys & Keys.Alt) != Keys.None;

            PanelProp pp = GetProp(e.X, e.Y);
            float x = Math.Max(0, (float)(viewPort.Width * e.X / this.Width) + pp.begin);

            if ((ppStart.panelType == SonarPanelType.Sep) && (e.Button == MouseButtons.Left))
            {
                Cursor = Cursors.HSplit;

                int h = this.Height - 4 * borderWidth;
                int hHF = Math.Max(e.Y - borderWidth, 20);
                int hNF = Math.Max(h - hHF, 20);
                hHF = h - hNF;
                panelYRatio = (double)hHF / (double)h;
                depthMeter.PanelYRatio = panelYRatio;
                RedrawSonarBmp();

                return;
            }

            if (pp.panelType == SonarPanelType.Sep)
                Cursor = Cursors.HSplit;
            else if (Cursor == Cursors.HSplit)
            {
                Cursor = Cursors.Default;
                ppStart = pp;
            }

            if (cutMode == CutMode.Nothing)
            {
                if ((selectMode) && (pp.panelType == ppStart.panelType))
                {
                    ptEnd = new Point((e.X > 0) ? e.X : 0, e.Y);
                    Invalidate();
                }

                return;
            }

            CutLine cl = GetCutLine(pp.panelType);

            if ((cl != null) && (e.Button == MouseButtons.Left))
            {
                if (cutMouseMode == CutMouseMode.Paint)
                {
                    cl.SafeInsert(new PointF(pp.pos, pp.y));
                    Invalidate();
                    return;
                }
                else if (cutMouseMode == CutMouseMode.Rubber)
                {
                    cl.Remove(GetCutLine(pp.panelType).GetNearestAt(pp.pos, 2));
                    Invalidate();
                    return;
                }
                else if (cutMouseMode == CutMouseMode.Disable || cutMouseMode == CutMouseMode.Remove)
                {
                    disableSelStop = Math.Min(Math.Max(pp.pos, 0), sonarLines.Count - 1);
                    Invalidate();
                }
                else if (altHold)
                {
                    if (x >= sonarLines.Count)
                        x = (float)sonarLines.Count - 1.0F;

                    try
                    {
                        cl.Move(selIndex, new PointF(x, pp.y));
                    }
                    catch (Exception ex)
                    {
                        DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.OnMouseMove: " + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }

            if (ctrlHold || altHold)
            {
                SelectNode(pp);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            int x = (e.X < 0) ? 0 : e.X;

            PanelProp pp = GetProp(x, e.Y);

            if ((pp.panelType != SonarPanelType.Sep) && (Cursor == Cursors.HSplit))
            {
                Cursor = Cursors.Default;
                ppStart.panelType = SonarPanelType.Void;
            }
            
            if (selectMode)
            {
                if (pp.panelType == ppStart.panelType)
                    ptEnd = new Point(x, e.Y);
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
            CutNow(SonarPanelType.HF);
            CutNow(SonarPanelType.NF);

            GSC.Settings.Changed = true;

            RedrawSonarBmp();
        }
            
        private void CutNow(SonarPanelType panelType)
        {
            // Prepare and cut.
            if ((sonarLines == null) || (panelType == SonarPanelType.Void))
                return;

            CutLineSet clSet = (panelType == SonarPanelType.HF) ? clSetHF : clSetNF;
            DataEntry entry;
            LineData data;
            int max = sonarLines.Count;

            for (int i = 0; i < max; i++)
            {
                data = (panelType == SonarPanelType.HF) ? sonarLines[i].HF : sonarLines[i].NF;

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
        }
    

        public void DisableRemoveSelectedCut(bool remove)
        {
            DisableRemoveSelectedCut(SonarPanelType.HF, remove);
            DisableRemoveSelectedCut(SonarPanelType.NF, remove);

            GSC.Settings.Changed = true;

            RedrawSonarBmp();
        }

        private void DisableRemoveSelectedCut(SonarPanelType panelType, bool remove)
        {
            // Prepare and cut.
            if ((sonarLines == null) || (panelType == SonarPanelType.Void))
                return;

            CutLine cl = null;

            if (remove)
            {
                cl = GetCutLine(panelType);
                if (cl != null)
                    cl.RemoveSelected(Math.Min(disableSelStart, disableSelStop), Math.Max(disableSelStart, disableSelStop));
            }
            else
            {

                cl = GetCutLine(panelType, false, Classes.CutMode.Bottom);

                if (cl != null)
                    cl.DisableSelected(Math.Min(disableSelStart, disableSelStop), Math.Max(disableSelStart, disableSelStop));
            }
            // cl = GetCutLine(panelType, false, Classes.CutMode.Top);

            //if (cl != null)
            //    cl.DisableSelected(Math.Min(disableSelStart, disableSelStop), Math.Max(disableSelStart, disableSelStop));

            disableSelStart = 0;
            disableSelStop = 0;
        }

        public CutLine GetCutLine(SonarPanelType panelType)
        {
            return GetCutLine(panelType, false, cutMode);
        }

        public CutLine GetCutLine(SonarPanelType panelType, bool invPanelType, CutMode mode)
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

        public void SelectNode(PanelProp pp)
        {
            bool shiftHold = (ModifierKeys & Keys.Shift) != Keys.None;
            
            if (shiftHold)
                pp.pos = (pp.pos < pp.mid) ? pp.begin : pp.end;

            try
            {
                CutLine cl = GetCutLine(pp.panelType);
                if (cl != null)
                    selIndex = cl.GetNearestAt(pp.pos);
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarPanel.SelectNode: " + ex.Message + "\n" + ex.StackTrace);
            }

            Invalidate();
        }
        #endregion

        #region Changed settings
        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            // Unclear: Several global settings properties affect record view
            RedrawSonarBmp();
        } 
        #endregion
    }
}
