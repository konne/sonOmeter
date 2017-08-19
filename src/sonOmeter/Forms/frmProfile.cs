using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using sonOmeter.Classes.Sonar2D;
using sonOmeter.Classes;
using System.Collections.ObjectModel;
using System.IO;
using UKLib.Arrays;

namespace sonOmeter
{
    public partial class frmProfile : Form
    {
        private IDList<Sonar2DElement> buoyConnectionsList = null;
        private SonarProject project = null;

        public frmProfile(SonarProject project)
        {
            InitializeComponent();
            this.project = project;
            this.buoyConnectionsList = project.BuoyConnectionList;

            btnDynamicRecords.Enabled = GSC.Settings.Lic[Module.Modules.DynamicProfiles];
        }

        private double SamplingRes
        {
            get
            {
                double samplingRes = 0;

                if (tbSamplingRes.Text.Contains(","))
                    if (MessageBox.Show("The sampling resolution was not entered correctly.\nPlease use '.' instead of ',' as a decimal seperator.\n\nThe ',' will be interpreted as '.'.", "Number format error!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        tbSamplingRes.Text = tbSamplingRes.Text.Replace(',', '.');

                double.TryParse(tbSamplingRes.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out samplingRes);
                return samplingRes;
            }
        }

        private double SamplingResZ
        {
            get
            {
                double samplingResZ = 0;

                if (tbSamplingRes.Text.Contains(","))
                    if (MessageBox.Show("The sampling resolution was not entered correctly.\nPlease use '.' instead of ',' as a decimal seperator.\n\nThe ',' will be interpreted as '.'.", "Number format error!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        tbSamplingRes.Text = tbSamplingRes.Text.Replace(',', '.');

                double.TryParse(tbSamplingResZ.Text, System.Globalization.NumberStyles.Float, GSC.Settings.NFI, out samplingResZ);
                return samplingResZ;
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            lbConnections.BeginUpdate();
            lbConnections.Items.Clear();

            foreach (BuoyConnection c in buoyConnectionsList)
                lbConnections.Items.Add(c);

            lbConnections.EndUpdate();
        }

        private List<SonarProfile> CreateProfiles(bool dynamic)
        {
            List<SonarProfile> profiles = new List<SonarProfile>();

            #region Set the default color and merge mode settings.
            bool[] colors = new bool[7];

            colors[0] = true;
            colors[1] = true;
            colors[2] = true;
            colors[3] = true;
            colors[4] = true;
            colors[5] = true;
            colors[6] = true;

            LineData.MergeMode mode = LineData.MergeMode.Strongest;
            #endregion

            for (int j = 0; j < lbConnections.SelectedItems.Count; j++)
            {
                BuoyConnection conn = lbConnections.SelectedItems[j] as BuoyConnection;
                SonarProfile profile = new SonarProfile(conn, dynamic);

                profile.NormalizeSL = cbNormSL.Checked;
                profile.NormalizeMP = cbNormMP.Checked;
                profile.MergeManualPoints = cbMergeManualPoints.Checked;
                profile.SamplingRes = this.SamplingRes;
                profile.DepthRes = this.SamplingResZ;
                profile.Mode = mode;
                profile.Colors = colors;

                profile.CreateProfile(project);
                profiles.Add(profile);
            }

            return profiles;
        }

        private void OnExport(object sender, EventArgs e)
        {
            StringCollection st = new StringCollection();

            frmExport form = new frmExport();
            if (form.ShowDialog() == DialogResult.OK)
            {
                dlgSave.FileName = Path.ChangeExtension(project.FileName, ".txt");
                if (dlgSave.ShowDialog() == DialogResult.OK)
                {
                    List<SonarProfile> profiles = CreateProfiles(false);

                    for (int j = 0; j < profiles.Count; j++)
                    {
                        SonarProfile profile = profiles[j];
                        BuoyConnection conn = profile.Profile;
                        
                        if (!cbOneFile.Checked)
                        {
                            st.Clear();
                            form.exp.CompileExportFunc(new List<UKLib.ErrorList.ErrorListItem>());
                        }

                        List<SonarLine> lines = new List<SonarLine>(profile.SonarLines());
                        List<ManualPoint> points = new List<ManualPoint>(profile.ManualPoints);

                        if (cbManualPointsDominate.Checked)
                        {
                            // Remove lines that are tags of manual points.
                            foreach (ManualPoint point in points)
                                if ((point.Tag is SonarLine) && lines.Contains(point.Tag as SonarLine))
                                    lines.Remove(point.Tag as SonarLine);
                        }

                        if (form.exp.ExpSettings.PitchH != 0 | form.exp.ExpSettings.PitchV != 0)
                            SonarRecord.ApplyScaleMarks(lines, form.exp.ExpSettings.PitchH, form.exp.ExpSettings.PitchV, form.exp.ExpSettings.ExportWithCut);

                        int max = lines.Count;
                        SonarLine line;

                        for (int i = 0; i < max; i++)
                        {
                            line = lines[i];
                            line.ApplyArchAndVolume(form.exp.ExpSettings.ExportWithCut);
                            line.HF.GetVolume(true);
                            line.HF.GetVolume(true);
                            DateTime time = DateTime.Now;

                            if ((form.exp.ExpSettings.PitchH == 0 & form.exp.ExpSettings.PitchV == 0) | (line.IsMarked))
                            {
                                string s = "";

                                switch (form.exp.ExpSettings.ExportType)
                                {
                                    case SonarPanelType.HF:
                                        s = form.exp.ExportSonarLine(line, SonarPanelType.HF, 0, time);
                                        if (s != "") st.Add(s);
                                        break;

                                    case SonarPanelType.NF:
                                        s = form.exp.ExportSonarLine(line, SonarPanelType.NF, 0, time);
                                        if (s != "") st.Add(s);
                                        break;

                                    case SonarPanelType.Void:
                                        s = form.exp.ExportSonarLine(line, SonarPanelType.HF, 0, time);
                                        if (s != "") st.Add(s);
                                        s = form.exp.ExportSonarLine(line, SonarPanelType.NF, 0, time);
                                        if (s != "") st.Add(s);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        foreach (ManualPoint pt in points)
                        {
                            string s = form.exp.ExportPoint(pt, 0);
                            if (s != "")
                                st.Add(s);
                        }
                        project.AppendBuoysToExport(st, form.exp);

                        if ((st.Count > 0) && (!cbOneFile.Checked | ((j + 1) == profiles.Count)))
                        {
                            string filename = Path.ChangeExtension(dlgSave.FileName, "-Connection [" + conn.ToString().Replace(" ", "") + "].txt");
                            if (cbOneFile.Checked) filename = dlgSave.FileName;
                            StreamWriter sw = new StreamWriter(filename);
                            foreach (string s in st)
                                sw.WriteLine(s);
                            sw.Close();
                            if (form.exp.ExportLog != "")
                            {
                                filename = Path.ChangeExtension(filename, ".log");
                                sw = new StreamWriter(filename);
                                sw.WriteLine(form.exp.ExportLog);
                                sw.Close();
                            }
                        }
                    }

                }
                Close();

            }
            form.Dispose();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            Close();
        }

        private void OnStaticRecords(object sender, EventArgs e)
        {
            #region Old code
            /*
            #region Read the dialog box Settings.Sets.
            bool[] colors = new bool[7];

            colors[0] = true;
            colors[1] = true;
            colors[2] = true;
            colors[3] = true;
            colors[4] = true;
            colors[5] = true;
            colors[6] = true;

            LineData.MergeMode mode = LineData.MergeMode.Strongest;
            #endregion

            double samplingRes = this.SamplingRes;

            for (int j = 0; j < lbConnections.SelectedItems.Count; j++)
            {
                BuoyConnection conn = lbConnections.SelectedItems[j] as BuoyConnection;
                SonarRecord record = new SonarRecord();
                record.Description = conn.ToString();
                record.Devices.Add(new SonarDevice(0, "Profile", 0, 0, 0, 0, 0, true, "", "", true, true, 0, 0, ""));
                DateTime timeStart = DateTime.MinValue;
                DateTime timeEnd = DateTime.MaxValue;
                DateTime time;
                record.ShowInTrace = false;
                record.IsProfile = true;

                List<SonarLine> lines = new List<SonarLine>();
                List<ManualPoint> points = new List<ManualPoint>();

                project.GetSortedCorridorLines(conn, lines, points, cbNormSL.Checked, cbNormMP.Checked, samplingRes, mode, colors, cbMergeManualPoints.Checked, cbManualPointsDominate.Checked, 0.1);

                int i = 0;
                int max = lines.Count;

                for (i = 0; i < max; i++)
                {
                    time = lines[i].Time;

                    if (lines[i].CoordRvHv.Type == UKLib.Survey.Math.CoordinateType.TransversMercator)
                    {
                        string s2 = "fixed;rv=" + (lines[i].CoordRvHv.HV).ToString("0.000", GSC.Settings.NFI) + ";hv=" + (lines[i].CoordRvHv.RV).ToString("0.000", GSC.Settings.NFI) + ";al=" + (lines[i].CoordRvHv.AL).ToString("0.000", GSC.Settings.NFI) + ";";
                        lines[i].PosList.Add(new SonarPos(time, PosType.Fixed, false, s2));
                    }

                    if (((time.CompareTo(timeStart) < 0) || (timeStart == DateTime.MinValue)) && (time != DateTime.MinValue))
                        timeStart = time;

                    if (((time.CompareTo(timeEnd) > 0) || (timeEnd == DateTime.MaxValue)) && (time != DateTime.MinValue))
                        timeEnd = time;

                    record.AddSonarLine(lines[i], false);
                }

                max = points.Count;

                for (i = 0; i < max; i++)
                    record.AddManualPoint(points[i]);

                if (cbAddBoyAsPoint.Checked)
                {
                    record.InsertManualPoint(new ManualPoint(conn.StartBuoy.Coord, conn.StartBuoy.Depth, conn.StartBuoy.Type, conn.StartBuoy.Description), 0);
                    record.AddManualPoint(new ManualPoint(conn.EndBuoy.Coord, conn.EndBuoy.Depth, conn.EndBuoy.Type, conn.EndBuoy.Description));
                }

                record.TimeStart = timeStart;
                record.TimeEnd = timeEnd;

                record.RefreshDevices();
                record.ShowInTrace = false;
                record.ShowManualPoints = false;
                project.AddRecord(record);
            }*/
            
            #endregion

            List<SonarProfile> profiles = CreateProfiles(false);

            foreach (SonarProfile profile in profiles)
            {
                SonarRecord record = profile.CreateSnapshot();
                profile.Dispose();

                if (cbAddBoyAsPoint.Checked)
                {
                    record.InsertManualPoint(new ManualPoint(profile.Profile.StartBuoy.Coord, profile.Profile.StartBuoy.Depth, profile.Profile.StartBuoy.Type, profile.Profile.StartBuoy.Description), 0);
                    record.AddManualPoint(new ManualPoint(profile.Profile.EndBuoy.Coord, profile.Profile.EndBuoy.Depth, profile.Profile.EndBuoy.Type, profile.Profile.EndBuoy.Description));
                }

                record.RefreshDevices();
                record.ShowInTrace = false;
                record.ShowManualPoints = false;
                project.AddRecord(record);
            }

            Close();
        }
        
        private void OnDynamicRecords(object sender, EventArgs e)
        {
            List<SonarProfile> profiles = CreateProfiles(true);

            foreach (SonarProfile profile in profiles)
                project.AddRecord(profile);

            Close();
        }

        private void cbNormalize_CheckedChanged(object sender, EventArgs e)
        {
            tbSamplingRes.Enabled = cbNormSL.Checked;
        }
    }
}