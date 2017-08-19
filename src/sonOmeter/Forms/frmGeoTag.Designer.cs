namespace sonOmeter
{
    partial class frmGeoTag
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
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbTransvers = new System.Windows.Forms.RadioButton();
            this.rbElliptic = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOverlays = new System.Windows.Forms.CheckBox();
            this.cbTransparentBG = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // sfd
            // 
            this.sfd.DefaultExt = "tif";
            this.sfd.Filter = "Bitmap files (BMP)|*.bmp|JPEG files|*.jpg|Portable network graphics files (PNG)|*" +
    ".png|TIFF files|*.tif|Graphics Interchange Format files (GIF)|*.gif|All files|*." +
    "*";
            // 
            // tbFile
            // 
            this.tbFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFile.Location = new System.Drawing.Point(12, 14);
            this.tbFile.Name = "tbFile";
            this.tbFile.ReadOnly = true;
            this.tbFile.Size = new System.Drawing.Size(294, 20);
            this.tbFile.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(312, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 123);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 123);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rbTransvers
            // 
            this.rbTransvers.AutoSize = true;
            this.rbTransvers.Location = new System.Drawing.Point(264, 40);
            this.rbTransvers.Name = "rbTransvers";
            this.rbTransvers.Size = new System.Drawing.Size(123, 17);
            this.rbTransvers.TabIndex = 8;
            this.rbTransvers.TabStop = true;
            this.rbTransvers.Text = "Transverse Mercator";
            this.rbTransvers.UseVisualStyleBackColor = true;
            // 
            // rbElliptic
            // 
            this.rbElliptic.AutoSize = true;
            this.rbElliptic.Checked = true;
            this.rbElliptic.Location = new System.Drawing.Point(203, 40);
            this.rbElliptic.Name = "rbElliptic";
            this.rbElliptic.Size = new System.Drawing.Size(55, 17);
            this.rbElliptic.TabIndex = 7;
            this.rbElliptic.TabStop = true;
            this.rbElliptic.Text = "Elliptic";
            this.rbElliptic.UseVisualStyleBackColor = true;
            this.rbElliptic.CheckedChanged += new System.EventHandler(this.rbElliptic_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "World file coordinate type:";
            // 
            // cbOverlays
            // 
            this.cbOverlays.AutoSize = true;
            this.cbOverlays.Checked = true;
            this.cbOverlays.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOverlays.Location = new System.Drawing.Point(12, 69);
            this.cbOverlays.Name = "cbOverlays";
            this.cbOverlays.Size = new System.Drawing.Size(237, 17);
            this.cbOverlays.TabIndex = 10;
            this.cbOverlays.Text = "Include overlays (buoys, record markers etc.)";
            this.cbOverlays.UseVisualStyleBackColor = true;
            // 
            // cbTransparentBG
            // 
            this.cbTransparentBG.AutoSize = true;
            this.cbTransparentBG.Checked = true;
            this.cbTransparentBG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTransparentBG.Location = new System.Drawing.Point(12, 92);
            this.cbTransparentBG.Name = "cbTransparentBG";
            this.cbTransparentBG.Size = new System.Drawing.Size(143, 17);
            this.cbTransparentBG.TabIndex = 10;
            this.cbTransparentBG.Text = "Transparent background";
            this.cbTransparentBG.UseVisualStyleBackColor = true;
            // 
            // frmGeoTag
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(399, 158);
            this.Controls.Add(this.cbTransparentBG);
            this.Controls.Add(this.cbOverlays);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbTransvers);
            this.Controls.Add(this.rbElliptic);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGeoTag";
            this.Text = "Create Geotagged Image";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbTransvers;
        private System.Windows.Forms.RadioButton rbElliptic;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbOverlays;
        private System.Windows.Forms.CheckBox cbTransparentBG;
    }
}