using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace UKLib.Controls
{
    public struct InstrumentControlPaintVars
    {
        #region Variables
        public int size;
        public int mx;
        public int my;
        public int top;
        public int left;
        public int bottom;
        public int right;
        public Pen pnFg;
        public SolidBrush sbFg;
        public SolidBrush sbText;
        public StringFormat sfRight2Left; 
        #endregion

        #region Constructor
        public InstrumentControlPaintVars(int width, int height, Color foreColor, Color textColor)
        {
            size = width;
            if (height < size) size = height;
            size -= 5;

            mx = width >> 1;
            my = height >> 1;

            top = (height - size) >> 1;
            left = (width - size) >> 1;
            bottom = height - top;
            right = width - left;

            pnFg = new Pen(foreColor, 1);
            sbFg = new SolidBrush(foreColor);
            sbText = new SolidBrush(textColor);
            sfRight2Left = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        } 
        #endregion
    }

    public partial class InstrumentControl : UserControl
    {
        #region Variables
        protected int minSize = 100;

        protected bool smooth = true;
        protected bool showFrame = true;

        protected Color textColor = Color.White;
        protected Color instrumentColor = Color.Black;

        protected NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        #region Properties
        [Description("Specifies the minimum size (width and height) of this control."), DisplayName("Minimum Size"), DefaultValue(100), Category("Instrument")]
        public int MinSize
        {
            get { return minSize; }
            set { minSize = value; OnResize(null); }
        }

        [Description("Toggles the value smoothing engine."), DisplayName("Smooth Values"), DefaultValue(true), Category("Instrument")]
        public bool Smooth
        {
            get { return smooth; }
            set { smooth = value; }
        }

        [Description("Toggles the display of a frame."), DisplayName("Show Frame"), DefaultValue(true), Category("Instrument")]
        public bool ShowFrame
        {
            get { return showFrame; }
            set
            {
                showFrame = value;
                Invalidate();
            }
        }

        [Description("Specifies the text color."), DisplayName("Text Color"), DefaultValue(KnownColor.White), Category("Instrument")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                Invalidate();
            }
        }

        [Description("Specifies the instrument color."), DisplayName("Instrument Color"), DefaultValue(KnownColor.Black), Category("Instrument")]
        public Color InstrumentColor
        {
            get { return instrumentColor; }
            set
            {
                instrumentColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public NumberFormatInfo NFI
        {
            get { return nfi; }
            set
            {
                nfi = value;
                Invalidate();
            }
        }
        #endregion

        #region Initialization
        public InstrumentControl()
        {
            InitializeComponent();

            InitializeControl();
        }

        protected virtual void InitializeControl()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        } 
        #endregion

        #region Parameter Smoothing
        protected virtual void SmoothAngle(ref double angle, double nangle, double threshold)
        {
            double diff = nangle - angle;

            if (System.Math.Abs(diff) > 180) diff = -System.Math.Sign(diff) * (360 - System.Math.Abs(diff));

            SmoothValue(ref angle, nangle, threshold, diff);
        }

        protected virtual void SmoothValue(ref double value, double nvalue, double threshold)
        {
            double diff = nvalue - value;

            SmoothValue(ref value, nvalue, threshold, diff);
        }

        protected void SmoothValue(ref double value, double nvalue, double threshold, double diff)
        {
            if (value == nvalue)
                return;

            diff /= 3.0;

            double diffAbs = System.Math.Abs(diff);
            double thresholdH = threshold / 2.0;

            if (diffAbs < thresholdH) diff /= (diffAbs / thresholdH);

            value += diff;

            if (System.Math.Abs(nvalue - value) < threshold) value = nvalue;
        }

        protected virtual void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e) { } 
        #endregion

        #region Resize and Paint
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            InstrumentControlPaintVars vars = new InstrumentControlPaintVars(this.Width, this.Height, this.ForeColor, textColor);

            g.Clear(instrumentColor);

            if (showFrame) g.DrawRectangle(vars.pnFg, vars.left, vars.top, vars.size - 1, vars.size - 1);

            PaintInstrument(g, ref vars);
        }

        public virtual void PaintInstrument(Graphics g, ref InstrumentControlPaintVars vars)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Height < minSize) this.Height = minSize;
            if (this.Width < minSize) this.Width = minSize;

            Invalidate();
        } 
        #endregion
    }
}
