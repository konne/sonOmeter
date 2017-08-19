using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace sample
{
	public class frmDoc : DockDotNET.DockWindow
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.ComponentModel.IContainer components = null;

		public frmDoc()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(312, 400);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "textBox1";
			// 
			// frmDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 400);
			this.Controls.Add(this.textBox1);
			this.DockType = DockDotNET.DockContainerType.Document;
			this.Name = "frmDoc";
			this.Text = "Document window";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDoc_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void frmDoc_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = false;
		}
	}
}

