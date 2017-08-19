namespace sonOmeter
{
	public partial class frmSat
	{
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private UKLib.Survey.Controls.SatControl satControl;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSat));
            this.satControl = new UKLib.Survey.Controls.SatControl();
            this.SuspendLayout();
            // 
            // satControl
            // 
            this.satControl.AutoScroll = true;
            this.satControl.BackColor = System.Drawing.Color.Blue;
            this.satControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.satControl.ForeColor = System.Drawing.Color.White;
            this.satControl.InnerSize = 12;
            this.satControl.InstrumentColor = System.Drawing.Color.Black;
            this.satControl.Location = new System.Drawing.Point(0, 0);
            this.satControl.Name = "satControl";
            this.satControl.NFI = ((System.Globalization.NumberFormatInfo)(resources.GetObject("satControl.NFI")));
            this.satControl.SatNum = 0;
            this.satControl.SatNumMin = 0;
            this.satControl.SatQuality = 0;
            this.satControl.SatQualityMeter = 0F;
            this.satControl.SatQualityMeterMin = 0F;
            this.satControl.SatQualityMin = 0;
            this.satControl.Size = new System.Drawing.Size(272, 272);
            this.satControl.TabIndex = 0;
            this.satControl.TextColor = System.Drawing.Color.White;
            // 
            // frmSat
            // 
            this.ClientSize = new System.Drawing.Size(272, 272);
            this.Controls.Add(this.satControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSat";
            this.Text = "Sat Finder";
            this.ResumeLayout(false);

		}
		#endregion

  }
}