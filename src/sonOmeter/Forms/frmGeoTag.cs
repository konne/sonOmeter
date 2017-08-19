using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Math;

namespace sonOmeter
{
    public partial class frmGeoTag : Form
    {
        public string FileName { get; set; }
        public CoordinateType Type { get; set; }
        public bool DrawOverlays { get; set; }
        public bool TransparentBG { get; set; }

        public frmGeoTag()
        {
            InitializeComponent();
            Type = CoordinateType.Elliptic;
            DrawOverlays = true;
            TransparentBG = true;

            this.DataBindings.Add("DrawOverlays", cbOverlays, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("TransparentBG", cbTransparentBG, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
            this.DataBindings.Add("FileName", tbFile, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            tbFile.Text = sfd.FileName;
        }

        private void rbElliptic_CheckedChanged(object sender, EventArgs e)
        {
            if (rbElliptic.Checked)
                Type = CoordinateType.Elliptic;
            else
                Type = CoordinateType.TransverseMercator;
        }
    }
}
