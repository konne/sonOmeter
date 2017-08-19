namespace sonOmeter
{
    partial class frmManualPoints
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
            XPTable.Models.DataSourceColumnBinder dataSourceColumnBinder1 = new XPTable.Models.DataSourceColumnBinder();
            this.xpTable = new XPTable.Models.Table();
            this.cm = new XPTable.Models.ColumnModel();
            this.textColID = new XPTable.Models.TextColumn();
            this.textColCoord = new sonOmeter.Classes.XPTableCoordColumn();
            this.textColDepth = new XPTable.Models.TextColumn();
            this.textColTotal = new XPTable.Models.TextColumn();
            this.colorCol = new XPTable.Models.ComboBoxColumn();
            this.textColDesc = new XPTable.Models.TextColumn();
            this.dateTimeCol = new XPTable.Models.DateTimeColumn();
            this.textColType = new XPTable.Models.TextColumn();
            this.cbColShowDepth = new XPTable.Models.CheckBoxColumn();
            this.cbColShowDesc = new XPTable.Models.CheckBoxColumn();
            this.cbColShowType = new XPTable.Models.CheckBoxColumn();
            this.tm = new XPTable.Models.TableModel();
            ((System.ComponentModel.ISupportInitialize)(this.xpTable)).BeginInit();
            this.SuspendLayout();
            // 
            // xpTable
            // 
            this.xpTable.AlternatingRowColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.xpTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xpTable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.xpTable.BorderColor = System.Drawing.Color.Black;
            this.xpTable.ColumnModel = this.cm;
            this.xpTable.DataMember = null;
            this.xpTable.DataSourceColumnBinder = dataSourceColumnBinder1;
            this.xpTable.EnableWordWrap = true;
            this.xpTable.FullRowSelect = true;
            this.xpTable.GridLines = XPTable.Models.GridLines.Both;
            this.xpTable.Location = new System.Drawing.Point(13, 12);
            this.xpTable.Name = "xpTable";
            this.xpTable.NoItemsText = "";
            this.xpTable.Size = new System.Drawing.Size(775, 504);
            this.xpTable.TabIndex = 0;
            this.xpTable.TableModel = this.tm;
            this.xpTable.UnfocusedBorderColor = System.Drawing.Color.Black;
            this.xpTable.CellCheckChanged += new XPTable.Events.CellCheckBoxEventHandler(this.xpTable_CellCheckChanged);
            this.xpTable.BeginEditing += new XPTable.Events.CellEditEventHandler(this.xpTable_BeginEditing);
            this.xpTable.EditingStopped += new XPTable.Events.CellEditEventHandler(this.xpTable_EditingStopped);
            this.xpTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.xpTable_KeyDown);
            // 
            // cm
            // 
            this.cm.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColID,
            this.textColCoord,
            this.textColDepth,
            this.textColTotal,
            this.colorCol,
            this.textColDesc,
            this.dateTimeCol,
            this.textColType,
            this.cbColShowDepth,
            this.cbColShowDesc,
            this.cbColShowType});
            this.cm.HeaderHeight = 22;
            // 
            // textColID
            // 
            this.textColID.IsTextTrimmed = false;
            this.textColID.Sortable = false;
            this.textColID.Text = "ID";
            this.textColID.Width = 45;
            // 
            // textColCoord
            // 
            this.textColCoord.IsTextTrimmed = false;
            this.textColCoord.Sortable = false;
            this.textColCoord.Text = "Coordinate";
            this.textColCoord.Width = 260;
            // 
            // textColDepth
            // 
            this.textColDepth.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.textColDepth.IsTextTrimmed = false;
            this.textColDepth.Sortable = false;
            this.textColDepth.Text = "Depth";
            // 
            // textColTotal
            // 
            this.textColTotal.Alignment = XPTable.Models.ColumnAlignment.Right;
            this.textColTotal.Editable = false;
            this.textColTotal.IsTextTrimmed = false;
            this.textColTotal.Sortable = false;
            this.textColTotal.Text = "Total";
            // 
            // colorCol
            // 
            this.colorCol.IsTextTrimmed = false;
            this.colorCol.Text = "Color";
            // 
            // textColDesc
            // 
            this.textColDesc.IsTextTrimmed = false;
            this.textColDesc.Sortable = false;
            this.textColDesc.Text = "Description";
            // 
            // dateTimeCol
            // 
            this.dateTimeCol.IsTextTrimmed = false;
            this.dateTimeCol.Sortable = false;
            this.dateTimeCol.Text = "Time and Date";
            this.dateTimeCol.Width = 150;
            // 
            // textColType
            // 
            this.textColType.IsTextTrimmed = false;
            this.textColType.Sortable = false;
            this.textColType.Text = "Type";
            // 
            // cbColShowDepth
            // 
            this.cbColShowDepth.Alignment = XPTable.Models.ColumnAlignment.Center;
            this.cbColShowDepth.IsTextTrimmed = false;
            this.cbColShowDepth.Sortable = false;
            this.cbColShowDepth.Text = "Z";
            this.cbColShowDepth.ToolTipText = "Show depth in record view";
            this.cbColShowDepth.Width = 30;
            // 
            // cbColShowDesc
            // 
            this.cbColShowDesc.Alignment = XPTable.Models.ColumnAlignment.Center;
            this.cbColShowDesc.IsTextTrimmed = false;
            this.cbColShowDesc.Sortable = false;
            this.cbColShowDesc.Text = "D";
            this.cbColShowDesc.ToolTipText = "Show description in record view";
            this.cbColShowDesc.Width = 30;
            // 
            // cbColShowType
            // 
            this.cbColShowType.Alignment = XPTable.Models.ColumnAlignment.Center;
            this.cbColShowType.IsTextTrimmed = false;
            this.cbColShowType.Sortable = false;
            this.cbColShowType.Text = "T";
            this.cbColShowType.ToolTipText = "Show type in record view";
            this.cbColShowType.Width = 30;
            // 
            // tm
            // 
            this.tm.RowHeight = 40;
            // 
            // frmManualPoints
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(799, 528);
            this.Controls.Add(this.xpTable);
            this.DockType = DockDotNET.DockContainerType.Document;
            this.MinimumSize = new System.Drawing.Size(552, 552);
            this.Name = "frmManualPoints";
            this.Text = "Manual Points";
            ((System.ComponentModel.ISupportInitialize)(this.xpTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XPTable.Models.Table xpTable;
        private XPTable.Models.ColumnModel cm;
        private XPTable.Models.TextColumn textColID;
        private XPTable.Models.TableModel tm;
        private sonOmeter.Classes.XPTableCoordColumn textColCoord;
        private XPTable.Models.TextColumn textColDepth;
        private XPTable.Models.TextColumn textColDesc;
        private XPTable.Models.DateTimeColumn dateTimeCol;
        private XPTable.Models.TextColumn textColType;
        private XPTable.Models.TextColumn textColTotal;
        private XPTable.Models.ComboBoxColumn colorCol;
        private XPTable.Models.CheckBoxColumn cbColShowDepth;
        private XPTable.Models.CheckBoxColumn cbColShowDesc;
        private XPTable.Models.CheckBoxColumn cbColShowType;
    }
}
