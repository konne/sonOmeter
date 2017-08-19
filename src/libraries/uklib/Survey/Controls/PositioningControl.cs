using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace UKLib.Survey.Controls
{
    public partial class PositioningControl : UKLib.Controls.InstrumentControl
    {
        public PositioningControl()
        {
            InitializeComponent();
            InitializeControl();
        }

        #region Variables
        private double distance = 0;
        private double ndistance = 0;

        private double station = 0;
        private double nstation = 0;

        private double depth = 0;
        private double ndepth = 0;

        private string depthType = "HF";
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the distance
        /// </summary>
        public double Distance
        {
            get { return distance; }
            set
            {
                ndistance = value;
                if (!smooth) distance = ndistance;
                tmSmoothRedraw.Enabled = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Get or set the station
        /// </summary>
        public double Station
        {
            get { return station; }
            set
            {
                nstation = value;
                if (!smooth) station = nstation;
                tmSmoothRedraw.Enabled = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Get or set the depth
        /// </summary>
        public double Depth
        {
            get { return depth; }
            set
            {
                ndepth = value;
                if (!smooth) depth = ndepth;
                tmSmoothRedraw.Enabled = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Get or set the depth type string (HF or NF)
        /// </summary>
        public string DepthType
        {
            get { return depthType; }
            set
            {
                depthType = value;
                Invalidate();
            }
        }
        #endregion

        #region Paint
        public override void PaintInstrument(Graphics g, ref UKLib.Controls.InstrumentControlPaintVars vars)
        {
            float scale = vars.size / 500F;
            int brd = 5;

            #region Fonts
            Font fnt = new Font(this.Font.Name, (scale * 40.0F));
            SizeF sz = g.MeasureString("-1000000.00", fnt);
            #endregion

            g.TranslateTransform(vars.mx, vars.my - 2.0F * (sz.Height + brd));

            DrawValue("Station", station, g, vars, brd, fnt, sz);
            DrawValue("Distance", distance, g, vars, brd, fnt, sz);
            DrawValue("Depth (" + depthType + ")", depth, g, vars, brd, fnt, sz);
        }

        private void DrawValue(string header, double value, Graphics g, UKLib.Controls.InstrumentControlPaintVars vars, int brd, Font fnt, SizeF sz)
        {
            StringFormat sfF = new StringFormat();
            sfF.Alignment = StringAlignment.Far;
            StringFormat sfC = new StringFormat();
            sfC.Alignment = StringAlignment.Center;

            g.DrawRectangle(vars.pnFg, -(sz.Width + brd) / 2.0F, -brd / 2.0F, sz.Width + brd, sz.Height + brd);

            if (!Double.IsNaN(value))
            {
                string str = value.ToString("F2", nfi);

                g.DrawString(header, fnt, vars.sbText, 0, -(sz.Height + brd / 2.0F), sfC);
                g.DrawString(str, fnt, vars.sbText, sz.Width / 2.0F, 0, sfF);
            }

            g.TranslateTransform(0, 2.0F * (sz.Height + brd));
        }
        #endregion

        #region Smooth Timer
        protected override void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SmoothValue(ref distance, ndistance, 0.02);
            SmoothValue(ref station, nstation, 0.02);
            SmoothValue(ref depth, ndepth, 0.02);

            if ((distance == ndistance) && (station == nstation) && (depth == ndepth))
                tmSmoothRedraw.Enabled = false;
            
            Invalidate();
        }
        #endregion
    }
}
