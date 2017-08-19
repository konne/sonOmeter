namespace sonOmeter
{
    partial class frmUbootControl
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
            this.ubootControl1 = new UKLib.Survey.Controls.UbootControl();
            this.tmSendStatus = new System.Windows.Forms.Timer(this.components);
            this.btnReConnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ubootControl1
            // 
            this.ubootControl1.BackColor = System.Drawing.Color.Blue;
            this.ubootControl1.Deep = 0F;
            this.ubootControl1.DeepChange = 0F;
            this.ubootControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubootControl1.ForeColor = System.Drawing.Color.White;
            this.ubootControl1.LeftRight = 0F;
            this.ubootControl1.LightL = false;
            this.ubootControl1.LightR = false;
            this.ubootControl1.Location = new System.Drawing.Point(0, 0);
            this.ubootControl1.Name = "ubootControl1";
            this.ubootControl1.Pump = false;
            this.ubootControl1.Size = new System.Drawing.Size(292, 292);
            this.ubootControl1.Speed = 0F;
            this.ubootControl1.SpeedMode = false;
            this.ubootControl1.TabIndex = 0;
            this.ubootControl1.UpDown = 0F;
            this.ubootControl1.Valve = false;
            // 
            // tmSendStatus
            // 
            this.tmSendStatus.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnReConnect
            // 
            this.btnReConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReConnect.BackColor = System.Drawing.Color.Red;
            this.btnReConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReConnect.Location = new System.Drawing.Point(0, 0);
            this.btnReConnect.Name = "btnReConnect";
            this.btnReConnect.Size = new System.Drawing.Size(292, 41);
            this.btnReConnect.TabIndex = 1;
            this.btnReConnect.Text = "Reconnect";
            this.btnReConnect.UseVisualStyleBackColor = false;
            this.btnReConnect.Visible = false;
            this.btnReConnect.Click += new System.EventHandler(this.btnReConnect_Click);
            // 
            // frmUbootControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 292);
            this.Controls.Add(this.ubootControl1);
            this.Controls.Add(this.btnReConnect);
            this.DockType = DockDotNET.DockContainerType.ToolWindow;
            this.Name = "frmUbootControl";
            this.Text = "Uboot";
            this.ResumeLayout(true);

        }

        #endregion

        private UKLib.Survey.Controls.UbootControl ubootControl1;
        private System.Windows.Forms.Timer tmSendStatus;
        private System.Windows.Forms.Button btnReConnect;
    }
}