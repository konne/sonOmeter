using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using UKLib.MathEx;
using UKLib.Survey.Math;
using UKLib.Xml;

namespace sonOmeter.Classes
{
    /// <summary>
    /// A wrapper class for blank line polygons including R/W operations.
    /// </summary>
    public class BlankLine
    {
        #region Properties
        /// <summary>
        /// Gets or sets the wrapped polygon object.
        /// </summary>
        /// <value>The poly.</value>
        [Browsable(false)]
        public PolygonD Poly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this blankline is shown in the trace.
        /// </summary>
        /// <value><c>true</c> if [show in trace]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        public bool ShowInTrace { get; set; }

        protected string name = "";
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { if (NodeRef != null) NodeRef.Text = value; name = value; }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the tree node reference.
        /// </summary>
        /// <value>The tree node reference.</value>
        [Browsable(false)]
        public TreeNode NodeRef { get; set; }
        #endregion

        #region Constructor
        public BlankLine()
        {
            Poly = new PolygonD();
            Color = Color.White;
            Name = "";
        }
        #endregion

        #region Read operations
        /// <summary>
        /// Reads from XML.
        /// </summary>
        /// <param name="reader">The reader object.</param>
        public void ReadFromXml(XmlTextReader reader)
        {
            Poly.Clear();

            Name = XmlReadConvert.Read(reader, "Name", "");
            ShowInTrace = XmlReadConvert.Read(reader, "ShowInTrace", true);
            string color = reader.GetAttribute("Color");
            if (color != null)
                Color = SerializeColor.Deserialize(color);

            reader.Read();

            while (!reader.EOF)
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "pointD":
                        Poly.Points.Add(new PointD(reader));
                        reader.Read();
                        break;

                    case "coordinate":
                        double la = Trigonometry.DMS2Rad(reader.GetAttribute("LA"), GSC.Settings.NFI);
                        double lo = Trigonometry.DMS2Rad(reader.GetAttribute("LO"), GSC.Settings.NFI);

                        Coordinate c = new Coordinate(la, lo, 0, CoordinateType.Elliptic);

                        c = GSC.Settings.InversTransform.Run(c, CoordinateType.TransverseMercator);

                        Poly.Points.Add(new PointD(c.RV, c.HV));
                        reader.Read();
                        break;

                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Reads from an XML file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void ReadFromFileXml(string fileName)
        {
            XmlTextReader reader = null;

            Poly.Clear();

            try
            {
                reader = new XmlTextReader(fileName);

                // Load the reader with the data file and ignore all white space nodes.         
                reader.WhitespaceHandling = WhitespaceHandling.None;

                reader.Read();

                while (!reader.EOF)
                {
                    if (reader.IsStartElement() && (reader.Name == "blankline"))
                        ReadFromXml(reader);
                    else
                        reader.Read();
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public void ReadFromFileSurfer(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            StreamReader reader = null;
            string line = "";
            string[] split;
            char[] sep = new char[1];
            double x = 0, y = 0;
            int max = 0;
            bool readPoints = false;

            sep[0] = ' ';

            Poly.Clear();

            try
            {
                reader = new StreamReader(fileName);

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line.Length == 0)
                        continue;

                    split = line.Split(sep);
                    double.TryParse(split[0], NumberStyles.Float, GSC.Settings.NFI, out x);
                    double.TryParse(split[1], NumberStyles.Float, GSC.Settings.NFI, out y);

                    if (!readPoints)
                    {
                        max = (int)x;
                        readPoints = true;
                    }
                    else if (max > 0)
                    {
                        Poly.Points.Add(new PointD(x, y));
                        max--;
                    }
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        #endregion

        #region Write operations
        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="elliptic">Toogles elliptic coordinates.</param>
        public void WriteToXml(XmlTextWriter writer, bool elliptic)
        {
            if (Poly.Points.Count > 0)
            {
                writer.WriteStartElement("blankline");
                writer.WriteAttributeString("Name", Name);
                writer.WriteAttributeString("ShowInTrace", ShowInTrace.ToString());
                writer.WriteAttributeString("Color", SerializeColor.Serialize(Color));
                foreach (PointD pt in Poly.Points)
                    if (elliptic)
                    {
                        Coordinate c = new Coordinate(pt.X, pt.Y, 0, CoordinateType.TransverseMercator);
                        c = GSC.Settings.ForwardTransform.Run(c, CoordinateType.Elliptic);

                        writer.WriteStartElement("coordinate");
                        writer.WriteAttributeString("LA", Trigonometry.Rad2DMS(c.LA, GSC.Settings.NFI, true));
                        writer.WriteAttributeString("LO", Trigonometry.Rad2DMS(c.LO, GSC.Settings.NFI, true));
                        writer.WriteEndElement();
                    }
                    else
                        pt.ToXml(writer, GSC.Settings.NFI);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes the blank line to a file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="elliptic">Toogles elliptic coordinates.</param>
        public void WriteToFileXml(string fileName, bool elliptic)
        {
            // Create writer.
            XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument(true);

            // Write blank line.
            WriteToXml(writer, elliptic);

            // Close the stream.
            writer.Close();
        }

        /// <summary>
        /// Writes the blank line to surfer format.
        /// </summary>
        /// <param name="blankLine">The blank line.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="elliptic">if set to <c>true</c> [elliptic].</param>
        public void WriteToFileSurfer(string fileName, bool elliptic)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US").NumberFormat;

            PointD pt;
            int i, max = Poly.Points.Count;

            if (max == 0)
                return;

            StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.ASCII);

            writer.WriteLine((max + 1).ToString() + ", 0");

            for (i = 0; i <= max; i++)
            {
                if (i == max)
                    pt = Poly.Points[0];
                else
                    pt = Poly.Points[i];
                if (elliptic)
                {
                    Coordinate c = new Coordinate(pt.X, pt.Y, 0, CoordinateType.TransverseMercator);
                    c = GSC.Settings.ForwardTransform.Run(c, CoordinateType.Elliptic);

                    nfi.NumberDecimalDigits = 3;
                    double LA = c.LA * 180 / System.Math.PI;
                    double LO = c.LO * 180 / System.Math.PI;

                    string s = "";
                    s += Trigonometry.Rad2Grad(c.LO).ToString("0.000000", CultureInfo.InvariantCulture);
                    s += " ";
                    s += Trigonometry.Rad2Grad(c.LA).ToString("0.000000", CultureInfo.InvariantCulture);

                    writer.WriteLine(s);
                }
                else
                    writer.WriteLine(pt.X.ToString(nfi) + ", " + pt.Y.ToString(nfi));
            }

            writer.Close();
        }
        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }
}
