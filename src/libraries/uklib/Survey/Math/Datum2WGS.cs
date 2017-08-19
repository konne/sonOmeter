using System;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using UKLib.Xml;
using System.Collections.ObjectModel;

namespace UKLib.Survey.Math
{
    /// <summary>
    /// Summary description for Datum2WGS.
    /// </summary>
    public class Datum2WGS
    {
        #region Static Items
        static public Collection<Datum2WGS> KnownList = new Collection<Datum2WGS>() { DHDN, WGS84 };

        public static Datum2WGS WGS84
        {
            get
            {
                Datum2WGS datum = new Datum2WGS();
                datum.DX = 0;
                datum.DY = 0;
                datum.DZ = 0;
                datum.RX = 0;
                datum.RY = 0;
                datum.RZ = 0;
                datum.Sc = 1;
                datum.Name = "WGS84";
                return datum;
            }
        }
        
        public static Datum2WGS DHDN
        {
            get
            {
                Datum2WGS datum = new Datum2WGS();
                datum.DX = 583;
                datum.DY = 68.2;
                datum.DZ = 399.1;
                datum.RX = 0;
                datum.RY = 0;
                datum.RZ = 1.2343356321048786E-05;
                datum.Sc = 1.0000105201106715;
                datum.Name = "DHDN";
                return datum;
            }
        }
        #endregion

        public Datum2WGS()
        {           
        }

        public Datum2WGS(Datum2WGS dat)
        {
            this.dX = dat.dX;
            this.dY = dat.dY;
            this.dZ = dat.dZ;
            this.rX = dat.rX;
            this.rY = dat.rY;
            this.rZ = dat.rZ;
            this.sc = dat.sc;
            this.name = dat.name;
        }

        public override string ToString()
        {
            return name;
        }

        #region Variables
        protected double dX = 0;
        protected double dY = 0;
        protected double dZ = 0;
        protected double rX = 0;
        protected double rY = 0;
        protected double rZ = 0;
        protected double sc = 1;

        protected string name = "Datum2WGS";
        #endregion

        #region Properties
        [XmlAttribute]
        [Description("Translation factor X."), Category("Translation"), DefaultValue(0)]
        public double DX
        {
            get { return dX; }
            set { dX = value; }
        }

        [XmlAttribute]
        [Description("Translation factor Y."), Category("Translation"), DefaultValue(0)]
        public double DY
        {
            get { return dY; }
            set { dY = value; }
        }

        [XmlAttribute]
        [Description("Translation factor Z."), Category("Translation"), DefaultValue(0)]
        public double DZ
        {
            get { return dZ; }
            set { dZ = value; }
        }

        [XmlAttribute]
        [Description("Rotation factor X."), Category("Rotation"), DefaultValue(0)]
        public double RX
        {
            get { return rX; }
            set { rX = value; }
        }

        [XmlAttribute]
        [Description("Rotation factor Y."), Category("Rotation"), DefaultValue(0)]
        public double RY
        {
            get { return rY; }
            set { rY = value; }
        }

        [XmlAttribute]
        [Description("Rotation factor Z."), Category("Rotation"), DefaultValue(0)]
        public double RZ
        {
            get { return rZ; }
            set { rZ = value; }
        }

        [XmlAttribute]
        [Description("Scale factor."), Category("Scale factor"), DefaultValue(1)]
        public double Sc
        {
            get { return sc; }
            set { sc = value; }
        }

        [XmlAttribute]
        [Description("Name of the source of the datum transformation to WGS84. Used for identification in XML files."), Category("General")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

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
                writer.WriteStartElement("datum");
                writer.WriteAttributeString("name", name);
                writer.WriteAttributeString("dx", dX.ToString(nfi));
                writer.WriteAttributeString("dy", dY.ToString(nfi));
                writer.WriteAttributeString("dz", dZ.ToString(nfi));
                writer.WriteAttributeString("rx", rX.ToString(nfi));
                writer.WriteAttributeString("ry", rY.ToString(nfi));
                writer.WriteAttributeString("rz", rZ.ToString(nfi));
                writer.WriteAttributeString("sc", sc.ToString(nfi));
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
        /// <param name="nfi">The number format info of the application.</param>
        public void FromXml(XmlTextReader reader)
        {
            try
            {
                name = XmlReadConvert.Read(reader, "name", "Datum2WGS");
                dX = XmlReadConvert.Read(reader, "dx", 0.0);
                dY = XmlReadConvert.Read(reader, "dy", 0.0);
                dZ = XmlReadConvert.Read(reader, "dz", 0.0);
                rX = XmlReadConvert.Read(reader, "rx", 0.0);
                rY = XmlReadConvert.Read(reader, "ry", 0.0);
                rZ = XmlReadConvert.Read(reader, "rz", 0.0);
                sc = XmlReadConvert.Read(reader, "sc", 1.0);
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
            writer.WriteStartElement("datumlist");

            foreach (Datum2WGS d in KnownList)
            {
                d.ToXml(writer, nfi);
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
                    if ((reader.Name == "datumlist") && (reader.NodeType == XmlNodeType.EndElement))
                        break;

                    if ((reader.Name == "datum") && reader.IsStartElement())
                    {
                        Datum2WGS d = new Datum2WGS();
                        Datum2WGS.KnownList.Add(d);
                        d.FromXml(reader);
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
