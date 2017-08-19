namespace sonOmeter
{
	partial class frmSON2XML
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSON2XML));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.Filename = new System.Windows.Forms.ColumnHeader();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lvBojen = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDZ = new System.Windows.Forms.TextBox();
            this.tbDY = new System.Windows.Forms.TextBox();
            this.tbDX = new System.Windows.Forms.TextBox();
            this.cbImportCut = new System.Windows.Forms.CheckBox();
            this.cbYears = new System.Windows.Forms.ComboBox();
            this.tbProjName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddSonFile = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tlsLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvFiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(554, 229);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sonar Files";
            // 
            // lvFiles
            // 
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.CheckBoxes = true;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.Filename});
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.Location = new System.Drawing.Point(13, 26);
            this.lvFiles.Margin = new System.Windows.Forms.Padding(10);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(528, 190);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Import";
            this.columnHeader1.Width = 42;
            // 
            // Filename
            // 
            this.Filename.Text = "Filename";
            this.Filename.Width = 441;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvBojen);
            this.groupBox2.Location = new System.Drawing.Point(12, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 191);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Buoys";
            // 
            // lvBojen
            // 
            this.lvBojen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBojen.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvBojen.FullRowSelect = true;
            this.lvBojen.Location = new System.Drawing.Point(13, 18);
            this.lvBojen.Margin = new System.Windows.Forms.Padding(10);
            this.lvBojen.Name = "lvBojen";
            this.lvBojen.Size = new System.Drawing.Size(276, 160);
            this.lvBojen.TabIndex = 1;
            this.lvBojen.UseCompatibleStateImageBehavior = false;
            this.lvBojen.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 40;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "HV";
            this.columnHeader3.Width = 81;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "RV";
            this.columnHeader4.Width = 81;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "AL";
            this.columnHeader5.Width = 53;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.tbDZ);
            this.groupBox4.Controls.Add(this.tbDY);
            this.groupBox4.Controls.Add(this.tbDX);
            this.groupBox4.Controls.Add(this.cbImportCut);
            this.groupBox4.Controls.Add(this.cbYears);
            this.groupBox4.Controls.Add(this.tbProjName);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(320, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(246, 162);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Input Data";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(158, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "DZ:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(82, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "DY:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "DX:";
            // 
            // tbDZ
            // 
            this.tbDZ.Location = new System.Drawing.Point(185, 108);
            this.tbDZ.Name = "tbDZ";
            this.tbDZ.Size = new System.Drawing.Size(46, 20);
            this.tbDZ.TabIndex = 7;
            this.tbDZ.Text = "0,0";
            // 
            // tbDY
            // 
            this.tbDY.Location = new System.Drawing.Point(109, 108);
            this.tbDY.Name = "tbDY";
            this.tbDY.Size = new System.Drawing.Size(46, 20);
            this.tbDY.TabIndex = 6;
            this.tbDY.Text = "0,0";
            // 
            // tbDX
            // 
            this.tbDX.Location = new System.Drawing.Point(39, 107);
            this.tbDX.Name = "tbDX";
            this.tbDX.Size = new System.Drawing.Size(40, 20);
            this.tbDX.TabIndex = 5;
            this.tbDX.Text = "0,0";
            // 
            // cbImportCut
            // 
            this.cbImportCut.AutoSize = true;
            this.cbImportCut.Location = new System.Drawing.Point(85, 73);
            this.cbImportCut.Name = "cbImportCut";
            this.cbImportCut.Size = new System.Drawing.Size(95, 17);
            this.cbImportCut.TabIndex = 4;
            this.cbImportCut.Text = "Import Cutlines";
            this.cbImportCut.UseVisualStyleBackColor = true;
            // 
            // cbYears
            // 
            this.cbYears.FormattingEnabled = true;
            this.cbYears.Location = new System.Drawing.Point(85, 46);
            this.cbYears.Margin = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.cbYears.Name = "cbYears";
            this.cbYears.Size = new System.Drawing.Size(146, 21);
            this.cbYears.TabIndex = 3;
            // 
            // tbProjName
            // 
            this.tbProjName.Location = new System.Drawing.Point(85, 23);
            this.tbProjName.Margin = new System.Windows.Forms.Padding(0, 0, 10, 10);
            this.tbProjName.Name = "tbProjName";
            this.tbProjName.Size = new System.Drawing.Size(148, 20);
            this.tbProjName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Year:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project Name:";
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "prj";
            this.dlgOpen.FileName = "*.son;*.prj";
            this.dlgOpen.Filter = "SON & PRJ Files|*.son;*.prj|All Files|*.*";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(320, 415);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(408, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnAddSonFile
            // 
            this.btnAddSonFile.Location = new System.Drawing.Point(491, 415);
            this.btnAddSonFile.Name = "btnAddSonFile";
            this.btnAddSonFile.Size = new System.Drawing.Size(75, 23);
            this.btnAddSonFile.TabIndex = 6;
            this.btnAddSonFile.Text = "Add File";
            this.btnAddSonFile.UseVisualStyleBackColor = true;
            this.btnAddSonFile.Click += new System.EventHandler(this.btnAddSonFile_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsLabel,
            this.tlsProgress,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 449);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(584, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tlsLabel
            // 
            this.tlsLabel.AutoSize = false;
            this.tlsLabel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.tlsLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tlsLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlsLabel.Name = "tlsLabel";
            this.tlsLabel.Size = new System.Drawing.Size(150, 17);
            this.tlsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tlsProgress
            // 
            this.tlsProgress.AutoSize = false;
            this.tlsProgress.Name = "tlsProgress";
            this.tlsProgress.Size = new System.Drawing.Size(330, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            // 
            // frmSON2XML
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 471);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnAddSonFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSON2XML";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Son-Files";
            this.Load += new System.EventHandler(this.frmSON2XML_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView lvFiles;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbProjName;
		private System.Windows.Forms.ComboBox cbYears;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.CheckBox cbImportCut;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbDX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDZ;
        private System.Windows.Forms.TextBox tbDY;
        private System.Windows.Forms.Button btnAddSonFile;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader Filename;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tlsLabel;
        private System.Windows.Forms.ToolStripProgressBar tlsProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ListView lvBojen;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
	}
}