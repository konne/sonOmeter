using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using UKLib.Debug;
using sonOmeter.Classes;
using System.Collections.Specialized;
using System.Xml;
using sonOmeter.Classes.Sonar2D;
using System.Globalization;

namespace sonOmeter
{
	public partial class frmSON2XML : Form
	{
        SonarProject project;

       

		public frmSON2XML(SonarProject project)
		{
			InitializeComponent();

            int i;
            for (i = 1995; i <= DateTime.Now.Year; i++)
                cbYears.Items.Add(i);
            cbYears.SelectedItem = cbYears.Items[cbYears.Items.Count - 1];

            this.project = project;
		}

        private void LoadFile(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".prj")
            {
                PrjFile prj = new PrjFile(filename);
                tbProjName.Text = prj.Name;
                foreach (string s in prj.SonFiles)
                {
                    ListViewItem itm = lvFiles.Items.Add("");
                    itm.Checked = true;
                    itm.SubItems.Add(s);
                }
                foreach (PrjFile.Boje boj in prj.Bojen)
                {
                    ListViewItem itm = lvBojen.Items.Add(boj.name);
                    itm.Tag = boj;
                    itm.SubItems.Add(boj.hw.ToString("0.00"));
                    itm.SubItems.Add(boj.rw.ToString("0.00"));
                    itm.SubItems.Add(boj.al.ToString("0.00"));
                }
            }
            else
            {
                ListViewItem itm = lvFiles.Items.Add("");
                itm.Checked = true;
                itm.SubItems.Add(filename);
            }
        }

		private void frmSON2XML_Load(object sender, EventArgs e)
		{
			if (dlgOpen.ShowDialog() != DialogResult.OK)
			{
				this.Close();
				return;
			}
            LoadFile(dlgOpen.FileName);          
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
            tlsProgress.Maximum = lvFiles.Items.Count;
            foreach (ListViewItem itm in lvFiles.Items)
            {
                if (itm.Checked)
                {
                    try
                    {
                        tlsLabel.Text = "Import: " + Path.GetFileName(itm.SubItems[1].Text);
                        Application.DoEvents();
                        sonFile son = new sonFile(itm.SubItems[1].Text, cbImportCut.Checked, cbYears.Text, tbDX.Text, tbDY.Text, tbDZ.Text, "0.0", "0.0");
                        project.AddRecord(son.record);
                    }
                    catch(Exception ex)
                    {
                        UKLib.Debug.DebugClass.SendDebugLine(this, DebugLevel.Red, itm.SubItems[1].Text+ " - " + ex.Message);
                    }
                }
                tlsProgress.Increment(1);
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }
            foreach (ListViewItem itm in lvBojen.Items)
            {
                PrjFile.Boje boje = (PrjFile.Boje)itm.Tag; 
                Buoy bj = new Buoy();
                bj.AL = boje.al;
                bj.HV = boje.hw;
                bj.RV = boje.rw;
                bj.Color = Color.Black;
                bj.Depth =0;
                bj.Description = boje.name;
                project.BuoyList.Add(bj);
            }
            this.Close();            
        }

        private void btnAddSonFile_Click(object sender, EventArgs e)
        {
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                LoadFile(dlgOpen.FileName);
            }
           
        }
	}
}