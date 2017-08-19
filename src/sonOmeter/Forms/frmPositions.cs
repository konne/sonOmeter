using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UKLib.Survey;
using UKLib.Survey.Parse;
using System.Globalization;
using sonOmeter.Classes;
using System.IO;
using System.Collections.Generic;
using UKLib.Hex;
using System.Data;
using System.Reflection;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmPositions.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmPositions : DockDotNET.DockWindow
    {      
        #region Variables
        private SonarProject project = null;
        private SonarRecord record = null;
        private SonarDevice device = null;

        private SonarLine workLine = null;

        DataTable posTable = null;

        BindingSource bsPosData = null;
        #endregion

        #region Create & Dispose
        public frmPositions(SonarProject prj, SonarRecord rec)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            #region Init DataTable
            posTable = new DataTable("Positions");
            posTable.Columns.Add("ID", typeof(int));
            posTable.Columns.Add("P", typeof(bool));
            posTable.Columns.Add("D", typeof(bool));
            posTable.Columns.Add("U", typeof(bool));
            posTable.Columns.Add("Type", typeof(PosType));
            posTable.Columns.Add("Time", typeof(string));
            posTable.Columns.Add("D1", typeof(string));
            posTable.Columns.Add("D2", typeof(string));
            posTable.Columns.Add("D3", typeof(string));
            posTable.Columns.Add("SonarPos", typeof(SonarPos));
            posTable.Columns.Add("Sonarline", typeof(SonarLine));
            posTable.RowChanged += new DataRowChangeEventHandler(posTable_RowChanged);

            bsPosData = new BindingSource();
            bsPosData.DataSource = posTable;
            #endregion

            #region Improve DataGridView
            Type type = this.dataGridView1.GetType();
            PropertyInfo f1 = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            f1.SetValue(this.dataGridView1, true, null);
            #endregion

            try
            {
                if ((prj == null) || (rec == null))
                    return;

                project = prj;
                record = rec;

                if (record is SonarProfile)
                    this.Text = "[" + record.Description + "]";
                else
                    this.Text = "[" + (project.IndexOf(record) + 1).ToString() + "]";

                this.Text += " - Position";

                foreach (SonarDevice dev in record.Devices)
                {
                    if (dev.SonID == 0)
                    {
                        device = dev;
                        break;
                    }
                }

                var filterlist = new List<GlobalNotifier.MsgTypes>();
                filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
                filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);

                GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalNotifier), filterlist);

                RebuildPosList();
            }
            catch
            {
            }

        }

        #endregion

        #region List Management
      



        public void RebuildPosList()
        {
            dataGridView1.DataSource = null;

            if (device != null)
            {
                Application.DoEvents();

                for (int lID = 0; lID < device.SonarLines.Count; lID++)
                {
                    SonarLine line = device.SonarLines[lID];

                    if (lID % 50 == 0)
                        Application.DoEvents();

                    AddSonarLine(line);
                }
            }

            dataGridView1.DataSource = bsPosData;

            dataGridView1.Columns["ID"].Width = 50;
            dataGridView1.Columns["ID"].ReadOnly = true;
            dataGridView1.Columns["ID"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["P"].Width = 30;
            dataGridView1.Columns["P"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["P"].ToolTipText = "Real Position";
            dataGridView1.Columns["D"].Width = 30;
            dataGridView1.Columns["D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["D"].ToolTipText = "Disabled";
            dataGridView1.Columns["U"].Width = 30;
            dataGridView1.Columns["U"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["U"].ReadOnly = true;
            dataGridView1.Columns["U"].ToolTipText = "Used";
            dataGridView1.Columns["Type"].Width = 80;
            dataGridView1.Columns["Type"].ReadOnly = true;
            dataGridView1.Columns["Time"].Width = 100;
            dataGridView1.Columns["Time"].ReadOnly = true;
            dataGridView1.Columns["Time"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["D1"].Width = 100;
            dataGridView1.Columns["D1"].ReadOnly = true;
            dataGridView1.Columns["D1"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["D2"].Width = 100;
            dataGridView1.Columns["D2"].ReadOnly = true;
            dataGridView1.Columns["D2"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["D3"].Width = 100;
            dataGridView1.Columns["D3"].ReadOnly = true;
            dataGridView1.Columns["D3"].CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns["SonarPos"].Width = 400;
            dataGridView1.Columns["SonarPos"].ReadOnly = true;
            dataGridView1.Columns["SonarLine"].Visible = false;

        }

        void posTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SonarPos pos = (e.Row["SonarPos"] as SonarPos);
            pos.Disabled = (bool)e.Row["D"];
        }

        private void AddSonarLine(SonarLine line)
        {
            for (int pID = 0; pID < line.PosList.Count; pID++)
            {
                SonarPos pos = line.PosList[pID] as SonarPos;

                string d1 = "-";
                string d2 = "-";
                string d3 = "-";

                try
                {
                    switch (pos.type)
                    {
                        case PosType.Compass:
                            var cmp = new Compass(pos.strPos) - GSC.Settings.CompassZero;
                            d1 = cmp.Yaw.ToString("0.0");
                            d2 = cmp.Pitch.ToString("0.0");
                            d3 = cmp.Roll.ToString("0.0");
                            break;
                        case PosType.HMR3300:
                            var cmp2 = new HMR3300(pos.strPos) - GSC.Settings.CompassZero;
                            d1 = cmp2.Yaw.ToString("0.0");
                            d2 = cmp2.Pitch.ToString("0.0");
                            d3 = cmp2.Roll.ToString("0.0");
                            break;
                        case PosType.SoSoZeiss:
                            SoSoZeiss soz = new SoSoZeiss(pos.strPos);
                            d1 = soz.Azimut.ToString();
                            d2 = soz.Elevation.ToString();
                            d3 = soz.Distance.ToString();
                            break;
                        case PosType.Geodimeter:
                            Geodimeter geo = new Geodimeter(pos.strPos);
                            d1 = geo.HighValue.ToString("0.000");
                            d2 = geo.RightValue.ToString("0.000");
                            d3 = geo.Altitude.ToString("0.00");
                            break;
                        case PosType.LeicaTPS:
                            LeicaTPS tps = new LeicaTPS(pos.strPos);
                            d1 = tps.HighValue.ToString("0.000");
                            d2 = tps.RightValue.ToString("0.000");
                            d3 = tps.Altitude.ToString("0.00");
                            break;
                        case PosType.NMEA:
                            {
                                NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);

                                string nmeaType = nmea.Type;
                                if (nmeaType.StartsWith("$GN"))
                                    nmeaType = nmeaType.Replace("$GN", "$GP");
                                switch (nmeaType)
                                {
                                    case "$GPGGA":
                                        d1 = nmea.Latitude.ToString();
                                        d2 = nmea.Longitude.ToString();
                                        d3 = (GSC.Settings.UseWGSAltitude ? nmea.AltitudeWGS84 : nmea.Altitude).ToString("0.00");
                                        break;
                                    case "$GPLLK":
                                        d1 = nmea.HighValue.ToString("0.000");
                                        d2 = nmea.RightValue.ToString("0.000");
                                        d3 = nmea.Altitude.ToString("0.00");
                                        break;
                                    case "$GPLLQ":
                                        d1 = nmea.HighValue.ToString("0.000");
                                        d2 = nmea.RightValue.ToString("0.000");
                                        d3 = nmea.Altitude.ToString("0.00");
                                        break;
                                    case "$PTNL":
                                        d1 = nmea.HighValue.ToString("0.000");
                                        d2 = nmea.RightValue.ToString("0.000");
                                        d3 = nmea.Altitude.ToString("0.00");
                                        break;
                                    case "$SBG01":
                                        d1 = nmea.Yaw.ToString("0.000");
                                        d2 = nmea.Pitch.ToString("0.000");
                                        d3 = nmea.Roll.ToString("0.000");
                                        break;
                                }
                                break;
                            }
                        case PosType.RD8000:
                            RD8000Message msg = new RD8000Message(HexStrings.ToByteArray(pos.strPos));
                            d1 = msg.Depth.ToString("0.00");
                            d2 = msg.Phase.ToString("0.0");
                            d3 = msg.Mode.ToString();
                            break;
                        case PosType.Fixed:
                            FixedRVHV fixd = new FixedRVHV(pos.strPos);
                            d1 = fixd.HighValue.ToString("0.000");
                            d2 = fixd.RightValue.ToString("0.000");
                            d3 = fixd.Altitude.ToString("0.00");
                            break;
                        case PosType.DLSB30:
                            DLSB30 antdepth = new DLSB30(pos.strPos);
                            d1 = antdepth.Distance.ToString("0.00");
                            break;
                        case PosType.Depth:
                            Nullable<float> f = SosoDepthGauge.ToFloat(pos.strPos);
                            if (f.HasValue)
                                d1 = f.Value.ToString("0.0") + " m";
                            else
                                d1 = "not valid";
                            break;
                    }
                }
                catch
                {
                }

                int cnt = posTable.Rows.Count + 1;
                posTable.Rows.Add(new object[] { cnt,pos.RealPos, pos.Disabled, pos.Used, pos.type, pos.time.ToLongTimeString(), d1, d2, d3, pos, line });
            }
        }

        #endregion

        #region GlobalNotifier
        private void OnWorkLineChanged(object sender, object args)
        {
            try
            {
                RecordEventArgs e = args as RecordEventArgs;

                SonarRecord workLineRec = e.Rec;
                SonarDevice workLineDev = e.Dev;

                if (workLineDev == null)
                    return;

                if ((workLineDev.SonID == 0) && (workLineRec == this.record))
                {
                    workLine = e.Tag as SonarLine;


                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        SonarLine line = (row.DataBoundItem as System.Data.DataRowView).Row.ItemArray[10] as SonarLine;
                        if (workLine == line)
                        {
                            if (row.DefaultCellStyle.BackColor != Color.Red)
                                row.DefaultCellStyle.BackColor = Color.Red;
                            if (!row.Displayed)
                                dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                        else
                        {
                            if (row.DefaultCellStyle.BackColor != Color.White)
                                row.DefaultCellStyle.BackColor = Color.White;
                        }

                    }

                }
                else
                {
                    if (workLine != null)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.DefaultCellStyle.BackColor != Color.White)
                                row.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                    workLine = null;
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Green, "frmPosition.OnWorkLineChanged(): " + ex.Message);
            }
        }

        private void OnGlobalNotifier(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    if (project.Tracking)
                        return;
                    if (device != sender as SonarDevice)
                        return;
                    AddSonarLine(args as SonarLine);                    
                    break;
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    OnWorkLineChanged(sender, args);
                    break;                
            }
        }
        #endregion
    
        private void cbVisibleCompass_CheckedChanged(object sender, EventArgs e)
        {
            ChangeFilterlist();
        }

        private void ChangeFilterlist()
        {
            string filter = "(1 = 1) ";
            if (!cbGeodimeter.Checked) filter += "AND (Type <> 5)";
            if (!cbLeicaTPS.Checked) filter += "AND (Type <> 7)";
            if (!cbVisibleNMEA.Checked) filter += "AND (Type <> 4)";
            if (!cbVisibleCompass.Checked) filter += "AND (Type <> 32)";
            if (!cbVisibleDisabled.Checked) filter += "AND (D <> true)";
            if (!cbVisibleUnused.Checked) filter += "AND (U <> false)";         

            bsPosData.Filter = filter;
        }

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in posTable.Rows)
            {
                row.BeginEdit();
                row[1] = false;
                row.AcceptChanges();
                //  (row.ItemArray[7] as SonarPos).Disabled = false;
            }           
        }

        private void saveAsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                TextWriter tw = new StreamWriter(dlgSave.FileName);
                tw.Write("Type;");
                tw.Write("Time;");
                tw.Write("D1;");
                tw.Write("D2;");
                tw.Write("D3;");
                tw.WriteLine("SonarPos");

                foreach (DataRow row in posTable.Rows)
                {
                    tw.Write(((PosType)row["Type"]).ToString() + ";");
                    tw.Write(row["Time"] + ";");
                    tw.Write(row["D1"] + ";");
                    tw.Write(row["D2"] + ";");
                    tw.Write(row["D3"] + ";");
                    tw.WriteLine((row["SonarPos"] as SonarPos).ToString());
                }                
                tw.Close();
            }
        }

    }
}
