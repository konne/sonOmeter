namespace sonOmeter
{
    partial class frmRenderVideo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.cbShowCompass = new System.Windows.Forms.CheckBox();
            this.cbShowHorizon = new System.Windows.Forms.CheckBox();
            this.pgbRenderVideo = new System.Windows.Forms.ProgressBar();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbShowCompass
            // 
            this.cbShowCompass.AutoSize = true;
            this.cbShowCompass.Location = new System.Drawing.Point(31, 86);
            this.cbShowCompass.Name = "cbShowCompass";
            this.cbShowCompass.Size = new System.Drawing.Size(69, 17);
            this.cbShowCompass.TabIndex = 2;
            this.cbShowCompass.Text = "Compass";
            this.cbShowCompass.UseVisualStyleBackColor = true;
            // 
            // cbShowHorizon
            // 
            this.cbShowHorizon.AutoSize = true;
            this.cbShowHorizon.Location = new System.Drawing.Point(33, 119);
            this.cbShowHorizon.Name = "cbShowHorizon";
            this.cbShowHorizon.Size = new System.Drawing.Size(62, 17);
            this.cbShowHorizon.TabIndex = 3;
            this.cbShowHorizon.Text = "Horizon";
            this.cbShowHorizon.UseVisualStyleBackColor = true;
            // 
            // pgbRenderVideo
            // 
            this.pgbRenderVideo.Location = new System.Drawing.Point(145, 23);
            this.pgbRenderVideo.Name = "pgbRenderVideo";
            this.pgbRenderVideo.Size = new System.Drawing.Size(291, 23);
            this.pgbRenderVideo.TabIndex = 4;
            // 
            // frmRenderVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 193);
            this.Controls.Add(this.pgbRenderVideo);
            this.Controls.Add(this.cbShowHorizon);
            this.Controls.Add(this.cbShowCompass);
            this.Controls.Add(this.button1);
            this.Name = "frmRenderVideo";
            this.Text = "frmRenderVideo";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRenderVideo_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbShowCompass;
        private System.Windows.Forms.CheckBox cbShowHorizon;
        private System.Windows.Forms.ProgressBar pgbRenderVideo;
        private System.Windows.Forms.SaveFileDialog dlgSave;
    }
}