namespace sonOmeter
{
	public partial class frmSplashScreen
	{
		#region Windows Variables
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel pnHideLine;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label lbStatus;
		private System.Windows.Forms.Label lbVersion;
		private System.Windows.Forms.Label lbRegistered;
		#endregion

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplashScreen));
            this.lbStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnHideLine = new System.Windows.Forms.Panel();
            this.lbVersion = new System.Windows.Forms.Label();
            this.lbRegistered = new System.Windows.Forms.Label();
            this.tmShowStatus = new System.Windows.Forms.Timer(this.components);
            this.btnAcceptDemo = new System.Windows.Forms.Button();
            this.btnSavePCID = new System.Windows.Forms.Button();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbStatus
            // 
            this.lbStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(194)))));
            this.lbStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.Location = new System.Drawing.Point(88, 247);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(272, 25);
            this.lbStatus.TabIndex = 0;
            this.lbStatus.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(408, 360);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pnHideLine
            // 
            this.pnHideLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(194)))));
            this.pnHideLine.Location = new System.Drawing.Point(12, 11);
            this.pnHideLine.Name = "pnHideLine";
            this.pnHideLine.Size = new System.Drawing.Size(24, 9);
            this.pnHideLine.TabIndex = 2;
            // 
            // lbVersion
            // 
            this.lbVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(194)))));
            this.lbVersion.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVersion.Location = new System.Drawing.Point(88, 192);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(280, 16);
            this.lbVersion.TabIndex = 3;
            this.lbVersion.Text = "Version 2010 - build";
            this.lbVersion.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lbRegistered
            // 
            this.lbRegistered.AutoEllipsis = true;
            this.lbRegistered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(194)))));
            this.lbRegistered.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbRegistered.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRegistered.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbRegistered.Location = new System.Drawing.Point(88, 218);
            this.lbRegistered.Name = "lbRegistered";
            this.lbRegistered.Size = new System.Drawing.Size(246, 29);
            this.lbRegistered.TabIndex = 4;
            this.lbRegistered.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // tmShowStatus
            // 
            this.tmShowStatus.Interval = 2000;
            this.tmShowStatus.Tick += new System.EventHandler(this.tmShowStatus_Tick);
            // 
            // btnAcceptDemo
            // 
            this.btnAcceptDemo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(194)))));
            this.btnAcceptDemo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcceptDemo.Location = new System.Drawing.Point(254, 261);
            this.btnAcceptDemo.Name = "btnAcceptDemo";
            this.btnAcceptDemo.Size = new System.Drawing.Size(80, 23);
            this.btnAcceptDemo.TabIndex = 12;
            this.btnAcceptDemo.Text = "Start Demo";
            this.btnAcceptDemo.UseVisualStyleBackColor = false;
            this.btnAcceptDemo.Visible = false;
            this.btnAcceptDemo.Click += new System.EventHandler(this.btnAcceptDemo_Click);
            
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "id";
            this.dlgSave.Filter = "ID-Files (*.id)|*.id|All-Files (*.*)|*.*";
            // 
            // frmSplashScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(392, 355);
            this.Controls.Add(this.btnSavePCID);
            this.Controls.Add(this.btnAcceptDemo);
            this.Controls.Add(this.lbRegistered);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.pnHideLine);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSplashScreen";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Transparent;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

  }
}