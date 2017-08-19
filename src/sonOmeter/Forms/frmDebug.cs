using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Debug;
using System.IO;

namespace sonOmeter
{
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmDebug : DockDotNET.DockWindow
    {
        public frmDebug(ISynchronizeInvoke sync)
        {
            DebugClass.SynchronizingObject = sync;
            DebugClass.OnDebugLines += new DebugLineEventHandler(DebugClass_OnDebugLines);
            InitializeComponent();
        }

        void DebugClass_OnDebugLines(object sender, DebugLineEventArgs e)
        {
            if (e.Level == DebugLevel.Red)
            {
                ListViewItem item = lvErrors.Items.Add(e.Level.ToString());
                item.SubItems.Add(e.DebugLine);
            }
        }

        private void lvErrors_Resize(object sender, EventArgs e)
        {
            try
            {
                lvErrors.Columns[1].Width = lvErrors.Width - 65;
            }
            catch
            {
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                TextWriter tw = new StreamWriter(dlgSave.FileName);
                for (int i = 0; i < lvErrors.Items.Count; i++)
                {
                    tw.WriteLine(lvErrors.Items[i].Text+"\t"+lvErrors.Items[i].SubItems[1].Text);
                }
                tw.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvErrors.Items.Clear();
        }
    }
}