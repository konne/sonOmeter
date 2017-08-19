namespace sonOmeter
{
	partial class frmPrepare3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrepare3D));
            this.btnGenerateRecord = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbTopColorMost = new System.Windows.Forms.RadioButton();
            this.rbTopColorStrongest = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tbGridX = new System.Windows.Forms.TextBox();
            this.tbGridY = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbLink = new System.Windows.Forms.CheckBox();
            this.lbBlankLines = new System.Windows.Forms.ListBox();
            this.tbDepthRes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rbDepthDistance = new System.Windows.Forms.RadioButton();
            this.rbDepthNumber = new System.Windows.Forms.RadioButton();
            this.labDepthMethod = new System.Windows.Forms.Label();
            this.tbDepthConstraint = new System.Windows.Forms.TextBox();
            this.tbIterationCount = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rbWeightingQuadratic = new System.Windows.Forms.RadioButton();
            this.rbWeightingLinear = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerateRecord
            // 
            this.btnGenerateRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerateRecord.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGenerateRecord.Location = new System.Drawing.Point(12, 397);
            this.btnGenerateRecord.Name = "btnGenerateRecord";
            this.btnGenerateRecord.Size = new System.Drawing.Size(93, 23);
            this.btnGenerateRecord.TabIndex = 13;
            this.btnGenerateRecord.Text = "New Record";
            this.btnGenerateRecord.UseVisualStyleBackColor = true;
            this.btnGenerateRecord.Click += new System.EventHandler(this.btnGenerateRecord_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(257, 397);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Select the criteria for top color selection:";
            // 
            // rbTopColorMost
            // 
            this.rbTopColorMost.AutoSize = true;
            this.rbTopColorMost.Location = new System.Drawing.Point(7, 56);
            this.rbTopColorMost.Name = "rbTopColorMost";
            this.rbTopColorMost.Size = new System.Drawing.Size(107, 17);
            this.rbTopColorMost.TabIndex = 1;
            this.rbTopColorMost.Text = "Most occurances";
            this.rbTopColorMost.UseVisualStyleBackColor = true;
            // 
            // rbTopColorStrongest
            // 
            this.rbTopColorStrongest.AutoSize = true;
            this.rbTopColorStrongest.Checked = true;
            this.rbTopColorStrongest.Location = new System.Drawing.Point(7, 30);
            this.rbTopColorStrongest.Name = "rbTopColorStrongest";
            this.rbTopColorStrongest.Size = new System.Drawing.Size(70, 17);
            this.rbTopColorStrongest.TabIndex = 0;
            this.rbTopColorStrongest.TabStop = true;
            this.rbTopColorStrongest.Text = "Strongest";
            this.rbTopColorStrongest.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Enter grid sizes (m):";
            // 
            // tbGridX
            // 
            this.tbGridX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGridX.Location = new System.Drawing.Point(238, 29);
            this.tbGridX.Name = "tbGridX";
            this.tbGridX.Size = new System.Drawing.Size(76, 20);
            this.tbGridX.TabIndex = 3;
            this.tbGridX.Text = "1";
            this.tbGridX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGridX.TextChanged += new System.EventHandler(this.tbGridX_TextChanged);
            // 
            // tbGridY
            // 
            this.tbGridY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGridY.Location = new System.Drawing.Point(238, 55);
            this.tbGridY.Name = "tbGridY";
            this.tbGridY.Size = new System.Drawing.Size(76, 20);
            this.tbGridY.TabIndex = 4;
            this.tbGridY.Text = "1";
            this.tbGridY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "X";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(218, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Y";
            // 
            // cbLink
            // 
            this.cbLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLink.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbLink.Image = ((System.Drawing.Image)(resources.GetObject("cbLink.Image")));
            this.cbLink.Location = new System.Drawing.Point(320, 29);
            this.cbLink.Name = "cbLink";
            this.cbLink.Size = new System.Drawing.Size(25, 46);
            this.cbLink.TabIndex = 5;
            this.cbLink.UseVisualStyleBackColor = true;
            this.cbLink.CheckedChanged += new System.EventHandler(this.cbLink_CheckedChanged);
            // 
            // lbBlankLines
            // 
            this.lbBlankLines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbBlankLines.Location = new System.Drawing.Point(12, 253);
            this.lbBlankLines.Name = "lbBlankLines";
            this.lbBlankLines.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbBlankLines.Size = new System.Drawing.Size(338, 134);
            this.lbBlankLines.TabIndex = 12;
            // 
            // tbDepthRes
            // 
            this.tbDepthRes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDepthRes.Location = new System.Drawing.Point(238, 81);
            this.tbDepthRes.Name = "tbDepthRes";
            this.tbDepthRes.Size = new System.Drawing.Size(76, 20);
            this.tbDepthRes.TabIndex = 6;
            this.tbDepthRes.Text = "0.1";
            this.tbDepthRes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(218, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Z";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 237);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Select the blank line(s):";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(186, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Select the depth interpolation method:";
            // 
            // rbDepthDistance
            // 
            this.rbDepthDistance.AutoSize = true;
            this.rbDepthDistance.Checked = true;
            this.rbDepthDistance.Location = new System.Drawing.Point(5, 29);
            this.rbDepthDistance.Name = "rbDepthDistance";
            this.rbDepthDistance.Size = new System.Drawing.Size(162, 17);
            this.rbDepthDistance.TabIndex = 8;
            this.rbDepthDistance.TabStop = true;
            this.rbDepthDistance.Text = "Maximum neighbour distance";
            this.rbDepthDistance.UseVisualStyleBackColor = true;
            // 
            // rbDepthNumber
            // 
            this.rbDepthNumber.AutoSize = true;
            this.rbDepthNumber.Location = new System.Drawing.Point(5, 55);
            this.rbDepthNumber.Name = "rbDepthNumber";
            this.rbDepthNumber.Size = new System.Drawing.Size(174, 17);
            this.rbDepthNumber.TabIndex = 7;
            this.rbDepthNumber.Text = "Maximum number of neighbours";
            this.rbDepthNumber.UseVisualStyleBackColor = true;
            this.rbDepthNumber.CheckedChanged += new System.EventHandler(this.rbDepthNumber_CheckedChanged);
            // 
            // labDepthMethod
            // 
            this.labDepthMethod.AutoSize = true;
            this.labDepthMethod.Location = new System.Drawing.Point(2, 83);
            this.labDepthMethod.Name = "labDepthMethod";
            this.labDepthMethod.Size = new System.Drawing.Size(114, 13);
            this.labDepthMethod.TabIndex = 20;
            this.labDepthMethod.Text = "Number of neighbours:";
            // 
            // tbDepthConstraint
            // 
            this.tbDepthConstraint.Location = new System.Drawing.Point(145, 80);
            this.tbDepthConstraint.Name = "tbDepthConstraint";
            this.tbDepthConstraint.Size = new System.Drawing.Size(53, 20);
            this.tbDepthConstraint.TabIndex = 9;
            this.tbDepthConstraint.Text = "5";
            this.tbDepthConstraint.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbIterationCount
            // 
            this.tbIterationCount.Location = new System.Drawing.Point(147, 81);
            this.tbIterationCount.Name = "tbIterationCount";
            this.tbIterationCount.Size = new System.Drawing.Size(53, 20);
            this.tbIterationCount.TabIndex = 2;
            this.tbIterationCount.Text = "5";
            this.tbIterationCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Color iteration count:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(12, 110);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(338, 4);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 5);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Depth weighting type:";
            // 
            // rbWeightingQuadratic
            // 
            this.rbWeightingQuadratic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbWeightingQuadratic.AutoSize = true;
            this.rbWeightingQuadratic.Checked = true;
            this.rbWeightingQuadratic.Location = new System.Drawing.Point(10, 29);
            this.rbWeightingQuadratic.Name = "rbWeightingQuadratic";
            this.rbWeightingQuadratic.Size = new System.Drawing.Size(71, 17);
            this.rbWeightingQuadratic.TabIndex = 11;
            this.rbWeightingQuadratic.TabStop = true;
            this.rbWeightingQuadratic.Text = "Quadratic";
            this.rbWeightingQuadratic.UseVisualStyleBackColor = true;
            // 
            // rbWeightingLinear
            // 
            this.rbWeightingLinear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbWeightingLinear.AutoSize = true;
            this.rbWeightingLinear.Location = new System.Drawing.Point(10, 55);
            this.rbWeightingLinear.Name = "rbWeightingLinear";
            this.rbWeightingLinear.Size = new System.Drawing.Size(54, 17);
            this.rbWeightingLinear.TabIndex = 10;
            this.rbWeightingLinear.Text = "Linear";
            this.rbWeightingLinear.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Location = new System.Drawing.Point(12, 224);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(338, 4);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbLink);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbIterationCount);
            this.panel1.Controls.Add(this.tbDepthRes);
            this.panel1.Controls.Add(this.tbGridY);
            this.panel1.Controls.Add(this.tbGridX);
            this.panel1.Controls.Add(this.rbTopColorStrongest);
            this.panel1.Controls.Add(this.rbTopColorMost);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(5, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(354, 107);
            this.panel1.TabIndex = 25;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tbDepthConstraint);
            this.panel2.Controls.Add(this.rbDepthNumber);
            this.panel2.Controls.Add(this.rbDepthDistance);
            this.panel2.Controls.Add(this.labDepthMethod);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(7, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(210, 106);
            this.panel2.TabIndex = 26;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbWeightingLinear);
            this.panel3.Controls.Add(this.rbWeightingQuadratic);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Location = new System.Drawing.Point(216, 118);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(140, 103);
            this.panel3.TabIndex = 27;
            // 
            // frmPrepare3D
            // 
            this.AcceptButton = this.btnGenerateRecord;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(362, 432);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbBlankLines);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnGenerateRecord);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(368, 368);
            this.Name = "frmPrepare3D";
            this.ShowInTaskbar = false;
            this.Text = "Prepare 3D Visualization";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGenerateRecord;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rbTopColorMost;
        private System.Windows.Forms.RadioButton rbTopColorStrongest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbGridX;
        private System.Windows.Forms.TextBox tbGridY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbLink;
        private System.Windows.Forms.ListBox lbBlankLines;
        private System.Windows.Forms.TextBox tbDepthRes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbDepthDistance;
        private System.Windows.Forms.RadioButton rbDepthNumber;
        private System.Windows.Forms.Label labDepthMethod;
        private System.Windows.Forms.TextBox tbDepthConstraint;
        private System.Windows.Forms.TextBox tbIterationCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rbWeightingQuadratic;
        private System.Windows.Forms.RadioButton rbWeightingLinear;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
	}
}