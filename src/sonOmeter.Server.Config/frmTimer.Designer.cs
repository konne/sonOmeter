namespace sonOmeter.Server.Config
{
	partial class frmTimer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTimer));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.pg = new System.Windows.Forms.ProgressBar();
            this.labText = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pg
            // 
            this.pg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pg.Location = new System.Drawing.Point(15, 32);
            this.pg.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.pg.Maximum = 50;
            this.pg.Name = "pg";
            this.pg.Size = new System.Drawing.Size(241, 23);
            this.pg.Step = 1;
            this.pg.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pg.TabIndex = 0;
            // 
            // labText
            // 
            this.labText.AutoSize = true;
            this.labText.Location = new System.Drawing.Point(12, 9);
            this.labText.Name = "labText";
            this.labText.Size = new System.Drawing.Size(70, 13);
            this.labText.TabIndex = 1;
            this.labText.Text = "Please wait...";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(262, 33);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmTimer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 68);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.labText);
            this.Controls.Add(this.pg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTimer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmTimer";
            this.Load += new System.EventHandler(this.frmTimer_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTimer_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.ProgressBar pg;
		private System.Windows.Forms.Label labText;
        private System.Windows.Forms.Button btnCancel;
	}
}