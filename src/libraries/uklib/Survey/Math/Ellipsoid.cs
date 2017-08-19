using System;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using UKLib.Xml;
using System.Collections.ObjectModel;

namespace UKLib.Survey.Math
{
    /// <summary>
    /// Summary description for Ellipsoid.
    /// </summary>
    public class Ellipsoid
    {
        #region Static Entries
        static public Collection<Ellipsoid> KnownList = new Collection<Ellipsoid>() {Bessel, WGS84 };

        public static Ellipsoid Bessel
        {
            get
            {                               
                return new Ellipsoid() {Name= "Bessel", A = 6377397.155, B = 6356078.963};
            }
        }

        public static Ellipsoid WGS84
        {
            get
            {                               
                return new Ellipsoid() {Name= "WGS84", A = 6378137, B = 6356752.3142451793};
            }
        }
        #endregion

        public Ellipsoid()
        {
           
        }

        public Ellipsoid(Ellipsoid e)
        {
            this.a = e.a;
            this.b = e.b;
            this.f = e.f;
            this.e = e.e;
            this.e_sqr = e.e_sqr;
            this.n = e.n;
            this.a0 = e.a0;
            this.a2 = e.a2;
            this.a4 = e.a4;
            this.a6 = e.a6;
            this.a8 = e.a8;
            this.name = e.name;
        }

        #region Variables
        protected double a = 0;
        protected double b = 0;
        protected double f = 0;

        protected double e = 0;
        protected double e_sqr = 0;
        protected double n = 0;
        protected double a0 = 0;
        protected double a2 = 0;
        protected double a4 = 0;
        protected double a6 = 0;
        protected double a8 = 0;

        protected string name = "Ellipsoid";
        #endregion

        #region Properties
        [XmlAttribute]
        [Description("Name of the Ellipsoid. Used for identification in XML files."), Category("General")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlAttribute]
        [Description("Defines the semimajor axis of the Ellipsoid."), Category("Geometry"), DefaultValue(0)]
        public double A
        {
            get { return a; }
            set
            {
                a = value;

                if (a == 0)
                    return;

                if (b != 0)
                    f = (a - b) / a;
                else if (f != 0)
                    b = a * (1 - f);

                UpdateParams();
            }
        }

        [XmlAttribute]
        [Description("Defines the semiminor axis of the Ellipsoid."), Category("Geometry"), DefaultValue(0)]
        public double B
        {
            get { return b; }
            set
            {
                b = value;

                if (b == 0)
                    return;

                if (a != 0)
                    f = (a - b) / a;

                UpdateParams();
            }
        }

        [XmlIgnore]
        [Description("Defines the oblateness of the Ellipsoid."), Category("Geometry"), DefaultValue(0)]
        public double F
        {
            get { return f; }
            set
            {
                f = value;

                if (f == 0)
                    return;

                if (a != 0)
                    b = a * (1 - f);

                UpdateParams();
            }
        }

        [XmlIgnore]
        [Description("Defines the excentricity of the Ellipsoid."), Category("Parameters")]
        public double E
        {
            get { return e; }
        }

        [XmlIgnore]
        [Description("Defines the squared excentricity of the Ellipsoid."), Category("Parameters")]
        public double E_sqr
        {
            get { return e_sqr; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double N
        {
            get { return n; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double A0
        {
            get { return a0; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double A2
        {
            get { return a2; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double A4
        {
            get { return a4; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double A6
        {
            get { return a6; }
        }

        [XmlIgnore]
        [Description("Important parameter for further calculations."), Category("Parameters")]
        public double A8
        {
            get { return a8; }
        }

        #endregion

        protected void UpdateParams()
        {
            try
            {
                e_sqr = 2 * f - f * f;
                e = System.Math.Sqrt(e_sqr);

                if (f == 0)
                    return;

                n = f / (2 - f);
                double n2 = n * n;
                double n3 = n * n * n;
                double n4 = n * n * n * n;

                a0 = 1 + n2 / 4 + n4 / 64;
                a2 = (3 * (n - n3 / 8)) / 2;
                a4 = (15 * (n2 - n4 / 4)) / 16;
                a6 = (35 * n3) / 48;
                a8 = (315 * n4) / 512;
            }
            catch
            {
            }
        }

        protected double Pow(double x, double y)
        {
            return System.Math.Pow(x, y);
        }

        public override string ToString()
        {
            return name;
        }

        #region XML r/w
        /// <summary>
        /// Writes the object to an XML stream.
        /// </summary>
        /// <param name="writer">XML writer object</param>
        /// <param name="nfi">The number format info of the application.</param>
        public void ToXml(XmlTextWriter writer, NumberFormatInfo nfi)
        {
            try
            {
                writer.WriteStartElement("ellipsoid");
                writer.WriteAttributeString("name", name);
                writer.WriteAttributeString("a", a.ToString(nfi));
                writer.WriteAttributeString("b", b.ToString(nfi));
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, e.Message);
            }
        }

        /// <summary>
        /// Reads the object from an XML stream.
        /// </summary>
        /// <param name="reader">XML reader object.</param>
        public void FromXml(XmlTextReader reader)
        {
            try
            {
                name = XmlReadConvert.Read(reader, "name", "Ellipsoid");
                a = XmlReadConvert.Read(reader, "a", 0.0);
                B = XmlReadConvert.Read(reader, "b", 0.0);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, e.Message);
            }
        }
        #endregion

        #region Static list
      
        /// <summary>
        /// Writes the static list to an XML stream.
        /// </summary>
        /// <param name="writer">XML writer object</param>
        /// <param name="nfi">The number format info of the application.</param>
        static public void KnownToXml(XmlTextWriter writer, NumberFormatInfo nfi)
        {
            writer.WriteStartElement("ellipsoidlist");

            foreach (Ellipsoid e in KnownList)
            {
                e.ToXml(writer, nfi);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Reads the static list from an XML stream.
        /// </summary>
        /// <param name="reader">XML reader object.</param>
        static public void KnownFromXml(XmlTextReader reader)
        {
            try
            {
                while (!reader.EOF)
                {
                    if ((reader.Name == "ellipsoidlist") && (reader.NodeType == XmlNodeType.EndElement))
                        break;

                    if ((reader.Name == "ellipsoid") && reader.IsStartElement())
                    {
                        Ellipsoid e = new Ellipsoid();
                        Ellipsoid.KnownList.Add(e);
                        e.FromXml(reader);
                    }

                    reader.Read();
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(null, UKLib.Debug.DebugLevel.Red, e.Message);
            }
        }
        #endregion
    }
}
