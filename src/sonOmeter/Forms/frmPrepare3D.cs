using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.MathEx;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;

namespace sonOmeter
{
    public partial class frmPrepare3D : Form
    {
        SonarProject project = null;

        public frmPrepare3D(SonarProject prj)
        {
            InitializeComponent();

            project = prj;

            lbBlankLines.DataSource = project.BlankLines;
        }

        #region Properties for text box conversion
        private double GridX
        {
            get
            {
                double gridX = 0;

                if (tbGridX.Text.Contains(","))
                    if (MessageBox.Show("The X grid was not entered correctly.\nPlease use '.' instead of ',' as a decimal seperator.\n\nThe ',' will be interpreted as '.'.", "Number format error!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        tbGridX.Text = tbGridX.Text.Replace(',', '.');

                double.TryParse(tbGridX.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out gridX);
                return gridX;
            }
        }

        private double GridY
        {
            get
            {
                double gridY = 0;

                if (tbGridY.Text.Contains(","))
                    if (MessageBox.Show("The Y grid was not entered correctly.\nPlease use '.' instead of ',' as a decimal seperator.\n\nThe ',' will be interpreted as '.'.", "Number format error!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        tbGridY.Text = tbGridY.Text.Replace(',', '.');

                double.TryParse(tbGridY.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out gridY);
                return gridY;
            }
        }

        private double DepthRes
        {
            get
            {
                double depthRes = 0;

                if (tbDepthRes.Text.Contains(","))
                    if (MessageBox.Show("The depth resolution was not entered correctly.\nPlease use '.' instead of ',' as a decimal seperator.\n\nThe ',' will be interpreted as '.'.", "Number format error!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        tbDepthRes.Text = tbDepthRes.Text.Replace(',', '.');

                double.TryParse(tbDepthRes.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out depthRes);
                return depthRes;
            }
        } 
        #endregion

        private void btnGenerateRecord_Click(object sender, EventArgs e)
        {
            #region Check if everything makes sense.
            if (project == null)
            {
                MessageBox.Show("Error! No project defined.", this.Text);
                Close();
                return;
            }

            PolygonD blankline = project.BlankLines[0].Poly;

            if (blankline.Count == 0)
            {
                MessageBox.Show("Please specify a blankline in the 2D view to determine the interpolation area.", this.Text);
                Close();
                return;
            }

            if (lbBlankLines.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select one or more blanklines from the list.", this.Text);
                return;
            }
            #endregion

            #region Read the dialog box Settings.Sets.
            LineData.MergeMode mode = rbTopColorMost.Checked ? LineData.MergeMode.Occurance : LineData.MergeMode.Strongest;
            Sonar3DIntMethod intMethod = rbDepthNumber.Checked ? Sonar3DIntMethod.NearestNeighbours : Sonar3DIntMethod.MaximumNeighbourDistance;
            Sonar3DIntWeighting intWeighting = rbWeightingLinear.Checked ? Sonar3DIntWeighting.Linear : Sonar3DIntWeighting.Quadratic;

            int intMethodNum = 5;

            if (!int.TryParse(tbDepthConstraint.Text, out intMethodNum))
            {
                MessageBox.Show("Error! Invalid number entered in depth constraint.", this.Text);
                return;
            }

            int intIterations = 5;

            if (!int.TryParse(tbIterationCount.Text, out intIterations))
            {
                MessageBox.Show("Error! Invalid number entered in iteration count.", this.Text);
                return;
            }
            #endregion

            for (int i = 0; i < lbBlankLines.SelectedItems.Count; i++)
            {
                Sonar3DRecord record = new Sonar3DRecord() { Project = project, GridX = this.GridX, GridY = this.GridY, DepthRes = this.DepthRes, Mode = mode, BlankLine = lbBlankLines.SelectedItems[i] as BlankLine, ShowInTrace = true, Int3DIterations = intIterations, Int3DMethod = intMethod, Int3DThreshold = intMethodNum, Int3DWeighting = intWeighting };
                record.EnableInterpolation = true;
                record.Interpolate();
                project.AddRecord(record);
            }

            // Finally, close the window.
            Close();
        }

        private void cbLink_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLink.Checked)
            {
                tbGridY.Text = tbGridX.Text;
                tbGridY.ReadOnly = true;
            }
            else
                tbGridY.ReadOnly = false;
        }

        private void tbGridX_TextChanged(object sender, EventArgs e)
        {
            if (cbLink.Checked)
                tbGridY.Text = tbGridX.Text;
        }

        private void rbDepthNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDepthNumber.Checked)
                labDepthMethod.Text = "Number of neighbours:";
            else
                labDepthMethod.Text = "Number of grid units:";
        }
    }
}