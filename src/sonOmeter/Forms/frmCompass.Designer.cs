namespace sonOmeter
{
	public partial class frmCompass
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private UKLib.Survey.Controls.CompassControl compassControl;

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


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompass));
            this.compassControl = new UKLib.Survey.Controls.CompassControl();
            this.SuspendLayout();
            // 
            // compassControl
            // 
            this.compassControl.AutoScroll = true;
            this.compassControl.Azimuth = 0.037037037037009668;
            this.compassControl.BackColor = System.Drawing.Color.Blue;
            this.compassControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compassControl.ForeColor = System.Drawing.Color.White;
            this.compassControl.Location = new System.Drawing.Point(0, 0);
            this.compassControl.Name = "compassControl";
            this.compassControl.ShowFrame = true;
            this.compassControl.ShowSpeed = true;
            this.compassControl.Size = new System.Drawing.Size(272, 271);
            this.compassControl.Smooth = true;
            this.compassControl.Speed = 0;
            this.compassControl.TabIndex = 0;
            this.compassControl.TextColor = System.Drawing.Color.White;
            // 
            // frmCompass
            // 
            this.Controls.Add(this.compassControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCompass";
            this.Text = "Compass";
            this.ResumeLayout(false);

		}
		#endregion

  }
}