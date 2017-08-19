namespace sonOmeter.Classes
{
    partial class frmPKTImport
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
            this.lbPKT = new System.Windows.Forms.ListBox();
            this.tbProfileStart = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnProfileStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbProfileEnd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnProfileEnd = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pgProfile = new System.Windows.Forms.PropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panMap = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lbPKT
            // 
            this.lbPKT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbPKT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPKT.FormattingEnabled = true;
            this.lbPKT.Location = new System.Drawing.Point(12, 25);
            this.lbPKT.Name = "lbPKT";
            this.lbPKT.Size = new System.Drawing.Size(150, 184);
            this.lbPKT.TabIndex = 0;
            this.lbPKT.SelectedIndexChanged += new System.EventHandler(this.lbPKT_SelectedIndexChanged);
            // 
            // tbProfileStart
            // 
            this.tbProfileStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProfileStart.Location = new System.Drawing.Point(201, 27);
            this.tbProfileStart.Name = "tbProfileStart";
            this.tbProfileStart.ReadOnly = true;
            this.tbProfileStart.Size = new System.Drawing.Size(150, 20);
            this.tbProfileStart.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "List of Points";
            // 
            // btnProfileStart
            // 
            this.btnProfileStart.Location = new System.Drawing.Point(168, 25);
            this.btnProfileStart.Name = "btnProfileStart";
            this.btnProfileStart.Size = new System.Drawing.Size(27, 23);
            this.btnProfileStart.TabIndex = 3;
            this.btnProfileStart.Text = ">>";
            this.btnProfileStart.UseVisualStyleBackColor = true;
            this.btnProfileStart.Click += new System.EventHandler(this.btnProfileStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Start of Profile";
            // 
            // tbProfileEnd
            // 
            this.tbProfileEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbProfileEnd.Location = new System.Drawing.Point(201, 69);
            this.tbProfileEnd.Name = "tbProfileEnd";
            this.tbProfileEnd.ReadOnly = true;
            this.tbProfileEnd.Size = new System.Drawing.Size(150, 20);
            this.tbProfileEnd.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(165, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "End of Profile";
            // 
            // btnProfileEnd
            // 
            this.btnProfileEnd.Location = new System.Drawing.Point(168, 67);
            this.btnProfileEnd.Name = "btnProfileEnd";
            this.btnProfileEnd.Size = new System.Drawing.Size(27, 23);
            this.btnProfileEnd.TabIndex = 3;
            this.btnProfileEnd.Text = ">>";
            this.btnProfileEnd.UseVisualStyleBackColor = true;
            this.btnProfileEnd.Click += new System.EventHandler(this.btnProfileEnd_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Profile Properties";
            // 
            // pgProfile
            // 
            this.pgProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgProfile.Location = new System.Drawing.Point(168, 109);
            this.pgProfile.Name = "pgProfile";
            this.pgProfile.Size = new System.Drawing.Size(183, 258);
            this.pgProfile.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 373);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(276, 373);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panMap
            // 
            this.panMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMap.Location = new System.Drawing.Point(12, 217);
            this.panMap.Name = "panMap";
            this.panMap.Size = new System.Drawing.Size(150, 150);
            this.panMap.TabIndex = 7;
            this.panMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panMap_Paint);
            // 
            // frmPKTImport
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(363, 405);
            this.Controls.Add(this.panMap);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pgProfile);
            this.Controls.Add(this.btnProfileEnd);
            this.Controls.Add(this.btnProfileStart);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbProfileEnd);
            this.Controls.Add(this.tbProfileStart);
            this.Controls.Add(this.lbPKT);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(379, 439);
            this.Name = "frmPKTImport";
            this.ShowInTaskbar = false;
            this.Text = "Import PKT File";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbPKT;
        private System.Windows.Forms.TextBox tbProfileStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnProfileStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbProfileEnd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnProfileEnd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PropertyGrid pgProfile;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panMap;
    }
}