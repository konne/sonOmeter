using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Drawing.Design;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using UKLib.Xml;
using UKLib.Net;
using UKLib.Survey.Math;
using UKLib.Survey.Editors;
using UKLib.Controls;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using sonOmeter.Server.Classes;
using UKLib.MathEx;
using UKLib.Arrays;
using UKLib.ExtendedAttributes;

namespace sonOmeter.Classes
{
    #region Enums
    /// <summary>
    /// The Viewmodes during the record of new sonar datas
    /// LeftFixed  : always the left side of the windows is fixed
    /// RightFixed : always the right side of the windows is fixed
    /// RadarLike  : new new data where drawed at free space if the
    ///              data arrive the end of the screen it will start
    ///              at the begin of the screen (left) 
    /// </summary>
    public enum SonarViewMode
    {
        LeftFixed,
        RightFixed,
        RadarLike
    }

    /// <summary>
    /// SonarPanelTypes.
    /// </summary>    
    [TypeConverter(typeof(LocalizedEnumTypeConverter))]
    public enum SonarPanelType
    {
        [EnumDisplayName("HF & NF")]
        Void,
        HF,
        NF,
        [Browsable(false)]
        Sep
    }

    /// <summary>
    /// DotTypes.
    /// </summary>
    public enum DotShapes
    {
        Rectangle,
        Circle
    }

    public enum Sonar2DViewMode
    {
        Fixed = 0,
        Follow = 1,
        FollowRotate = 2
    }
    #endregion

    #region SonarEntryConfig
    public class SonarEntryConfig : INotifyPropertyChanged
    {
        #region Properties
        private int sonarID;
        [XmlAttribute]
        public int SonarID
        {
            get
            {
                return sonarID;
            }
            set
            {
                sonarID = value;
                NotifyPropertyChanged("SonarID");
            }
        }

        private Color sonarColor;
        [XmlIgnore]
        public Color SonarColor
        {
            get
            {
                return sonarColor;
            }
            set
            {
                sonarColor = value;
                NotifyPropertyChanged("SonarColor");
            }
        }
        [XmlAttribute, Browsable(false)]
        public string XmlSonarColor
        {
            get { return SerializeColor.Serialize(sonarColor); }
            set { sonarColor = SerializeColor.Deserialize(value); }
        }

        private bool sonarColorVisible = true;
        [XmlAttribute, DefaultValue(true)]
        [DisplayName("Visible")]
        public bool SonarColorVisible
        {
            get
            {
                return sonarColorVisible;
            }
            set
            {
                sonarColorVisible = value;
                NotifyPropertyChanged("SonarColorVisible");
            }
        }

        private int volumeWeight;
        [XmlAttribute]
        public int VolumeWeight
        {
            get
            {
                return volumeWeight;
            }
            set
            {
                volumeWeight = value;
                NotifyPropertyChanged("VolumeWeight");
            }
        }

        private bool archStop = false;
        [XmlAttribute, Browsable(false), DefaultValue(false)]
        public bool ArchStop
        {
            get
            {
                return archStop;
            }
            set
            {
                archStop = value;
                NotifyPropertyChanged("ArchStop");
            }
        }

        private bool archStopColor = false;
        [XmlAttribute, Browsable(false), DefaultValue(false)]
        public bool ArchStopColor
        {
            get
            {
                return archStopColor;
            }
            set
            {
                archStopColor = value;
                NotifyPropertyChanged("ArchStopColor");
            }
        }

        private bool usedIn3D = true;
        [XmlAttribute, DefaultValue(true)]
        public bool UsedIn3D
        {
            get
            {
                return usedIn3D;
            }
            set
            {
                usedIn3D = value;
                NotifyPropertyChanged("UsedIn3D");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override string ToString()
        {
            return "SonarTypeID " + sonarID.ToString();
        }
    }
    #endregion

    #region ArchDepthConverter
    public class ArchDepthConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if ((sourceType == typeof(string)) || (sourceType == typeof(int)))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            float f = 0;

            if (value is string)
            {
                float.TryParse((string)value, NumberStyles.Float, culture.NumberFormat, out f);
                return f;
            }
            else if (value is int)
            {
                f = (float)Math.Round(Math.Pow(10, (double)((int)value) / 10.0 - 1.0), 2);
                return f;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if ((destinationType == typeof(string)) || (destinationType == typeof(int)))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((float)value).ToString(culture.NumberFormat);
            }
            else if (destinationType == typeof(int))
            {
                return (int)((Math.Log10((float)value) + 1.0) * 10.0);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    #endregion

    public class GlobalSettings : FilterablePropertyBase, INotifyPropertyChanged
    {
        #region Variables
        // [general]
        double wheelSpeed = 10;
        double autoSave = 15.0;
        int hostPort = 9099;
        int controlPort = 9100;
        PosType scanPosType = PosType.NMEA;
        string[] scanPosStrings = { "$GPLLK" };
        string hostName = "localhost";
        float minQualityMeter = 0.5f;
        int minSatellites = 4;
        int minQuality = 3;
        int maxPosDistance = 50;
        bool useWGSAltitude = false;
        private bool changed = false;
        double interpolationThreshold = 500000;
        double interpolationAltitudeThreshold = 100;
        bool mergeWithAbsoluteDepths = true;
        RotD compassZero = new RotD();
        bool usePitchRoll = true;

        // [record view]
        double depthTop = 0;
        double depthBottom = -100;
        double pitchH = 0.0;
        double pitchV = 0.0;
        SonarViewMode viewMode = SonarViewMode.RadarLike;
        SerializeDictionary<int, MPConnStyle> mpConnStyles = new SerializeDictionary<int, MPConnStyle>();
        bool showAntDepth = true;

        // [trace view]
        double gridSize = 50;
        double traceSize = 5;
        double dotSize = 1;
        double buoySize = 30;
        double lockOffsetX = 0.0;
        double lockOffsetY = 0.0;
        double buoyConnectionGrid = 5.0;
        int smallGridDiv = 5;
        int buoyConnectionGridDiv = 4;
        int corridorBeepInterval = 1000;
        bool corridorBeep = true;
        bool compassVector = false;
        DotShapes dotShape = DotShapes.Rectangle;
        Sonar2DViewMode viewMode2D = Sonar2DViewMode.Fixed;
        Sonar2D.Pointer.PointerType boatVector = Sonar2D.Pointer.PointerType.Cross;
        Sonar2D.Buoy.BuoySizeUnit buoySizeUnit = Sonar2D.Buoy.BuoySizeUnit.Pixels;

        // [DXF or trace]
        double skipDetail = 0.5;

        // [3D options]
        bool show3DWalls = true;
        bool show3DSurfaceGrid = false;
        bool extended3DArch = false;
        float heightFactor = 1.0F;
        Sonar3DDrawMode drawMode3D = Sonar3DDrawMode.Solid;
        Sonar3DDrawMode dragDrawMode3D = Sonar3DDrawMode.Solid;

        // [misc objects]
        ColorScheme cs = new ColorScheme();
        Module lic = null;

        // [coordinate transformations]
        Ellipsoid srcEllipsoid = Ellipsoid.Bessel;
        Ellipsoid dstEllipsoid = Ellipsoid.WGS84;
        Datum2WGS srcDatum = Datum2WGS.DHDN;
        Datum2WGS dstDatum = Datum2WGS.WGS84;
        Transform transfrm = new Transform();

        // [export]
        string[] exportFunction;
        bool exportWithCut = true;
        bool exportWithArch = false;
        bool exportWithBuoys = false;
        double exportMinDepth = 0.0;
        double exportMaxDepth = -100.0;
        SonarPanelType exportSonarType = SonarPanelType.Void;

        // [calcDeep]
        float calcdMaxChange = 1.8F;
        int calcdAverage = 2;
        float calcdLinearisation = 0.0005F;
        float calcdMaxSpace = 0.2F;
        double calcdTop = 0;
        double calcdBottom = -100;

        // [Camera]
        string cameraHostname = "192.168.1.111";
        string cameraUsername = "root";
        string cameraPassword = "password";
        string cameraVideoDir = "c:\\video\\";

        // [Submarine]
        string submarineHostname = "192.168.1.102";
        int submarineDepthMarker = 5;

        // [number format]
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        // sonOmeterServer
        bool serverSonID0Timer = false;
        #endregion

        #region Properties
        #region General
        [XmlIgnore]
        [Browsable(false)]
        public bool Changed
        {
            set { changed = value; }
            get { return changed; }
        }

#if DEBUG
        double simulationSpeed = 1.0;
        [XmlIgnore]
        [DefaultValue(1.0), Category("General")]
        [DisplayName("Simulation Speed"), Description("The playback speed during simulation.")]
        public double SimulationSpeed
        {
            set { simulationSpeed = value; }
            get { return simulationSpeed; }
        }
#endif

        /// <summary>
        /// Gets or sets the host port.
        /// </summary>                
        [DefaultValue(9099), Category("General")]
        [DisplayName("Host Port"), Description("The program connects on this port to receive data.")]
        public int HostPort
        {
            get { return hostPort; }
            set { hostPort = value; NotifyPropertyChanged("HostPort"); }
        }

        /// <summary>
        /// Gets or sets the control port.
        /// </summary>
        [DefaultValue(9100), Category("General")]
        [DisplayName("Control Port"), Description("The program connects on this port to control the hardware.")]
        public int ControlPort
        {
            get { return controlPort; }
            set { controlPort = value; NotifyPropertyChanged("ControlPort"); }
        }

        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        [DefaultValue("localhost"), Category("General")]
        [DisplayName("Host Name"), Description("The program connects to this host to receive data.")]
        public string HostName
        {
            get { return hostName; }
            set { hostName = value; NotifyPropertyChanged("HostName"); }
        }

        /// <summary>
        /// Gets or sets the autosave interval in minutes.
        /// <summary>
        [DefaultValue(15.0), Category("General")]
        [DisplayName("Autosave Intervall"), Description("The autosave intervall in minutes.")]
        public double AutoSave
        {
            get { return autoSave; }
            set { autoSave = value; NotifyPropertyChanged("AutoSave"); }
        }

        /// <summary>
        /// Gets or sets the speed multiplier for mouse wheel operations, e.g. zooming.
        /// </summary>
        [DefaultValue(10.0), Category("General")]
        [DisplayName("Mouse Wheel Speed"), Description("The speed of the mouse wheel.")]
        public double WheelSpeed
        {
            get { return wheelSpeed; }
            set { wheelSpeed = value; NotifyPropertyChanged("WheelSpeed"); }
        }

        /// <summary>
        /// Gets or sets the flag indicating the usage of absolute depths for merge operations during interpolation.
        /// </summary>
        [DefaultValue(true), Category("General")]
        [DisplayName("Merge With Absolute Depths"), Description("Use absolute flags for merge operations during interpolation.")]
        public bool MergeWithAbsoluteDepths
        {
            get { return mergeWithAbsoluteDepths; }
            set { mergeWithAbsoluteDepths = value; NotifyPropertyChanged("MergeWithAbsoluteDepths"); }
        }
        #endregion

        #region Interpolation
        [Category("Interpolation")]
        [DisplayName("Compass Zero"), Description("Enter the calibrated compass zero here.")]
        public RotD CompassZero
        {
            get { return compassZero; }
            set { compassZero = value; NotifyPropertyChanged("CompassZero"); }
        }

        [DefaultValue(true), Category("Interpolation")]
        [DisplayName("Pitch/Roll Correction"), Description("Toggles the pitch and roll coordinate correction")]
        public bool UsePitchRoll
        {
            get { return usePitchRoll; }
            set { usePitchRoll = value; NotifyPropertyChanged("UsePitchRoll"); }
        }
        
        [DefaultValue(4), Category("Interpolation")]
        [DisplayName("Min. Satellites"), Description("The minimum number of satellites to accept a position for interpolation.")]
        public int MinSatellites
        {
            get { return minSatellites; }
            set { minSatellites = value; NotifyPropertyChanged("MinSatellites"); }
        }

        [DefaultValue(3), Category("Interpolation")]
        [DisplayName("Min. GPS Quality"), Description("The minimum quality to accept a position for interpolation.")]
        public int MinQuality
        {
            get { return minQuality; }
            set { minQuality = value; NotifyPropertyChanged("MinQuality"); }
        }

        [DefaultValue(0.5F), Category("Interpolation")]
        [DisplayName("Min. GPS MeterQuality"), Description("The minimum meterquality to accept a position for interpolation.")]
        public float MinQualityMeter
        {
            get { return minQualityMeter; }
            set { minQualityMeter = value; NotifyPropertyChanged("MinQualityMeter"); }
        }

        [DefaultValue(50), Category("Interpolation")]
        [DisplayName("Max. Position Distance"), Description("The maximum number of lines without a valid position.")]
        public int MaxPosDistance
        {
            get { return maxPosDistance; }
            set { maxPosDistance = value; NotifyPropertyChanged("MaxPosDistance"); }
        }

        /// <summary>
        /// Gets or sets the preferred type of positions.
        /// </summary>
        [DefaultValue(PosType.NMEA), Category("Interpolation")]
        [DisplayName("Preferred Pos Type"), Description("The preferred type of position markers used for interpolation.")]
        public PosType ScanPosType
        {
            get { return scanPosType; }
            set { scanPosType = value; }
        }

        /// <summary>
        /// Gets or sets the subtype of ScanPosType.
        /// </summary>
        [Category("Interpolation")]
        [DisplayName("Scan Pos Strings"), Description("The allowed identifiers of position markers used for interpolation.")]
        public string[] ScanPosStrings
        {
            get { return scanPosStrings; }
            set { scanPosStrings = value; }
        }


        /// <summary>
        /// Gets or sets the interpolation threshold.
        /// </summary>
        [DefaultValue(500000.0), Category("Interpolation")]
        [DisplayName("Interpol. Threshold (Hor)"), Description("The RV & HV plane interpolation threshold. Use zero to disable this function.")]
        public double InterpolationThreshold
        {
            get { return interpolationThreshold; }
            set { interpolationThreshold = value; NotifyPropertyChanged("InterpolationThreshold"); }
        }

        /// <summary>
        /// Gets or sets the interpolation threshold.
        /// </summary>
        [DefaultValue(100.0), Category("Interpolation")]
        [DisplayName("Interpol. Threshold (Ver)"), Description("The altitude interpolation threshold. Use zero to disable this function.")]
        public double InterpolationAltitudeThreshold
        {
            get { return interpolationAltitudeThreshold; }
            set { interpolationAltitudeThreshold = value; NotifyPropertyChanged("InterpolationAltitudeThreshold"); }
        }

        /// <summary>
        /// Gets or sets the GPGGA altitude type.
        /// </summary>
        [DefaultValue(false), Category("Interpolation")]
        [DisplayName("Use WGS Altitude"), Description("The altitude type used for GPGGA positions.")]
        public bool UseWGSAltitude
        {
            get { return useWGSAltitude; }
            set { useWGSAltitude = value; NotifyPropertyChanged("UseWGSAltitude"); }
        }
        #endregion

        #region 1D
        /// <summary>
        /// Gets or sets the list of connection styles of manual points.
        /// </summary>
        [Description("Connection styles of manual points."), Category("Record view"), DisplayName("Point Connect Styles"), Editor(typeof(MPConnEditor), typeof(UITypeEditor)), LicencedProperty(Module.Modules.DynamicProfiles)]
        public SerializeDictionary<int, MPConnStyle> MPConnStyles
        {
            get { return mpConnStyles; }
            set { mpConnStyles = value; NotifyPropertyChanged("MPConnStyles"); }
        }

        /// <summary>
        /// Gets or sets the mode of the record views during recording.
        /// </summary>
        [Description("The view mode of sonar records during recording."), Category("Record view"), DefaultValue(SonarViewMode.RadarLike), DisplayName("View Mode")]
        public SonarViewMode ViewMode
        {
            get { return viewMode; }
            set { viewMode = value; NotifyPropertyChanged("ViewMode"); }
        }

        /// <summary>
        /// Gets or sets the top depth.
        /// </summary>
        [Description("This value controls the top limit in the record view in meters."), Category("Record view"), DefaultValue(0.0), DisplayName("Depth Limit (Top)")]
        public double DepthTop
        {
            get { return depthTop; }
            set { depthTop = value; NotifyPropertyChanged("DepthTop"); }
        }

        /// <summary>
        /// Gets or sets the bottom depth.
        /// </summary>
        [Description("This value controls the bottom limit in the record view in meters."), Category("Record view"), DefaultValue(-100.0), DisplayName("Depth Limit (Bottom)")]
        public double DepthBottom
        {
            get { return depthBottom; }
            set { depthBottom = value; NotifyPropertyChanged("DepthBottom"); }
        }

        /// <summary>
        /// Gets or sets the horizontal pitch.
        /// </summary>
        [Description("This value sets the horizontal marker pitch in meters. Choose 0 to switch this feature off."), Category("Record view"), DefaultValue(0.0), DisplayName("Pitch (Hor)")]
        public double PitchH
        {
            get { return pitchH; }
            set { pitchH = value; NotifyPropertyChanged("PitchH"); }
        }

        /// <summary>
        /// Gets or sets the vertical pitch.
        /// </summary>
        [Description("This value sets the vertical marker pitch in meters. Choose 0 to switch this feature off."), Category("Record view"), DefaultValue(0.0), DisplayName("Pitch (Ver)")]
        public double PitchV
        {
            get { return pitchV; }
            set { pitchV = value; NotifyPropertyChanged("PitchV"); }
        }

        /// <summary>
        /// Gets or sets the vertical pitch.
        /// </summary>
        [Description("Toggles the display of the antenna depth."), Category("Record view"), DefaultValue(true), DisplayName("Show Antenna Depth"), LicencedProperty(Module.Modules.RadioDetection)]
        public bool ShowAntDepth
        {
            get { return showAntDepth; }
            set { showAntDepth = value; NotifyPropertyChanged("ShowAntDepth"); }
        }
        #endregion

        #region 2D
        /// <summary>
        /// Gets or sets the beep modulation interval when passing corridors.
        /// </summary>
        [Description("The beep modulation interval when passing corridors."), Category("Trace view"), DefaultValue(1000), DisplayName("Corridor Beep Interval (ms)")]
        public int CorridorBeepInterval
        {
            get { return corridorBeepInterval; }
            set { corridorBeepInterval = value; NotifyPropertyChanged("CorridorBeepInterval"); }
        }

        /// <summary>
        /// Gets or sets the flag that enables the beep modulation when passing corridors.
        /// </summary>
        [Description("Toggle the beep modulation when passing corridors."), Category("Trace view"), DefaultValue(true), DisplayName("Corridor Beep Enable")]
        public bool CorridorBeep
        {
            get { return corridorBeep; }
            set { corridorBeep = value; NotifyPropertyChanged("CorridorBeep"); }
        }

        /// <summary>
        /// Gets or sets the usage of the compass as boat vector.
        /// </summary>
        [Description("Toggle the usage of the compass for calculating the boat movement vector."), Category("Trace view"), DefaultValue(false), DisplayName("Use Compass Vector")]
        public bool CompassVector
        {
            get { return compassVector; }
            set { compassVector = value; NotifyPropertyChanged("CompassVector"); }
        }

        /// <summary>
        /// Gets or sets the type of the boat vector.
        /// </summary>
        [Description("Select the display type of the boat movement vector."), Category("Trace view"), DefaultValue(Sonar2D.Pointer.PointerType.Cross), DisplayName("Boat Vector Type")]
        public Sonar2D.Pointer.PointerType BoatVector
        {
            get { return boatVector; }
            set { boatVector = value; NotifyPropertyChanged("BoatVector"); }
        }

        /// <summary>
        /// Gets or sets the size of the big grid in meters.
        /// </summary>
        [Description("Specifies the main grid spacing in meters."), Category("Trace view"), DefaultValue(50.0), DisplayName("Grid Size (Main)")]
        public double GridSize
        {
            get { return gridSize; }
            set { gridSize = value; NotifyPropertyChanged("GridSize"); }
        }

        /// <summary>
        /// Gets or sets the size of the trace grid in meters.
        /// </summary>
        [Description("Specifies the trace grid spacing in meters."), Category("Trace view"), DefaultValue(5.0), DisplayName("Grid Size (Trace)")]
        public double TraceSize
        {
            get { return traceSize; }
            set { traceSize = value; NotifyPropertyChanged("TraceSize"); }
        }

        /// <summary>
        /// Gets or sets the size of the buoy bonnection grid in meters.
        /// </summary>
        [Description("Specifies the buoy bonnection grid spacing in meters."), Category("Trace view"), DefaultValue(5.0), DisplayName("Buoy Connection Grid"), LicencedProperty(Module.Modules.V22)]
        public double BuoyConnectionGrid
        {
            get { return buoyConnectionGrid; }
            set
            {
                buoyConnectionGrid = Math.Abs(value);
                NotifyPropertyChanged("BuoyConnectionGrid");
            }
        }

        /// <summary>
        /// Gets or sets the amount of small grid divs per buoy bonnection grid div.
        /// </summary>
        [Description("Specifies the amount of small ticks between two main buoy bonnection grid lines."), Category("Trace view"), DefaultValue(4), DisplayName("Buoy Connection Grid Divider Dots"), LicencedProperty(Module.Modules.V22)]
        public int BuoyConnectionGridDiv
        {
            get { return buoyConnectionGridDiv; }
            set
            {
                buoyConnectionGridDiv = Math.Abs(value);
                NotifyPropertyChanged("BuoyConnectionGridDiv");
            }
        }

        /// <summary>
        /// Gets or sets the amount of small grid divs per big grid div.
        /// </summary>
        [Description("Specifies the amount of small ticks between two main grid lines."), Category("Trace view"), DefaultValue(5), DisplayName("Grid Divider Dots")]
        public int SmallGridDiv
        {
            get { return smallGridDiv; }
            set { smallGridDiv = value; NotifyPropertyChanged("SmallGridDiv"); }
        }

        /// <summary>
        /// Gets or sets the dot size in meters.
        /// </summary>
        [Description("Specifies the dot size in meters."), Category("Trace view"), DefaultValue(1.0), DisplayName("Sonar Dot Size")]
        public double DotSize
        {
            get { return dotSize; }
            set { dotSize = value; NotifyPropertyChanged("DotSize"); }
        }

        /// <summary>
        /// Gets or sets the dot shape.
        /// </summary>
        [Description("Specifies the dot shape."), Category("Trace view"), DefaultValue(DotShapes.Rectangle), DisplayName("Sonar Dot Shape")]
        public DotShapes DotShape
        {
            get { return dotShape; }
            set { dotShape = value; NotifyPropertyChanged("DotShape"); }
        }

        /// <summary>
        /// Gets or sets the buoy size type.
        /// </summary>
        [Description("Specifies the unit of the buoy size value."), Category("Trace view"), DefaultValue(Sonar2D.Buoy.BuoySizeUnit.Pixels), DisplayName("Buoy Size Unit"), LicencedProperty(Module.Modules.V22)]
        public Sonar2D.Buoy.BuoySizeUnit BuoySizeUnit
        {
            get { return buoySizeUnit; }
            set { buoySizeUnit = value; NotifyPropertyChanged("BuoySizeUnit"); }
        }

        /// <summary>
        /// Gets or sets the buoy size.
        /// </summary>
        [Description("Specifies the buoy size."), Category("Trace view"), DefaultValue(30.0), DisplayName("Buoy Size"), LicencedProperty(Module.Modules.V22)]
        public double BuoySize
        {
            get { return buoySize; }
            set { buoySize = value; NotifyPropertyChanged("BuoySize"); }
        }

        /// <summary>
        /// Gets or sets the offset for buoy connection locking.
        /// </summary>
        [Description("Specifies the x offset for buoy connection locking."), DefaultValue(0.0), Category("Trace view"), DisplayName("Lock Offset X"), LicencedProperty(Module.Modules.V22)]
        public double LockOffsetX
        {
            get { return lockOffsetX; }
            set { lockOffsetX = value; NotifyPropertyChanged("LockOffsetX"); }
        }

        /// <summary>
        /// Gets or sets the offset for buoy connection locking.
        /// </summary>
        [Description("Specifies the y offset for buoy connection locking."), DefaultValue(0.0), Category("Trace view"), DisplayName("Lock Offset Y"), LicencedProperty(Module.Modules.V22)]
        public double LockOffsetY
        {
            get { return lockOffsetY; }
            set { lockOffsetY = value; NotifyPropertyChanged("LockOffsetY"); }
        }

        /// <summary>
        /// Gets or sets the offset for buoy connection locking.
        /// </summary>
        [Description("Details in the trace view below this threshold are not drawn to increase speed. Zoom in to make them visible again."), DefaultValue(0.5), Category("Trace view"), DisplayName("Skip Details Below"), LicencedProperty(Module.Modules.DXF)]
        public double SkipDetail
        {
            get { return skipDetail; }
            set { skipDetail = value; NotifyPropertyChanged("SkipDetail"); }
        }

        /// <summary>
        /// Gets or sets the 2D view mode.
        /// </summary>
        [Browsable(false)]
        public Sonar2DViewMode ViewMode2D
        {
            get { return viewMode2D; }
            set { viewMode2D = value; NotifyPropertyChanged("ViewMode2D"); }
        }
        #endregion

        #region 3D
        /// <summary>
        /// Gets or sets the 3D draw mode.
        /// </summary>
        [Browsable(false), Description("Specifies the 3D draw mode."), Category("3D view"), DefaultValue(Sonar3DDrawMode.Solid), DisplayName("Draw Mode"), LicencedProperty(Module.Modules.ThreeD)]
        public Sonar3DDrawMode DrawMode3D
        {
            get { return drawMode3D; }
            set { drawMode3D = value; NotifyPropertyChanged("DrawMode3D"); }
        }

        /// <summary>
        /// Gets or sets the 3D draw mode while the camera is moved.
        /// </summary>
        [Browsable(false), Description("Specifies the 3D draw mode while the camera is moved."), Category("3D view"), DefaultValue(Sonar3DDrawMode.Solid), DisplayName("Movement Draw Mode"), LicencedProperty(Module.Modules.ThreeD)]
        public Sonar3DDrawMode DragDrawMode3D
        {
            get { return dragDrawMode3D; }
            set { dragDrawMode3D = value; NotifyPropertyChanged("DragDrawMode3D"); }
        }

        /// <summary>
        /// Gets or sets the height factor.
        /// </summary>
        [Description("Specifies the height factor of the surface."), Category("3D view"), DefaultValue(1.0F), DisplayName("Height Factor"), LicencedProperty(Module.Modules.ThreeD)]
        public float HeightFactor
        {
            get { return heightFactor; }
            set { heightFactor = value; NotifyPropertyChanged("HeightFactor"); }
        }

        /// <summary>
        /// Gets or sets the wall flag.
        /// </summary>
        [Description("Toggles the creation of a surface grid in 3D."), Category("3D view"), DefaultValue(false), DisplayName("Show Surface Grid"), LicencedProperty(Module.Modules.ThreeD)]
        public bool Show3DSurfaceGrid
        {
            get { return show3DSurfaceGrid; }
            set { show3DSurfaceGrid = value; NotifyPropertyChanged("Show3DSurfaceGrid"); }
        }

        /// <summary>
        /// Gets or sets the wall flag.
        /// </summary>
        [Description("Toggles the creation of walls in 3D."), Category("3D view"), DefaultValue(true), DisplayName("Show Walls"), LicencedProperty(Module.Modules.ThreeD)]
        public bool Show3DWalls
        {
            get { return show3DWalls; }
            set { show3DWalls = value; NotifyPropertyChanged("Show3DWalls"); }
        }

        /// <summary>
        /// Gets or sets the extended 3D arch flag.
        /// </summary>
        [Description("Toggles the extended arch mode in 3D."), Category("3D view"), DefaultValue(false), DisplayName("Extended Arch"), LicencedProperty(Module.Modules.ThreeD)]
        public bool Extended3DArch
        {
            get { return extended3DArch; }
            set { extended3DArch = value; NotifyPropertyChanged("Extended3DArch"); }
        }

        [XmlIgnore(), Browsable(false)]
        public bool[] ColorUsedIn3D
        {
            get
            {
                int count = SECL.Count;
                bool[] usedIn3D = new bool[count];

                for (int i = 0; i < count; i++)
                    usedIn3D[i] = secl[i].UsedIn3D;

                return usedIn3D;
            }
        }
        #endregion

        #region Transform
        /// <summary>
        /// Sets the transformation parameter.
        /// </summary>
        [Description("A specific transformation parameter."), Category("Transform"), DefaultValue(-1.0), DisplayName("Transform Parameter")]
        public double TransformParam
        {
            set { transfrm.Param = value; }
            get { return transfrm.Param; }
        }

        /// <summary>
        /// Gets or sets the source for coordinate transformations.
        /// </summary>
        [Description("The source ellipsoid."), Category("Transform"), DisplayName("Source Ellipsoid")]
        [Editor(typeof(EllipsoidEditor), typeof(UITypeEditor))]
        public Ellipsoid SrcEllipsoid
        {
            get { return srcEllipsoid; }
            set { srcEllipsoid = value; NotifyPropertyChanged("SrcEllipsoid"); }
        }

        /// <summary>
        /// Gets or sets the destination for coordinate transformations.
        /// </summary>
        [Description("The destination ellipsoid."), Category("Transform"), DisplayName("Destination Ellipsoid")]
        [Editor(typeof(EllipsoidEditor), typeof(UITypeEditor))]
        public Ellipsoid DstEllipsoid
        {
            get { return dstEllipsoid; }
            set { dstEllipsoid = value; NotifyPropertyChanged("DstEllipsoid"); }
        }

        /// <summary>
        /// Gets or sets the source for coordinate transformations.
        /// </summary>
        [Description("The source datum."), Category("Transform"), DisplayName("Source Datum")]
        [Editor(typeof(DatumEditor), typeof(UITypeEditor))]
        public Datum2WGS SrcDatum
        {
            get { return srcDatum; }
            set { srcDatum = value; NotifyPropertyChanged("SrcDatum"); }
        }

        /// <summary>
        /// Gets or sets the destination for coordinate transformations.
        /// </summary>
        [Description("The destination datum."), Category("Transform"), DisplayName("Destination Datum")]
        [Editor(typeof(DatumEditor), typeof(UITypeEditor))]
        public Datum2WGS DstDatum
        {
            get { return dstDatum; }
            set { dstDatum = value; NotifyPropertyChanged("DstDatum"); }
        }
        #endregion

        #region Export
        [Description("Export data with cut lines enabled."), Category("Export"), DisplayName("Enable Cut"), DefaultValue(true)]
        public bool ExportWithCut
        {
            get { return exportWithCut; }
            set { exportWithCut = value; }
        }

        [Description("Export data with virtual archeology enabled."), Category("Export"), DisplayName("Enable Arch"), DefaultValue(false)]
        public bool ExportWithArch
        {
            get { return exportWithArch; }
            set { exportWithArch = value; }
        }

        [Description("Append buoys to export (formatted like manual points)."), Category("Export"), DisplayName("Append Buoys"), DefaultValue(false)]
        public bool ExportWithBuoys
        {
            get { return exportWithBuoys; }

            set { exportWithBuoys = value; }
        }

        [Description("Only export data with this sonartype"), Category("Export"), DisplayName("Sonar Type"), DefaultValue(SonarPanelType.HF)]
        public SonarPanelType ExportSonarType
        {
            get { return exportSonarType; }
            set { exportSonarType = value; }
        }

        [XmlIgnore]
        [Description("Used for customized export formats."), Category("Export"), DisplayName("Export Function")]
        public string[] ExportFunction
        {
            get { return exportFunction; }
            set { exportFunction = value; }
        }

        [DefaultValue(0.0), Category("Export")]
        [DisplayName("Min Depth"), Description("Only export data with depth >")]
        public double ExportMinDepth
        {
            get { return exportMinDepth; }
            set { exportMinDepth = value; }
        }

        [DefaultValue(-100.0), Category("Export")]
        [DisplayName("Max Depth"), Description("Only export data with depth <")]
        public double ExportMaxDepth
        {
            get { return exportMaxDepth; }
            set { exportMaxDepth = value; }
        }

        #endregion

        #region Camera
        /// <summary>
        /// Gets or sets the cameraHostname.
        /// </summary>
        [DefaultValue("192.168.1.111")]
        [Description("The program connects on this IP to the Camera"), Category("Camera"), DisplayName("IP-Adress"), LicencedProperty(Module.Modules.Submarine)]
        public string CameraHostname
        {
            get { return cameraHostname; }
            set { cameraHostname = value; NotifyPropertyChanged("CameraHostname"); }
        }

        /// <summary>
        /// Gets or sets the cameraUsername.
        /// </summary>
        [DefaultValue("root")]
        [Description("The Username for the Camera"), Category("Camera"), DisplayName("Username"), LicencedProperty(Module.Modules.Submarine)]
        public string CameraUsername
        {
            get { return cameraUsername; }
            set { cameraUsername = value; NotifyPropertyChanged("CameraUsername"); }
        }

        /// <summary>
        /// Gets or sets the cameraPassword.
        /// </summary>
        [DefaultValue("password")]
        [Description("The Password for the Camera"), Category("Camera"), DisplayName("Password"), LicencedProperty(Module.Modules.Submarine)]
        public string CameraPassword
        {
            get { return cameraPassword; }
            set { cameraPassword = value; NotifyPropertyChanged("CameraPassword"); }
        }

        /// <summary>
        /// Gets or sets the cameraVideoDir.
        /// </summary>

        [DefaultValue("c:\\video\\")]
        [Description("The VideoDir for the Camera"), Category("Camera"), DisplayName("VideoDir"), LicencedProperty(Module.Modules.Submarine)]
        public string CameraVideoDir
        {
            get { return cameraVideoDir; }
            set { cameraVideoDir = value; NotifyPropertyChanged("CameraVideoDir"); }
        }

        #endregion

        #region Submarine
        /// <summary>
        /// Gets or sets the IP address of the submarine.
        /// </summary>
        [Description("The program connects on this IP to the submarine."), Category("Submarine"), DefaultValue("192.168.1.102"), DisplayName("IP-Adress"), LicencedProperty(Module.Modules.Submarine)]
        public string SubmarineHostname
        {
            get { return submarineHostname; }
            set { submarineHostname = value; NotifyPropertyChanged("SubmarineHostname"); }
        }

        /// <summary>
        /// Gets or sets the submarine depth marker interval.
        /// </summary>
        [Description("This sets the number of sonar lines between two submarine depth markers."), Category("Submarine"), DefaultValue(5), DisplayName("Depth Marker Interval"), LicencedProperty(Module.Modules.Submarine)]
        public int SubmarineDepthMarker
        {
            get { return submarineDepthMarker; }
            set { if (value < 1) return; submarineDepthMarker = value; NotifyPropertyChanged("SubmarineDepthMarker"); }
        }
        #endregion

        #region CalcDepth
        [DefaultValue(1.8F)]
        [Description("Max Change between Ticks"), Category("CalcDepth"), LicencedProperty(Module.Modules.CalcDepth)]
        public float CalcdMaxChange
        {
            get { return calcdMaxChange; }
            set { calcdMaxChange = value; }
        }

        [DefaultValue(2)]
        [Description("Average Ticks"), Category("CalcDepth"), LicencedProperty(Module.Modules.CalcDepth)]
        public int CalcdAverage
        {
            get { return calcdAverage; }
            set
            {
                if (value > 0)
                {
                    calcdAverage = value;
                }
            }
        }

        [DefaultValue(0.0005F)]
        [Description("Linearization"), Category("CalcDepth"), LicencedProperty(Module.Modules.CalcDepth)]
        public float CalcdLinearisation
        {
            get { return calcdLinearisation; }
            set
            {
                if (value > 0)
                {
                    calcdLinearisation = value;
                }
            }
        }

        [DefaultValue(0.2F)]
        [Description("Max Blue Water"), Category("CalcDepth"), LicencedProperty(Module.Modules.CalcDepth)]
        public float CalcdMaxSpace
        {
            get { return calcdMaxSpace; }
            set
            {
                if (value > 0)
                {
                    calcdMaxSpace = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the top depth of the automatic cut.
        /// </summary>
        [Description("Controls the top limit of the automatic cut in meters."), Category("CalcDepth"), DefaultValue(0.0), DisplayName("Depth Limit (Top)")]
        public double CalcdTop
        {
            get { return calcdTop; }
            set { calcdTop = value; NotifyPropertyChanged("CalcdTop"); }
        }

        /// <summary>
        /// Gets or sets the bottom depth of the automatic cut.
        /// </summary>
        [Description("Controls the bottom limit of the automatic cut in meters."), Category("CalcDepth"), DefaultValue(-100.0), DisplayName("Depth Limit (Bottom)")]
        public double CalcdBottom
        {
            get { return calcdBottom; }
            set { calcdBottom = value; NotifyPropertyChanged("CalcdBottom"); }
        }
        #endregion

        #region Server
        [DefaultValue(false)]
        [Description("sonID0Timer"), Category("Server")]
        public bool ServerSonID0Timer
        {
            get { return serverSonID0Timer; }
            set
            {
                if (value != serverSonID0Timer)
                {
                    serverSonID0Timer = value;
                    NotifyPropertyChanged("ServerSonID0Timer");
                }
            }
        }

        private bool serverAutoStart = true;
        [DefaultValue(true)]
        [Description("Autostart Device Server"), Category("Server")]
        public bool ServerAutoStart
        {
            get { return serverAutoStart; }
            set
            {
                if (value != serverAutoStart)
                {
                    serverAutoStart = value;
                    NotifyPropertyChanged("ServerAutoStart");
                }
            }
        }

        SonarDeviceCollection sonDeviceList = new SonarDeviceCollection();
        [Category("Server"), DisplayName("Sonardevice List"), Description("Configure the Sonar Devicelist Config")]
        public SonarDeviceCollection SonDeviceList
        {
            get { return sonDeviceList; }
            set
            {
                if (value != sonDeviceList)
                {
                    sonDeviceList = value;
                    NotifyPropertyChanged("SonarDeviceList");
                }
            }
        }

        PositionDeviceCollection posDeviceList = new PositionDeviceCollection();
        [Category("Server"), DisplayName("Positiondevice List"), Description("Configure the Sonar Devicelist Config")]
        public PositionDeviceCollection PosDeviceList
        {
            get { return posDeviceList; }
            set
            {
                if (value != posDeviceList)
                {
                    posDeviceList = value;
                    NotifyPropertyChanged("PosDeviceList");
                }
            }
        }
        #endregion

        #region SonarEntryConfig
        private BindingList<SonarEntryConfig> secl;

        [Category("General"), DisplayName("Sonarentry Config")]
        public BindingList<SonarEntryConfig> SECL
        {
            get
            {
                return secl;
            }
            set
            {
                if (value != null)
                {
                    if (value != secl)
                    {
                        secl = value;
                        secl.ListChanged += new ListChangedEventHandler(secl_ListChanged);
                    }
                }
                else
                    secl.Clear();
            }
        }
        #endregion

        #region Arch
        float archDepth = 0.1F;
        [Browsable(false)]
        [TypeConverter(typeof(ArchDepthConverter))]
        public float ArchDepth
        {
            get { return archDepth; }
            set
            {
                if ((value >= 0) & (value != archDepth))
                {
                    archDepth = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ArchDepth"));
                }
            }
        }

        float archTopColorDepth = 0.1F;
        [Browsable(false)]
        [TypeConverter(typeof(ArchDepthConverter))]
        public float ArchTopColorDepth
        {
            get { return archTopColorDepth; }
            set
            {
                if ((value >= 0) & (value != archTopColorDepth))
                {
                    archTopColorDepth = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ArchTopColorDepth"));
                }
            }
        }

        bool archDepthsIndependent = false;
        [Browsable(false)]
        public bool ArchDepthsIndependent
        {
            get { return extended3DArch ? archDepthsIndependent : false; }
            set
            {
                if (value != archDepthsIndependent)
                {
                    archDepthsIndependent = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ArchDepthsIndependent"));
                }
            }
        }

        bool archActive = false;
        [Browsable(false)]
        public bool ArchActive
        {
            get { return archActive; }
            set
            {
                if (value != archActive)
                {
                    archActive = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ArchActive"));
                }
            }
        }

        bool archstopAtStrongLayer = false;
        [Browsable(false)]
        public bool ArchStopAtStrongLayer
        {
            get { return archstopAtStrongLayer; }
            set
            {
                if (value != archstopAtStrongLayer)
                {
                    archstopAtStrongLayer = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ArchStopAtStrongLayer"));
                }
            }

        }

        #endregion

        #region Not browsable
        /// <summary>
        /// Gets or sets the licencing object.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public Module Lic
        {
            get
            {
                if (lic == null)
                    lic = new Module();

                return lic;
            }
            set { lic = value; }
        }

        /// <summary>
        /// Gets or sets the global number format used in the application.
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public NumberFormatInfo NFI
        {
            get { return nfi; }
            set { nfi = value; }
        }

        /// <summary>
        /// Gets the basic transform object of the application configured for forward operation.
        /// </summary>
        [Browsable(false), XmlIgnore]
        public Transform ForwardTransform
        {
            get
            {
                transfrm.SrcDatum = srcDatum;
                transfrm.DstDatum = dstDatum;
                transfrm.SrcEllipsoid = srcEllipsoid;
                transfrm.DstEllipsoid = dstEllipsoid;

                return transfrm;
            }
        }

        /// <summary>
        /// Gets the basic transform object of the application configured for invers operation.
        /// </summary>
        [Browsable(false), XmlIgnore]
        public Transform InversTransform
        {
            get
            {
                transfrm.SrcDatum = dstDatum;
                transfrm.DstDatum = srcDatum;
                transfrm.SrcEllipsoid = dstEllipsoid;
                transfrm.DstEllipsoid = srcEllipsoid;

                return transfrm;
            }
        }

        /// <summary>
        /// Gets or sets the color scheme of the application.        
        /// </summary>        

        [XmlElement("colors")]
        [Category("General"), DisplayName("Color Settings"), ReadOnly(true)]
        public ColorScheme CS
        {
            get { return cs; }
            set
            {
                cs = value;
                cs.PropertyChanged += new PropertyChangedEventHandler(cs_PropertyChanged);
                NotifyPropertyChanged("CS");
            }
        }

        ///// <summary>
        ///// Gets or sets the weights for volume calculation.
        ///// </summary>
        //[Browsable(false)]       
        //public VolumeWeights VW
        //{
        //    get { return vw; }
        //    set { vw = value; NotifyPropertyChanged("prop"); }
        //}
        #endregion
        #endregion

        #region Constructor
        public GlobalSettings()
        {
            transfrm.SrcDatum = srcDatum;
            transfrm.DstDatum = dstDatum;
            transfrm.SrcEllipsoid = srcEllipsoid;
            transfrm.DstEllipsoid = dstEllipsoid;
            transfrm.Param = -1;

            SECL = new BindingList<SonarEntryConfig>();
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region PropertyChanged
        [XmlIgnore, Browsable(false)]
        public Dictionary<string, string> PropertyCategory = new Dictionary<string, string>();

        private bool noNotifyPropertyChanged = false;

        public void BeginChanging()
        {
            noNotifyPropertyChanged = true;
        }

        public void EndChanging()
        {
            noNotifyPropertyChanged = false;
            NotifyPropertyChanged("All");
        }


        private void NotifyPropertyChanged(String info)
        {
            if (noNotifyPropertyChanged) return;

            if (PropertyCategory.Count == 0)
            {
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor pd in pdc)
                {
                    PropertyCategory.Add(pd.Name, pd.Category);
                }
                pdc = TypeDescriptor.GetProperties(this.CS);
                foreach (PropertyDescriptor pd in pdc)
                {
                    PropertyCategory.Add("CS." + pd.Name, pd.Category);
                }
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        void secl_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                string Item = e.PropertyDescriptor.Name;
                NotifyPropertyChanged("SECL[" + e.NewIndex + "]." + Item);
            }
        }

        void cs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("CS." + e.PropertyName);
        }
        #endregion
    }

    /// <summary>
    /// This class encapsulates all settings of the sonOmeter application.
    /// </summary>
    public class GSC
    {
        #region Properties
        private static GlobalSettings settings = new GlobalSettings();
        public static GlobalSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value != settings)
                {
                    settings = value;
                    settings.ExportFunction = ReadExportFunction();
                    settings.PropertyChanged += new PropertyChangedEventHandler(sets_PropertyChanged);
                    sets_PropertyChanged(null, new PropertyChangedEventArgs("Settings"));
                }
            }
        }
        #endregion

        #region Events
        static void sets_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, e);
        }


        #region INotifyPropertyChanged Members

        public static event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region XML r/w
        public static void ReadXmlFile(string filename)
        {
            try
            {
                var sr = File.OpenRead(filename);
                var xs = new XmlSerializer(typeof(GlobalSettings));
                GlobalSettings set = (GlobalSettings)xs.Deserialize(sr);
                sr.Close();
                Settings = set;
            }
            catch
            {
            }
        }

        public static void ReadXmlString(string xml)
        {
            try
            {
                var sr = new StringReader(xml);
                var xs = new XmlSerializer(typeof(GlobalSettings));
                GlobalSettings set = (GlobalSettings)xs.Deserialize(sr);
                sr.Close();
                Settings = set;
            }
            catch
            {
            }
        }

        private static string[] ReadExportFunction()
        {
            string[] st = { "" };
            string fileNameExport = AppDomain.CurrentDomain.BaseDirectory + "ExportFunction.cs";
            try
            {
                if (!File.Exists(fileNameExport))
                    throw new Exception();
                StreamReader sr = new StreamReader(fileNameExport);

                st = sr.ReadToEnd().Split('\n');

                sr.Close();
            }
            catch
            {
                UKLib.Debug.DebugClass.SendDebugLine(Settings, UKLib.Debug.DebugLevel.Red, "No default config file found.");
            }

            return st;
        }

        /// <summary>
        /// Reads the configuration from an XML reader.
        /// </summary>
        /// <param name="reader">A properly configured XML reader object.</param>
        public static void ReadOldXml(string xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            reader.WhitespaceHandling = WhitespaceHandling.None;

            #region OldFile
            settings.BeginChanging();
            try
            {
                while (!reader.EOF)
                {
                    reader.Read();

                    try
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "general":
                                    Settings.AutoSave = XmlReadConvert.Read(reader, "autosave", 15);
                                    Settings.MinSatellites = XmlReadConvert.Read(reader, "minsat", 4);
                                    Settings.MinQuality = XmlReadConvert.Read(reader, "minqual", 3);
                                    Settings.MinQualityMeter = XmlReadConvert.Read(reader, "minqualm", 0.05f);
                                    Settings.MaxPosDistance = XmlReadConvert.Read(reader, "maxposdist", 50);
                                    Settings.ScanPosStrings = new string[1];
                                    Settings.ScanPosStrings[0] = (XmlReadConvert.Read(reader, "scanposstring", "$GPLLK"));
                                    Settings.ScanPosType = (PosType)XmlReadConvert.Read(reader, "scanpostype", (int)PosType.NMEA);
                                    Settings.InterpolationThreshold = XmlReadConvert.Read(reader, "interpolth", 500000.0);
                                    Settings.InterpolationAltitudeThreshold = XmlReadConvert.Read(reader, "interpolalth", 100.0);
                                    break;
                                case "recordview":
                                    Settings.DepthTop = XmlReadConvert.Read(reader, "top", 0.0);
                                    Settings.DepthBottom = XmlReadConvert.Read(reader, "bottom", -100.0);
                                    Settings.PitchH = XmlReadConvert.Read(reader, "hpitch", 1.0);
                                    Settings.PitchV = XmlReadConvert.Read(reader, "vpitch", 0.0);
                                    break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            #endregion

            reader.Close();
            settings.EndChanging();
        }

        /// <summary>
        /// Writes the configuration to an XML file.
        /// </summary>
        /// <param name="strFile">Path and name of the XML file.</param>
        public static void WriteXml(string strFile)
        {
            TextWriter tw = null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GlobalSettings));
                tw = File.CreateText(strFile);
                xs.Serialize(tw, Settings);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (tw != null) tw.Close();
        }

        public static string WriteXmlGetString()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(GlobalSettings));
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, Settings);

                string s = sw.ToString();
                s = s.Substring(s.IndexOf("<GlobalSettings"));
                s += "\r\n";
                s = s.Replace("\r\n", "\r\n  ");
                s = "\r\n    " + s;
                return s;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// Writes the configuration to an XML writer.
        /// </summary>
        /// <param name="writer">A properly configured XML writer object.</param>
        public static void WriteXml(XmlTextWriter writer)
        {
            try
            {
                writer.WriteRaw(WriteXmlGetString());
            }
            catch
            {
            }
        }
        #endregion
    }
}