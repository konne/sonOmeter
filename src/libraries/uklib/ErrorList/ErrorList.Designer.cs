namespace UKLib.ErrorList
{
    partial class ErrorList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Errors", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Warnings", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorList));
            this.lvItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.ilWhiteIcons = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnErorrs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnWarnings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMessages = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvItems
            // 
            this.lvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvItems.FullRowSelect = true;
            this.lvItems.GridLines = true;
            listViewGroup3.Header = "Errors";
            listViewGroup3.Name = "Errors";
            listViewGroup4.Header = "Warnings";
            listViewGroup4.Name = "Warnings";
            this.lvItems.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.lvItems.Location = new System.Drawing.Point(0, 26);
            this.lvItems.MultiSelect = false;
            this.lvItems.Name = "lvItems";
            this.lvItems.ShowGroups = false;
            this.lvItems.ShowItemToolTips = true;
            this.lvItems.Size = new System.Drawing.Size(359, 248);
            this.lvItems.SmallImageList = this.ilWhiteIcons;
            this.lvItems.TabIndex = 0;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            this.lvItems.DoubleClick += new System.EventHandler(this.lvItems_DoubleClick);
            this.lvItems.Resize += new System.EventHandler(this.lvItems_Resize);
            this.lvItems.SelectedIndexChanged += new System.EventHandler(this.lvItems_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 24;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "";
            this.columnHeader2.Width = 24;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 215;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Line";
            this.columnHeader4.Width = 47;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Column";
            this.columnHeader5.Width = 47;
            // 
            // ilWhiteIcons
            // 
            this.ilWhiteIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilWhiteIcons.ImageStream")));
            this.ilWhiteIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilWhiteIcons.Images.SetKeyName(0, "message_w.png");
            this.ilWhiteIcons.Images.SetKeyName(1, "warning_w.png");
            this.ilWhiteIcons.Images.SetKeyName(2, "error_w.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.btnErorrs,
            this.toolStripSeparator1,
            this.btnWarnings,
            this.toolStripSeparator2,
            this.btnMessages});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(360, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.ForeColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // btnErorrs
            // 
            this.btnErorrs.Checked = true;
            this.btnErorrs.CheckOnClick = true;
            this.btnErorrs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnErorrs.Image = ((System.Drawing.Image)(resources.GetObject("btnErorrs.Image")));
            this.btnErorrs.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnErorrs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnErorrs.Name = "btnErorrs";
            this.btnErorrs.Size = new System.Drawing.Size(23, 24);
            this.btnErorrs.Click += new System.EventHandler(this.btn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnWarnings
            // 
            this.btnWarnings.Checked = true;
            this.btnWarnings.CheckOnClick = true;
            this.btnWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnWarnings.Image = ((System.Drawing.Image)(resources.GetObject("btnWarnings.Image")));
            this.btnWarnings.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnWarnings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWarnings.Name = "btnWarnings";
            this.btnWarnings.Size = new System.Drawing.Size(23, 24);
            this.btnWarnings.Click += new System.EventHandler(this.btn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnMessages
            // 
            this.btnMessages.Checked = true;
            this.btnMessages.CheckOnClick = true;
            this.btnMessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnMessages.Image = ((System.Drawing.Image)(resources.GetObject("btnMessages.Image")));
            this.btnMessages.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMessages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMessages.Name = "btnMessages";
            this.btnMessages.Size = new System.Drawing.Size(23, 24);
            this.btnMessages.Click += new System.EventHandler(this.btn_Click);
            // 
            // ErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lvItems);
            this.Name = "ErrorList";
            this.Size = new System.Drawing.Size(359, 274);
            this.Load += new System.EventHandler(this.ErrorList_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnErorrs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnWarnings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnMessages;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ImageList ilWhiteIcons;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
