using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using sonOmeter.Classes;
using sonOmeter.Properties;
using System.Globalization;

namespace sonOmeter
{
    public partial class frmExport3D : Form
    {
        public Sonar3DRecord Rec3D { get; set; }

        public frmExport3D()
        {
            InitializeComponent();
        }

        private void frmExport3D_Load(object sender, EventArgs e)
        {
            cbDepth.Checked = Settings.Default.frmExport3D_CreateFileZ;
            cbBlankline.Checked = Settings.Default.frmExport3D_CreateFileB;
            cbVolume.Checked = Settings.Default.frmExport3D_CreateFileV;
            cbTopColor.Checked = Settings.Default.frmExport3D_CreateFileC;
            rbLALO.Checked = Settings.Default.frmExport3D_CoordTypeLALO;
            rbRVHV.Checked = !Settings.Default.frmExport3D_CoordTypeLALO;
            rbNF.Checked = Settings.Default.frmExport3D_SonarTypeNF;
            rbHF.Checked = !Settings.Default.frmExport3D_SonarTypeNF;

            tbSweepFrom.Text = GSC.Settings.ArchDepth.ToString(NumberFormatInfo.InvariantInfo);
            tbSweepTo.Text = GSC.Settings.ArchDepth.ToString(NumberFormatInfo.InvariantInfo);
            tbSweepCount.Text = "1";
        }

        private void frmExport3D_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.frmExport3D_CreateFileZ = cbDepth.Checked;
            Settings.Default.frmExport3D_CreateFileB = cbBlankline.Checked;
            Settings.Default.frmExport3D_CreateFileV = cbVolume.Checked;
            Settings.Default.frmExport3D_CreateFileC = cbTopColor.Checked;
            Settings.Default.frmExport3D_CoordTypeLALO = rbLALO.Checked;
            Settings.Default.frmExport3D_SonarTypeNF = rbNF.Checked;
            Settings.Default.Save();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string file = saveFileDialog.FileName;

            if (Path.GetFileName(file).Length == 0)
                return;

            tbFile.Text = file;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (tbFile.Text.Length == 0)
            {
                MessageBox.Show("Please enter a destination file name.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            float from = 0;
            float to = 0;
            int steps = 1;

            if (!float.TryParse(tbSweepFrom.Text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out from) ||
                !float.TryParse(tbSweepTo.Text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out to) ||
                !int.TryParse(tbSweepCount.Text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out steps))
            {
                MessageBox.Show("Please enter valid sweep numbers.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            if (from <= 0)
            {
                MessageBox.Show("From must be greater than 0.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            if (steps < 1)
            {
                MessageBox.Show("Please enter a valid sweep step number.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            if (to < from)
            {
                MessageBox.Show("Please enter a valid sweep range.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            if ((to != from) && (steps == 1))
            {
                MessageBox.Show("Please enter a valid sweep step number for the specified range.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            float arch = GSC.Settings.ArchDepth;
            progressBar.Maximum = steps;

            for (int a = 0; a < steps; a++)
            {
                arch = from + (to - from) * (float)a / (float)(steps - 1);
                string file = Path.GetDirectoryName(tbFile.Text) + "\\" + Path.GetFileNameWithoutExtension(tbFile.Text) + ((steps > 1) ? "_" + (arch * 10.0F).ToString("000") : "") + Path.GetExtension(tbFile.Text);

                Rec3D.ApplySpecialArchSync(rbHF.Checked ? SonarPanelType.HF : SonarPanelType.NF, arch, arch);
                Rec3D.WriteSurfer(file, cbDepth.Checked, cbBlankline.Checked, cbVolume.Checked, cbTopColor.Checked, rbLALO.Checked, rbHF.Checked ? SonarPanelType.HF : SonarPanelType.NF);

                progressBar.Value = a;
                Application.DoEvents();
            }

            progressBar.Value = steps;
            Application.DoEvents();

            arch = GSC.Settings.ArchDepth;
            Rec3D.ApplySpecialArchSync(rbHF.Checked ? SonarPanelType.HF : SonarPanelType.NF, arch, arch);

            this.Close();
        }
    }
}
