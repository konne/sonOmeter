namespace UKLib.Controls
{
    partial class InstrumentControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tmSmoothRedraw = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.tmSmoothRedraw)).BeginInit();
            this.SuspendLayout();
            // 
            // tmSmoothRedraw
            // 
            this.tmSmoothRedraw.SynchronizingObject = this;
            this.tmSmoothRedraw.Elapsed += new System.Timers.ElapsedEventHandler(this.OnUpdateTimerElapsed);
            // 
            // SmoothRedrawControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SmoothRedrawControl";
            ((System.ComponentModel.ISupportInitialize)(this.tmSmoothRedraw)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Timers.Timer tmSmoothRedraw;
    }
}
