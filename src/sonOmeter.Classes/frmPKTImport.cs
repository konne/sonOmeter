using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Math;
using System.Globalization;
using System.IO;
using sonOmeter.Classes.Sonar2D;
using UKLib.MathEx;

namespace sonOmeter.Classes
{
    #region PKT definition
    public struct PKTdata
    {
        public int ID;
        public int Type;
        public Coordinate Coord;

        public PKTdata(string line)
        {
            string[] parts = line.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 5)
                throw new ArgumentException("The line cannot be parsed to PKT data.");

            ID = 0;
            Type = 0;
            double rv = 0.0;
            double hv = 0.0;
            double al = 0.0;

            int.TryParse(parts[0], out ID);
            int.TryParse(parts[1], out Type);
            double.TryParse(parts[2], NumberStyles.Float, GSC.Settings.NFI, out rv);
            double.TryParse(parts[3], NumberStyles.Float, GSC.Settings.NFI, out hv);
            double.TryParse(parts[4], NumberStyles.Float, GSC.Settings.NFI, out al);

            Coord = new Coordinate(rv, hv, al, CoordinateType.TransverseMercator);
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        /// <summary>
        /// Reads all PKT items from a file and computes the bounding box.
        /// </summary>
        /// <param name="file">The input file.</param>
        /// <param name="rcBounds">The bounding box rectangle.</param>
        /// <returns></returns>
        static public List<PKTdata> ReadListFromFile(string file, out RectangleD rcBounds)
        {
            List<PKTdata> list = new List<PKTdata>();
            StreamReader reader = new StreamReader(file);
            rcBounds = null;

            while (!reader.EndOfStream)
            {
                try
                {
                    PKTdata item = new PKTdata(reader.ReadLine());
                    list.Add(item);
                    if (rcBounds == null)
                        rcBounds = new RectangleD(item.Coord.Point);
                    else
                        rcBounds.Merge(item.Coord.Point);
                }
                catch (Exception e)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(typeof(PKTdata), UKLib.Debug.DebugLevel.Red, "SonarRecord.BuildDeviceTree: " + e.Message);
                }
            }

            reader.Close();
            return list;
        }
    }
    #endregion

    public partial class frmPKTImport : Form
    {
        public BuoyConnection Connection { get; set; }
        public List<PKTdata> List { get; set; }
        private string file;
        private RectangleD rcBounds = new RectangleD();

        public frmPKTImport(string file)
        {
            InitializeComponent();

            this.file = file;
            List = PKTdata.ReadListFromFile(file, out rcBounds);

            lbPKT.DataSource = List;
            panMap.Invalidate();
            btnProfileStart.Enabled = btnProfileEnd.Enabled = (List.Count > 1);
        }

        private void btnProfileStart_Click(object sender, EventArgs e)
        {
            if (lbPKT.SelectedIndex != -1)
            {
                if (lbPKT.SelectedItem == tbProfileEnd.Tag)
                {
                    tbProfileEnd.Tag = null;
                    tbProfileEnd.Text = "";
                }

                tbProfileStart.Tag = lbPKT.SelectedItem;
                tbProfileStart.Text = lbPKT.SelectedItem.ToString();

                UpdateProfile();
            }
        }

        private void btnProfileEnd_Click(object sender, EventArgs e)
        {
            if (lbPKT.SelectedIndex != -1)
            {
                if (lbPKT.SelectedItem == tbProfileStart.Tag)
                {
                    tbProfileStart.Tag = null;
                    tbProfileStart.Text = "";
                }

                tbProfileEnd.Tag = lbPKT.SelectedItem;
                tbProfileEnd.Text = lbPKT.SelectedItem.ToString();

                UpdateProfile();
            }
        }

        private void UpdateProfile()
        {
            if ((tbProfileStart.Tag == null) || (tbProfileEnd.Tag == null))
                return;

            if (pgProfile.SelectedObject == null)
            {
                Connection = new BuoyConnection() { Corridor = 1.0, Description = Path.GetFileNameWithoutExtension(file) };
                pgProfile.SelectedObject = Connection;
            }

            PKTdata start = (PKTdata)tbProfileStart.Tag;
            PKTdata end = (PKTdata)tbProfileEnd.Tag;

            Connection.StartBuoy = new Buoy() { Coord = start.Coord, ID = start.ID, Type = start.Type };
            Connection.EndBuoy = new Buoy() { Coord = end.Coord, ID = end.ID, Type = end.Type };
        }

        private void panMap_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brushSel = new SolidBrush(GSC.Settings.CS.ForeColor);
            SolidBrush brush = new SolidBrush(GSC.Settings.CS.WorkLineColor);
            Graphics g = e.Graphics;
            PointD ptCenter = rcBounds.Center;
            PointF ptSel = PointF.Empty;
            float size = (float)(Math.Max(rcBounds.Width, rcBounds.Height) * 1.1);
            float scale = (float)panMap.Width / size;
            float ptSize = 4.0F / scale;
            float ptSizeH = ptSize / 2.0F;
            bool sel;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(GSC.Settings.CS.BackColor);
            g.ScaleTransform(scale, -scale);
            g.TranslateTransform(size / 2.0F, -size / 2.0F);

            foreach (PKTdata item in List)
            {
                sel = (lbPKT.SelectedIndex != -1) && (item.ID == ((PKTdata)lbPKT.SelectedItem).ID);
                PointF pt = (item.Coord.Point - ptCenter).PointF;
                if (sel)
                    ptSel = pt;
                else
                    g.FillEllipse(brush, pt.X - ptSizeH, pt.Y - ptSizeH, ptSize, ptSize);
            }

            g.FillEllipse(brushSel, ptSel.X - ptSizeH, ptSel.Y - ptSizeH, ptSize, ptSize);
        }

        private void lbPKT_SelectedIndexChanged(object sender, EventArgs e)
        {
            panMap.Invalidate();
        }
    }
}
