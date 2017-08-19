namespace sonOmeter
{
	partial class frm3D
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.pan3D = new Panel3D();
			this.SuspendLayout();
			// 
			// pan3D
			// 
			this.pan3D.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pan3D.Location = new System.Drawing.Point(0, 0);
			this.pan3D.Name = "pan3D";
			this.pan3D.Size = new System.Drawing.Size(256, 228);
			this.pan3D.TabIndex = 0;
			// 
			// frm3D
			// 
			this.ClientSize = new System.Drawing.Size(256, 228);
			this.Controls.Add(this.pan3D);
			this.DockType = DockDotNET.DockContainerType.Document;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frm3D";
			this.Text = "3D surface";
			this.ResumeLayout(false);

		}

		#endregion

		private Panel3D pan3D;
	}
}
