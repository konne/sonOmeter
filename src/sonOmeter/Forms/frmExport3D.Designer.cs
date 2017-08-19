namespace sonOmeter
{
    partial class frmExport3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExport3D));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDepth = new System.Windows.Forms.CheckBox();
            this.cbBlankline = new System.Windows.Forms.CheckBox();
            this.cbVolume = new System.Windows.Forms.CheckBox();
            this.cbTopColor = new System.Windows.Forms.CheckBox();
            this.rbRVHV = new System.Windows.Forms.RadioButton();
            this.rbLALO = new System.Windows.Forms.RadioButton();
            this.rbHF = new System.Windows.Forms.RadioButton();
            this.rbNF = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbSweepFrom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSweepTo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSweepCount = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(327, 213);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(15, 213);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "grd";
            this.saveFileDialog.Filter = "Surfer grid files|*.grd|All files|*.*";
            // 
            // tbFile
            // 
            this.tbFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFile.Location = new System.Drawing.Point(15, 28);
            this.tbFile.Name = "tbFile";
            this.tbFile.Size = new System.Drawing.Size(316, 20);
            this.tbFile.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(340, 26);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the path and name of the Surfer grid files:";
            // 
            // cbDepth
            // 
            this.cbDepth.AutoSize = true;
            this.cbDepth.Checked = true;
            this.cbDepth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDepth.Location = new System.Drawing.Point(9, 19);
            this.cbDepth.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.cbDepth.Name = "cbDepth";
            this.cbDepth.Size = new System.Drawing.Size(73, 17);
            this.cbDepth.TabIndex = 0;
            this.cbDepth.Text = "Height (Z)";
            this.cbDepth.UseVisualStyleBackColor = true;
            // 
            // cbBlankline
            // 
            this.cbBlankline.AutoSize = true;
            this.cbBlankline.Checked = true;
            this.cbBlankline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlankline.Location = new System.Drawing.Point(9, 42);
            this.cbBlankline.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.cbBlankline.Name = "cbBlankline";
            this.cbBlankline.Size = new System.Drawing.Size(69, 17);
            this.cbBlankline.TabIndex = 1;
            this.cbBlankline.Text = "Blank file";
            this.cbBlankline.UseVisualStyleBackColor = true;
            // 
            // cbVolume
            // 
            this.cbVolume.AutoSize = true;
            this.cbVolume.Checked = true;
            this.cbVolume.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVolume.Location = new System.Drawing.Point(116, 42);
            this.cbVolume.Name = "cbVolume";
            this.cbVolume.Size = new System.Drawing.Size(77, 17);
            this.cbVolume.TabIndex = 3;
            this.cbVolume.Text = "Volume (V)";
            this.cbVolume.UseVisualStyleBackColor = true;
            // 
            // cbTopColor
            // 
            this.cbTopColor.AutoSize = true;
            this.cbTopColor.Checked = true;
            this.cbTopColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTopColor.Location = new System.Drawing.Point(116, 19);
            this.cbTopColor.Name = "cbTopColor";
            this.cbTopColor.Size = new System.Drawing.Size(87, 17);
            this.cbTopColor.TabIndex = 2;
            this.cbTopColor.Text = "Top color (C)";
            this.cbTopColor.UseVisualStyleBackColor = true;
            // 
            // rbRVHV
            // 
            this.rbRVHV.AutoSize = true;
            this.rbRVHV.Checked = true;
            this.rbRVHV.Location = new System.Drawing.Point(9, 19);
            this.rbRVHV.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.rbRVHV.Name = "rbRVHV";
            this.rbRVHV.Size = new System.Drawing.Size(66, 17);
            this.rbRVHV.TabIndex = 0;
            this.rbRVHV.TabStop = true;
            this.rbRVHV.Text = "RV / HV";
            this.rbRVHV.UseVisualStyleBackColor = true;
            // 
            // rbLALO
            // 
            this.rbLALO.AutoSize = true;
            this.rbLALO.Location = new System.Drawing.Point(9, 42);
            this.rbLALO.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.rbLALO.Name = "rbLALO";
            this.rbLALO.Size = new System.Drawing.Size(63, 17);
            this.rbLALO.TabIndex = 1;
            this.rbLALO.Text = "LA / LO";
            this.rbLALO.UseVisualStyleBackColor = true;
            // 
            // rbHF
            // 
            this.rbHF.AutoSize = true;
            this.rbHF.Checked = true;
            this.rbHF.Location = new System.Drawing.Point(9, 19);
            this.rbHF.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.rbHF.Name = "rbHF";
            this.rbHF.Size = new System.Drawing.Size(39, 17);
            this.rbHF.TabIndex = 0;
            this.rbHF.TabStop = true;
            this.rbHF.Text = "HF";
            this.rbHF.UseVisualStyleBackColor = true;
            // 
            // rbNF
            // 
            this.rbNF.AutoSize = true;
            this.rbNF.Location = new System.Drawing.Point(9, 42);
            this.rbNF.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.rbNF.Name = "rbNF";
            this.rbNF.Size = new System.Drawing.Size(39, 17);
            this.rbNF.TabIndex = 1;
            this.rbNF.Text = "NF";
            this.rbNF.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDepth);
            this.groupBox1.Controls.Add(this.cbBlankline);
            this.groupBox1.Controls.Add(this.cbTopColor);
            this.groupBox1.Controls.Add(this.cbVolume);
            this.groupBox1.Location = new System.Drawing.Point(15, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 70);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Created files";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbRVHV);
            this.groupBox2.Controls.Add(this.rbLALO);
            this.groupBox2.Location = new System.Drawing.Point(232, 54);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(102, 70);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Coordinate type";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbHF);
            this.groupBox3.Controls.Add(this.rbNF);
            this.groupBox3.Location = new System.Drawing.Point(340, 54);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(75, 70);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sonar type";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.progressBar);
            this.groupBox4.Controls.Add(this.tbSweepCount);
            this.groupBox4.Controls.Add(this.tbSweepTo);
            this.groupBox4.Controls.Add(this.tbSweepFrom);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(15, 130);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(400, 77);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Arch sweep";
            // 
            // tbSweepFrom
            // 
            this.tbSweepFrom.Location = new System.Drawing.Point(45, 19);
            this.tbSweepFrom.Name = "tbSweepFrom";
            this.tbSweepFrom.Size = new System.Drawing.Size(62, 20);
            this.tbSweepFrom.TabIndex = 1;
            this.tbSweepFrom.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "From:";
            // 
            // tbSweepTo
            // 
            this.tbSweepTo.Location = new System.Drawing.Point(142, 19);
            this.tbSweepTo.Name = "tbSweepTo";
            this.tbSweepTo.Size = new System.Drawing.Size(62, 20);
            this.tbSweepTo.TabIndex = 3;
            this.tbSweepTo.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(223, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Number of steps:";
            // 
            // tbSweepCount
            // 
            this.tbSweepCount.Location = new System.Drawing.Point(316, 19);
            this.tbSweepCount.Name = "tbSweepCount";
            this.tbSweepCount.Size = new System.Drawing.Size(62, 20);
            this.tbSweepCount.TabIndex = 5;
            this.tbSweepCount.Text = "1";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 45);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(385, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 6;
            // 
            // frmExport3D
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(428, 251);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbFile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmExport3D";
            this.Text = "Export 3D Data to Surfer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmExport3D_FormClosing);
            this.Load += new System.EventHandler(this.frmExport3D_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDepth;
        private System.Windows.Forms.CheckBox cbBlankline;
        private System.Windows.Forms.CheckBox cbVolume;
        private System.Windows.Forms.CheckBox cbTopColor;
        private System.Windows.Forms.RadioButton rbRVHV;
        private System.Windows.Forms.RadioButton rbLALO;
        private System.Windows.Forms.RadioButton rbHF;
        private System.Windows.Forms.RadioButton rbNF;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbSweepFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbSweepCount;
        private System.Windows.Forms.TextBox tbSweepTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}