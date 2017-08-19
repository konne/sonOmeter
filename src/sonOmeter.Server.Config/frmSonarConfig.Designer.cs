namespace sonOmeter.Server.Config
{
	partial class frmSonarConfig
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSonarConfig));
            this.btnReStart = new System.Windows.Forms.Button();
            this.pgSonarConfig = new System.Windows.Forms.PropertyGrid();
            this.pgServerConfig = new System.Windows.Forms.PropertyGrid();
            this.lbSonars = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbTryRead = new System.Windows.Forms.Label();
            this.tmTryGetSonConf = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.btnWriteSonConf = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnReStart
            // 
            this.btnReStart.Location = new System.Drawing.Point(12, 41);
            this.btnReStart.Name = "btnReStart";
            this.btnReStart.Size = new System.Drawing.Size(117, 23);
            this.btnReStart.TabIndex = 14;
            this.btnReStart.Text = "Restart Server";
            this.btnReStart.UseVisualStyleBackColor = true;
            this.btnReStart.Click += new System.EventHandler(this.btnReStart_Click);
            // 
            // pgSonarConfig
            // 
            this.pgSonarConfig.HelpVisible = false;
            this.pgSonarConfig.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgSonarConfig.Location = new System.Drawing.Point(142, 164);
            this.pgSonarConfig.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.pgSonarConfig.Name = "pgSonarConfig";
            this.pgSonarConfig.Size = new System.Drawing.Size(193, 134);
            this.pgSonarConfig.TabIndex = 13;
            this.pgSonarConfig.ToolbarVisible = false;
            // 
            // pgServerConfig
            // 
            this.pgServerConfig.HelpVisible = false;
            this.pgServerConfig.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.pgServerConfig.Location = new System.Drawing.Point(142, 41);
            this.pgServerConfig.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.pgServerConfig.Name = "pgServerConfig";
            this.pgServerConfig.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.pgServerConfig.Size = new System.Drawing.Size(193, 95);
            this.pgServerConfig.TabIndex = 12;
            this.pgServerConfig.ToolbarVisible = false;
            // 
            // lbSonars
            // 
            this.lbSonars.FormattingEnabled = true;
            this.lbSonars.Location = new System.Drawing.Point(12, 164);
            this.lbSonars.Name = "lbSonars";
            this.lbSonars.Size = new System.Drawing.Size(117, 108);
            this.lbSonars.TabIndex = 15;
            this.lbSonars.SelectedIndexChanged += new System.EventHandler(this.lbSonars_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(139, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Server Configuration:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Active Sonardevices";
            // 
            // lbTryRead
            // 
            this.lbTryRead.AutoSize = true;
            this.lbTryRead.BackColor = System.Drawing.Color.White;
            this.lbTryRead.Location = new System.Drawing.Point(172, 220);
            this.lbTryRead.Name = "lbTryRead";
            this.lbTryRead.Size = new System.Drawing.Size(115, 13);
            this.lbTryRead.TabIndex = 18;
            this.lbTryRead.Text = "Try Read Sonar Config";
            this.lbTryRead.Visible = false;
            // 
            // tmTryGetSonConf
            // 
            this.tmTryGetSonConf.Interval = 200;
            this.tmTryGetSonConf.Tick += new System.EventHandler(this.tmTryGetSonConf_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(139, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Sonar Configuration:";
            // 
            // btnWriteSonConf
            // 
            this.btnWriteSonConf.Location = new System.Drawing.Point(12, 275);
            this.btnWriteSonConf.Name = "btnWriteSonConf";
            this.btnWriteSonConf.Size = new System.Drawing.Size(117, 23);
            this.btnWriteSonConf.TabIndex = 20;
            this.btnWriteSonConf.Text = "Write SonarConfig";
            this.btnWriteSonConf.UseVisualStyleBackColor = true;
            this.btnWriteSonConf.Click += new System.EventHandler(this.btnWriteSonConf_Click);
            // 
            // frmSonarConfig
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(349, 316);
            this.Controls.Add(this.btnWriteSonConf);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbTryRead);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbSonars);
            this.Controls.Add(this.btnReStart);
            this.Controls.Add(this.pgSonarConfig);
            this.Controls.Add(this.pgServerConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSonarConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sonar Config";
            this.Load += new System.EventHandler(this.frmSonarConfig_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSonarConfig_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button btnReStart;
        private System.Windows.Forms.PropertyGrid pgSonarConfig;
        private System.Windows.Forms.PropertyGrid pgServerConfig;
        private System.Windows.Forms.ListBox lbSonars;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbTryRead;
        private System.Windows.Forms.Timer tmTryGetSonConf;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWriteSonConf;

    }
}