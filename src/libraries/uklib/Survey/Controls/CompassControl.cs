using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace UKLib.Survey.Controls
{
    public partial class CompassControl : UKLib.Controls.InstrumentControl
    {
        public CompassControl()
        {
            InitializeComponent();
            InitializeControl();
        }

        #region Variables
        double azimuth = 0;
        double nazimuth;
        double speed = 0;

        bool showSpeed = true;
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set the Speed
        /// </summary>
        public double Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                if (showSpeed) Invalidate();
            }
        }

        /// <summary>
        /// Get or Set the visible of speed
        /// </summary>
        public bool ShowSpeed
        {
            get { return showSpeed; }
            set
            {
                showSpeed = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Set the Azimuth
        /// </summary>
        public double Azimuth
        {
            get { return azimuth; }
            set
            {
                nazimuth = value;
                while (nazimuth >= 360) nazimuth = nazimuth - 360;
                while (nazimuth < 0) nazimuth = nazimuth + 360;
                if ((!smooth) | Double.IsNaN(azimuth))
                    azimuth = nazimuth;
                else
                    tmSmoothRedraw.Enabled = true;
            }
        }
        #endregion

        #region Paint
        public override void PaintInstrument(Graphics g, ref UKLib.Controls.InstrumentControlPaintVars vars)
        {
            float scale = vars.size / 500F;
            Pen pnSc = new Pen(this.ForeColor, (scale * 2.0F + 1));

            #region Fonts
            Font fntAz = new Font(Font.Name, (scale * 40.0F));
            SizeF szAz = g.MeasureString("360°", fntAz);

            Font fntNS = new Font(Font.Name, (scale * 45.0F), FontStyle.Bold);
            SizeF szNS = g.MeasureString(" ", fntNS);

            Font fntNr = new Font(Font.Name, (scale * 27.0F));
            #endregion

            int brd = 10;
            int off = 0;
            if (showSpeed) off = (int)((szAz.Height + brd) / 2.0);

            #region Show Speed
            if (showSpeed)
            {
                g.DrawRectangle(vars.pnFg, vars.mx - (szAz.Width + brd) / 2, vars.my + (szAz.Height + brd) / 2 - off, szAz.Width + brd, szAz.Height + brd);
                string sspeed = speed.ToString("0.0");
                sspeed = sspeed.Replace(",", ".");
                g.DrawString(sspeed, fntAz, vars.sbText, vars.mx + szAz.Width / 2, vars.my + szAz.Height / 2 - brd, vars.sfRight2Left);
            }
            #endregion

            #region Show Azimuth
            double az = 0;
            g.DrawRectangle(vars.pnFg, vars.mx - (szAz.Width + brd) / 2, vars.my - (szAz.Height + brd) / 2 - off, szAz.Width + brd, szAz.Height + brd);

            if (!Double.IsNaN(azimuth))
            {
                string sazimuth = System.Math.Round(azimuth).ToString();
                if (sazimuth.Length < 3) sazimuth = "0" + sazimuth;
                if (sazimuth.Length < 3) sazimuth = "0" + sazimuth;
                sazimuth = sazimuth + '°';
                g.DrawString(sazimuth, fntAz, vars.sbText, vars.mx + szAz.Width / 2, vars.my - szAz.Height / 2 - off, vars.sfRight2Left);
                az = azimuth;
            }
            #endregion

            g.SmoothingMode = SmoothingMode.AntiAlias;

            #region Show Cirle
            g.TranslateTransform(vars.mx, vars.my);

            vars.size = (int)((vars.size - 5) / 2 - szNS.Height);

            g.RotateTransform(-(float)az);

            int size2 = (int)(vars.size - 1);
            int size3 = (int)(scale * 15.0);

            for (int i = 0; i < 36 * 2; i++)
            {
                if (i % 6 == 0)
                {
                    string deg = "";

                    if (i % 18 == 0)
                    {
                        #region Switch N/S/W/E
                        switch (i / 2)
                        {
                            case 0:
                                deg = "N";
                                break;
                            case 9:
                                deg = "E";
                                break;
                            case 18:
                                deg = "S";
                                break;
                            case 27:
                                deg = "W";
                                break;
                        }
                        #endregion

                        szNS = g.MeasureString(deg, fntNS);
                        g.DrawString(deg, fntNS, vars.sbText, -szNS.Width / 2, -vars.size - szNS.Height);
                    }
                    else
                    {
                        deg = (i / 2).ToString();
                        SizeF szNr = g.MeasureString(deg, fntNr);
                        g.DrawString(deg, fntNr, vars.sbText, -szNr.Width / 2, -vars.size - szNr.Height * 1.2F);
                    }
                }
                if (i % 2 == 0)
                    g.DrawLine(pnSc, 0, -size2 + size3, 0, -size2 - size3);
                else
                    g.DrawLine(pnSc, 0, -size2 + size3 / 2, 0, -size2 - size3 / 2);

                g.RotateTransform(5);
            }
            #endregion
            g.ResetTransform();
        }
        #endregion

        #region Smooth Timer
        protected override void OnUpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SmoothAngle(ref azimuth, nazimuth, 2.0);

            while (azimuth >= 360) azimuth = azimuth - 360;
            while (azimuth < 0) azimuth = azimuth + 360;
            Invalidate();

            if ((int)azimuth == (int)nazimuth) tmSmoothRedraw.Enabled = false;
        }
        #endregion
    }
}
