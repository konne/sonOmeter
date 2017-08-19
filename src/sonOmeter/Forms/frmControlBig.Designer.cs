namespace sonOmeter
{
	public partial class frmControlBig
	{
        private System.Windows.Forms.Button btnPrjSave;
        private System.Windows.Forms.Button btnStartStopRec;
        private System.Windows.Forms.Button btnNextRecord;
        private System.Windows.Forms.Button btnTracking;
        private System.Windows.Forms.Button btnRestartServer;
        private System.Windows.Forms.Button btnManualInput;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmControlBig));
            this.btnStartStopRec = new System.Windows.Forms.Button();
            this.btnPrjSave = new System.Windows.Forms.Button();
            this.btnNextRecord = new System.Windows.Forms.Button();
            this.btnTracking = new System.Windows.Forms.Button();
            this.btnRestartServer = new System.Windows.Forms.Button();
            this.btnManualInput = new System.Windows.Forms.Button();
            this.btnSim = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartStopRec
            // 
            this.btnStartStopRec.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStopRec.Location = new System.Drawing.Point(8, 8);
            this.btnStartStopRec.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnStartStopRec.Name = "btnStartStopRec";
            this.btnStartStopRec.Size = new System.Drawing.Size(80, 80);
            this.btnStartStopRec.TabIndex = 4;
            this.btnStartStopRec.Text = "Start";
            this.btnStartStopRec.Click += new System.EventHandler(this.btnStartStopRec_Click);
            // 
            // btnPrjSave
            // 
            this.btnPrjSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrjSave.Location = new System.Drawing.Point(200, 8);
            this.btnPrjSave.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnPrjSave.Name = "btnPrjSave";
            this.btnPrjSave.Size = new System.Drawing.Size(80, 80);
            this.btnPrjSave.TabIndex = 6;
            this.btnPrjSave.Text = "Save";
            this.btnPrjSave.Click += new System.EventHandler(this.btnPrjSave_Click);
            // 
            // btnNextRecord
            // 
            this.btnNextRecord.Enabled = false;
            this.btnNextRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNextRecord.Location = new System.Drawing.Point(104, 8);
            this.btnNextRecord.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnNextRecord.Name = "btnNextRecord";
            this.btnNextRecord.Size = new System.Drawing.Size(80, 80);
            this.btnNextRecord.TabIndex = 7;
            this.btnNextRecord.Text = "Next";
            this.btnNextRecord.Click += new System.EventHandler(this.btnNextRecord_Click);
            // 
            // btnTracking
            // 
            this.btnTracking.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTracking.Location = new System.Drawing.Point(296, 8);
            this.btnTracking.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnTracking.Name = "btnTracking";
            this.btnTracking.Size = new System.Drawing.Size(80, 80);
            this.btnTracking.TabIndex = 6;
            this.btnTracking.Text = "Start Track";
            this.btnTracking.Click += new System.EventHandler(this.btnTracking_Click);
            // 
            // btnRestartServer
            // 
            this.btnRestartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestartServer.Location = new System.Drawing.Point(392, 8);
            this.btnRestartServer.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnRestartServer.Name = "btnRestartServer";
            this.btnRestartServer.Size = new System.Drawing.Size(80, 80);
            this.btnRestartServer.TabIndex = 8;
            this.btnRestartServer.Text = "Restart Server";
            this.btnRestartServer.Click += new System.EventHandler(this.btnConfigServer_Click);
            // 
            // btnManualInput
            // 
            this.btnManualInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualInput.Location = new System.Drawing.Point(488, 8);
            this.btnManualInput.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnManualInput.Name = "btnManualInput";
            this.btnManualInput.Size = new System.Drawing.Size(80, 80);
            this.btnManualInput.TabIndex = 8;
            this.btnManualInput.Text = "Manual Input";
            this.btnManualInput.Click += new System.EventHandler(this.btnManualInput_Click);
            // 
            // btnSim
            // 
            this.btnSim.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSim.Location = new System.Drawing.Point(584, 8);
            this.btnSim.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnSim.Name = "btnSim";
            this.btnSim.Size = new System.Drawing.Size(80, 80);
            this.btnSim.TabIndex = 9;
            this.btnSim.Text = "Sim";
            this.btnSim.Click += new System.EventHandler(this.btnSim_Click);
            // 
            // frmControlBig
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(674, 96);
            this.Controls.Add(this.btnStartStopRec);
            this.Controls.Add(this.btnPrjSave);
            this.Controls.Add(this.btnTracking);
            this.Controls.Add(this.btnNextRecord);
            this.Controls.Add(this.btnRestartServer);
            this.Controls.Add(this.btnManualInput);
            this.Controls.Add(this.btnSim);
            this.DockType = DockDotNET.DockContainerType.ToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmControlBig";
            this.Text = "Controls";
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.Button btnSim;

	}
}	
