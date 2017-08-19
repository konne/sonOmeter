using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for SonarDepthMeter.
    /// </summary>
    public class SonarDepthMeter : System.Windows.Forms.UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public SonarDepthMeter()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dmGlobal = new UKLib.Survey.Controls.DeepMeter();
            this.dmHF = new UKLib.Survey.Controls.DeepMeter();
            this.dmNF = new UKLib.Survey.Controls.DeepMeter();
            this.panWorkLine = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // dmGlobal
            // 
            this.dmGlobal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dmGlobal.Digits = 1;
            this.dmGlobal.Location = new System.Drawing.Point(0, 0);
            this.dmGlobal.Margin = new System.Windows.Forms.Padding(1);
            this.dmGlobal.Max = -100F;
            this.dmGlobal.Min = 0F;
            this.dmGlobal.Name = "dmGlobal";
            this.dmGlobal.Orientation = UKLib.Survey.Controls.OrientationType.Left;
            this.dmGlobal.Size = new System.Drawing.Size(40, 462);
            this.dmGlobal.StubLen = 5;
            this.dmGlobal.TabIndex = 0;
            // 
            // dmHF
            // 
            this.dmHF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmHF.Digits = 1;
            this.dmHF.Location = new System.Drawing.Point(141, 0);
            this.dmHF.Margin = new System.Windows.Forms.Padding(1);
            this.dmHF.Max = -100F;
            this.dmHF.Min = 0F;
            this.dmHF.Name = "dmHF";
            this.dmHF.Orientation = UKLib.Survey.Controls.OrientationType.Right;
            this.dmHF.Size = new System.Drawing.Size(40, 230);
            this.dmHF.StubLen = 5;
            this.dmHF.TabIndex = 0;
            // 
            // dmNF
            // 
            this.dmNF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dmNF.Digits = 2;
            this.dmNF.Location = new System.Drawing.Point(141, 233);
            this.dmNF.Margin = new System.Windows.Forms.Padding(1);
            this.dmNF.Max = -100F;
            this.dmNF.Min = 0F;
            this.dmNF.Name = "dmNF";
            this.dmNF.Orientation = UKLib.Survey.Controls.OrientationType.Right;
            this.dmNF.Size = new System.Drawing.Size(40, 230);
            this.dmNF.StubLen = 5;
            this.dmNF.TabIndex = 0;
            // 
            // panWorkLine
            // 
            this.panWorkLine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panWorkLine.Location = new System.Drawing.Point(41, 1);
            this.panWorkLine.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.panWorkLine.Name = "panWorkLine";
            this.panWorkLine.Size = new System.Drawing.Size(99, 461);
            this.panWorkLine.TabIndex = 1;
            // 
            // SonarDepthMeter
            // 
            this.BackColor = System.Drawing.Color.Blue;
            this.Controls.Add(this.panWorkLine);
            this.Controls.Add(this.dmNF);
            this.Controls.Add(this.dmHF);
            this.Controls.Add(this.dmGlobal);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SonarDepthMeter";
            this.Size = new System.Drawing.Size(181, 464);
            this.ResumeLayout(false);

        }
        #endregion

        #region Variables
        bool showHF = true;
        bool showNF = true;

        int borderX = 1;
        int borderYr = 1;
        int borderYl = 1;
        int bottomWidth = 0;

        double depthZoomHi = 0.0;
        double depthZoomLo = -100.0;
        double panelYRatio = 0.5;

        private UKLib.Survey.Controls.DeepMeter dmGlobal;
        private UKLib.Survey.Controls.DeepMeter dmHF;
        private UKLib.Survey.Controls.DeepMeter dmNF;
        private Panel panWorkLine;
        BorderStyle borderStyle = BorderStyle.None;
        #endregion

        #region Properties
        public bool ShowHF
        {
            get { return showHF; }
            set
            {
                showHF = value;
                if (!(showHF | showNF))
                    showNF = true;
                OnResize(null);
            }
        }

        public bool ShowNF
        {
            get { return showNF; }
            set
            {
                showNF = value;
                if (!(showHF | showNF))
                    showHF = true;
                OnResize(null);
            }
        }

        public int BorderX
        {
            get { return borderX; }
            set
            {
                borderX = value;
                dmGlobal.StubLen = borderX;
                dmHF.StubLen = borderX;
                dmNF.StubLen = borderX;
            }
        }

        public int BorderYl
        {
            get { return borderYl; }
            set
            {
                borderYl = value;
                OnResize(null);
            }
        }

        public int BorderYr
        {
            get { return borderYr; }
            set
            {
                borderYr = value;
                OnResize(null);
            }
        }

        public double DepthZoomHi
        {
            get { return depthZoomHi; }
            set
            {
                if (value <= depthZoomLo)
                    return;

                depthZoomHi = value;
                dmNF.Min = (float)depthZoomHi;
                dmHF.Min = (float)depthZoomHi;
            }
        }

        public double DepthZoomLo
        {
            get { return depthZoomLo; }
            set
            {
                if (value >= depthZoomHi)
                    return;

                depthZoomLo = value;
                dmNF.Max = (float)depthZoomLo;
                dmHF.Max = (float)depthZoomLo;
            }
        }

        public float DepthBottom
        {
            get { return dmGlobal.Max; }
            set
            {
                dmGlobal.Max = value;
                dmHF.Max = dmNF.Max = Math.Max(dmHF.Max, value);
                Invalidate();
            }
        }

        public float DepthTop
        {
            get { return dmGlobal.Min; }
            set
            {
                dmGlobal.Min = value;
                dmHF.Min = dmNF.Min = Math.Min(dmHF.Min, value);
                Invalidate();
            }
        }

        public new BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set
            {
                borderStyle = value;
                Invalidate();
            }
        }

        public double PanelYRatio
        {
            get { return panelYRatio; }
            set
            {
                if (value < 0.0)
                    value = 0.0;
                if (value > 1.0)
                    value = 1.0;
                panelYRatio = value;
                OnResize(null);
            }
        }

        public int BottomWidth
        {
            get { return bottomWidth; }
            set
            {
                bottomWidth = value;
                OnResize(null);
            }
        }

        public Graphics WorkLinePaintObj
        {
            get { return Graphics.FromHwnd(panWorkLine.Handle); }
        }

        public Size WorkLinePaintSize
        {
            get { return panWorkLine.Size; }
        }
        #endregion

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        #region Paint events
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // draw border
            switch (borderStyle)
            {
                case BorderStyle.FixedSingle:
                    g.DrawRectangle(SystemPens.WindowFrame, 0, 0, this.Width - 1, this.Height - 1);
                    break;
                case BorderStyle.Fixed3D:
                    g.DrawLine(SystemPens.ControlDark, 0, 0, this.Width - 1, 0);
                    g.DrawLine(SystemPens.ControlDark, 0, 0, 0, this.Height - 1);
                    g.DrawLine(SystemPens.ControlLightLight, this.Width - 1, 0, this.Width - 1, this.Height - 1);
                    g.DrawLine(SystemPens.ControlLightLight, 0, this.Height - 1, this.Width - 1, this.Height - 1);
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            double ratio = panelYRatio;

            dmGlobal.Height = this.Height - (borderYl << 1);
            dmGlobal.Top = borderYl;
            dmGlobal.RedrawBmp();

            if (showHF & showNF)
            {
                dmHF.Height = (int)((double)this.Height * panelYRatio) - borderYr - bottomWidth - 1;
                dmHF.Top = borderYr;
                dmHF.Visible = true;
                dmHF.RedrawBmp();

                dmNF.Height = (int)((double)this.Height * (1 - panelYRatio)) - borderYr - bottomWidth - 1;
                dmNF.Top = dmHF.Bottom + 2 + bottomWidth;
                dmNF.Visible = true;
                dmNF.RedrawBmp();
            }
            else if (showHF)
            {
                dmHF.Height = this.Height - (borderYr << 1) - bottomWidth;
                dmHF.Top = borderYr;
                dmHF.Visible = true;
                dmHF.RedrawBmp();

                dmNF.Visible = false;
            }
            else if (showNF)
            {
                dmNF.Height = this.Height - (borderYr << 1) - bottomWidth;
                dmNF.Top = borderYr;
                dmNF.Visible = true;
                dmNF.RedrawBmp();

                dmHF.Visible = false;
            }

            Invalidate(true);
        }
        #endregion

        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                bool all = e.PropertyName == "All";
                bool inv = false;

                if (e.PropertyName.StartsWith("CS.") || all)
                {
                    this.ForeColor = GSC.Settings.CS.ForeColor;
                    this.BackColor = GSC.Settings.CS.BackColor;
                    inv = true;
                }

                if (inv)
                    Invalidate();
            }
            catch
            {
            }
        }
    }
}
