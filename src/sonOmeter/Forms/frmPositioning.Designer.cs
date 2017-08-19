namespace sonOmeter
{
    partial class frmPositioning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPositioning));
            this.positioningControl = new UKLib.Survey.Controls.PositioningControl();
            this.SuspendLayout();
            // 
            // positioningControl
            // 
            this.positioningControl.AutoScroll = true;
            this.positioningControl.BackColor = System.Drawing.Color.Blue;
            this.positioningControl.Depth = 0;
            this.positioningControl.DepthType = "HF";
            this.positioningControl.Distance = 0;
            this.positioningControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positioningControl.ForeColor = System.Drawing.Color.White;
            this.positioningControl.InstrumentColor = System.Drawing.Color.Blue;
            this.positioningControl.Location = new System.Drawing.Point(0, 0);
            this.positioningControl.Name = "positioningControl";
            this.positioningControl.NFI = ((System.Globalization.NumberFormatInfo)(resources.GetObject("positioningControl.NFI")));
            this.positioningControl.Size = new System.Drawing.Size(272, 272);
            this.positioningControl.Station = 0;
            this.positioningControl.TabIndex = 0;
            this.positioningControl.TextColor = System.Drawing.Color.White;
            this.positioningControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.positioningControl_MouseDown);
            // 
            // frmPositioning
            // 
            this.ClientSize = new System.Drawing.Size(272, 272);
            this.Controls.Add(this.positioningControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPositioning";
            this.Text = "Positioning";
            this.ResumeLayout(false);

        }

        #endregion

        private UKLib.Survey.Controls.PositioningControl positioningControl;
    }
}
