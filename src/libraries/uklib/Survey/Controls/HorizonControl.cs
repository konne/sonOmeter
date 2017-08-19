using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Drawing2D;

namespace UKLib.Survey.Controls
{
    public partial class HorizonControl : UKLib.Controls.InstrumentControl
    {
        public HorizonControl()
        {
            InitializeComponent();
            InitializeControl();
        }

        #region Variables
        private double elevation = 0;
        private double nelevation;

        private double bank = 0;
        private double nbank;

        private int innerSize = 20;
        private int fromBorder = 10;

        private float pitch = 30;
        private float num = 2;

        private Color skyColor = Color.DeepSkyBlue;
        private Color groundColor = Color.Chocolate;

        private bool showRect = true;
        private bool shade = true;
        private bool pry = false;
        #endregion

        #region Properties
        /// <summary>
        /// Set the distance of the inner ring from the border
        /// </summary>
        public int InnerSize
        {
            get { return innerSize; }
            set
            {
                if ((value <= 0) || (value > 100))
                    value = innerSize;
                innerSize = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the distance of the outer ring from the border
        /// </summary>
        public int FromBorder
        {
            get { return fromBorder; }
            set
            {
                fromBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the number of lines except equatorial line
        /// </summary>
        public float Num
        {
            get { return num; }
            set
            {
                if (value < 0)
                    value = -value;
                if (value * pitch >= 90)
                    value = 0;
                num = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the pich
        /// </summary>
        public float Pitch
        {
            get { return pitch; }
            set
            {
                if (value == 0)
                    value = pitch;
                if (value < 0)
                    value = -value;
                if (value * num >= 90)
                    num = 0;
                pitch = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Elevation
        /// </summary>
        public double Elevation
        {
            get { return elevation; }
            set
            {
                nelevation = value;
                while (nelevation >= 90) nelevation = nelevation - 360;
                while (nelevation < -90) nelevation = nelevation + 360;
                if ((!smooth) | Double.IsNaN(elevation)) elevation = nelevation;
                tmSmoothRedraw.Enabled = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Bank
        /// </summary>
        public double Bank
        {
            get { return bank; }
            set
            {
                nbank = value;
                while (nbank >= 90) nbank = nbank - 360;
                while (nbank < -90) nbank = nbank + 360;
                if ((!smooth) | Double.IsNaN(bank)) bank = nbank;
                tmSmoothRedraw.Enabled = true;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Color of the Sky
        /// </summary>
        public Color SkyColor
        {
            get { return skyColor; }
            set
            {
                skyColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Color of the Ground
        /// </summary>
        public Color GroundColor
        {
            get { return groundColor; }
            set
            {
                groundColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// If the rectangle aroung the compass is visible
        /// </summary>
        public bool ShowRect
        {
            get { return showRect; }
            set
            {
                showRect = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Enable shading
        /// </summary>
        public bool Shade
        {
            get { return shade; }
            set
            {
                shade = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Switch to Pitch/Roll/Yaw
        /// </summary>
        public bool PRY
        {
            get { return pry; }
            set
            {
                pry = value;
                Invalidate();
            }
        }
        #endregion

        #region Paint
        public override void PaintInstrument(Graphics g, ref UKLib.Controls.InstrumentControlPaintVars vars)
        {
            if (Double.IsNaN(elevation) || Double.IsNaN(bank))
                return;

            float scale = (float)innerSize * (float)vars.size / 100.0f;
            vars.pnFg.Width = 1.0F / 200.0F;

            #region Fonts
            Font fnt = new Font(Font.Name, (vars.size / 25.0F));
            #endregion

            g.SmoothingMode = SmoothingMode.AntiAlias;

            nfi.NumberDecimalDigits = 1;
            string s1 = bank.ToString("f", nfi);
            string s2 = elevation.ToString("f", nfi);

            if (pry)
            {
                s1 += "°\nRoll";
                s2 += "°\nPitch";
            }
            else
            {
                s1 += "°\nBank";
                s2 += "°\nElevation";
            }

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;

            vars.pnFg.Width *= (2 * (vars.size - scale));
            g.DrawEllipse(vars.pnFg, vars.left + fromBorder / 2, vars.top + fromBorder / 2, vars.size - fromBorder, vars.size - fromBorder);
            vars.pnFg.Width /= (2 * (vars.size - scale));

            SizeF sz1 = g.MeasureString(s1, fnt);
            g.DrawString(s1, fnt, vars.pnFg.Brush, vars.left + 1, vars.bottom - sz1.Height - 2);
            SizeF sz2 = g.MeasureString(s2, fnt);
            g.DrawString(s2, fnt, vars.pnFg.Brush, vars.right - 2, vars.bottom - sz2.Height - 2, sf);

            g.TranslateTransform(vars.mx, vars.my);

            vars.pnFg.Width *= (2 * (vars.size - scale));

            float x1big = (vars.size - fromBorder) / 2 - scale / 10;
            float x1small = (vars.size - fromBorder) / 2 - scale / 5;
            float x2 = (vars.size - scale) / 2;

            g.RotateTransform(180);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(45);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(35);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(5);
            g.DrawLine(vars.pnFg, x1small, 0, x2, 0);
            g.RotateTransform(5);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(5);
            g.DrawLine(vars.pnFg, x1small, 0, x2, 0);
            g.RotateTransform(5);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(35);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);
            g.RotateTransform(45);
            g.DrawLine(vars.pnFg, x1big, 0, x2, 0);

            vars.pnFg.Width /= (2 * (vars.size - scale));

            g.ScaleTransform(vars.size - scale, vars.size - scale);
            g.RotateTransform(-(float)bank);

            float el = -(float)elevation;
            float sinE = (float)System.Math.Sin(el * System.Math.PI / 180.0f);
            float corr = (el < 0) ? 0 : System.Math.Abs(sinE);

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            path.AddArc(-0.5f, -0.5f, 1, 1, 0, 180);

            if (el == 0)
                path.AddLine(-0.5f, 0, 0.5f, 0);
            else if (el < 0)
                path.AddArc(-0.5f, -sinE / 2 - System.Math.Abs(sinE) + corr, 1, System.Math.Abs(sinE), 0, 180);
            else
                path.AddArc(-0.5f, -sinE / 2 - System.Math.Abs(sinE) + corr, 1, System.Math.Abs(sinE), 180, 180);

            RectangleF rcFull = new RectangleF(-0.5F, -0.5F, 1, 1);
            g.FillEllipse(new SolidBrush(SkyColor), rcFull);
            g.FillPath(new SolidBrush(groundColor), path);

            for (float angle = -pitch * num; angle <= pitch * num; angle += pitch)
            {
                float cosA = (float)System.Math.Cos(angle * System.Math.PI / 180.0f);
                float sinA = (float)System.Math.Sin(angle * System.Math.PI / 180.0f);
                float sinEmA = (float)System.Math.Sin((el + angle) * System.Math.PI / 180.0f);
                float delta = (el < 0) ? -90 : 90;

                corr = (el < 0) ? 0 : System.Math.Abs(cosA * sinE);

                if (el == 0)
                {
                    g.DrawLine(vars.pnFg, -cosA / 2, -sinA / 2, cosA / 2, -sinA / 2);
                }
                else
                {
                    float phi = 180;

                    if (angle > 0)
                    {
                        phi = 180 * (1 + 1 - (float)System.Math.Sqrt(1 - System.Math.Pow(el / (angle - 90), 2)));

                        if (elevation > 0)
                            phi = 360 - phi;
                        else if (float.IsNaN(phi))
                            phi = 360;
                    }
                    else if (angle < 0)
                    {
                        phi = 180 * ((float)System.Math.Sqrt(1 - System.Math.Pow(el / (angle + 90), 2)));

                        if (elevation > 0)
                        {
                            if (float.IsNaN(phi))
                                phi = 360;
                            else
                                phi = 360 - phi;
                        }
                    }
                    else
                    {
                        if (System.Math.Abs(elevation) == 90)
                            phi = 360;
                    }

                    if (float.IsNaN(phi))
                        continue;

                    g.DrawArc(vars.pnFg, -cosA / 2, -sinEmA / 2 - System.Math.Abs(cosA * sinE) + corr, cosA, System.Math.Abs(cosA * sinE), (360 - phi) / 2 + delta, phi);
                }
            }

            if (shade)
            {
                path = new GraphicsPath();
                path.AddEllipse(rcFull);
                PathGradientBrush gradBrush = new PathGradientBrush(path);
                Color[] c = { Color.FromArgb(150, Color.Black) };
                gradBrush.CenterColor = Color.FromArgb(100, Color.White);
                gradBrush.CenterPoint = new PointF(-0.17F, -0.2F);
                gradBrush.SurroundColors = c;
                g.FillEllipse(gradBrush, rcFull);
            }

            vars.pnFg.Width *= 2;

            g.DrawEllipse(vars.pnFg, rcFull);
            g.RotateTransform((float)bank);

            g.DrawLine(vars.pnFg, -0.25f, 0, 0.25f, 0);
            g.DrawLine(vars.pnFg, 0, -0.25f, 0, 0.25f);
        }
        #endregion

        #region Timer
        protected override void SmoothAngle(ref double angle, double nangle, double threshold)
        {
            base.SmoothAngle(ref angle, nangle, threshold);

            while (angle > 90) angle = angle - 360;
            while (angle < -90) angle = angle + 360;
        }

        protected override void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SmoothAngle(ref elevation, nelevation, 2.0);
            SmoothAngle(ref bank, nbank, 2.0);
            Invalidate();

            if (((int)elevation == (int)nelevation) && ((int)bank == (int)nbank)) tmSmoothRedraw.Enabled = false;
        }
        #endregion
    }
}
