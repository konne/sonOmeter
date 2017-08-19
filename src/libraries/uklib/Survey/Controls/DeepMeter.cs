using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using UKLib.Debug;

namespace UKLib.Survey.Controls
{
    #region Enum
    public enum OrientationType 
    {
        Left, Right, Top, Bottom
    }
    #endregion

    public partial class DeepMeter : UserControl
    {
        #region Constructor
        public DeepMeter()
        {
            InitializeComponent();
            nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalDigits = 1;
        }
        #endregion

        #region Variables
        int stubLen = 5;
        float min = -100;
        float max = 0;
        OrientationType orientation;
        private System.Globalization.NumberFormatInfo nfi;
        private float scale = 1;

        BackgroundWorker<object, Bitmap> bwDrawBmp = null;
        Bitmap bmpBW = null;
        Bitmap bmp = null;
        bool bwNeedStartAgain = false;
        #endregion

        #region Properties
        public int StubLen
        {
            get { return stubLen; }
            set
            {
                if (value >= 0)
                {
                    stubLen = value;                    
                }
            }
        }

        public float Min
        {
            get { return min; }
            set
            {
                min = value;
                RedrawBmp();                
            }
        }

        public float Max
        {
            get { return max; }
            set
            {
                max = value;
                RedrawBmp();                
            }
        }

        public OrientationType Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
            }
        }

        public int Digits
        {
            get { return nfi.NumberDecimalDigits; }
            set
            {
                if (value >=0) 
                {
                    nfi.NumberDecimalDigits = value;                  
                }
            }
        }
        #endregion

        #region Stubs
        private void DrawStub(Graphics g, Pen pen, float pos, int len)
        {
            float posp = (pos - min) / scale; 
            switch (orientation)
            {
                case OrientationType.Left:
                    g.DrawLine(pen, 0, posp, len, posp);
                    break;
                case OrientationType.Right :
                    g.DrawLine(pen, Width, posp, Width-len, posp);
                    break;
                case OrientationType.Top:
                    g.DrawLine(pen, posp, 0, posp, len);
                    break;
                case OrientationType.Bottom:
                    g.DrawLine(pen, posp, Height, posp, Height-len);
                    break;
            }
        }

        private void DrawStubText(Graphics g, Brush brush, float pos)
        {
            float posp = (pos - min) / scale;

            StringFormat sfRight2Left = new StringFormat(StringFormatFlags.DirectionVertical);
            string s = pos.ToString("f", nfi);
            SizeF size = g.MeasureString(s, Font);

            
            if ((orientation == OrientationType.Left) | (orientation == OrientationType.Right))
            {
                if ((posp + size.Height / 2) >= (Height - 1)) 
                    posp = Height - size.Height;
                else
                    posp = posp - size.Height / 2;
            }
            else
            {
                if ((posp + size.Height / 2) >= (Width - 1)) 
                    posp = Width - size.Height;
                else
                    posp = posp - size.Height / 2;
            }
            if (posp < 0) posp = 0;

            switch (orientation)
            {
                case OrientationType.Left:

                    g.DrawString(s, Font, brush, stubLen + 1, posp);
                    break;
                case OrientationType.Right:
                    g.DrawString(s, Font, brush, Width - stubLen - 1 - size.Width, posp);
                    break;
                case OrientationType.Top:
                    g.RotateTransform(-90);
                    g.DrawString(s, Font, brush, -size.Width - stubLen - 1, posp);
                    g.ResetTransform();
                    break;
                case OrientationType.Bottom:
                    g.RotateTransform(-90);
                    g.DrawString(s, Font, brush, -Height + stubLen + 1, posp);
                    g.ResetTransform();
                    break;
            }
        }
        #endregion

        #region onPaint
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                int w = this.Width;
                int h = this.Height;

                if ((w <= 0) || (h <= 0) || (bmp == null))
                    return;

                // Get graphics object.
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
               // g.Clear(this.BackColor);

                if ((w != bmp.Width) || (h != bmp.Height))
                    g.DrawImage(bmp, 0, 0, w, h);
                else
                    g.DrawImageUnscaled(bmp, 0, 0, w, h);
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "DeepMeter.OnPaint: " + ex.Message + "\n" + ex.StackTrace);
            }
        }
        #endregion

        #region Redraw background worker
        #region Start
        public void RedrawBmp()
        {
            if (bwDrawBmp == null)
            {
                bwDrawBmp = new BackgroundWorker<object, Bitmap>();
                bwDrawBmp.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<Bitmap>>(bwDrawBmp_RunWorkerCompleted);
                bwDrawBmp.DoWork += new DoWorkEventHandler<DoWorkEventArgs<object, Bitmap>>(bwDrawBmp_DoWork);
            }
            if (!bwDrawBmp.IsBusy)
            {
                bwDrawBmp.RunWorkerAsync();
            }
            else
            {
                bwNeedStartAgain = true;
            }
        }
        #endregion

        #region Completed
        void bwDrawBmp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs<Bitmap> e)
        {
            if (e.Result != null)
                bmp = e.Result;

            Invalidate();

            if (bwNeedStartAgain)
            {
                bwNeedStartAgain = false;
                RedrawBmp();
            }
        }
        #endregion

        #region Do work
        void bwDrawBmp_DoWork(object sender, DoWorkEventArgs<object, Bitmap> e)
        {
            try
            {
                int w = this.Width;
                int h = this.Height;

                if ((w <= 0) || (h <= 0))
                    return;

                // Rebuild bitmap structure if needed.
                if ((bmpBW == null) || (w != bmpBW.Width) || (h != bmpBW.Height))
                {
                    if (bmpBW != null)
                        bmpBW.Dispose();
                    bmpBW = new Bitmap(w, h);
                }

                // Get graphics object.
                Graphics g = Graphics.FromImage(bmpBW);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Flush old bitmap.
                g.Clear(this.BackColor);

                // Draw.
                if ((orientation == OrientationType.Left) | (orientation == OrientationType.Right))
                    scale = (float)((max - min) / (Height - 1));
                else
                    scale = (float)((max - min) / (Width - 1));

                SolidBrush brFCol = new SolidBrush(ForeColor);
                SolidBrush brBCol = new SolidBrush(BackColor);
                Pen pnFCol = new Pen(ForeColor);
                Pen pnBCol = new Pen(BackColor);

                SizeF size;
                size = g.MeasureString(min.ToString("f", nfi), Font);

                int fac = ((int)System.Math.Pow(10, nfi.NumberDecimalDigits));

                float fc = System.Math.Abs(1F / (scale * (float)fac));

                int imin = (int)(min * fac);
                int imax = (int)(max * fac);
                int df = 1;
                if (imax - imin < 0) df = -1;
                int i = imin;

                int count = 0;
                imax += df;

                int digits = Digits + 2;

                while ((i != imax) & (count < 80000))
                {
                    float ft = 1;
                    for (int j = 0; j < (digits); j++)
                    {
                        if (((i % (5 * ft) == 0) & ((fc * ft) > 5) & ((fc * ft) <= 15)) | ((i % (2 * ft) == 0) & ((fc * ft) > 15)) | ((i % (1 * ft) == 0) & ((fc * ft) > 25)))
                        {
                            DrawStubText(g, brFCol, (float)i / (float)fac);
                            DrawStub(g, pnFCol, (float)i / (float)fac, stubLen);
                        }
                        else
                        {
                            if (((i % (1 * ft) == 0) & ((fc * ft) > 5)) | ((i % (2 * ft) == 0) & ((fc * ft) > 2.5)))
                                DrawStub(g, pnFCol, (float)i / (float)fac, stubLen / 2);
                        }
                        ft = ft * 10;
                    }


                    i += df;
                    count++;
                }

                g.Dispose();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "DeepMeter.RedrawBmp: " + ex.Message);
            }
            finally
            {
                // Create result.
                e.Result = (bmpBW != null) ? (Bitmap)bmpBW.Clone() : null;
            }
        }
        #endregion
        #endregion
    }
}
