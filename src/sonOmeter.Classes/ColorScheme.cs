using System;
using System.Xml;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using UKLib.Xml;
using System.Xml.Serialization;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Summary description for ColorScheme.
    /// </summary>
    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class ColorScheme : INotifyPropertyChanged
    {
        #region Variables
        Color backColor = Color.Blue;
        Color foreColor = Color.White;
        Color cutLineColor = Color.Black;
        Color workLineColor = Color.Black;
        Color posLineColor = Color.Black;
        Color posLineColorUnused = Color.LightSalmon;
        Color surfLineColor = Color.White;
        Color posTickColor = Color.Magenta;
        Color bigGridColor = Color.White;
        Color smallGridColor = Color.LightGray;
        Color traceColor = Color.Yellow;
        Color buoyColor = Color.White;
        Color blankLineColor = Color.White;
        Color recordLimitColor = Color.White;
        Color archNoDataColor = Color.White;
        Color manualPointColor = Color.Gray;
        Color disableSelColor = Color.Green;
        Color antennaColor = Color.Red;
        Color recordMarkerColor = Color.Black;
        Color cameraColor = Color.Black;

        private Color colorNotUsedGPS = Color.Red;

        byte alphaChannel = 127;
        #endregion

        #region Properties

        [XmlIgnore, Category("Colors"), DisplayName("Sat GPS Not Used "), DefaultValue(KnownColor.Red)]
        public Color ColorNotUsedGPS
        {
            get
            {
                return colorNotUsedGPS;
            }
            set
            {
                colorNotUsedGPS = value;
                NotifyPropertyChanged(nameof(ColorNotUsedGPS));
            }
        }
        [Browsable(false), DefaultValue("Red")]
        public string XmlColorNotUsedGPS
        {
            get { return SerializeColor.Serialize(ColorNotUsedGPS); }
            set { ColorNotUsedGPS = SerializeColor.Deserialize(value); }
        }

        private Color colorNotUsedGlonas = Color.OrangeRed;
        [XmlIgnore, Category("Colors"), DisplayName("Sat GLONAS Not Used "), DefaultValue(KnownColor.OrangeRed)]
        public Color ColorNotUsedGlonas
        {
            get
            {
                return colorNotUsedGlonas;
            }
            set
            {
                colorNotUsedGlonas = value;
                NotifyPropertyChanged(nameof(ColorNotUsedGlonas));
            }
        }
        [Browsable(false), DefaultValue("OrangeRed")]
        public string XmlColorNotUsedGlonas
        {
            get { return SerializeColor.Serialize(ColorNotUsedGlonas); }
            set { ColorNotUsedGlonas = SerializeColor.Deserialize(value); }
        }

        private Color colorInUsedGPS = Color.Lime;
        [XmlIgnore, Category("Colors"), DisplayName("Sat GPS In Used "), DefaultValue(KnownColor.Lime)]
        public Color ColorInUsedGPS
        {
            get
            {
                return colorInUsedGPS;
            }
            set
            {
                colorInUsedGPS = value;
                NotifyPropertyChanged(nameof(ColorInUsedGPS));
            }
        }
        [Browsable(false), DefaultValue("Lime")]
        public string XmlColorInUsedGPS
        {
            get { return SerializeColor.Serialize(ColorInUsedGPS); }
            set { ColorInUsedGPS = SerializeColor.Deserialize(value); }
        }

        private Color colorInUsedGlonas = Color.LightGreen;
        [XmlIgnore, Category("Colors"), DisplayName("Sat GLONAS In Used "), DefaultValue(KnownColor.LightGreen)]
        public Color ColorInUsedGlonas
        {
            get
            {
                return colorInUsedGlonas;
            }
            set
            {
                colorInUsedGlonas = value;
                NotifyPropertyChanged(nameof(ColorInUsedGlonas));
            }
        }
        [Browsable(false), DefaultValue("LightGreen")]
        public string XmlColorInUsedGlonas
        {
            get { return SerializeColor.Serialize(ColorInUsedGlonas); }
            set { ColorInUsedGlonas = SerializeColor.Deserialize(value); }
        }


        private Color colorTryUsedGPS = Color.Yellow;
        [XmlIgnore, Category("Colors"), DisplayName("Sat GPS Try Used "), DefaultValue(KnownColor.Yellow)]
        public Color ColorTryUsedGPS
        {
            get
            {
                return colorTryUsedGPS;
            }
            set
            {
                colorTryUsedGPS = value;
                NotifyPropertyChanged(nameof(ColorTryUsedGPS));
            }
        }
        [Browsable(false), DefaultValue("Yellow")]
        public string XmlColorTryUsedGPS
        {
            get { return SerializeColor.Serialize(ColorTryUsedGPS); }
            set { ColorTryUsedGPS = SerializeColor.Deserialize(value); }
        }

        private Color colorTryUsedGlonas = Color.YellowGreen;
        [XmlIgnore, Category("Colors"), DisplayName("Sat GLONAS Try Used "), DefaultValue(KnownColor.YellowGreen)]
        public Color ColorTryUsedGlonas
        {
            get
            {
                return colorTryUsedGlonas;
            }
            set
            {
                colorTryUsedGlonas = value;
                NotifyPropertyChanged(nameof(ColorTryUsedGlonas));
            }
        }
        [Browsable(false), DefaultValue("YellowGreen")]
        public string XmlColorTryUsedGlonas
        {
            get { return SerializeColor.Serialize(ColorTryUsedGlonas); }
            set { ColorTryUsedGlonas = SerializeColor.Deserialize(value); }
        }



        [XmlAttribute, Category("Colors"), DisplayName("Alpha Channel"), DefaultValue((byte)127)]
        [Description("This is the alpha channel for all translucent colors. Values range from 0 to 255.")]
        public byte AlphaChannel
        {
            get { return alphaChannel; }
            set { alphaChannel = value; NotifyPropertyChanged("AlphaChannel"); }
        }
       
        [XmlIgnore]
        [Description("This is the back color of most windows."), Category("Colors"), DisplayName("Background"), DefaultValue(KnownColor.Blue)]
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; NotifyPropertyChanged("BackColor"); }
        }
        [Browsable(false), DefaultValue("Blue")]
        public string XmlBackColor
        {
            get { return SerializeColor.Serialize(BackColor); }
            set { BackColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the fore color of most windows."), Category("Colors"), DisplayName("Foreground"), DefaultValue(KnownColor.White)]
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; NotifyPropertyChanged("ForeColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlForeColor
        {
            get { return SerializeColor.Serialize(ForeColor); }
            set { ForeColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of buoys in the trace view."), Category("Colors"), DisplayName("Buoys"), DefaultValue(KnownColor.White), Browsable(false)]
        public Color BuoyColor
        {
            get { return buoyColor; }
            set { buoyColor = value; NotifyPropertyChanged("BuoyColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlBuoyColor
        {
            get { return SerializeColor.Serialize(BuoyColor); }
            set { BuoyColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of the blank line in the trace view."), Category("Colors"), DisplayName("Blank Line"), DefaultValue(KnownColor.White), Browsable(false)]
        public Color BlankLineColor
        {
            get { return blankLineColor; }
            set { blankLineColor = value; NotifyPropertyChanged("BlankLineColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlBlankLineColor
        {
            get { return SerializeColor.Serialize(BlankLineColor); }
            set { BlankLineColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the color of record limit markers."), Category("Colors"), DisplayName("Record Limit Markers"), DefaultValue(KnownColor.White), Browsable(false)]
        public Color RecordLimitColor
        {
            get { return recordLimitColor; }
            set { recordLimitColor = value; NotifyPropertyChanged("RecordLimitColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlRecordLimitColor
        {
            get { return SerializeColor.Serialize(RecordLimitColor); }
            set { RecordLimitColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of the cut lines in the record view."), Category("Colors"), DisplayName("Cut Lines"), DefaultValue(KnownColor.Black)]
        public Color CutLineColor
        {
            get { return cutLineColor; }
            set { cutLineColor = value; NotifyPropertyChanged("CutLineColor"); }
        }
        [Browsable(false), DefaultValue("Black")]
        public string XmlCutLineColor
        {
            get { return SerializeColor.Serialize(CutLineColor); }
            set { CutLineColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the color of the work line."), Category("Colors"), DisplayName("Work Line"), DefaultValue(KnownColor.Black)]
        public Color WorkLineColor
        {
            get { return workLineColor; }
            set { workLineColor = value; NotifyPropertyChanged("WorkLineColor"); }
        }
        [Browsable(false), DefaultValue("Black")]
        public string XmlWorkLineColor
        {
            get { return SerializeColor.Serialize(WorkLineColor); }
            set { WorkLineColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the blending accent of the used position lines."), Category("Colors"), DisplayName("Position Line, Used"), DefaultValue(KnownColor.Black)]
        public Color PosLineColor
        {
            get { return posLineColor; }
            set { posLineColor = value; NotifyPropertyChanged("PosLineColor"); }
        }
        [Browsable(false), DefaultValue("Black")]
        public string XmlPosLineColor
        {
            get { return SerializeColor.Serialize(PosLineColor); }
            set { PosLineColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the blending accent of the unused position lines."), Category("Colors"), DisplayName("Position Line, Unused"), DefaultValue(KnownColor.LightSalmon)]
        public Color PosLineColorUnused
        {
            get { return posLineColorUnused; }
            set { posLineColorUnused = value; NotifyPropertyChanged("PosLineColorUnused"); }
        }
        [Browsable(false), DefaultValue("LightSalmon")]
        public string XmlPosLineColorUnused
        {
            get { return SerializeColor.Serialize(PosLineColorUnused); }
            set { PosLineColorUnused = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("This is the color of the surface line in the trace view."), Category("Colors"), DisplayName("Surface Line"), DefaultValue(KnownColor.White)]
        public Color SurfLineColor
        {
            get { return surfLineColor; }
            set { surfLineColor = value; NotifyPropertyChanged("SurfLineColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlSurfLineColor
        {
            get { return SerializeColor.Serialize(SurfLineColor); }
            set { SurfLineColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("Specifies the color of the position ticks in the bottom of the record view."), Category("Colors"), DisplayName("Position Ticks"), DefaultValue(KnownColor.Magenta)] 
        public Color PosTickColor
        {
            get { return posTickColor; }
            set { posTickColor = value; NotifyPropertyChanged("PosTickColor"); }
        }
        [Browsable(false), DefaultValue("Magenta")]
        public string XmlPosTickColor
        {
            get { return SerializeColor.Serialize(PosTickColor); }
            set { PosTickColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("Specifies the color of the big grid lines."), Category("Colors"), DisplayName("Big Grid"), DefaultValue(KnownColor.White)]
        public Color BigGridColor
        {
            get { return bigGridColor; }
            set { bigGridColor = value; NotifyPropertyChanged("BigGridColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlBigGridColor
        {
            get { return SerializeColor.Serialize(BigGridColor); }
            set { BigGridColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("Specifies the color of the small grid points."), Category("Colors"), DisplayName("Small Grid"), DefaultValue(KnownColor.LightGray)]
        public Color SmallGridColor
        {
            get { return smallGridColor; }
            set { smallGridColor = value; NotifyPropertyChanged("SmallGridColor"); }
        }
        [Browsable(false), DefaultValue("LightGray")]
        public string XmlSmallGridColor
        {
            get { return SerializeColor.Serialize(SmallGridColor); }
            set { SmallGridColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("Specifies the color of the trace."), Category("Colors"), DisplayName("Trace"), DefaultValue(KnownColor.Yellow)]
        public Color TraceColor
        {
            get { return traceColor; }
            set { traceColor = value; NotifyPropertyChanged("TraceColor"); }
        }
        [Browsable(false), DefaultValue("Yellow")]
        public string XmlTraceColor
        {
            get { return SerializeColor.Serialize(TraceColor); }
            set { TraceColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("Specifies the color of invalid data during an arch process."), Category("Colors"), DisplayName("Invalid Arch Data"), DefaultValue(KnownColor.White)]
        public Color ArchNoDataColor
        {
            get { return archNoDataColor; }
            set { archNoDataColor = value; NotifyPropertyChanged("ArchNoDataColor"); }
        }
        [Browsable(false), DefaultValue("White")]
        public string XmlArchNoDataColor
        {
            get { return SerializeColor.Serialize(ArchNoDataColor); }
            set { ArchNoDataColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of manual points."), Category("Colors"), DisplayName("Manual Points"), DefaultValue(KnownColor.Gray)]
        public Color ManualPointColor
        {
            get { return manualPointColor; }
            set { manualPointColor = value; NotifyPropertyChanged("ManualPointColor"); }
        }
        [Browsable(false), DefaultValue("Gray")]
        public string XmlManualPointColor
        {
            get { return SerializeColor.Serialize(ManualPointColor); }
            set { ManualPointColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of disable selection in cut mode."), Category("Colors"), DisplayName("Disable Cut Selection"), DefaultValue(KnownColor.Green)]
        public Color DisableSelColor
        {
            get { return disableSelColor; }
            set { disableSelColor = value; NotifyPropertyChanged("EraseSelColor"); }
        }
        [Browsable(false), DefaultValue("Green")]
        public string XmlDisableSelColor
        {
            get { return SerializeColor.Serialize(DisableSelColor); }
            set { DisableSelColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of the antenna marker."), Category("Colors"), DisplayName("Antenna Color"), DefaultValue(KnownColor.Red), LicencedProperty(Module.Modules.RadioDetection)]
        public Color AntennaColor
        {
            get { return antennaColor; }
            set { antennaColor = value; NotifyPropertyChanged("AntennaColor"); }
        }
        [Browsable(false), DefaultValue("Red")]
        public string XmlAntennaColor
        {
            get { return SerializeColor.Serialize(AntennaColor); }
            set { AntennaColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of the camera marker."), Category("Colors"), DisplayName("Camera Color"), DefaultValue(KnownColor.Black), LicencedProperty(Module.Modules.Testing)]
        public Color CameraColor
        {
            get { return cameraColor; }
            set { cameraColor = value; NotifyPropertyChanged("CameraColor"); }
        }
        [Browsable(false), DefaultValue("Black")]
        public string XmlCameraColor
        {
            get { return SerializeColor.Serialize(CameraColor); }
            set { CameraColor = SerializeColor.Deserialize(value); }
        }

        [XmlIgnore]
        [Description("The color of the record start/end markers."), Category("Colors"), DisplayName("Record Marker Color"), DefaultValue(KnownColor.Black), LicencedProperty(Module.Modules.Testing)]
        public Color RecordMarkerColor
        {
            get { return recordMarkerColor; }
            set { recordMarkerColor = value; NotifyPropertyChanged("RecordMarkerColor"); }
        }
        [Browsable(false), DefaultValue("Black")]
        public string XmlRecordMarkerColor
        {
            get { return SerializeColor.Serialize(RecordMarkerColor); }
            set { RecordMarkerColor = SerializeColor.Deserialize(value); }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override string ToString()
        {
            return "(...)";
        }
    }
}
