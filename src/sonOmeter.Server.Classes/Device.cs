using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace sonOmeter.Server.Classes
{
    #region Format Enums
    public enum SonarDeviceFormat
    {
        /// <summary>
        /// V1 altes Sonar 5,6 (7) ohne Prog
        /// </summary>
        SonarV1,
        /// <summary>
        /// V2 altes 7 Farb prog aber ohne GPS
        /// </summary>
        SonarV2,
        /// <summary>
        ///  V3 altes 7 fabr prog mit GPS
        /// </summary>
        SonarV3, // 
        /// <summary>
        /// V4 sonar von Mürger nicht programmierfähig
        /// </summary>
        SonarV4, // 
        /// <summary>
        /// V5 neue Version ab 2006
        /// </summary>
        SonarV5, // 
    }

    public enum PositionDeviceFormat
    {
        NMEA,
        Compass,
        Geodimeter,
        LeicaTPS,
        RD8000,
        RD4000,
        HMR3300,
        DLSB30
    }
    #endregion

    #region DataDevice
    public abstract class DataDevice
    {
        #region Variables
        protected string desc = "No desc";
        protected string port = "COM1";
        protected int parameter = 9600;
        #endregion

        #region Properties
        [XmlAttribute]
        [Category("Datasource"), DisplayName("COM / IP")]
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        [XmlAttribute]
        [Category("Datasource"), DisplayName("Paramter")]
        public int Parameter
        {
            get { return (parameter); }
            set { parameter = value; }
        }

        [XmlAttribute]
        [Category("Datasource")]
        public string Description
        {
            get { return desc; }
            set { desc = value; }
        }
        #endregion

        public override string ToString()
        {
            return Port + "@" + Parameter;
        }
    }
    #endregion

    #region SonarDevice
    public class SonarDevice : DataDevice, ICloneable
    {
        #region Constructor
        public SonarDevice()
        {
            Format = SonarDeviceFormat.SonarV3;
        }
        #endregion

        #region Properties
        int id = -1;
        [Browsable(false), XmlIgnore]
        public int ID
        {
            get { return id; }
            internal set { id = value; }
        }

        #region General
        /// <summary>
        /// Gets or sets the device format.
        /// </summary>
        [XmlAttribute]
        [Category("General"), DefaultValue(SonarDeviceFormat.SonarV3)]
        public SonarDeviceFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the device displacement of the x-coordinate.
        /// </summary>
        [XmlAttribute]
        [Category("General"), DefaultValue(0.0)]
        public double DX { get; set; }

        /// <summary>
        /// Gets or sets the device displacement of the y-coordinate.
        /// </summary>
        [XmlAttribute]
        [Category("General"), DefaultValue(0.0)]
        public double DY { get; set; }

        /// <summary>
        /// Gets or sets the device displacement of the z-coordinate.
        /// </summary>
        [XmlAttribute]
        [Category("General"), DefaultValue(0.0)]
        public double DZ { get; set; }

        [XmlAttribute]
        [Category("General"), DefaultValue(0.0)]
        public double DP { get; set; }

        [XmlAttribute]
        [Category("General"), DefaultValue(0.0)]
        public double DR { get; set; }

        #endregion
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            SonarDevice sd = new SonarDevice();
            sd.Description = Description;
            sd.Port = Port;
            sd.Parameter = Parameter;

            sd.id = ID;

            sd.DP = DP;
            sd.DR = DR;
            sd.DX = DX;
            sd.DY = DY;
            sd.DZ = DZ;

            sd.Format = Format;

            return sd;
        }

        #endregion
    }
    #endregion

    #region SonarDeviceCollection
    public class SonarDeviceCollection : Collection<SonarDevice>
    {
        private void RefreshIDs()
        {
            int idx = 0;
            foreach (SonarDevice item in this.Items)
            {
                item.ID = idx++;
            }
        }

        protected override void InsertItem(int index, SonarDevice item)
        {
            base.InsertItem(index, item);
            RefreshIDs();
        }

        protected override void SetItem(int index, SonarDevice item)
        {
            base.SetItem(index, item);
            RefreshIDs();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            RefreshIDs();
        }
    }
    #endregion

    #region PositionDevice
    public class PositionDevice : DataDevice, ICloneable
    {
        #region General
        /// <summary>
        /// Gets or sets the device format.
        /// </summary>
        [XmlAttribute]
        [Category("General"), DefaultValue(PositionDeviceFormat.NMEA)]
        public PositionDeviceFormat Format { get; set; }

        #endregion

        #region Constructor
        public PositionDevice()
        {
            Format = PositionDeviceFormat.NMEA;
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            PositionDevice pd = new PositionDevice();
            pd.Description = Description;
            pd.Port = Port;
            pd.Parameter = Parameter;

            pd.Format = Format;

            return pd;
        }

        #endregion
    }
    #endregion

    #region PositionDeviceCollection
    public class PositionDeviceCollection : Collection<PositionDevice>
    {
    }
    #endregion

    #region SonarDeviceConfig
    public struct SonarDeviceConfig
    {
        #region Enum Depths
        public enum SonarDepths
        {
            Depth50 = 50,
            Depth100 = 100
        }
        #endregion

        #region Variables
        private SonarDepths depth;
        private bool hf;
        private bool nf;
        private double sonicSpeed;
        private string gpsMask;

        string lisencedFor;
        string lisencedNr;
        #endregion

        #region SonarConfig
        [ReadOnly(true)]
        [Category("Sonar Config"), DisplayName("Licensed For")]
        public string LicencedFor
        {
            get { return (lisencedFor); }
            set { lisencedFor = value; }
        }

        [ReadOnly(true)]
        [Category("Sonar Config"), DisplayName("Licence Number")]
        public string LicencedNr
        {
            get { return (lisencedNr); }
            set { lisencedNr = value; }
        }

        /// <summary>
        /// Gets or sets the device depth.
        /// </summary>
        [Category("Sonar Config"), DefaultValue(SonarDepths.Depth100)]
        public SonarDepths Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        /// <summary>
        /// Gets or sets the HF sonar flag.
        /// </summary>
        [Category("Sonar Config"), DisplayName("Frequency HF"), DefaultValue(true)]
        public bool HF
        {
            get { return hf; }
            set
            {
                hf = value;
                if (!hf & !nf) nf = true;
            }
        }

        /// <summary>
        /// Gets or sets the NF sonar flag.
        /// </summary>
        [Category("Sonar Config"), DisplayName("Frequency NF"), DefaultValue(true)]
        public bool NF
        {
            get { return nf; }
            set
            {
                nf = value;
                if (!hf & !nf) hf = true;
            }
        }

        /// <summary>
        /// Gets or sets the sonic speed.
        /// </summary>
        [Category("Sonar Config"), DefaultValue(1460.0), DisplayName("Sonic Speed")]
        public double SonicSpeed
        {
            get { return sonicSpeed; }
            set { sonicSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the GPS mask.
        /// </summary>
        [Category("Sonar Config"), DefaultValue("GPGGA"), DisplayName("GPS Mask")]
        public string GPSmask
        {
            get { return gpsMask; }
            set { gpsMask = value; }
        }
        #endregion

        #region Functions
        public static bool IsProgrammable(SonarDeviceFormat sdf)
        {
            return ((sdf != SonarDeviceFormat.SonarV1) & (sdf != SonarDeviceFormat.SonarV4));
        }

        public static bool IsProgGPSMask(SonarDeviceFormat sdf)
        {
            return ((sdf == SonarDeviceFormat.SonarV3) | (sdf == SonarDeviceFormat.SonarV5));
        }
        

        public string Write()
        {
            System.Text.Encoding t = System.Text.Encoding.GetEncoding("ibm850");
            string confString = "";

            byte[] bi = new byte[4];
            if (HF) bi[0] += 1;
            if (NF) bi[0] += 2;
            bi[1] = 5;
            if (depth == SonarDepths.Depth100)
                bi[1] = 10;
            bi[2] = 1;
            bi[3] = 13;
            confString += "%i" + t.GetString(bi);

            int speed = (int)Math.Round(1200000.0 / SonicSpeed);
            byte[] bs = new byte[3];
            bs[0] = (byte)(speed);
            bs[1] = (byte)(speed / 256);
            bs[2] = 13;

            confString += "%speed" + t.GetString(bs);

            confString += "%GPS" + gpsMask + "\r";

            confString += "%burne\r";
           
            return confString;
        }
        #endregion

        #region ToString
        public string ToXMLString()
        {
            string s = "";
            s += "LicencedFor=\"" + lisencedFor + "\" ";
            s += "LicencedNr=\"" + lisencedNr + "\" ";
            s += "HF=\"" + hf.ToString() + "\" ";
            s += "NF=\"" + nf.ToString() + "\" ";
            s += "SonicSpeed=\"" + ((int)Math.Round(sonicSpeed)).ToString() + "\" ";
            s += "Depth=\"" + ((int)depth).ToString() + "\" ";
            s += "GPSmask=\"" + gpsMask + "\" ";
            return s;
        }
        #endregion

        #region Constructor
        public SonarDeviceConfig(string content)
        {
            System.Text.Encoding t = System.Text.Encoding.GetEncoding("ibm850");

            // Configdaten SonarV2 & V3            
            #region LicenceInfos
            lisencedNr = content.Substring(0, content.IndexOf("!"));
            content = content.Remove(0, content.IndexOf("!") + 1);
            lisencedFor = content.Substring(0, content.IndexOf("!"));
            content = content.Remove(0, content.IndexOf("!") + 1);
            #endregion

            #region HF / NF
            byte[] contentb = t.GetBytes(content);            
            switch (contentb[0])
            {
                case 0x02:
                    nf = true;
                    hf = false;
                    break;
                case 0x01:
                    hf = true;
                    nf = false;
                    break;
                case 0x03:
                default:
                    hf = true;
                    nf = true;
                    break;
            }
            #endregion

            #region Depth
            if (contentb[1] == 0x05)
                depth = SonarDepths.Depth50;
            else
                depth = SonarDepths.Depth100;
            #endregion

            #region Sonic Speed
            sonicSpeed = Math.Round(1.0 / (contentb[4] * 256 + contentb[3]) * 1200000.0);
            #endregion

            #region GPS Mask
            gpsMask = content.Substring(5);
            if (gpsMask.IndexOf("\0") != -1)
                gpsMask = gpsMask.Remove(gpsMask.IndexOf("\0"));
            #endregion
        }
        #endregion
    }
    #endregion

    #region sonOmeterServerConfig
    public class sonOmeterServerConfig
    {
        public SonarDeviceCollection SonDeviceList { get; set; }
        public PositionDeviceCollection PosDeviceList { get; set; }

        public int DataPort { get; set; }

        public int ConfigPort { get; set; }

        [XmlAttribute]
        [DefaultValue(false)]
        public bool SonID0Timer { get; set; }

        public sonOmeterServerConfig()
        {
            DataPort = 9099;
            ConfigPort = 9100;

            SonDeviceList = new SonarDeviceCollection();
            PosDeviceList = new PositionDeviceCollection();
        }

        public void CopyContent(sonOmeterServerConfig set)
        {
            this.ConfigPort = set.ConfigPort;
            this.DataPort = set.DataPort;          
            this.SonID0Timer = set.SonID0Timer;
            SonDeviceList.Clear();
            PosDeviceList.Clear();
            foreach (var item in set.SonDeviceList)
                SonDeviceList.Add(item);
            foreach (var item in set.PosDeviceList)
                PosDeviceList.Add(item);

        }
    }
    #endregion
}