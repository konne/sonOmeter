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
	/// Summary description for SonarInfoBar.
	/// </summary>
	public class SonarInfoBar : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label labVolHF;
		private System.Windows.Forms.Label labVolNF;
		private System.Windows.Forms.Label labVolToggle;
		private System.Windows.Forms.Label labPitch;
		private sonOmeter.EditBar editBar;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;

		public SonarInfoBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));			
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
			this.labVolHF = new System.Windows.Forms.Label();
			this.labVolNF = new System.Windows.Forms.Label();
			this.labVolToggle = new System.Windows.Forms.Label();
			this.labPitch = new System.Windows.Forms.Label();
			this.editBar = new sonOmeter.EditBar();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// labVolHF
			// 
			this.labVolHF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.labVolHF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labVolHF.Location = new System.Drawing.Point(24, 0);
			this.labVolHF.Name = "labVolHF";
			this.labVolHF.Size = new System.Drawing.Size(48, 24);
			this.labVolHF.TabIndex = 0;
			this.labVolHF.Text = "HF:";
			this.labVolHF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labVolHF.Paint += new System.Windows.Forms.PaintEventHandler(this.labVolHF_Paint);
			this.labVolHF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labVolHF_MouseMove);
			// 
			// labVolNF
			// 
			this.labVolNF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.labVolNF.BackColor = System.Drawing.Color.Blue;
			this.labVolNF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labVolNF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labVolNF.Location = new System.Drawing.Point(72, 0);
			this.labVolNF.Name = "labVolNF";
			this.labVolNF.Size = new System.Drawing.Size(48, 24);
			this.labVolNF.TabIndex = 1;
			this.labVolNF.Text = "NF:";
			this.labVolNF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labVolNF.Paint += new System.Windows.Forms.PaintEventHandler(this.labVolNF_Paint);
			this.labVolNF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labVolNF_MouseMove);
			// 
			// labVolToggle
			// 
			this.labVolToggle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.labVolToggle.BackColor = System.Drawing.Color.Blue;
			this.labVolToggle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labVolToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labVolToggle.ForeColor = System.Drawing.Color.White;
			this.labVolToggle.Location = new System.Drawing.Point(0, 0);
			this.labVolToggle.Name = "labVolToggle";
			this.labVolToggle.Size = new System.Drawing.Size(24, 24);
			this.labVolToggle.TabIndex = 5;
			this.labVolToggle.Text = "Vol";
			this.labVolToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labVolToggle.Click += new System.EventHandler(this.labVolToggle_Click);
			// 
			// labPitch
			// 
			this.labPitch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labPitch.BackColor = System.Drawing.Color.Blue;
			this.labPitch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labPitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labPitch.Location = new System.Drawing.Point(320, 0);
			this.labPitch.Name = "labPitch";
			this.labPitch.Size = new System.Drawing.Size(32, 24);
			this.labPitch.TabIndex = 6;
			this.labPitch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editBar
			// 
			this.editBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.editBar.BackColor = System.Drawing.Color.Blue;
			this.editBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.editBar.EditMode = sonOmeter.EditModes.Nothing;
			this.editBar.ForeColor = System.Drawing.Color.White;
			this.editBar.Location = new System.Drawing.Point(120, 0);
			this.editBar.Name = "editBar";
			this.editBar.Size = new System.Drawing.Size(200, 24);
			this.editBar.TabIndex = 7;
			this.editBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.editBar_MouseMove);
			// 
			// toolTip
			// 
			this.toolTip.ShowAlways = true;
			// 
			// SonarInfoBar
			// 
			this.BackColor = System.Drawing.Color.Blue;
			this.Controls.Add(this.editBar);
			this.Controls.Add(this.labPitch);
			this.Controls.Add(this.labVolToggle);
			this.Controls.Add(this.labVolNF);
			this.Controls.Add(this.labVolHF);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "SonarInfoBar";
			this.Size = new System.Drawing.Size(352, 24);
			this.ResumeLayout(false);

		}
		#endregion

		#region Variables
		Color colorOff = Color.Gray;
		Color colorOn = Color.White;
		
		bool showVol = false;
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

		public bool ShowVol
		{
			get { return showVol; }
			set
			{
				showVol = value;
				SetColors();
				Invalidate();
				if (ToggleVol != null)
					ToggleVol(this, new System.EventArgs());
			}
		}

		public EditModes EditMode
		{
			get { return editBar.EditMode; }
			set { editBar.EditMode = value; }
		}

		public CutMode CutMode
		{
			get { return editBar.CutMode; }
			set { editBar.CutMode = value; }
		}

		public string EditString
		{
			set { editBar.EditString = value; }
		}

		public string LabelText
		{
			get { return editBar.Label.Text; }
			set
			{
				editBar.Label.Text = value;
				toolTip.SetToolTip(editBar.Label, editBar.Label.Text);
			}
		}

		public EditBar EditBar
		{
			get { return editBar; }
		}

		public Label LabPitch
		{
			get { return labPitch; }
		}
		#endregion

		#region Events
		public event System.EventHandler ToggleVol;
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
		
			labPitch.Font = Font;
			labVolHF.Font = Font;
			labVolNF.Font = Font;
			labVolToggle.Font = Font;
		}

		private void SetColors()
		{
			if (showVol)
				labVolToggle.ForeColor = colorOn;
			else
				labVolToggle.ForeColor = colorOff;
		}
		#endregion

		#region HF/NF Volume
		public void SetVolume(int volHF, int volNF)
		{
			if (volHF < 0)
				volHF = 0;
			if (volNF < 0)
				volNF = 0;

			labVolHF.Text = "HF: " + volHF.ToString("d03");
			labVolNF.Text = "NF: " + volNF.ToString("d03");

			toolTip.SetToolTip(labVolHF, labVolHF.Text);
			toolTip.SetToolTip(labVolNF, labVolNF.Text);
		}

		private void labVolToggle_Click(object sender, System.EventArgs e)
		{
			ShowVol = !showVol;
		}
		#endregion

		#region Editing
		public void Edit(System.Windows.Forms.KeyEventArgs e)
		{
			editBar.Edit(e);
		}
		#endregion

        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("CS.") || (e.PropertyName == "All"))
            {
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;
                labVolHF.ForeColor = GSC.Settings.CS.ForeColor;
                labVolHF.BackColor = GSC.Settings.CS.BackColor;
                labVolNF.ForeColor = GSC.Settings.CS.ForeColor;
                labVolNF.BackColor = GSC.Settings.CS.BackColor;
                labVolToggle.ForeColor = GSC.Settings.CS.ForeColor;
                labVolToggle.BackColor = GSC.Settings.CS.BackColor;
                labPitch.ForeColor = GSC.Settings.CS.ForeColor;
                labPitch.BackColor = GSC.Settings.CS.BackColor;

                SetColors();

                Invalidate();
            }
		}

		private void labVolHF_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(labVolHF, labVolHF.Text);
		}

		private void labVolNF_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(labVolNF, labVolNF.Text);
		}

		private void editBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(editBar.Label, editBar.Label.Text);
		}

		private void labVolHF_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			StringFormat sf = StringFormat.GenericDefault;
			sf.Trimming = StringTrimming.EllipsisCharacter;
			
			Graphics g = e.Graphics;
			SizeF strSize = g.MeasureString(labVolHF.Text, this.Font);
			g.Clear(this.BackColor);
			g.DrawString(labVolHF.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(0, (labVolHF.Height-strSize.Height)/2-1, labVolHF.Width, strSize.Height), sf);
		}

		private void labVolNF_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			StringFormat sf = StringFormat.GenericDefault;
			sf.Trimming = StringTrimming.EllipsisCharacter;
			
			Graphics g = e.Graphics;
			SizeF strSize = g.MeasureString(labVolNF.Text, this.Font);
			g.Clear(this.BackColor);
			g.DrawString(labVolNF.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(0, (labVolNF.Height-strSize.Height)/2-1, labVolNF.Width, strSize.Height), sf);
		}
	}
}
