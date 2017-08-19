using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace UKLib.ErrorList
{
    public partial class ErrorList : UserControl
    {
        #region Variables
        public List<ErrorListItem> Items = new List<ErrorListItem>();

        public event ErrorListEventHandler ItemDblClick;

        #endregion

        #region Properties

        public bool MessagesVisible
        {
            get { return btnMessages.Checked; }
            set 
            {
                btnMessages.Checked = value;
                RefreshView();
            }
        }

        public bool WarningsVisible
        {
            get { return btnWarnings.Checked; } 
            set 
            {
                btnWarnings.Checked = value;
                RefreshView();
            }
        }

        public bool ErrorsVisible
        {
            get { return btnErorrs.Checked; }
            set 
            {
                btnErorrs.Checked = value;
                RefreshView();
            }
        }

        #endregion

        #region Constructor
        public ErrorList()
        {
            InitializeComponent();
            RefreshView();
        }
        #endregion

        #region Functions
        public void RefreshView()
        {
            lvItems.Visible = false;
            int cntErros = 0;
            int cntMessages = 0;
            int cntWarnings = 0;

            lvItems.Items.Clear();
            for (int i = 0; i < Items.Count; i++ )
            {
                switch (Items[i].Status)
                {
                    case ErrorStatus.Error:
                        if (!btnErorrs.Checked) continue;
                        break;
                    case ErrorStatus.Warning:
                        if (!btnWarnings.Checked) continue;
                        break;
                    case ErrorStatus.Message:
                        if (!btnMessages.Checked) continue;
                        break;
                }

                ListViewItem item;
                ListViewItem.ListViewSubItemCollection itm;

                item = lvItems.Items.Add("");
                item.Tag = Items[i];
                item.ToolTipText = Items[i].Description;

                switch (Items[i].Status)
                {
                    case ErrorStatus.Error:
                        item.ImageIndex = 2;
                        cntErros++;
                        break;
                    case ErrorStatus.Warning:
                        item.ImageIndex = 1;
                        cntWarnings++;
                        break;
                    case ErrorStatus.Message:
                        item.ImageIndex = 0;
                        cntMessages++;
                        break;
                }

                itm = item.SubItems;
                itm.Add(i.ToString());
                itm.Add(Items[i].Description);
                itm.Add(Items[i].Line.ToString());
                itm.Add(Items[i].Column.ToString());
            }
            btnErorrs.Text = cntErros.ToString() + " Error";
            btnWarnings.Text = cntWarnings.ToString() + " Warning";
            btnMessages.Text = cntMessages.ToString() + " Message";
            if (cntErros != 1) btnErorrs.Text += "s";
            if (cntWarnings != 1) btnWarnings.Text += "s";
            if (cntMessages != 1) btnMessages.Text += "s";
            lvItems.Visible = true;
        }
        #endregion

        #region Events
        private void lvItems_Resize(object sender, EventArgs e)
        {
            lvItems.Columns[2].Width = lvItems.Width - 144;
        }

        private void ErrorList_Load(object sender, EventArgs e)
        {
            lvItems_Resize(null, null);
        }
        

        private void btn_Click(object sender, EventArgs e)
        {
            RefreshView();
        }
        
        private void lvItems_DoubleClick(object sender, EventArgs e)
        {
            if (lvItems.SelectedItems.Count > 0)
            {
                if (ItemDblClick != null)

                    try
                    {
                        ErrorListItem itm = lvItems.SelectedItems[0].Tag as ErrorListItem;
                        ItemDblClick(this, new ErrorListArgs(itm));
                    }
                    catch
                    { 
                    }
            }
        }
        #endregion

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

    public class ErrorListArgs : EventArgs
    {
        private ErrorListItem item;

        public ErrorListItem Item
        {
            get { return item; }
        }

        public ErrorListArgs(ErrorListItem item)
        {
            this.item = item;
        }
    }

    public delegate void ErrorListEventHandler(object sender, ErrorListArgs e);

    public enum ErrorStatus
    {
        Message = 0,
        Warning = 1,
        Error = 2
    }

    public class ErrorListItem
    {
        #region Variables
        
        private ErrorStatus status;
        private string description;
        private int line;
        private int column;

        #endregion

        #region Properties
        public ErrorStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        
        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        #endregion

        #region Constructor
        public ErrorListItem(ErrorStatus status, string description, int line, int column)
        {
            this.status = status;
            this.description = description;
            this.line = line;
            this.column = column;
        }
        #endregion
    }
}
