namespace sonOmeter
{
	public partial class frmHorizon
	{	
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private UKLib.Survey.Controls.HorizonControl horizonControl;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHorizon));
            this.horizonControl = new UKLib.Survey.Controls.HorizonControl();
            this.SuspendLayout();
            // 
            // horizonControl
            // 
            this.horizonControl.AutoScroll = true;
            this.horizonControl.BackColor = System.Drawing.Color.Blue;
            this.horizonControl.Bank = 0D;
            this.horizonControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizonControl.Elevation = 0D;
            this.horizonControl.ForeColor = System.Drawing.Color.White;
            this.horizonControl.FromBorder = 10;
            this.horizonControl.GroundColor = System.Drawing.Color.Chocolate;
            this.horizonControl.InnerSize = 20;
            this.horizonControl.InstrumentColor = System.Drawing.Color.Blue;
            this.horizonControl.Location = new System.Drawing.Point(0, 0);
            this.horizonControl.Name = "horizonControl";
            this.horizonControl.NFI = ((System.Globalization.NumberFormatInfo)(resources.GetObject("horizonControl.NFI")));
            this.horizonControl.Num = 2F;
            this.horizonControl.Pitch = 30F;
            this.horizonControl.PRY = true;
            this.horizonControl.Shade = true;
            this.horizonControl.ShowRect = true;
            this.horizonControl.Size = new System.Drawing.Size(272, 272);
            this.horizonControl.SkyColor = System.Drawing.Color.DeepSkyBlue;
            this.horizonControl.TabIndex = 0;
            this.horizonControl.TextColor = System.Drawing.Color.White;
            // 
            // frmHorizon
            // 
            this.ClientSize = new System.Drawing.Size(272, 272);
            this.Controls.Add(this.horizonControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmHorizon";
            this.Text = "Horizon";
            this.ResumeLayout(false);

		}
		#endregion
	}
}
