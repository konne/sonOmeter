namespace UKLib.DXF
{
    partial class DXFLayerForm
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
            this.clbLayers = new System.Windows.Forms.CheckedListBox();
            this.pgLayer = new System.Windows.Forms.PropertyGrid();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clbLayers
            // 
            this.clbLayers.FormattingEnabled = true;
            this.clbLayers.Location = new System.Drawing.Point(12, 12);
            this.clbLayers.Name = "clbLayers";
            this.clbLayers.Size = new System.Drawing.Size(172, 334);
            this.clbLayers.TabIndex = 0;
            this.clbLayers.SelectedIndexChanged += new System.EventHandler(this.clbLayers_SelectedIndexChanged);
            // 
            // pgLayer
            // 
            this.pgLayer.Location = new System.Drawing.Point(190, 12);
            this.pgLayer.Name = "pgLayer";
            this.pgLayer.Size = new System.Drawing.Size(179, 304);
            this.pgLayer.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(190, 322);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(179, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // DXFLayerForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 357);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pgLayer);
            this.Controls.Add(this.clbLayers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DXFLayerForm";
            this.ShowInTaskbar = false;
            this.Text = "DXF Layers";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbLayers;
        private System.Windows.Forms.PropertyGrid pgLayer;
        private System.Windows.Forms.Button btnOK;
    }
}