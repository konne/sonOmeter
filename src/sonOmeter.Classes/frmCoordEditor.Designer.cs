namespace sonOmeter.Classes
{
	partial class frmCoordEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCoordEditor));
            this.labD1 = new System.Windows.Forms.Label();
            this.labD2 = new System.Windows.Forms.Label();
            this.labD3 = new System.Windows.Forms.Label();
            this.rbElliptic = new System.Windows.Forms.RadioButton();
            this.rbTransvers = new System.Windows.Forms.RadioButton();
            this.tbD1 = new System.Windows.Forms.TextBox();
            this.tbD2 = new System.Windows.Forms.TextBox();
            this.tbD3 = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labD1
            // 
            this.labD1.AutoSize = true;
            this.labD1.Location = new System.Drawing.Point(12, 61);
            this.labD1.Name = "labD1";
            this.labD1.Size = new System.Drawing.Size(45, 13);
            this.labD1.TabIndex = 2;
            this.labD1.Text = "Latitude";
            // 
            // labD2
            // 
            this.labD2.AutoSize = true;
            this.labD2.Location = new System.Drawing.Point(12, 87);
            this.labD2.Name = "labD2";
            this.labD2.Size = new System.Drawing.Size(54, 13);
            this.labD2.TabIndex = 3;
            this.labD2.Text = "Longitude";
            // 
            // labD3
            // 
            this.labD3.AutoSize = true;
            this.labD3.Location = new System.Drawing.Point(12, 113);
            this.labD3.Name = "labD3";
            this.labD3.Size = new System.Drawing.Size(42, 13);
            this.labD3.TabIndex = 4;
            this.labD3.Text = "Altitude";
            // 
            // rbElliptic
            // 
            this.rbElliptic.AutoSize = true;
            this.rbElliptic.Location = new System.Drawing.Point(91, 12);
            this.rbElliptic.Name = "rbElliptic";
            this.rbElliptic.Size = new System.Drawing.Size(55, 17);
            this.rbElliptic.TabIndex = 5;
            this.rbElliptic.TabStop = true;
            this.rbElliptic.Text = "Elliptic";
            this.rbElliptic.UseVisualStyleBackColor = true;
            this.rbElliptic.CheckedChanged += new System.EventHandler(this.CoordTypeChanged);
            // 
            // rbTransvers
            // 
            this.rbTransvers.AutoSize = true;
            this.rbTransvers.Location = new System.Drawing.Point(91, 35);
            this.rbTransvers.Name = "rbTransvers";
            this.rbTransvers.Size = new System.Drawing.Size(125, 17);
            this.rbTransvers.TabIndex = 6;
            this.rbTransvers.TabStop = true;
            this.rbTransvers.Text = "Transverse Mercator";
            this.rbTransvers.UseVisualStyleBackColor = true;
            this.rbTransvers.CheckedChanged += new System.EventHandler(this.CoordTypeChanged);
            // 
            // tbD1
            // 
            this.tbD1.Location = new System.Drawing.Point(91, 58);
            this.tbD1.Name = "tbD1";
            this.tbD1.Size = new System.Drawing.Size(166, 20);
            this.tbD1.TabIndex = 7;
            // 
            // tbD2
            // 
            this.tbD2.Location = new System.Drawing.Point(91, 84);
            this.tbD2.Name = "tbD2";
            this.tbD2.Size = new System.Drawing.Size(166, 20);
            this.tbD2.TabIndex = 7;
            // 
            // tbD3
            // 
            this.tbD3.Location = new System.Drawing.Point(91, 110);
            this.tbD3.Name = "tbD3";
            this.tbD3.Size = new System.Drawing.Size(166, 20);
            this.tbD3.TabIndex = 7;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(91, 136);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(182, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmCoordEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(269, 178);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbD3);
            this.Controls.Add(this.tbD2);
            this.Controls.Add(this.tbD1);
            this.Controls.Add(this.rbTransvers);
            this.Controls.Add(this.rbElliptic);
            this.Controls.Add(this.labD3);
            this.Controls.Add(this.labD2);
            this.Controls.Add(this.labD1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCoordEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Coordinate Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labD1;
		private System.Windows.Forms.Label labD2;
		private System.Windows.Forms.Label labD3;
		private System.Windows.Forms.RadioButton rbElliptic;
		private System.Windows.Forms.RadioButton rbTransvers;
		private System.Windows.Forms.TextBox tbD1;
		private System.Windows.Forms.TextBox tbD2;
		private System.Windows.Forms.TextBox tbD3;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;

	}
}