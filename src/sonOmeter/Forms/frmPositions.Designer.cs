namespace sonOmeter
{
	public partial class frmPositions
	{
		private System.ComponentModel.IContainer components;
		
				#region Form Variables

        private System.Windows.Forms.CheckBox cbVisibleNMEA;
        private System.Windows.Forms.CheckBox cbVisibleCompass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbGeodimeter;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.CheckBox cbLeicaTPS;
        private System.Windows.Forms.CheckBox cbVisibleDisabled;
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPositions));
            this.cbVisibleNMEA = new System.Windows.Forms.CheckBox();
            this.cbVisibleCompass = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbGeodimeter = new System.Windows.Forms.CheckBox();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.cbLeicaTPS = new System.Windows.Forms.CheckBox();
            this.cbVisibleDisabled = new System.Windows.Forms.CheckBox();
            this.textColumn1 = new XPTable.Models.TextColumn();
            this.textColumn2 = new XPTable.Models.TextColumn();
            this.textColumn3 = new XPTable.Models.TextColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmsDataGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.enableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbVisibleUnused = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.cmsDataGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbVisibleNMEA
            // 
            this.cbVisibleNMEA.Checked = true;
            this.cbVisibleNMEA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisibleNMEA.Location = new System.Drawing.Point(86, 8);
            this.cbVisibleNMEA.Name = "cbVisibleNMEA";
            this.cbVisibleNMEA.Size = new System.Drawing.Size(69, 24);
            this.cbVisibleNMEA.TabIndex = 5;
            this.cbVisibleNMEA.Text = "NMEA";
            this.cbVisibleNMEA.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // cbVisibleCompass
            // 
            this.cbVisibleCompass.Checked = true;
            this.cbVisibleCompass.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisibleCompass.Location = new System.Drawing.Point(161, 8);
            this.cbVisibleCompass.Name = "cbVisibleCompass";
            this.cbVisibleCompass.Size = new System.Drawing.Size(72, 24);
            this.cbVisibleCompass.TabIndex = 6;
            this.cbVisibleCompass.Text = "Compass";
            this.cbVisibleCompass.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 8;
            this.label1.Text = "Type Visible:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbGeodimeter
            // 
            this.cbGeodimeter.Checked = true;
            this.cbGeodimeter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGeodimeter.Location = new System.Drawing.Point(239, 8);
            this.cbGeodimeter.Name = "cbGeodimeter";
            this.cbGeodimeter.Size = new System.Drawing.Size(90, 24);
            this.cbGeodimeter.TabIndex = 13;
            this.cbGeodimeter.Text = "Geodimeter";
            this.cbGeodimeter.UseVisualStyleBackColor = true;
            this.cbGeodimeter.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "*.csv";
            // 
            // cbLeicaTPS
            // 
            this.cbLeicaTPS.Checked = true;
            this.cbLeicaTPS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLeicaTPS.Location = new System.Drawing.Point(325, 8);
            this.cbLeicaTPS.Name = "cbLeicaTPS";
            this.cbLeicaTPS.Size = new System.Drawing.Size(73, 24);
            this.cbLeicaTPS.TabIndex = 14;
            this.cbLeicaTPS.Text = "LeicaTPS";
            this.cbLeicaTPS.UseVisualStyleBackColor = true;
            this.cbLeicaTPS.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // cbVisibleDisabled
            // 
            this.cbVisibleDisabled.AutoSize = true;
            this.cbVisibleDisabled.Checked = true;
            this.cbVisibleDisabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisibleDisabled.Location = new System.Drawing.Point(404, 12);
            this.cbVisibleDisabled.Name = "cbVisibleDisabled";
            this.cbVisibleDisabled.Size = new System.Drawing.Size(67, 17);
            this.cbVisibleDisabled.TabIndex = 15;
            this.cbVisibleDisabled.Text = "Disabled";
            this.cbVisibleDisabled.UseVisualStyleBackColor = true;
            this.cbVisibleDisabled.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // textColumn1
            // 
            this.textColumn1.Editable = false;
            // 
            // textColumn2
            // 
            this.textColumn2.Editable = false;
            // 
            // textColumn3
            // 
            this.textColumn3.Editable = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.cmsDataGrid;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(9, 41);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 20;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(647, 479);
            this.dataGridView1.TabIndex = 16;
            // 
            // cmsDataGrid
            // 
            this.cmsDataGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableAllToolStripMenuItem,
            this.saveAsCSVToolStripMenuItem});
            this.cmsDataGrid.Name = "cmsDataGrid";
            this.cmsDataGrid.Size = new System.Drawing.Size(146, 48);
            this.cmsDataGrid.Text = "Enable All";
            // 
            // enableAllToolStripMenuItem
            // 
            this.enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            this.enableAllToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.enableAllToolStripMenuItem.Text = "Enable All";
            this.enableAllToolStripMenuItem.Click += new System.EventHandler(this.enableAllToolStripMenuItem_Click);
            // 
            // saveAsCSVToolStripMenuItem
            // 
            this.saveAsCSVToolStripMenuItem.Name = "saveAsCSVToolStripMenuItem";
            this.saveAsCSVToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.saveAsCSVToolStripMenuItem.Text = "Save as CSV";
            this.saveAsCSVToolStripMenuItem.Click += new System.EventHandler(this.saveAsCSVToolStripMenuItem_Click);
            // 
            // cbVisibleUnused
            // 
            this.cbVisibleUnused.AutoSize = true;
            this.cbVisibleUnused.Checked = true;
            this.cbVisibleUnused.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisibleUnused.Location = new System.Drawing.Point(477, 12);
            this.cbVisibleUnused.Name = "cbVisibleUnused";
            this.cbVisibleUnused.Size = new System.Drawing.Size(63, 17);
            this.cbVisibleUnused.TabIndex = 15;
            this.cbVisibleUnused.Text = "Unused";
            this.cbVisibleUnused.UseVisualStyleBackColor = true;
            this.cbVisibleUnused.CheckedChanged += new System.EventHandler(this.cbVisibleCompass_CheckedChanged);
            // 
            // frmPositions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(664, 528);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbVisibleNMEA);
            this.Controls.Add(this.cbVisibleCompass);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cbLeicaTPS);
            this.Controls.Add(this.cbVisibleDisabled);
            this.Controls.Add(this.cbGeodimeter);
            this.Controls.Add(this.cbVisibleUnused);
            this.DockType = DockDotNET.DockContainerType.Document;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(552, 552);
            this.Name = "frmPositions";
            this.Text = "";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.cmsDataGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
        }
        private XPTable.Models.TextColumn textColumn1;
        private XPTable.Models.TextColumn textColumn2;
        private XPTable.Models.TextColumn textColumn3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip cmsDataGrid;
        private System.Windows.Forms.ToolStripMenuItem enableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsCSVToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbVisibleUnused;

  }
}  