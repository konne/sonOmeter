using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UKLib.Survey.Controls
{
    public partial class UbootControl : UserControl
    {
        #region Constructor
        public UbootControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        #endregion

        #region Variables & Properties

        bool speedmode = false;
        public bool SpeedMode
        {
            get { return speedmode; }
            set
            {
                speedmode = value;
                Invalidate();
            }
        }

        private float speed;
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                if (speed > 10) speed = 10;
                if (speed < -10) speed = -10;
                Invalidate();
            }
        }

        private float leftRigth;
        public float LeftRight
        {
            get { return leftRigth; }
            set
            {
                leftRigth = value;
                if (leftRigth > 10) leftRigth = 10;
                if (leftRigth < -10) leftRigth = -10;
                Invalidate();
            }
        }

        private float upDown;

        public float UpDown
        {
            get { return upDown; }
            set
            {
                upDown = value;
                if (upDown > 10) upDown = 10;
                if (upDown < -10) upDown = -10;
                Invalidate();
            }
        }

        private bool pump;
        public bool Pump
        {
            get { return pump; }
            set
            {
                pump = value;
                Invalidate();
            }
        }

        private bool valve;
        public bool Valve
        {
            get { return valve; }
            set
            {
                valve = value;
                Invalidate();
            }
        }

        private float deep;
        public float Deep
        {
            get { return deep; }
            set
            {
                deep = value;
                Invalidate();
            }
        }

        private float deepChange;
        public float DeepChange
        {
            get { return deepChange; }
            set
            {
                deepChange = value;
                Invalidate();
            }
        }

        private bool lightL;

        public bool LightL
        {
            get { return lightL; }
            set
            {
                lightL = value;
                Invalidate();
            }
        }

        private bool lightR;

        public bool LightR
        {
            get { return lightR; }
            set
            {
                lightR = value;
                Invalidate();
            }
        }


        int size = 0;

        #endregion

        #region OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            StringFormat sfRight2Left = new StringFormat(StringFormatFlags.DirectionRightToLeft);
            SolidBrush sbFg = new SolidBrush(ForeColor);
            SolidBrush sbText = new SolidBrush(ForeColor);

            size = Width;
            if (Height < size) size = Height;
            size -= 5;
            int mx = Width / 2;
            int my = Height / 2;
            int top = (Height - size) / 2;
            int left = (Width - size) / 2;
            float scale = size / 500F;
            Pen pnFg = new Pen(ForeColor, 1);
            Pen pnSc = new Pen(ForeColor, (scale * 2.0F + 1));

            bool showRect = true;
            if (showRect) g.DrawRectangle(pnFg, left, top, size - 1, size - 1);

            Rectangle rectLR = new Rectangle(left, top, size, size / 5);
            Rectangle rectUD = new Rectangle(left, top + rectLR.Height, size / 5, size - rectLR.Height);
            Rectangle rectSP = new Rectangle(left + size - size / 5, rectUD.Top, rectUD.Width, rectUD.Height);
            Rectangle rectDP = new Rectangle(left + rectUD.Width, rectUD.Top, size - rectUD.Width * 2, rectUD.Height / 4);
            //rectDP.Offset(0, size / 10);
            Rectangle rectDC = rectDP;
            rectDC.Offset(0, size / 5);

            Rectangle rectPV = rectDC;
            rectPV.Offset(0, size / 5);
            //rectPV.Inflate(0, -size / 30);

            Rectangle rectLG = rectPV;
            rectLG.Offset(0, size / 5);

            #region Deep
            rectDP.Inflate(-5, 0);
            g.DrawRectangle(pnFg, rectDP);

            Font fntDeep = new Font(Font.Name, (scale * 55.0F));
            SizeF szDeep = g.MeasureString("300,0m", fntDeep);

            string sdeep = deep.ToString("0.0").Replace(",", ".") + "m";
            g.DrawString(sdeep, fntDeep, sbText, rectDP.X + rectDP.Width, rectDP.Y + (rectDP.Height - szDeep.Height) / 2, sfRight2Left);
            #endregion

            #region DeepChange
            rectDC.Inflate(-5, 0);
            g.DrawRectangle(pnFg, rectDC);

            Font fntDeepCH = new Font(Font.Name, (scale * 40.0F));
            SizeF szDeepCH = g.MeasureString("3,00m/s", fntDeepCH);

            string sdeepch = System.Math.Abs(deepChange).ToString("0.00").Replace(",", ".") + "m/s";
            g.DrawString(sdeepch, fntDeepCH, sbText, rectDC.X + rectDC.Width, rectDC.Y + (rectDP.Height - szDeepCH.Height) / 2, sfRight2Left);


            float aw = size / 10;
            float atop = rectDC.Y + (rectDC.Height - aw) / 2;
            float aleft = rectDC.X + aw / 2;
            PointF[] arrowU = new PointF[3];
            arrowU[0] = new PointF(aleft + aw / 2, atop);
            arrowU[1] = new PointF(aleft, atop + aw);
            arrowU[2] = new PointF(aleft + aw, atop + aw);

            PointF[] arrowD = new PointF[3];
            arrowD[0] = new PointF(aleft + aw / 2, atop + aw);
            arrowD[1] = new PointF(aleft, atop);
            arrowD[2] = new PointF(aleft + aw, atop);

            if (System.Math.Abs(deepChange) > 0.001F)
            {
                if (deepChange >= 0)
                    g.FillPolygon(sbFg, arrowD);
                else
                    g.FillPolygon(sbFg, arrowU);
            }
            #endregion

            #region Light
           
            rectLG.Inflate(-5, 0);
            g.DrawRectangle(pnFg, rectLG);
            Font fntLight = new Font(Font.Name, (scale * 35.0F));
            SizeF szLightL = g.MeasureString("LightL", fntLight);
            SizeF szLightR = g.MeasureString("LightR", fntLight);


            RectangleF rectLiL = rectLG;
            rectLiL.Width = rectLG.Width / 2;
            rectLiL.Inflate(-2, -2);

            RectangleF rectLiR = rectLG;
            rectLiR.Width = rectLG.Width / 2;
            rectLiR.X = rectLG.X + rectLG.Width - rectLG.Width / 2;
            rectLiR.Inflate(-2, -2);
            if (lightL) g.FillRectangle(new SolidBrush(Color.Green), rectLiL);
            if (lightR) g.FillRectangle(new SolidBrush(Color.Green), rectLiR);

            g.DrawString("LightL", fntLight, sbText, rectLG.X, rectLG.Y + (rectLiL.Height - szLightL.Height) / 2);
            g.DrawString("LightR", fntLight, sbText, rectLG.X + rectLG.Width - szLightL.Width, rectLG.Y + (rectLiL.Height - szLightL.Height) / 2);
            
            #endregion

            #region Pump Valve
            rectPV.Inflate(-5, 0);
            g.DrawRectangle(pnFg, rectPV);
            Font fntPV = new Font(Font.Name, (scale * 35.0F));
            SizeF szPump = g.MeasureString("Pump", fntPV);
            SizeF szValve = g.MeasureString("Valve", fntPV);


            RectangleF rectP = rectPV;
            rectP.Width = rectPV.Width / 2;
            rectP.Inflate(-2, -2);

            RectangleF rectV = rectPV;
            rectV.Width = rectPV.Width / 2;
            rectV.X = rectPV.X + rectPV.Width - rectPV.Width / 2;
            rectV.Inflate(-2, -2);
            if (pump) g.FillRectangle(new SolidBrush(Color.Red), rectP);
            if (valve) g.FillRectangle(new SolidBrush(Color.Red), rectV);

            g.DrawString("Pump", fntPV, sbText, rectPV.X, rectPV.Y + (rectDP.Height - szPump.Height) / 2);
            g.DrawString("Valve", fntPV, sbText, rectPV.X + rectPV.Width - szValve.Width, rectPV.Y + (rectDP.Height - szPump.Height) / 2);
            #endregion

            #region Scales
            //g.DrawRectangle(pnFg, rectLR);
            //g.DrawRectangle(pnFg, rectUD);
            //g.DrawRectangle(pnFg, rectSP); 

            DrawScale(g, rectLR, leftRigth, false, Color.Red);

            if (!speedmode)
                DrawScale(g, rectSP, -speed, true, Color.Red);
            else
                DrawScale(g, rectSP, -speed, true, Color.LightGreen);

            DrawScale(g, rectUD, upDown, true, Color.Red);

            Font fntSpeed = new Font(Font.Name, (scale * 20.0F));
            SizeF szSpeed = g.MeasureString("100", fntSpeed);
            g.DrawString((speed*10).ToString("0"), fntSpeed, sbText, rectSP.Left+(rectSP.Width-szSpeed.Width)/2, rectSP.Top-szSpeed.Height/2);            
            #endregion
        }
        #endregion

        #region DrawScale
        private void DrawScale(Graphics g, Rectangle rect, float Value, bool rotate, Color col)
        {
            Pen pnFg = new Pen(ForeColor, 1);
            Pen pnFg2 = new Pen(ForeColor, 2);
            g.TranslateTransform(rect.Left, rect.Top);
            float height = rect.Height;
            float width = rect.Width;

            // g.TranslateTransform(width / 2, height / 2);

            if (rotate)
            {
                g.RotateTransform(90);
                height = rect.Width;
                width = rect.Height;
                g.TranslateTransform(0, -height);
            }

            float LRy1 = height / 3F;
            float LRy2 = LRy1 + height / 3F;
            float LRym = height / 2F;
            g.DrawLine(pnFg, width / 22F * 1, LRym, width / 22F * 21, LRym);
            for (int i = 1; i <= 21; i++)
            {
                float LRx = width / 22F * i;
                if ((i != 1) & (i != 11) & (i != 21))
                    g.DrawLine(pnFg, LRx, LRy1 + size / 60, LRx, LRy2 - size / 60);
                else
                    g.DrawLine(pnFg2, LRx, LRy1, LRx, LRy2);
            }
            float LRw = size / 40;

            float LRv = width / 22F * (Value + 11F);
            g.FillRectangle(new SolidBrush(col), LRv - LRw / 2F, LRy1 - LRw, LRw, LRy2 - LRy1 + 2 * LRw);
            g.ResetTransform();
        }
        #endregion

        #region SizeChanged
        private void UbootControl_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
        #endregion
    }
}
