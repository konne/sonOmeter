namespace sonOmeter
{
    partial class frmProfile
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
            this.lbConnections = new System.Windows.Forms.ListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.cbOneFile = new System.Windows.Forms.CheckBox();
            this.btnStaticRecords = new System.Windows.Forms.Button();
            this.cbNormSL = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSamplingRes = new System.Windows.Forms.TextBox();
            this.cbMergeManualPoints = new System.Windows.Forms.CheckBox();
            this.cbManualPointsDominate = new System.Windows.Forms.CheckBox();
            this.cbNormMP = new System.Windows.Forms.CheckBox();
            this.cbAddBoyAsPoint = new System.Windows.Forms.CheckBox();
            this.btnDynamicRecords = new System.Windows.Forms.Button();
            this.tbSamplingResZ = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbConnections
            // 
            this.lbConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbConnections.Location = new System.Drawing.Point(12, 9);
            this.lbConnections.Name = "lbConnections";
            this.lbConnections.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbConnections.Size = new System.Drawing.Size(294, 238);
            this.lbConnections.TabIndex = 6;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(12, 381);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(87, 23);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "Export Data";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.OnExport);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(227, 381);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.OnCancel);
            // 
            // cbOneFile
            // 
            this.cbOneFile.AutoSize = true;
            this.cbOneFile.Location = new System.Drawing.Point(12, 253);
            this.cbOneFile.Name = "cbOneFile";
            this.cbOneFile.Size = new System.Drawing.Size(126, 17);
            this.cbOneFile.TabIndex = 9;
            this.cbOneFile.Text = "One File (only Export)";
            this.cbOneFile.UseVisualStyleBackColor = true;
            // 
            // btnStaticRecords
            // 
            this.btnStaticRecords.Location = new System.Drawing.Point(12, 352);
            this.btnStaticRecords.Name = "btnStaticRecords";
            this.btnStaticRecords.Size = new System.Drawing.Size(139, 23);
            this.btnStaticRecords.TabIndex = 10;
            this.btnStaticRecords.Text = "New Static Records";
            this.btnStaticRecords.UseVisualStyleBackColor = true;
            this.btnStaticRecords.Click += new System.EventHandler(this.OnStaticRecords);
            // 
            // cbNormSL
            // 
            this.cbNormSL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNormSL.AutoSize = true;
            this.cbNormSL.Checked = true;
            this.cbNormSL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNormSL.Location = new System.Drawing.Point(156, 253);
            this.cbNormSL.Name = "cbNormSL";
            this.cbNormSL.Size = new System.Drawing.Size(131, 17);
            this.cbNormSL.TabIndex = 11;
            this.cbNormSL.Text = "Normalize Sonar Lines";
            this.cbNormSL.UseVisualStyleBackColor = true;
            this.cbNormSL.CheckedChanged += new System.EventHandler(this.cbNormalize_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 324);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Grid x (m):";
            // 
            // tbSamplingRes
            // 
            this.tbSamplingRes.Location = new System.Drawing.Point(72, 321);
            this.tbSamplingRes.Name = "tbSamplingRes";
            this.tbSamplingRes.Size = new System.Drawing.Size(66, 20);
            this.tbSamplingRes.TabIndex = 13;
            this.tbSamplingRes.Text = "0";
            // 
            // cbMergeManualPoints
            // 
            this.cbMergeManualPoints.AutoSize = true;
            this.cbMergeManualPoints.Location = new System.Drawing.Point(12, 276);
            this.cbMergeManualPoints.Name = "cbMergeManualPoints";
            this.cbMergeManualPoints.Size = new System.Drawing.Size(126, 17);
            this.cbMergeManualPoints.TabIndex = 11;
            this.cbMergeManualPoints.Text = "Merge Manual Points";
            this.cbMergeManualPoints.UseVisualStyleBackColor = true;
            this.cbMergeManualPoints.CheckedChanged += new System.EventHandler(this.cbNormalize_CheckedChanged);
            // 
            // cbManualPointsDominate
            // 
            this.cbManualPointsDominate.AutoSize = true;
            this.cbManualPointsDominate.Location = new System.Drawing.Point(12, 299);
            this.cbManualPointsDominate.Name = "cbManualPointsDominate";
            this.cbManualPointsDominate.Size = new System.Drawing.Size(139, 17);
            this.cbManualPointsDominate.TabIndex = 14;
            this.cbManualPointsDominate.Text = "Manual Points dominate";
            this.cbManualPointsDominate.UseVisualStyleBackColor = true;
            // 
            // cbNormMP
            // 
            this.cbNormMP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNormMP.AutoSize = true;
            this.cbNormMP.Checked = true;
            this.cbNormMP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNormMP.Location = new System.Drawing.Point(156, 276);
            this.cbNormMP.Name = "cbNormMP";
            this.cbNormMP.Size = new System.Drawing.Size(142, 17);
            this.cbNormMP.TabIndex = 11;
            this.cbNormMP.Text = "Normalize Manual Points";
            this.cbNormMP.UseVisualStyleBackColor = true;
            this.cbNormMP.CheckedChanged += new System.EventHandler(this.cbNormalize_CheckedChanged);
            // 
            // cbAddBoyAsPoint
            // 
            this.cbAddBoyAsPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAddBoyAsPoint.AutoSize = true;
            this.cbAddBoyAsPoint.Location = new System.Drawing.Point(156, 299);
            this.cbAddBoyAsPoint.Name = "cbAddBoyAsPoint";
            this.cbAddBoyAsPoint.Size = new System.Drawing.Size(150, 17);
            this.cbAddBoyAsPoint.TabIndex = 15;
            this.cbAddBoyAsPoint.Text = "Add Boys as Manual Point";
            this.cbAddBoyAsPoint.UseVisualStyleBackColor = true;
            // 
            // btnDynamicRecords
            // 
            this.btnDynamicRecords.Location = new System.Drawing.Point(156, 352);
            this.btnDynamicRecords.Name = "btnDynamicRecords";
            this.btnDynamicRecords.Size = new System.Drawing.Size(150, 23);
            this.btnDynamicRecords.TabIndex = 10;
            this.btnDynamicRecords.Text = "New Dynamic Records";
            this.btnDynamicRecords.UseVisualStyleBackColor = true;
            this.btnDynamicRecords.Click += new System.EventHandler(this.OnDynamicRecords);
            // 
            // tbSamplingResZ
            // 
            this.tbSamplingResZ.Location = new System.Drawing.Point(211, 321);
            this.tbSamplingResZ.Name = "tbSamplingResZ";
            this.tbSamplingResZ.Size = new System.Drawing.Size(66, 20);
            this.tbSamplingResZ.TabIndex = 17;
            this.tbSamplingResZ.Text = "0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 324);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Grid z (m):";
            // 
            // frmProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(318, 415);
            this.Controls.Add(this.tbSamplingResZ);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAddBoyAsPoint);
            this.Controls.Add(this.cbManualPointsDominate);
            this.Controls.Add(this.tbSamplingRes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbMergeManualPoints);
            this.Controls.Add(this.cbNormMP);
            this.Controls.Add(this.cbNormSL);
            this.Controls.Add(this.btnDynamicRecords);
            this.Controls.Add(this.btnStaticRecords);
            this.Controls.Add(this.cbOneFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lbConnections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(301, 361);
            this.Name = "frmProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Connection";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbConnections;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStaticRecords;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.CheckBox cbOneFile;
        private System.Windows.Forms.CheckBox cbNormSL;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbSamplingRes;
		private System.Windows.Forms.CheckBox cbMergeManualPoints;
		private System.Windows.Forms.CheckBox cbManualPointsDominate;
		private System.Windows.Forms.CheckBox cbNormMP;
        private System.Windows.Forms.CheckBox cbAddBoyAsPoint;
        private System.Windows.Forms.Button btnDynamicRecords;
        private System.Windows.Forms.TextBox tbSamplingResZ;
        private System.Windows.Forms.Label label2;
    }
}