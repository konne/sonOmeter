using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using sonOmeter.Classes.Sonar2D;
using UKLib.Arrays;
using UKLib.MathEx;
using UKLib.Survey.Math;
using UKLib.Xml;
using UKLib;
using UKLib.Debug;

namespace sonOmeter.Classes
{
    public struct ArchBounds
    {
        public float MaxHF;
        public float MinHF;
        public float MaxNF;
        public float MinNF;
    }

    /// <summary>
    /// Class holding record data.
    /// </summary>
    public class SonarRecord : object
    {
        #region Variables
        protected DateTime timeStart = DateTime.Now;
        protected DateTime timeEnd = DateTime.Now;

        protected bool isProfile = false;
        protected bool recording = false;
        protected bool interpolated = false;
        protected bool showManualPoints = true;

        protected int autoSaveIndexSL = 0;
        protected int autoSaveIndexMP = 0;

        protected double lastAltitude = Double.NaN;
        protected float maxDepth = Single.NaN;
        protected double alMax = Double.MinValue;
        protected double alMin = Double.MaxValue;
        protected double depthRes = 0.1;

        protected ArchBounds depthFieldBounds = new ArchBounds() { MaxHF = (float)GSC.Settings.DepthTop, MaxNF = (float)GSC.Settings.DepthTop, MinHF = (float)GSC.Settings.DepthBottom, MinNF = (float)GSC.Settings.DepthBottom };

        protected List<SonarLine> sonarLines = new List<SonarLine>();
        protected List<SonarDevice> sonarDevices = new List<SonarDevice>();
        protected IDList<ManualPoint> manualPoints = new IDList<ManualPoint>();

        protected string desc = "";

        protected RectangleD coordLimits = new RectangleD();

        protected TreeNode nodeRef = null;

        protected string linkedVideoFile = "";

        [Browsable(false)]
        public Guid RawDataGuid { get; set; }

        // Temporary variables, not to be saved in the XML stream...
        protected Coordinate startCoord = Coordinate.Empty;
        protected Coordinate startCoord2 = Coordinate.Empty;
        protected Coordinate endCoord = Coordinate.Empty;
        protected Coordinate endCoord2 = Coordinate.Empty;

        protected BackgroundWorker<bool, bool> bwArch = null;
        protected bool bwArchRestart = false;
        #endregion

        #region Properties
        [Browsable(false)]
        public bool ApplyingArchAndVolume
        {
            get { return (bwArch == null) ? false : bwArch.IsBusy; }
        }

        [Browsable(false)]
        public Coordinate StartCoord
        {
            get { return startCoord; }
        }

        [Browsable(false)]
        public Coordinate StartCoord2
        {
            get { return startCoord2; }
        }

        [Browsable(false)]
        public Coordinate EndCoord
        {
            get { return endCoord; }
        }

        [Browsable(false)]
        public Coordinate EndCoord2
        {
            get { return endCoord2; }
        }

        [Browsable(false)]
        public TreeNode NodeRef
        {
            get { return nodeRef; }
            set { nodeRef = value; }
        }

        [Browsable(false)]
        public bool IsProfile
        {
            get { return isProfile; }
            set { isProfile = value; }
        }

        [Description("Linked VideoFile."), Category("Camera")]
        public virtual string LinkedVideoFile
        {
            get { return linkedVideoFile; }
            set { linkedVideoFile = value; }
        }

        /// <summary>
        /// Gets the minimum altitude of all sonar lines.
        /// </summary>
        [Browsable(false)]
        public double ALMin
        {
            get { return alMin; }
        }

        /// <summary>
        /// Gets the maximum altitude of all sonar lines.
        /// </summary>
        [Browsable(false)]
        public double ALMax
        {
            get { return alMax; }
        }

        /// <summary>
        /// Determines if the project is visible in the trace window.
        /// </summary>
        [Browsable(false)]
        public bool ShowInTrace
        {
            get
            {
                for (int i = 0; i < sonarDevices.Count; i++)
                    if (sonarDevices[i].ShowInTrace)
                        return true;

                return false;
            }
            set
            {
                for (int i = 0; i < sonarDevices.Count; i++)
                    sonarDevices[i].ShowInTrace = value;
            }
        }

        /// <summary>
        /// Gets the flag for the interpolation state of the record.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the flag for the interpolation state of the record."), Category("Record")]
        public virtual bool Interpolated
        {
            get { return interpolated; }
            set { interpolated = value; }
        }

        /// <summary>
        /// Gets the array of devices.
        /// </summary>
        [Browsable(false)]
        public List<SonarDevice> Devices
        {
            get { return sonarDevices; }
        }

        /// <summary>
        /// Gets the array of manual points.
        /// </summary>
        [Browsable(false)]
        public IDList<ManualPoint> ManualPoints
        {
            get { return manualPoints; }
        }

        /// <summary>
        /// Gets or sets the recording state of the record.
        /// </summary>
        [Browsable(false)]
        public bool Recording
        {
            get { return recording; }
            set
            {
                recording = value;
                GlobalNotifier.Invoke(this, recording, GlobalNotifier.MsgTypes.RecordingChanged);
            }
        }

        /// <summary>
        /// Gets or sets the autosave position in the sonar line array.
        /// </summary>
        [Browsable(false)]
        public int AutoSaveIndex
        {
            get { return autoSaveIndexSL; }
            set { autoSaveIndexSL = value; }
        }
        [Browsable(false)]
        public int AutoSaveIndexSL
        {
            get { return autoSaveIndexSL; }
            set { autoSaveIndexSL = value; }
        }
        [Browsable(false)]
        public int AutoSaveIndexMP
        {
            get { return autoSaveIndexMP; }
            set { autoSaveIndexMP = value; }
        }

        [Browsable(false)]
        public bool ShowManualPoints
        {
            get { return showManualPoints; }
            set { showManualPoints = value; }
        }

        /// <summary>
        /// The bounding rectangle of the sonar data.
        /// </summary>
        [Description("The bounding rectangle of the sonar data."), Category("Record")]
        public RectangleD CoordLimits
        {
            get { return coordLimits; }
        }

        /// <summary>
        /// The start time of the record.
        /// </summary>
        [Description("The start time of the record."), Category("Record")]
        public virtual DateTime TimeStart
        {
            get { return timeStart; }
            set { timeStart = value; }
        }

        /// <summary>
        /// The end time of the record.
        /// </summary>
        [Description("The end time of the record."), Category("Record")]
        public virtual DateTime TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        [Browsable(false)]
        public long Duration
        {
            get { return timeEnd.Ticks - timeStart.Ticks; }
        }

        /// <summary>
        /// The record description.
        /// </summary>
        [Description("The record description."), Category("Record")]
        public string Description
        {
            get { return desc; }
            set
            {
                desc = value;
                if (nodeRef != null)
                    nodeRef.Text = ToString();
            }
        }

        /// <summary>
        /// The total number of devices in the record.
        /// </summary>
        [Browsable(false)]
        public int DeviceCount
        {
            get { return sonarDevices.Count; }
        }

        /// <summary>
        /// The greatest depth of the record.
        /// </summary>
        [Browsable(false)]
        public double MaxDepth
        {
            get { return maxDepth; }
        }

        [Browsable(true), Category("Record"), DisplayName("Z depth resolution (m)"), DefaultValue(0.1)]
        public double DepthRes
        {
            get { return depthRes; }
            set { depthRes = value; }
        }

        [Browsable(false)]
        public ArchBounds DepthFieldBounds
        {
            get { return depthFieldBounds; }
            set { depthFieldBounds = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public SonarRecord()
        {
            sonarLines.Clear();
            sonarDevices.Clear();

            RawDataGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Constructor that reads the record data from an XML reader.
        /// </summary>
        /// <param name="readerXML">The XML file reader.</param>
        public SonarRecord(XmlTextReader readerXML)
        {
            string s = "";

            if ((s = readerXML.GetAttribute("btime")) != null)
                this.TimeStart = DateTime.Parse(s);
            
            if ((s = readerXML.GetAttribute("etime")) != null)
                this.TimeEnd = DateTime.Parse(s);
            
            if ((s = readerXML.GetAttribute("date")) != null)
            {
                DateTime date = DateTime.Parse(s);
                this.TimeStart = date.Add(this.TimeStart.TimeOfDay);
                this.TimeEnd = date.Add(this.TimeEnd.TimeOfDay);
            }
            
            if ((s = readerXML.GetAttribute("desc")) != null)
                this.Description = s;
            
            if ((s = readerXML.GetAttribute("interpolated")) != null)
                this.Interpolated = bool.Parse(s);
            
            if ((s = readerXML.GetAttribute("profile")) != null)
                this.IsProfile = bool.Parse(s);

            if ((s = readerXML.GetAttribute("videofile")) != null)
                this.LinkedVideoFile = s;

            if ((s = readerXML.GetAttribute("rawdataguid")) != null)
                this.RawDataGuid = new Guid(s);

            if ((s = readerXML.GetAttribute("depthRes")) != null)
            {

                // TODO: Bad Fix of writing DepthRes without correct numberformatinfo
                this.DepthRes = 0.1;
                double d=0.1;
                if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,out d))
                    this.DepthRes = d;

                if (double.TryParse(s, out d))
                    this.DepthRes = d;                                        
            }
        }

        /// <summary>
        /// Releases allocated memory.
        /// </summary>
        public virtual void Dispose()
        {
            sonarLines.Clear();
            sonarDevices.Clear();
        }
        #endregion

        #region Data r/w
        #region Read XML
        /// <summary>
        /// Reads the device list from an XML string.
        /// </summary>
        /// <param name="xml">The source XML string.</param>
        public void ReadDeviceList(string xml)
        {
            SonarDevice dev;

            // Setup reader.
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            reader.WhitespaceHandling = WhitespaceHandling.None;

            // parse the line
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                switch (reader.Name)
                {
                    case "devicelist":
                        // Gather general properties.
                        break;

                    case "sondevice":
                        // Add new sonar device.
                        string s = reader.GetAttribute("desc");
                       
                        dev = new SonarDevice(
                            XmlReadConvert.Read(reader, "sonid", 0),
                            s,
                            XmlReadConvert.Read(reader, "dx", 0.0),
                            XmlReadConvert.Read(reader, "dy", 0.0),
                            XmlReadConvert.Read(reader, "dz", 0.0),
                            XmlReadConvert.Read(reader, "dp", 0.0),
                            XmlReadConvert.Read(reader, "dr", 0.0),
                            XmlReadConvert.Read(reader, "show", true),
                            XmlReadConvert.Read(reader, "LicencedFor", ""),
                            XmlReadConvert.Read(reader, "LicenceNr", ""),
                            XmlReadConvert.Read(reader, "HF", true),
                            XmlReadConvert.Read(reader, "NF", true),
                            XmlReadConvert.Read(reader, "SonicSpeed", 0),
                            XmlReadConvert.Read(reader, "Depth", 0.0),
                            XmlReadConvert.Read(reader, "GPSmask", ""));

                        sonarDevices.Add(dev);
                        break;
                }
            }

            GlobalNotifier.Invoke(this, sonarDevices, GlobalNotifier.MsgTypes.NewDeviceList);
        } 
        #endregion

        #region Write XML
        protected virtual void WriteHeader(XmlTextWriter writer)
        {
            try
            {
                // Start record node.
                writer.WriteStartElement("record");

                // Write attributes.
                writer.WriteAttributeString("btime", timeStart.ToLongTimeString());
                writer.WriteAttributeString("etime", timeEnd.ToLongTimeString());
                writer.WriteAttributeString("date", timeStart.ToShortDateString());
                writer.WriteAttributeString("desc", desc);
                writer.WriteAttributeString("profile", isProfile.ToString());
                writer.WriteAttributeString("interpolated", interpolated.ToString());
                writer.WriteAttributeString("videofile", linkedVideoFile);
                writer.WriteAttributeString("rawdataguid", RawDataGuid.ToString());
                writer.WriteAttributeString("depthRes", DepthRes.ToString());

                // Start device list node.
                writer.WriteStartElement("devicelist");
                // Write each device.
                foreach (SonarDevice dev in sonarDevices)
                {
                    dev.Write(writer);
                }
                // End device list node.
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarRecord.WriteHeader: " + e.Message);
            }
        }

        protected void WriteFooter(XmlTextWriter writer)
        {
            try
            {
                // End record node.
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarRecord.WriteFooter: " + e.Message);
            }
        }

        private void Write(XmlTextWriter writerXML, BinaryWriter writerBin, ref int startSL, ref int startMP, bool binary)
        {
            int i = 0;

            // Get line count.
            int countSL = sonarLines.Count;
            int countMP = manualPoints.Count;

            if (countSL + countMP == 0)
                return;

            // Write header.
            if (startSL + startMP == 0)
                WriteHeader(writerXML);

            // Prepare the time interpolation.
            DateTime time = timeStart;
            long timeDiv = this.Duration;

            if (countSL > 0)
                timeDiv /= countSL;

            // Write each sonar line.
            for (i = startSL; i < countSL; i++)
            {
                sonarLines[i].Write(writerXML, writerBin, time, binary);
                time = time.AddTicks(timeDiv);
            }

            // Write each manual point.
            for (i = startMP; i < countMP; i++)
            {
                manualPoints[i].WriteXml(writerXML);
            }

            // Write footer.
            WriteFooter(writerXML);

            startSL = countSL;
            startMP = countMP;
        }

        public virtual void Write(XmlTextWriter writerXML, BinaryWriter writerBin, bool binary)
        {
            try
            {
                int startSL = 0;
                int startMP = 0;

                Write(writerXML, writerBin, ref startSL, ref startMP, binary);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarRecord.Write: " + e.Message);
            }
        }

        public void WriteAutoSave(XmlTextWriter writerXML, BinaryWriter writerBin, bool binary)
        {
            try
            {
                Write(writerXML, writerBin, ref autoSaveIndexSL, ref autoSaveIndexMP, binary);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarRecord.WriteAutoSave: " + e.Message);
            }
        } 
        #endregion
        #endregion

        #region Arch
        public virtual void ApplyArchAndVolume()
        {
            ApplyArchAndVolume(false);
        }

        public void ApplyArchAndVolume(bool tryKeepAppend)
        {
            if (bwArch == null)
            {
                bwArch = new BackgroundWorker<bool, bool>();
                bwArch.WorkerSupportsCancellation = true;
                bwArch.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<bool>>(OnBwArchCompleted);
                bwArch.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, bool>>(OnBwArchDoWork);
            }            
            if (!bwArch.IsBusy)
            {
                bwArch.RunWorkerAsync(tryKeepAppend);
            }
            else
            {
                bwArchRestart = true;
                bwArch.CancelAsync();
            }
        }

        public event EventHandler ArchCompleted;

        protected virtual void OnBwArchCompleted(object sender, RunWorkerCompletedEventArgs<bool> e)
        {
            try
            {
                if (!e.Cancelled)
                {
                    if (ArchCompleted != null)
                        ArchCompleted(this, EventArgs.Empty);
                }

                if (bwArchRestart)
                {
                    bwArchRestart = false;
                    ApplyArchAndVolume(e.Result);
                }
            }
            catch (Exception ex) { 
                DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); 
            }
        }

        protected virtual ArchBounds ApplyArchAndVolumeToDevice(SonarDevice dev)
        {
            return dev.ApplyArchAndVolume(null);
        }

        protected virtual void OnBwArchDoWork(object sender, DoWorkEventArgs<bool, bool> e)
        {
            try
            {
                ArchBounds recBounds = new ArchBounds() { MaxHF = (float)GSC.Settings.DepthBottom, MinHF = (float)GSC.Settings.DepthTop, MaxNF = (float)GSC.Settings.DepthBottom, MinNF = (float)GSC.Settings.DepthTop };
                ArchBounds devBounds;

                int maxDev = sonarDevices.Count;

                for (int i = 0; i < maxDev; i++)
                {
                    SonarDevice dev = sonarDevices[i];

                    devBounds = ApplyArchAndVolumeToDevice(dev);

                    if (recBounds.MaxHF < devBounds.MaxHF)
                        recBounds.MaxHF = devBounds.MaxHF;
                    if (recBounds.MinHF > devBounds.MinHF)
                        recBounds.MinHF = devBounds.MinHF;

                    if (recBounds.MaxNF < devBounds.MaxNF)
                        recBounds.MaxNF = devBounds.MaxNF;
                    if (recBounds.MinNF > devBounds.MinNF)
                        recBounds.MinNF = devBounds.MinNF;
                }

                depthFieldBounds = recBounds;

                e.Result = e.Argument;
            }
            catch (Exception ex) { 
                DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); 
            }
        }
        #endregion

        #region List handling
        public void CorrectLineTimestamps()
        {
            foreach (var dev in this.Devices)
                dev.CorrectLineTimestamps(timeStart, timeEnd);
        }

        /// <summary>
        /// Add a list of PKT data items to the manual points of this record.
        /// </summary>
        /// <param name="list">The PKT data list.</param>
        public void AddPKTList(List<PKTdata> list)
        {
            foreach (PKTdata item in list)
            {
                ManualPoint point = new ManualPoint(item.Coord, 0.0F, item.Type, "PKT");
                point.ID = item.ID;
                RefreshCoordLimits(item.Coord, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);
                manualPoints.Add(point);
            }
        }
        
        /// <summary>
        /// Delete one single sonardevice.
        /// </summary>
        /// <param name="sonID">Sonard ID</param>
        public void DeleteDevice(int sonID)
        {
            if (sonID != 0)
            {
                for (int i = 0; i < sonarLines.Count; i++)
                {
                    if (sonarLines[i].SonID == sonID)
                    {
                        sonarLines.RemoveAt(i);
                        i--;
                    }
                }
                SonarDevice dev = Device(sonID);
                dev.SonarLines.Clear();
                Devices.Remove(dev);
                dev = null;
            }
        }

        /// <summary>
        /// Returns the sonar data.
        /// </summary>
        /// <returns>Sonar data.</returns>
        public List<SonarLine> SonarLines()
        {
            return sonarLines;
        }

        /// <summary>
        /// Returns a specific device.
        /// </summary>
        /// <param name="sonID">The device ID.</param>
        /// <returns>The device with the given ID.</returns>
        public SonarDevice Device(int sonID)
        {
            foreach (SonarDevice device in sonarDevices)
            {
                if (device.SonID == sonID)
                    return device;
            }

            return null;
        }

        /// <summary>
        /// Gets the index of the given device.
        /// </summary>
        /// <param name="device">The sonar device.</param>
        /// <returns>The index of the device.</returns>
        public int IndexOf(SonarDevice device)
        {
            return sonarDevices.IndexOf(device);
        }

        /// <summary>
        /// Gets the index of the given line.
        /// </summary>
        /// <param name="device">The sonar line.</param>
        /// <returns>The index of the line.</returns>
        public int IndexOf(SonarLine line)
        {
            return sonarLines.IndexOf(line);
        }

        /// <summary>
        /// Returns a specific sonar line.
        /// </summary>
        /// <param name="index">The index of the line.</param>
        /// <returns>The sonar line with the given index.</returns>
        public SonarLine SonarLine(int index)
        {
            if ((index > -1) && (index < sonarLines.Count))
                return (SonarLine)sonarLines[index];
            else
                return null;
        }

        /// <summary>
        /// Adds a new line to the list and recalculates the bounding rectangle.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <param name="scanPos">Scan for positions in the sonar line.</param>
        /// <returns>The sonar device.</returns>
        public virtual object AddSonarLine(SonarLine line, bool scanPos)
        {
            if (isProfile | line.IsProfile)
            {
                line.IsProfile = true;
                isProfile = true;
            }

            if (scanPos)
                line.ScanPositions(this);
            else
                RefreshCoordLimits(line.CoordRvHv, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);
            
            UpdateMaxDepth(line);

            // Add the line to the own list.
            sonarLines.Add(line);

            // Add the line to the corresponding device.
            SonarDevice dev = null;

            for (int j = 0; j < sonarDevices.Count; j++)
                if (sonarDevices[j].SonID == line.SonID)
                {
                    dev = sonarDevices[j];
                    break;
                }

            if (dev == null)
                return null;

            if (dev.SonID != line.SonID)
            {
                int max = sonarDevices.Count;

                for (int i = 0; i < max; i++)
                {
                    dev = sonarDevices[i];
                    if (dev.SonID == line.SonID)
                        break;
                }
            }

            dev.AddLine(line);

            return dev;
        }

        protected void UpdateMaxDepth(SonarLine line)
        {
            // New depth record?
            float d = line.GetMaxDepth(true);

            if (!float.IsNaN(d))
                maxDepth = Math.Max(float.IsNaN(maxDepth) ? d : Math.Min(maxDepth, d), (float)GSC.Settings.DepthBottom);
            else
                maxDepth = float.IsNaN(maxDepth) ? (float)GSC.Settings.DepthBottom : Math.Max(maxDepth, (float)GSC.Settings.DepthBottom);
        }

        /// <summary>
        /// Adds a new manual point to the list.
        /// </summary>
        /// <param name="point">The manual point.</param>
        /// <returns>The index of the line.</returns>
        public virtual int AddManualPoint(ManualPoint point)
        {
            manualPoints.Add(point);
            
            RefreshCoordLimits(point.GetCoordRVHV(), GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);

            GlobalNotifier.Invoke(this, point, GlobalNotifier.MsgTypes.PlaceManualPoint);

            return manualPoints.IndexOf(point);
        }

        /// <summary>
        /// Inserts a new manual point to the list.
        /// </summary>
        /// <param name="point">The manual point.</param>
        /// <param name="index">The target index.</param>
        /// <returns>The index of the line.</returns>
        public int InsertManualPoint(ManualPoint point, int index)
        {
            manualPoints.Insert(index, point);

            return manualPoints.IndexOf(point);
        }

        public virtual void UpdateAllCoordinates()
        {
            if (isProfile || sonarDevices.Count == 0)
                return;

            Transform tr = GSC.Settings.ForwardTransform;
            RotD lastRotation = new RotD(double.NaN, double.NaN, double.NaN);
            RotD currRotation = new RotD(double.NaN, double.NaN, double.NaN);
            Coordinate currCoord = Coordinate.Empty;
            SonarLine line, lastLineWithCoord = null;
            SonarDevice device = sonarDevices[0];
            int count = sonarLines.Count;
            lastAltitude = Double.NaN;
            coordLimits = new RectangleD();
            
            for (int i = 0; i < count; i++)
            {
                line = sonarLines[i];

                line.CoordRvHv = line.CoordLaLo = Coordinate.Empty;
                line.SrcCoordType = CoordinateType.Empty;
                line.Rotation = new RotD(double.NaN, double.NaN, double.NaN);

                //if (line.SonID == 1 && line.time.ToLongTimeString() == "12:03:13")
                //    Console.WriteLine("");

                if (line.PosList.Count > 0)
                {
                    // Scan for position definitions in the line
                    line.ScanPositions(this);
                    
                    // Skip, if the line has no coordinate.
                    if (line.SrcCoordType != CoordinateType.Empty)
                    {
                        // Interpolate all lines in between this line and the previous one.
                        UpdateCoordinates(line, ref currRotation, ref currCoord);
                        
                        // If a previous line exists, update its coordinate now.
                        if (lastLineWithCoord != null)
                            device.UpdateCoordinateOfLine(lastLineWithCoord, null, lastRotation, tr);

                        // Set this line to the next "previous" line.
                        lastLineWithCoord = line;
                        lastRotation = currRotation;
                    }
                }
            }

            // The loop will always leave the last line with coordinates without being updated.
            // Update its coordinate now.
            if (lastLineWithCoord != null)
                device.UpdateCoordinateOfLine(lastLineWithCoord, null, lastRotation, tr);

            interpolated = true;
        }

        /// <summary>
        /// Updates the last coordinates.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <param name="currAngle">The current angle.</param>
        /// <param name="currCoord">The current coordinate.</param>
        /// <returns>Determines if the bounding rect changed.</returns>
        public bool UpdateCoordinates(SonarLine line, ref RotD currRotation, ref Coordinate currCoord)
        {
            #region Variables
            SonarDevice device;
            Coordinate startCoord = Coordinate.Empty; ;
            SonarLine line2 = null;
            Transform tr = GSC.Settings.ForwardTransform;
            DateTime time = DateTime.MinValue;
            RotD startRotation = new RotD(double.NaN, double.NaN, double.NaN);
            double dPitch;
            double dRoll;
            double dYaw;
            double dRV, dHV, dAL;
            int[] k = new int[sonarDevices.Count];
            int[] l = new int[sonarDevices.Count];
            int index = sonarLines.IndexOf(line);
            int i = 0, m = index, start, end, index0_i, index0_m;
            #endregion

            if ((line.PosList.Count <= 0) | (line.SonID != 0))
                return false;

            // No coordinate found - skip sonar line.
            if (line.SrcCoord.Type == CoordinateType.Empty)
                return false;

            device = sonarDevices[0];
            
            // Find last line with a coordinate.
            for (i = m - 1; i >= 0; i--)
            {
                line2 = sonarLines[i];

                // Check if a valid rotation was found.
                if (!double.IsNaN(line2.Rotation.Pitch))
                {
                    if (double.IsNaN(currRotation.Yaw))
                        currRotation = new RotD(line2.Rotation);
                    // This iteration goes backwards - therefore overwrite the start rotation each time a prior line carries rotation information.
                    startRotation = new RotD(line2.Rotation);
                }

                // Check if a coordinate was found.
                if ((line2.SrcCoordType != CoordinateType.Empty) & (line2.SonID == 0))
                {
                    startCoord = line2.CoordRvHv;
                    break;
                }
            }

            if (double.IsNaN(startRotation.Yaw))
                startRotation = new RotD(currRotation);

            // Found one?
            if (startCoord.Type != CoordinateType.Empty)
            {
                // Calculate coordinate delta.
                currCoord = line.CoordRvHv;

                dRV = currCoord.RV - startCoord.RV;
                dHV = currCoord.HV - startCoord.HV;
                dAL = currCoord.AL - startCoord.AL;

                // Calculate angle between coordinates.
                if (GSC.Settings.CompassVector)
                {
                    dYaw = line.Rotation.Yaw;
                    dPitch = line.Rotation.Pitch;
                    dRoll = line.Rotation.Roll;
                }
                else
                { 
                    // Calculate yaw from HV and RV deltas. No pitch and roll information available.
                    Point3D ptD = new Point3D(dRV, dHV, 0).NEDtoENU;
                    dYaw = Math.Atan2(ptD.Y, ptD.X) * 180 / Math.PI;
                    dPitch = dRoll = 0.0;
                }

                // Copy back 
                if (double.IsNaN(dYaw))
                    dYaw = currRotation.Yaw;
                if (double.IsNaN(dPitch))
                    dPitch = currRotation.Pitch;
                if (double.IsNaN(dRoll))
                    dRoll = currRotation.Roll;

                // Store current angle and velocity in record.
                currRotation = new RotD(dYaw, dPitch, dRoll);
                //currVelocity = Math.Sqrt(dRV * dRV + dHV * dHV);

                // Set up start and stop index for interpolation.
                start = i;
                end = m;

                index0_i = device.IndexOf(sonarLines[i]) + 1;
                index0_m = device.IndexOf(sonarLines[m]) - 1;

                List<SonarLine> lines;

                // Difference too big?
                if (index0_m - index0_i < GSC.Settings.MaxPosDistance)
                {
                    // No - Interpolate!
                    lines = device.UpdateCoordinates(index0_i, index0_m, startCoord.Point3D, currCoord.Point3D - startCoord.Point3D, startRotation, currRotation - startRotation, tr);

                    FillDeviceListsFromLineIndexDelta(k, l, start, end);

                    for (m = 1; m < sonarDevices.Count; m++)
                        lines.AddRange(sonarDevices[m].UpdateCoordinates(k[m], l[m], startCoord.Point3D, currCoord.Point3D - startCoord.Point3D, startRotation, currRotation - startRotation, tr));

                    GlobalNotifier.Invoke(this, lines, GlobalNotifier.MsgTypes.UpdateCoordinates);
                }
                else
                {
                    // Yes - set all lines in between to zero.
                    for (i = start + 1; i <= end - 1; i++)
                    {
                        sonarLines[i].CoordRvHv = Coordinate.Empty;
                        sonarLines[i].CoordLaLo = Coordinate.Empty;
                        sonarLines[i].SrcCoordType = CoordinateType.Empty;
                    }
                }
            }

            return true;
        }

        private void FillDeviceListsFromLineIndexDelta(int[] k, int[] l, int start, int end)
        {
            int i = 0, j = 0, m = 0, id = 0;
            SonarDevice device = null;
            SonarLine line;

            for (m = 0; m < sonarDevices.Count; m++)
                k[m] = l[m] = -1;

            for (i = start; i <= end; i++)
            {
                line = sonarLines[i];
                id = line.SonID;

                device = null;

                for (j = 0; j < sonarDevices.Count; j++)
                    if (sonarDevices[j].SonID == id)
                    {
                        device = sonarDevices[j];
                        break;
                    }

                if (device == null)
                    continue;

                if (k[j] == -1)
                    k[j] = device.IndexOf(line);
                else
                    l[j] = device.IndexOf(line);
            }
        }

        /// <summary>
        /// Creates child nodes for every device of the record.
        /// </summary>
        /// <param name="node">The root node.</param>
        public void BuildDeviceTree(TreeNode node)
        {
            try
            {
                TreeNode devNode;

                // Flush the old data and copy reference.
                node.Nodes.Clear();

                // Add each used sonar device.
                foreach (SonarDevice device in sonarDevices)
                {
                    devNode = new TreeNode("Sonar " + (isProfile ? "data" : (device.SonID + 1).ToString()));
                    devNode.Tag = device;
                    devNode.Checked = device.ShowInTrace;
                    node.Nodes.Add(devNode);
                }

                devNode = new TreeNode("Points");
                devNode.Tag = this;
                devNode.Checked = showManualPoints;
                node.Nodes.Add(devNode);

                // If there are any positions, add a position node.
                node.Nodes.Add(new TreeNode("Position"));
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarRecord.BuildDeviceTree: " + e.Message);
            }
        }

        /// <summary>
        /// Associates all sonar lines to their corresponding devices.
        /// </summary>
        public void RefreshDevices()
        {
            foreach (SonarDevice device in sonarDevices)
            {
                device.RefreshCutLines();
                device.CorrectLineTimestamps(timeStart, timeEnd);
            }
        }

        /// <summary>
        /// Recalculates the bounding box with a specific coordinate.
        /// </summary>
        /// <param name="coord">The coordinate to merge with the existing bounding box.</param>
        /// <param name="maxRVHV">The maximum difference between the coordinate and the limit bounds.</param>
        /// <param name="maxAL">The maximum difference betweeen the coordinate altitude and the limit bounds.</param>
        /// <returns>True, if the limits were updated.</returns>
        public virtual bool RefreshCoordLimits(Coordinate coord, double maxRVHV, double maxAL)
        {
            // Calculate new bounding box.
            if (coord.IsZero)
                return false;

            if (maxRVHV == 0)
                maxRVHV = double.MaxValue;
            if (maxAL == 0)
                maxAL = double.MaxValue;

            double rv = coord.RV;
            double hv = coord.HV;
            double al = coord.AL;

            if (coordLimits.IsZero)
                coordLimits = new RectangleD(rv, hv);
            else
            {
                if ((Math.Abs(rv - coordLimits.Right) > maxRVHV) |
                    (Math.Abs(rv - coordLimits.Left) > maxRVHV) |
                    (Math.Abs(hv - coordLimits.Top) > maxRVHV) |
                    (Math.Abs(hv - coordLimits.Bottom) > maxRVHV) |
                    ((maxAL > 0) && !Double.IsNaN(lastAltitude) && (Math.Abs(al - lastAltitude) > maxAL)))
                    return false;

                coordLimits.Merge(rv, hv);
                lastAltitude = al;

                if (al < alMin)
                    alMin = al;
                if (al > alMax)
                    alMax = al;
            }

            return true;
        }

        /// <summary>
        /// Removes all LineData elements that are invisible due to cut lines.
        /// </summary>
        internal void DropInvisibleData()
        {
            SonarLine line;
            int count = sonarLines.Count;

            for (int i = 0; i < count; i++)
            {
                line = sonarLines[i];
                line.HF.DropInvisibleData();
                line.NF.DropInvisibleData();
            }
        }

        /// <summary>
        /// Find a suitable SonarLine to each marker coordinate and add it as a tag.
        /// </summary>
        /// <param name="buoyList">The buoy list.</param>
        internal void MatchMarkersToSonarLines(IDList<Sonar2DElement> buoyList)
        {
            Coordinate coord;
            SonarLine line;
            ManualPoint point;
            Buoy buoy;
            int count = sonarLines.Count;
            int buoys = buoyList.Count;
            int points = manualPoints.Count;
            int i, j;

            for (i = 0; i < count; i++)
            {
                line = sonarLines[i];
                coord = line.CoordRvHv;

                if (coord.Type != CoordinateType.TransverseMercator)
                    continue;

                for (j = 0; j < buoys; j++)
                {
                    buoy = buoyList[j] as Buoy;

                    if (!coord.IsSame(buoy.Coord))
                        continue;

                    buoy.Tag = line;
                }

                for (j = 0; j < points; j++)
                {
                    point = manualPoints[j];

                    if (!coord.IsSame(point.Coord))
                        continue;

                    point.Tag = line;
                }
            }
        }

        internal void RecalcVolume()
        {
            int count = sonarLines.Count;

            for (int i = 0; i < count; i++)
            {
                SonarLine line = sonarLines[i];

                try
                {
                    line.HF.GetVolume(true);
                    line.NF.GetVolume(true);
                }
                catch
                {
                    // Do nothing.
                }
            }
        }

        /// <summary>
        /// Rasterizes the surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="gridX">The X grid.</param>
        /// <param name="gridY">The Y grid.</param>
        public void RasterizeSurface(object[,] surface, int xMin, int xMax, int yMin, int yMax, double gridX, double gridY)
        {
            foreach (SonarDevice dev in sonarDevices)
                if (dev.ShowInTrace)
                    dev.RasterizeSurface(surface, xMin, xMax, yMin, yMax, gridX, gridY);
        }

        /// <summary>
        /// Gets the lines inside the rectangle.
        /// </summary>
        /// <param name="rc">The rectangle.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        internal void GetRectangleLines(RectangleD rc, List<SonarLine> listSL, List<ManualPoint> listMP)
        {
            foreach (SonarDevice dev in sonarDevices)
                if (dev.ShowInTrace)
                    dev.GetRectangleLines(rc, listSL, listMP);

            int i = 0;
            int max = manualPoints.Count;
            ManualPoint point;

            for (i = 0; i < max; i++)
            {
                point = manualPoints[i];

                if (rc.Contains(point.GetPointRVHV()))
                    listMP.Add(point);
            }
        }

        /// <summary>
        /// Gets the lines inside the corridor.
        /// </summary>
        /// <param name="conn">The BuoyConnection.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        /// <param name="normalizeSL">Use this flag to normalize the SonarLine coordinates to the connection.</param>
        /// <param name="normalizeMP">Use this flag to normalize the ManualPoint coordinates to the connection.</param>
        /// <param name="linkObjects">This flag controls the linking of the SonarLine and ManualPoint objects instead of creating new ones.</param>
        internal void GetCorridorLines(BuoyConnection conn, List<SonarLine> listSL, List<ManualPoint> listMP, bool normalizeSL, bool normalizeMP, bool linkObjects)
        {
            foreach (SonarDevice dev in sonarDevices)
                if (dev.ShowInTrace)
                    dev.GetCorridorLines(conn, listSL, listMP, normalizeSL, linkObjects);

            int i = 0;
            int max = manualPoints.Count;
            ManualPoint point, newPoint;
            PointD pt, ptn;
            double al;

            for (i = 0; i < max; i++)
            {
                point = manualPoints[i];

                pt = point.GetPointRVHV();
                al = point.AL;

                // Filter points that are not inside the corridor.
                if (!conn.IsInCorridor(pt))
                    continue;

                // Add the object directly?
                if (linkObjects)
                {
                    listMP.Add(point);
                    continue;
                }

                // Normalize coordinate to the corridor connection and save it to the sonar line.
                if (normalizeMP)
                    ptn = conn.NormalizeToCorridor(pt);
                else
                    ptn = pt;

                newPoint = new ManualPoint(point);
                newPoint.RV = ptn.X;
                newPoint.HV = ptn.Y;
                listMP.Add(newPoint);
            }
        }

        public void RecalcRecordMarkers()
        {
            startCoord = Coordinate.Empty;
            startCoord2 = Coordinate.Empty;
            endCoord = Coordinate.Empty;
            endCoord2 = Coordinate.Empty;

            if (sonarDevices.Count < 1)
                return;

            SonarDevice dev = sonarDevices[0];
            Coordinate coord;
            int max = dev.SonarLines.Count;
            int i = 0;

            // Find start coordinate and direction.
            for (i = 0; i < max; i++)
            {
                coord = dev.SonarLines[i].CoordRvHv;

                if (coord.Type != CoordinateType.TransverseMercator)
                    continue;

                if (startCoord.IsEmpty)
                {
                    // No start found yet - take this one.
                    startCoord = coord;

                    // By adding one to the RV of the second coord, a default direction is implemented for the case of a lacking second coordinate.
                    startCoord2 = coord;
                    startCoord2.RV += 1.0;
                }
                else
                {
                    // Add direction to start now.
                    startCoord2 = coord;

                    // Stop searching.
                    break;
                }
            }

            // No end marker while recording.
            if (recording)
                return;

            // Find end coordinate and direction.
            for (i = max - 1; i >= 0; i--)
            {
                coord = dev.SonarLines[i].CoordRvHv;

                if (coord.Type != CoordinateType.TransverseMercator)
                    continue;

                if (endCoord.IsEmpty)
                {
                    // No end found yet - take this one.
                    endCoord = coord;

                    // By adding one to the HV of the second coord, a default direction is implemented for the case of a lacking second coordinate.
                    endCoord2 = coord;
                    endCoord2.HV += 1.0;
                }
                else
                {
                    // Add direction to end now.
                    endCoord = coord;

                    // Stop searching.
                    break;
                }
            }
        }
        #endregion

        #region Interpolation
        #region Interpolate lists to SonarLine and ManualPoint by BuoyConnection
        protected ManualPoint InterpolateListToPoint(List<ManualPoint> list, double distance, BuoyConnection conn)
        {
            // calculate the new coordinate
            PointD ptNew = conn.EndBuoy.GetPointRVHV() - conn.StartBuoy.GetPointRVHV();
            ptNew = conn.StartBuoy.GetPointRVHV() + ptNew.Normalize() * distance;

            return InterpolateListToPoint(list, ptNew, conn.Corridor);
        }

        protected SonarLine InterpolateListToLine(List<SonarLine> list, double distance, BuoyConnection conn, LineData.MergeMode mode, bool[] colors, double depthResolution, bool mergeWithAbsoluteDepths)
        {
            // calculate the new coordinate
            PointD ptNew = conn.EndBuoy.GetPointRVHV() - conn.StartBuoy.GetPointRVHV();
            ptNew = conn.StartBuoy.GetPointRVHV() + ptNew.Normalize() * distance;

            return InterpolateListToLine(list, ptNew, conn.Corridor, mode, colors, depthResolution, mergeWithAbsoluteDepths);
        }
        #endregion

        #region Interpolate lists to SonarLine and ManualPoint by center point of rectangle
        protected ManualPoint InterpolateListToPoint(List<ManualPoint> list, RectangleD rc)
        {
            return InterpolateListToPoint(list, rc.Center, rc.Diag);
        }

        protected SonarLine InterpolateListToLine(List<SonarLine> list, RectangleD rc, LineData.MergeMode mode, bool[] colors, double depthResolution, bool mergeWithAbsoluteDepths)
        {
            return InterpolateListToLine(list, rc.Center, rc.Diag, mode, colors, depthResolution, mergeWithAbsoluteDepths);
        }
        #endregion

        #region Interpolation basics
        protected ManualPoint InterpolateListToPoint(List<ManualPoint> list, PointD ptRef, double norm)
        {
            if (list == null)
                return null;

            int i, count = list.Count;

            if (count == 0)
                return null;

            ManualPoint point = new ManualPoint();
            ManualPoint pt;

            double depth = 0;
            double depthAL = 0;
            double div = 0;
            int type = list[0].Type;

            // retrieve the weighted depth
            for (i = 0; i < count; i++)
            {
                pt = list[i];

                // calculate weight
                double d = 1.0 - Math.Pow(ptRef.Distance(list[i].GetPointRVHV()) / norm, 2.0);

                // add the weighted depth
                div += d;
                depth += d * pt.Depth;
                depthAL += d * pt.AL;

                // compare type to common list type
                // if all list entries share the same type, set the new type to this value - otherwise revert to 0
                if (type != pt.Type)
                    type = 0;
            }

            if (div == 0)
                return null;

            // normalize depth
            depth /= div;
            depthAL /= div;

            point.Coord = new Coordinate(ptRef.X, ptRef.Y, depthAL, CoordinateType.TransverseMercator);
            point.Depth = (float)depth;
            point.Type = type;
            point.Description = "Interpolated manual point";

            return point;
        }

        /// <summary>
        /// Interpolates the line list to a single line.
        /// </summary>
        /// <param name="list">The line list. This list should be sorted with the highest line at the first position.</param>
        /// <param name="ptRef">The reference point.</param>
        /// <param name="norm">The normalization factor for distance weighting.</param>
        /// <param name="mode">The merge mode.</param>
        /// <param name="colors">The selected colors.</param>
        /// <param name="depthResolution">The depth resolution.</param>
        /// <param name="mergeWithAbsoluteDepths">A flag indicating whether the merge process uses absolute depths.</param>
        /// <returns></returns>
        protected SonarLine InterpolateListToLine(List<SonarLine> list, PointD ptRef, double norm, LineData.MergeMode mode, bool[] colors, double depthResolution, bool mergeWithAbsoluteDepths, Sonar3DIntWeighting weighting = Sonar3DIntWeighting.Quadratic, SonarLine prevInterpolatedLine = null, bool topDepthOnly = false)
        {
            SonarLine line = new SonarLine();
            SonarLine l;

            try
            {
                bool noWeight = double.IsNaN(norm);

                int i, count;
                int entryCount = (int)Math.Ceiling((GSC.Settings.DepthTop - GSC.Settings.DepthBottom) / depthResolution);

                double depthAL = 0;
                double divAL = 0;
                double alMax = 0.0;
                double d = 1.0, al;
                int[] colorCounterHF = null;
                int[] colorCounterNF = null;

                line.IsProfile = true;
                line.IsCut = true;
                line.SonID = 0;

                LineData dataHF = line.HF;
                LineData dataNF = line.NF;

                float? prevHighHF = null;
                float? prevHighNF = null;

                if (prevInterpolatedLine != null)
                {
                    if ((prevInterpolatedLine.HF.Entries != null) && (prevInterpolatedLine.HF.Entries.Length > 0))
                        prevHighHF = prevInterpolatedLine.HF.Entries[0].high;
                    if ((prevInterpolatedLine.NF.Entries != null) && (prevInterpolatedLine.NF.Entries.Length > 0))
                        prevHighNF = prevInterpolatedLine.NF.Entries[0].high;
                }

                if (list != null)
                {
                    count = list.Count;

                    // initialize the data entries of HF and NF fields
                    if (mode == LineData.MergeMode.Occurance)
                    {
                        colorCounterHF = new int[entryCount * 7];
                        colorCounterNF = new int[entryCount * 7];
                    }
                    else
                    {
                        dataHF.InitializeEntries(entryCount, (float)depthResolution);
                        dataNF.InitializeEntries(entryCount, (float)depthResolution);
                    }

                    // assume that the first line is the highest
                    if (count > 0)
                        alMax = list[0].CoordRvHv.AL;

                    // retrieve the weighted depth
                    for (i = 0; i < count; i++)
                    {
                        l = list[i];

                        if (l == null)
                            continue;

                        if (!noWeight)
                        {
                            // calculate weight
                            if (weighting == Sonar3DIntWeighting.Quadratic)
                                d = 1.0 - Math.Pow(ptRef.Distance(list[i].CoordRvHv.Point) / norm, 2.0);
                            else
                                d = 1.0 - ptRef.Distance(list[i].CoordRvHv.Point) / norm;

                            if (d < 0)
                                continue;
                        }

                        al = l.CoordRvHv.AL;
                        divAL += d;
                        depthAL += d * al;

                        if (mergeWithAbsoluteDepths)
                            al = alMax;

                        // merge data entries
                        dataHF.MergeData(l.HF.Entries, mode, colors, colorCounterHF, depthResolution, alMax - al, prevHighHF, topDepthOnly);
                        dataNF.MergeData(l.NF.Entries, mode, colors, colorCounterNF, depthResolution, alMax - al, prevHighNF, topDepthOnly);

                        // copy date
                        line.Time = l.Time.Date;
                    }

                    if (mode == LineData.MergeMode.Occurance)
                    {
                        dataHF.InitializeEntries(colorCounterHF, depthResolution);
                        dataNF.InitializeEntries(colorCounterNF, depthResolution);
                    }

                    // normalize depth
                    if (divAL != 0)
                        depthAL /= divAL;

                    // compress data entries
                    dataHF.CompressEntries(0);
                    dataNF.CompressEntries(0);
                }

                line.SetCoordinate(new Coordinate(ptRef.X, ptRef.Y, depthAL, CoordinateType.TransverseMercator));
            }
            catch (Exception ex) { }
            
            return line;
        }
        #endregion
        #endregion

        #region Export
        public void Export(StringCollection list, SonarExport cfg, int recordnr)
        {
            if (list == null)
                list = new StringCollection();
            foreach (SonarDevice device in sonarDevices)
            {
                Export(list, cfg, device.SonID, recordnr);
            }
            if (showManualPoints)
            {
                foreach (ManualPoint p in manualPoints)
                {
                    string s = cfg.ExportPoint(p, recordnr);
                    if (s != "")
                        list.Add(s);
                }
            }
        }

        public void Export(StringCollection list, SonarExport cfg, int lineID, int recordnr)
        {
            if (list == null)
                list = new StringCollection();

            List<SonarLine> sline = Device(lineID).SonarLines;

            if (Device(lineID).ShowInTrace)
            {
                if (cfg.ExpSettings.PitchH != 0 | cfg.ExpSettings.PitchV != 0)
                    ApplyScaleMarks(sline, cfg.ExpSettings.PitchH, cfg.ExpSettings.PitchV, cfg.ExpSettings.ExportWithCut);

                int max = sline.Count;
                SonarLine line;

                for (int i = 0; i < max; i++)
                {
                    line = sline[i];

                    line.ApplyArchAndVolume(cfg.ExpSettings.ExportWithCut);
                    line.HF.GetVolume(true);
                    line.NF.GetVolume(true);
                    DateTime time = timeStart;

                    if ((cfg.ExpSettings.PitchH == 0 & cfg.ExpSettings.PitchV == 0) | (line.IsMarked))
                    {
                        string s = "";

                        switch (cfg.ExpSettings.ExportType)
                        {
                            case SonarPanelType.HF:
                                s = cfg.ExportSonarLine(line, SonarPanelType.HF, recordnr, time);
                                if (s != "") list.Add(s);
                                break;

                            case SonarPanelType.NF:
                                s = cfg.ExportSonarLine(line, SonarPanelType.NF, recordnr, time);
                                if (s != "") list.Add(s);
                                break;

                            case SonarPanelType.Void:
                                s = cfg.ExportSonarLine(line, SonarPanelType.HF, recordnr, time);
                                if (s != "") list.Add(s);
                                s = cfg.ExportSonarLine(line, SonarPanelType.NF, recordnr, time);
                                if (s != "") list.Add(s);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Static functions
        static public void ApplyScaleMarks(SonarProject project, double pitchH, double pitchV, bool cut)
        {
            int nRec = project.RecordCount;

            for (int i = 0; i < nRec; i++)
            {
                SonarRecord record = project.Record(i);

                if (record == null)
                    continue;

                int nDev = record.DeviceCount;

                for (int j = 0; j < nDev; j++)
                {
                    SonarDevice device = record.Devices[j];

                    if (device == null)
                        continue;

                    ApplyScaleMarks(device.SonarLines, pitchH, pitchV, cut);
                }
            }
        }

        static public void ApplyScaleMarks(List<SonarLine> lines, double pitchH, double pitchV, bool cut)
        {
            bool first = true;
            double dist = 0;
            double dZ = 0;
            SonarLine lastLine = null;
            SonarLine line;
            int max = lines.Count;
            int i;

            for (i = 0; i < max; i++)
            {
                line = lines[i];

                if (lastLine == null)
                {
                    lastLine = line;
                    continue;
                }

                if ((line.CoordRvHv.Type != CoordinateType.Empty) & (lastLine.CoordRvHv.Type != CoordinateType.Empty))
                {
                    dist += Math.Sqrt(Math.Pow(line.CoordRvHv.HV - lastLine.CoordRvHv.HV, 2) + Math.Pow(line.CoordRvHv.RV - lastLine.CoordRvHv.RV, 2));

                    if (cut)
                    {
                        double deep1 = 0;
                        if (line.HF.Entries != null)
                            foreach (DataEntry entry in line.HF.Entries)
                            {
                                if (entry.visible)
                                {
                                    deep1 = entry.high;
                                    break;
                                }
                            }
                        double deep2 = 0;
                        if (lastLine.HF.Entries != null)
                            foreach (DataEntry entry in lastLine.HF.Entries)
                            {
                                if (entry.visible)
                                {
                                    deep2 = entry.high;
                                    break;
                                }
                            }
                        dZ += Math.Abs(deep1 - deep2);
                    }
                    else
                        dZ += Math.Abs(line.HF.Depth - lastLine.HF.Depth);

                    if (first)
                    {
                        line.IsMarked = true;
                        first = false;
                    }
                    else if ((dist >= pitchH) & (pitchH > 0))
                    {
                        line.IsMarked = true;
                        while (dist >= pitchH) dist -= pitchH;
                        dZ = 0;

                    }
                    else if ((Math.Abs(dZ) >= pitchV) && (pitchV > 0))
                    {
                        line.IsMarked = true;
                        while (dZ >= pitchV) dZ -= pitchV;
                        dist = 0;

                    }
                    else
                        line.IsMarked = false;
                }

                lastLine = line;
            }
        }
        #endregion

        #region ToString()
        public override string ToString()
        {
            return desc + " - (" + timeStart.ToString("T") + "-" + timeEnd.ToString("T") + ")";
        }
        #endregion
    }
}