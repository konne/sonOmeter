using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for SonarStatusBar.
	/// </summary>
	public class SonarStatusBar : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label labCUT;
		private System.Windows.Forms.Label labNF;
		private System.Windows.Forms.Label labHF;
		private System.Windows.Forms.Label labColors;
		private System.Windows.Forms.ToolTip toolTipColors;
		private System.Windows.Forms.Label labPos;
		private System.Windows.Forms.Label labRuler;
		private System.ComponentModel.IContainer components;

		public SonarStatusBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();            

			this.ForeColor = GSC.Settings.CS.ForeColor;
			this.BackColor = GSC.Settings.CS.BackColor;
			labNF.BackColor = GSC.Settings.CS.BackColor;
			labHF.BackColor = GSC.Settings.CS.BackColor;
			labCUT.BackColor = GSC.Settings.CS.BackColor;
			labPos.BackColor = GSC.Settings.CS.BackColor;
			labRuler.BackColor = GSC.Settings.CS.BackColor;

			SetColors();

			Invalidate();

			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
      

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.labCUT = new System.Windows.Forms.Label();
			this.labNF = new System.Windows.Forms.Label();
			this.labHF = new System.Windows.Forms.Label();
			this.labColors = new System.Windows.Forms.Label();
			this.toolTipColors = new System.Windows.Forms.ToolTip(this.components);
			this.labRuler = new System.Windows.Forms.Label();
			this.labPos = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labCUT
			// 
			this.labCUT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labCUT.BackColor = System.Drawing.Color.Blue;
			this.labCUT.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labCUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labCUT.ForeColor = System.Drawing.Color.White;
			this.labCUT.Location = new System.Drawing.Point(72, 0);
			this.labCUT.Name = "labCUT";
			this.labCUT.Size = new System.Drawing.Size(24, 24);
			this.labCUT.TabIndex = 2;
			this.labCUT.Text = "Cu";
			this.labCUT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labCUT.Click += new System.EventHandler(this.labCUT_Click);
			// 
			// labNF
			// 
			this.labNF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labNF.BackColor = System.Drawing.Color.Blue;
			this.labNF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labNF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labNF.ForeColor = System.Drawing.Color.White;
			this.labNF.Location = new System.Drawing.Point(24, 0);
			this.labNF.Name = "labNF";
			this.labNF.Size = new System.Drawing.Size(24, 24);
			this.labNF.TabIndex = 1;
			this.labNF.Text = "NF";
			this.labNF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labNF.Click += new System.EventHandler(this.labNF_Click);
			// 
			// labHF
			// 
			this.labHF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labHF.BackColor = System.Drawing.Color.Blue;
			this.labHF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labHF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labHF.ForeColor = System.Drawing.Color.White;
			this.labHF.Location = new System.Drawing.Point(0, 0);
			this.labHF.Name = "labHF";
			this.labHF.Size = new System.Drawing.Size(24, 24);
			this.labHF.TabIndex = 0;
			this.labHF.Text = "HF";
			this.labHF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labHF.Click += new System.EventHandler(this.labHF_Click);
			// 
			// labColors
			// 
			this.labColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labColors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labColors.Location = new System.Drawing.Point(120, 0);
			this.labColors.Name = "labColors";
			this.labColors.Size = new System.Drawing.Size(48, 24);
			this.labColors.TabIndex = 3;
			this.labColors.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labColors_MouseDown);
			this.labColors.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labColors_MouseMove);
			this.labColors.Paint += new System.Windows.Forms.PaintEventHandler(this.labColors_Paint);
			// 
			// labRuler
			// 
			this.labRuler.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labRuler.BackColor = System.Drawing.Color.Blue;
			this.labRuler.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labRuler.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labRuler.ForeColor = System.Drawing.Color.White;
			this.labRuler.Location = new System.Drawing.Point(96, 0);
			this.labRuler.Name = "labRuler";
			this.labRuler.Size = new System.Drawing.Size(24, 24);
			this.labRuler.TabIndex = 4;
			this.labRuler.Text = "Ru";
			this.labRuler.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labRuler.Click += new System.EventHandler(this.labRuler_Click);
			// 
			// labPos
			// 
			this.labPos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.labPos.BackColor = System.Drawing.Color.Blue;
			this.labPos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labPos.ForeColor = System.Drawing.Color.White;
			this.labPos.Location = new System.Drawing.Point(48, 0);
			this.labPos.Name = "labPos";
			this.labPos.Size = new System.Drawing.Size(24, 24);
			this.labPos.TabIndex = 5;
			this.labPos.Text = "PS";
			this.labPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labPos.Click += new System.EventHandler(this.labPos_Click);
			// 
			// SonarStatusBar
			// 
			this.BackColor = System.Drawing.Color.Blue;
			this.Controls.Add(this.labPos);
			this.Controls.Add(this.labRuler);
			this.Controls.Add(this.labColors);
			this.Controls.Add(this.labCUT);
			this.Controls.Add(this.labNF);
			this.Controls.Add(this.labHF);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "SonarStatusBar";
			this.Size = new System.Drawing.Size(168, 24);
			this.ResumeLayout(false);

		}
		#endregion

		#region Variables
		Color colorOff = Color.Gray;
		Color colorOn = Color.White;

		bool showHF = true;
		bool showNF = true;
		bool showPos = false;
		bool isCut = true;
		bool showRuler = false;
		#endregion

		#region Properties
		public Color ColorOff
		{
			get { return colorOff; }
			set { colorOff = value; Invalidate(); }
		}

		public Color ColorOn
		{
			get { return colorOn; }
			set { colorOn = value; Invalidate(); }
		}

		public bool ShowHF
		{
			get { return showHF; }
			set
			{
				showHF = value;
				if (!(showHF | showNF))
					showNF = true;
				SetColors();
				Invalidate();
				if (ToggleHF != null)
					ToggleHF(this, new System.EventArgs());
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
				SetColors();
				Invalidate();
				if (ToggleNF != null)
					ToggleNF(this, new System.EventArgs());
			}
		}

		public bool ShowPos
		{
			get { return showPos; }
			set
			{
				showPos = value;
				if (showPos)
					labPos.ForeColor = colorOn;
				else
					labPos.ForeColor = colorOff;
				SetColors();
				Invalidate();
				if (TogglePos != null)
					TogglePos(this, new System.EventArgs());
			}
		}

		public bool ShowRuler
		{
			get { return showRuler; }
			set
			{
				showRuler = value;
				if (showRuler)
					labRuler.ForeColor = colorOn;
				else
					labRuler.ForeColor = colorOff;
				SetColors();
				Invalidate();
				if (ToggleRuler != null)
					ToggleRuler(this, new System.EventArgs());
			}
		}

		public bool IsCut
		{
			get { return isCut; }
			set
			{
				isCut = value;
				if (isCut)
					labCUT.ForeColor = colorOn;
				else
					labCUT.ForeColor = colorOff;
				SetColors();
				Invalidate();
				if (ToggleCUT != null)
					ToggleCUT(this, new System.EventArgs());
			}
		}
		#endregion
		
		#region Events
		public event System.EventHandler ToggleHF;
		public event System.EventHandler ToggleNF;
		public event System.EventHandler ToggleCUT;
		public event System.EventHandler ToggleColor;
		public event System.EventHandler TogglePos;
		public event System.EventHandler ToggleRuler;
		#endregion

		#region Other Tabs
		private void labHF_Click(object sender, System.EventArgs e)
		{
			ShowHF = !showHF;
		}

		private void labNF_Click(object sender, System.EventArgs e)
		{
			ShowNF = !showNF;
		}

		private void labCUT_Click(object sender, System.EventArgs e)
		{
			IsCut = !isCut;
		}

		private void labPos_Click(object sender, System.EventArgs e)
		{
			ShowPos = !showPos;
		}

		private void labRuler_Click(object sender, System.EventArgs e)
		{
			ShowRuler = !showRuler;
		}
		#endregion

		#region General
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetColors();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
		
			labHF.Font = Font;
			labNF.Font = Font;
			labCUT.Font = Font;
			labPos.Font = Font;
			labRuler.Font = Font;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetColors();
		}

		private void SetColors()
		{
			if (showHF)
				labHF.ForeColor = colorOn;
			else
				labHF.ForeColor = colorOff;

			if (showNF)
				labNF.ForeColor = colorOn;
			else
				labNF.ForeColor = colorOff;

			if (isCut)
				labCUT.ForeColor = colorOn;
			else
				labCUT.ForeColor = colorOff;

			if (showPos)
				labPos.ForeColor = colorOn;
			else
				labPos.ForeColor = colorOff;

			if (showRuler)
				labRuler.ForeColor = colorOn;
			else
				labRuler.ForeColor = colorOff;
		}
		#endregion

		#region Keyboard events
		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}
		#endregion

		#region Color tab
		private void labColors_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (GSC.Settings == null)
				GSC.Settings = new GlobalSettings();

			int b = 3;

			int w = labColors.Width / 4;
			int h = labColors.Height / 2 - 1;

			Graphics graphics = e.Graphics;

			SolidBrush bg = new SolidBrush(GSC.Settings.CS.BackColor);
			graphics.FillRectangle(bg, 0, 0, w, h);

            if (GSC.Settings.SECL != null)
            {
                for (int i = 1; i < GSC.Settings.SECL.Count+1; i++)
                {
                    SolidBrush col;
                    col = new SolidBrush(GSC.Settings.SECL[i - 1].SonarColor);

                    graphics.FillRectangle(col, (i % 4) * w - 1, h * (int)(i / 4), w, h);
                    if (!GSC.Settings.SECL[i - 1].SonarColorVisible) graphics.FillRectangle(bg, (i % 4) * w - 1 + b, h * (int)(i / 4) + b, w - b * 2, h - b * 2);
                }
            }
		}

		private void labColors_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int w = labColors.Width / 4;
			int h = labColors.Height / 2 - 1;

			if (e.X >= 3*w-1)
			{
				if (e.Y < h)
					toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[2].SonarColor.ToString());
				else
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[6].SonarColor.ToString());
			}
			else if (e.X >= 2*w-1)
			{
				if (e.Y < h)
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[1].SonarColor.ToString());
				else
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[5].SonarColor.ToString());
			}
			else if (e.X >= w-1)
			{
				if (e.Y < h)
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[0].SonarColor.ToString());
				else
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[4].SonarColor.ToString());
			}
			else
			{
				if (e.Y < h)
					toolTipColors.SetToolTip(labColors, GSC.Settings.CS.BackColor.ToString());
				else
                    toolTipColors.SetToolTip(labColors, GSC.Settings.SECL[3].SonarColor.ToString());
			}
		}

		private void labColors_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int w = labColors.Width / 4;
			int h = labColors.Height / 2 - 1;

			if (e.X >= 3*w-1)
			{
				if (e.Y < h)
					GSC.Settings.SECL[2].SonarColorVisible = !GSC.Settings.SECL[2].SonarColorVisible;
				else
					GSC.Settings.SECL[6].SonarColorVisible = !GSC.Settings.SECL[6].SonarColorVisible;
			}
			else if (e.X >= 2*w-1)
			{
				if (e.Y < h)
					GSC.Settings.SECL[1].SonarColorVisible = !GSC.Settings.SECL[1].SonarColorVisible;
				else
					GSC.Settings.SECL[5].SonarColorVisible = !GSC.Settings.SECL[5].SonarColorVisible;
			}
			else if (e.X >= w-1)
			{
				if (e.Y < h)
					GSC.Settings.SECL[0].SonarColorVisible = !GSC.Settings.SECL[0].SonarColorVisible;
				else
					GSC.Settings.SECL[4].SonarColorVisible = !GSC.Settings.SECL[4].SonarColorVisible;
			}
			else
			{
				if (e.Y >= h)
					GSC.Settings.SECL[3].SonarColorVisible = !GSC.Settings.SECL[3].SonarColorVisible;
			}

			ToggleColor(this, new System.EventArgs());
			labColors.Invalidate();
		}
		#endregion

        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("CS.") || (e.PropertyName == "All"))
            {
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;
                labNF.BackColor = GSC.Settings.CS.BackColor;
                labHF.BackColor = GSC.Settings.CS.BackColor;
                labCUT.BackColor = GSC.Settings.CS.BackColor;
                labPos.BackColor = GSC.Settings.CS.BackColor;
                labRuler.BackColor = GSC.Settings.CS.BackColor;

                SetColors();

                ToggleColor(this, new System.EventArgs());

                labColors.Invalidate();

                Invalidate();
            }
		}
	}
}
