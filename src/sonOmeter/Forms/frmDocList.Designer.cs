namespace sonOmeter
{
	public partial class frmDocList
	{
			private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Button btnBringToFront;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmDocList));
			this.listBox = new System.Windows.Forms.ListBox();
			this.btnBringToFront = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox
			// 
			this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox.Location = new System.Drawing.Point(0, 0);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(296, 277);
			this.listBox.TabIndex = 0;
			// 
			// btnBringToFront
			// 
			this.btnBringToFront.Location = new System.Drawing.Point(8, 288);
			this.btnBringToFront.Name = "btnBringToFront";
			this.btnBringToFront.Size = new System.Drawing.Size(80, 23);
			this.btnBringToFront.TabIndex = 1;
			this.btnBringToFront.Text = "Bring to front";
			this.btnBringToFront.Click += new System.EventHandler(this.btnBringToFront_Click);
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(96, 288);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(104, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close document";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(208, 288);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// frmDocList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(298, 322);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnBringToFront);
			this.Controls.Add(this.listBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmDocList";
			this.ShowInTaskbar = false;
			this.Text = "Active Documents";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmDocList_Closing);
			this.ResumeLayout(false);

		}
		#endregion

	}
}