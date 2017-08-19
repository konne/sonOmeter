using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Parse;
using System.Drawing.Drawing2D;

namespace UKLib.Survey.Controls
{
    public partial class SatControl : UKLib.Controls.InstrumentControl
    {
        public SatControl()
        {
            InitializeComponent();
            InitializeControl();
        }

        #region Variables
        private SatSNR[] satList = new SatSNR[128];

        private int innerSize = 30;
        private float bubbleSize = 0.2f;

        private int satNum = 0;
        private int satQuality = 0;
        private int satNumMin = 0;
        private int satQualityMin = 0;
        private float satQualityMeter = 0.0F;
        private float satQualityMeterMin = 0.0F;
        #endregion

        #region Properties
   
        private Color colorNotUsedGPS = Color.Red;
        public Color ColorNotUsedGPS
        {
            get
            {
                return colorNotUsedGPS;
            }
            set
            {
                colorNotUsedGPS = value;
            }
        }

        private Color colorNotUsedGlonas = Color.OrangeRed;
        public Color ColorNotUsedGlonas
        {
            get
            {
                return colorNotUsedGlonas;
            }
            set
            {
                colorNotUsedGlonas = value;
            }
        }

        private Color colorInUsedGPS = Color.Lime;
        public Color ColorInUsedGPS
        {
            get
            {
                return colorInUsedGPS;
            }
            set
            {
                colorInUsedGPS = value;
            }
        }

        private Color colorInUsedGlonas = Color.LightGreen;
        public Color ColorInUsedGlonas
        {
            get
            {
                return colorInUsedGlonas;
            }
            set
            {
                colorInUsedGlonas = value;
            }
        }


        private Color colorTryUsedGPS = Color.Yellow;
        public Color ColorTryUsedGPS
        {
            get
            {
                return colorTryUsedGPS;
            }
            set
            {
                colorTryUsedGPS = value;
            }
        }

        private Color colorTryUsedGlonas = Color.YellowGreen;
        public Color ColorTryUsedGlonas
        {
            get
            {
                return colorTryUsedGlonas;
            }
            set
            {
                colorTryUsedGlonas = value;
            }
        }

        /// <summary>
        /// Set the number of sats
        /// </summary>
        public int SatNum
        {
            get { return satNum; }
            set
            {
                satNum = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the quality of the sats
        /// </summary>
        public int SatQuality
        {
            get { return satQuality; }
            set
            {
                satQuality = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Min number of sats
        /// </summary>
        public int SatNumMin
        {
            get { return satNumMin; }
            set
            {
                satNumMin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Min quality of the sats
        /// </summary>
        public int SatQualityMin
        {
            get { return satQualityMin; }
            set
            {
                satQualityMin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Min MeterQuality of the sats
        /// </summary>
        public float SatQualityMeter
        {
            get { return satQualityMeter; }
            set
            {
                satQualityMeter = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Min MeterQuality of the sats
        /// </summary>
        public float SatQualityMeterMin
        {
            get { return satQualityMeterMin; }
            set
            {
                satQualityMeterMin = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the distance of the inner ring from the border
        /// </summary>
        public int InnerSize
        {
            get { return innerSize; }
            set
            {
                innerSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Azimuth
        /// </summary>
        public SatSNR[] SatList
        {
            get { return satList; }
            set
            {
                satList = value;
                Invalidate();
            }
        }
        #endregion

        #region Paint
        public override void PaintInstrument(Graphics g, ref UKLib.Controls.InstrumentControlPaintVars vars)
        {
            float scale = (float)innerSize * (float)vars.size / 100.0f;

            Font fnt = new Font(Font.Name, 0.1f);
            Font fnt2 = new Font(Font.Name, 0.06f * (vars.size - scale));
            StringFormat sf = new StringFormat();

            string s1 = satNum.ToString() + "\nCount";
            string s2 = satQuality.ToString() + "\nQuality";
            string s3 = "MQuality\n" + satQualityMeter.ToString();
            sf.Alignment = StringAlignment.Far;

            SizeF sz1 = g.MeasureString(s1, fnt2);
            if (satNum < satNumMin)
                vars.pnFg.Color = Color.Red;
            else
                vars.pnFg.Color = this.ForeColor;
            g.DrawString(s1, fnt2, vars.pnFg.Brush, vars.left + 1, vars.bottom - sz1.Height - 2);
            SizeF sz2 = g.MeasureString(s2, fnt2);
            if (satQuality < satQualityMin)
                vars.pnFg.Color = Color.Red;
            else
                vars.pnFg.Color = this.ForeColor;
            g.DrawString(s2, fnt2, vars.pnFg.Brush, vars.right - 2, vars.bottom - sz2.Height - 2, sf);

            SizeF sz3 = g.MeasureString(s3, fnt2);
            if (satQualityMeter > satQualityMeterMin)
                vars.pnFg.Color = Color.Red;
            else
                vars.pnFg.Color = this.ForeColor;
            g.DrawString(s3, fnt2, vars.pnFg.Brush, vars.right - 2, vars.top - 2, sf);

            vars.pnFg.Color = this.ForeColor;
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            if (showFrame) g.DrawRectangle(vars.pnFg, vars.left, vars.top, vars.size - 1, vars.size - 1);

            g.TranslateTransform(vars.mx, vars.my);
            g.ScaleTransform((vars.size - scale) / 2, (vars.size - scale) / 2);

            vars.pnFg.Width /= (vars.size - scale);

            g.DrawEllipse(vars.pnFg, -1, -1, 2, 2);
            g.DrawEllipse(vars.pnFg, -0.5f, -0.5f, 1, 1);

            RectangleF rcBubble;

            rcBubble = new RectangleF(-1 - bubbleSize / 2, -bubbleSize / 2, bubbleSize, bubbleSize);
            g.FillRectangle(new SolidBrush(this.BackColor), rcBubble);
            g.DrawString("W", fnt, vars.pnFg.Brush, rcBubble, sf);
            rcBubble = new RectangleF(-bubbleSize / 2, -1 - bubbleSize / 2, bubbleSize, bubbleSize);
            g.FillRectangle(new SolidBrush(this.BackColor), rcBubble);
            g.DrawString("N", fnt, vars.pnFg.Brush, rcBubble, sf);
            rcBubble = new RectangleF(1 - bubbleSize / 2, -bubbleSize / 2, bubbleSize, bubbleSize);
            g.FillRectangle(new SolidBrush(this.BackColor), rcBubble);
            g.DrawString("E", fnt, vars.pnFg.Brush, rcBubble, sf);
            rcBubble = new RectangleF(-bubbleSize / 2, 1 - bubbleSize / 2, bubbleSize, bubbleSize);
            g.FillRectangle(new SolidBrush(this.BackColor), rcBubble);
            g.DrawString("S", fnt, vars.pnFg.Brush, rcBubble, sf);

            vars.pnFg.Color = Color.Black;

            for (int i = 0; i < 128; i++)
            {
                SatSNR sat = satList[i];

                if ((sat.Elevation < 0) || (sat.Elevation > 90))
                    continue;

                float r = 1 - (float)sat.Elevation / 90.0f;
                float x = r * (float)System.Math.Sin((float)sat.Azimuth * System.Math.PI / 180.0f);
                float y = r * -(float)System.Math.Cos((float)sat.Azimuth * System.Math.PI / 180.0f);

                rcBubble = new RectangleF(x - bubbleSize / 2, y - bubbleSize / 2, bubbleSize, bubbleSize);

                switch (sat.Status)
                {
                    case SatStatus.NotUsed:
                        if (i > 64)
                            g.FillEllipse(new SolidBrush(colorNotUsedGlonas), rcBubble);
                        else
                            g.FillEllipse(new SolidBrush(colorNotUsedGPS), rcBubble);
                        break;
                    case SatStatus.InUse:
                        if (i > 64)
                            g.FillEllipse(new SolidBrush(colorInUsedGlonas), rcBubble);
                        else
                            g.FillEllipse(new SolidBrush(colorInUsedGPS), rcBubble);

                        break;
                    case SatStatus.TryUse:
                        if (i > 64)
                            g.FillEllipse(new SolidBrush(colorTryUsedGlonas), rcBubble);
                        else
                            g.FillEllipse(new SolidBrush(colorTryUsedGPS), rcBubble);
                        break;
                }
                g.DrawEllipse(vars.pnFg, rcBubble);
                g.DrawString((i + 1).ToString(), fnt, vars.pnFg.Brush, rcBubble, sf);
            }
        }      
        #endregion

        #region MouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int size = this.Width;
            if (Height < size)
                size = this.Height;
            size -= 5;
            int mx = this.Width / 2;
            int my = this.Height / 2;
            float scale = (float)innerSize * (float)size / 100.0f;

            Matrix mat = new Matrix();
            mat.Translate(mx, my);
            mat.Scale((size - scale) / 2, (size - scale) / 2);
            mat.Invert();

            PointF[] pts = new PointF[1];
            pts[0] = new PointF(e.X, e.Y);
            mat.TransformPoints(pts);
            RectangleF rcBubble;

            for (int i = 0; i < 32; i++)
            {
                SatSNR sat = satList[i];

                if ((sat.Elevation < 0) || (sat.Elevation > 90))
                    continue;

                float r = 1 - (float)sat.Elevation / 90.0f;
                float x = r * (float)System.Math.Sin((float)sat.Azimuth * System.Math.PI / 180.0f);
                float y = r * -(float)System.Math.Cos((float)sat.Azimuth * System.Math.PI / 180.0f);

                rcBubble = new RectangleF(x - bubbleSize / 2, y - bubbleSize / 2, bubbleSize, bubbleSize);

                if (rcBubble.Contains(pts[0]))
                {
                    toolTip.SetToolTip(this, "SNR: " + sat.SNR.ToString());
                    toolTip.Active = true;
                    return;
                }
            }

            toolTip.Active = false;
        }
        #endregion
    }
}
