using System;
using System.IO;
using System.Xml;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using UKLib.MathEx;
using UKLib.Survey.Math;
using UKLib.Net.Sockets;
using UKLib.Xml;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Specialized;
using UKLib.DXF;
using UKLib.Arrays;

namespace sonOmeter.Classes
{
    /// <summary>
    /// The project class.
    /// </summary>
    public class SonarProject : object
    {
        #region Variables
        List<SonarRecord> recordList = new List<SonarRecord>();
        List<SonarProfile> profileList = new List<SonarProfile>();
        List<Sonar3DRecord> recordList3D = new List<Sonar3DRecord>();
        IDList<Sonar2DElement> buoyList = new IDList<Sonar2DElement>();
        IDList<Sonar2DElement> buoyConnectionList = new IDList<Sonar2DElement>();

        BuoyConnection selectedCorridor = null;

        List<BlankLine> blankLines = new List<BlankLine>();
        BlankLine editBlankLine = null;

        string fileName = "";
        string name = "";

        TcpClientThread recordThread = new TcpClientThread();
        SimulationThread simulationThread = new SimulationThread();
        SonarRecord newRecord = null;

        // Last coordinate information.
        private Coordinate lastCoord = Coordinate.Empty;
        private RotD lastRotation = new RotD();

        private ISynchronizeInvoke synchronizingObject;

        private bool recording = false;
        private bool tracking = false;
        private bool simulation = false;

        private List<DXFFile> dxfFiles = new List<DXFFile>();

        public bool IsBinary { get; set; }

        private const string autoSaveFileName = "autosave.xml";
        private const string defaultFileName = "defaultview.xml";
        #endregion

        #region Properties
        [Browsable(false)]
        public BlankLine EditBlankLine
        {
            get { return editBlankLine; }
            set { editBlankLine = value; }
        }

        [Browsable(false)]
        public BuoyConnection SelectedCorridor
        {
            get { return selectedCorridor; }
            set { selectedCorridor = value; }
        }

        [Browsable(false)]
        public ISynchronizeInvoke SynchronizingObject
        {
            get { return synchronizingObject; }
            set
            {
                synchronizingObject = value;
                recordThread.SynchronizingObject = value;
            }
        }

        [Browsable(false)]
        public bool Recording
        {
            get { return recording; }
        }

        [Browsable(false)]
        public RotD LastRotation
        {
            get { return lastRotation; }
        }

        [Browsable(false)]
        public Coordinate LastCoord
        {
            get { return lastCoord; }
        }

        [Browsable(false)]
        public PointD LastLockOffset
        {
            get
            {
                Point3D pt = new Point3D(GSC.Settings.LockOffsetX, GSC.Settings.LockOffsetY, 0);

                QuaternionD qDev = new QuaternionD(lastRotation);
                pt = qDev.Rotate(pt);
                
                return pt.NEDtoENU.PointXY;
            }
        }

        [Browsable(false)]
        public bool Tracking
        {
            get { return tracking; }
        }

        [Browsable(false)]
        public List<BlankLine> BlankLines
        {
            get { return blankLines; }
            set { blankLines = value; }
        }

        [Description("The list of all buoys."), Category("Project")]
        [Editor(typeof(BuoyEditor), typeof(UITypeEditor))]
        public IDList<Sonar2DElement> BuoyList
        {
            get { return buoyList; }
            set { buoyList = value; }
        }

        [Description("The list of all buoy connections."), Category("Project")]
        [Editor(typeof(BuoyEditor), typeof(UITypeEditor))]
        public IDList<Sonar2DElement> BuoyConnectionList
        {
            get { return buoyConnectionList; }
            set { buoyConnectionList = value; }
        }

        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (GSC.Settings.PropertyCategory.ContainsKey(e.PropertyName))
            {
                string cat = GSC.Settings.PropertyCategory[e.PropertyName];
                if ((cat == "Transform") | (cat == "Interpolation"))
                {
                    foreach (SonarRecord record in recordList)
                        record.Interpolated = false;
                }

            }
        }

        /// <summary>
        /// Gets the number of records stored in the project.
        /// </summary>
        [Browsable(false)]
        public int RecordCount
        {
            get { return recordList.Count; }
        }

        /// <summary>
        /// Gets the number of profiles stored in the project.
        /// </summary>
        [Browsable(false)]
        public int ProfileCount
        {
            get { return profileList.Count; }
        }

        /// <summary>
        /// Gets the number of 3D records stored in the project.
        /// </summary>
        [Browsable(false)]
        public int Record3DCount
        {
            get { return recordList3D.Count; }
        }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        [Description("The project name."), Category("Project")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Browsable(false)]
        public bool NewFile
        {
            get { return fileName == ""; }
        }

        [Browsable(false)]
        public string FileName
        {
            get { return fileName; }
        }

        [Browsable(false)]
        public SonarRecord NewRecord
        {
            get { return newRecord; }
        }

        /// <summary>
        /// Gets the array of records.
        /// </summary>
        [Browsable(false)]
        public List<SonarRecord> Records
        {
            get { return recordList; }
        }

        /// <summary>
        /// Gets the array of profiles.
        /// </summary>
        [Browsable(false)]
        public List<SonarProfile> Profiles
        {
            get { return profileList; }
        }

        /// <summary>
        /// Gets the array of 3D records.
        /// </summary>
        [Browsable(false)]
        public List<Sonar3DRecord> Records3D
        {
            get { return recordList3D; }
        }

        [Browsable(false)]
        public List<DXFFile> DXFFiles
        {
            get { return dxfFiles; }
            set { dxfFiles = value; }
        }
        #endregion

        #region Events
        public event EventHandler StartRecording;
        public event EventHandler StopRecoding;
        #endregion

        #region Constructor / Dispose
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public SonarProject()
        {
            recordList.Clear();
            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);
        }

        /// <summary>
        /// Call this method to release all data of the project.
        /// </summary>
        public void Dispose()
        {
            recordList.Clear();
        }

        static public string TempPath
        {
            get
            {
                return Path.GetTempPath() + "sonOmeter";
            }
        }

        static public bool TempPathExists
        {
            get
            {
                return Directory.Exists(TempPath);
            }
        }

        static public void ClearTempPath()
        {
            if (Directory.Exists(TempPath))
                Directory.Delete(TempPath, true);
        }

        static public void CreateTempPath()
        {
            string tempPath = SonarProject.TempPath + "\\";
            string tempRawPath = tempPath + "raw\\";
            string tempCfgPath = tempPath + "cfg\\";
            string tempDXFPath = tempPath + "dxf\\";

            Directory.CreateDirectory(tempRawPath);
            Directory.CreateDirectory(tempCfgPath);
            Directory.CreateDirectory(tempDXFPath);
        }
        #endregion

        #region List handling
        #region Standard list actions
        public void Clear()
        {
            recordList.Clear();
            profileList.Clear();
            recordList3D.Clear();
            blankLines.Clear();
            buoyList.Clear();
            buoyConnectionList.Clear();
            blankLines.Clear();
            dxfFiles.Clear();
            fileName = "";
            GlobalNotifier.Invoke(this, null, GlobalNotifier.MsgTypes.Close);
        }

        /// <summary>
        /// Returns the record of the specified index.
        /// </summary>
        /// <param name="index">The index of the record.</param>
        /// <returns>A record object or null if not present.</returns>
        public SonarRecord Record(int index)
        {
            if ((index < 0) || (index >= recordList.Count))
                return null;
            return recordList[index];
        }

        /// <summary>
        /// Returns the profile of the specified index.
        /// </summary>
        /// <param name="index">The index of the profile.</param>
        /// <returns>A profile object or null if not present.</returns>
        public SonarProfile Profile(int index)
        {
            if ((index < 0) || (index >= profileList.Count))
                return null;
            return profileList[index];
        }

        /// <summary>
        /// Returns the 3D record of the specified index.
        /// </summary>
        /// <param name="index">The index of the 3D record.</param>
        /// <returns>A 3D record object or null if not present.</returns>
        public Sonar3DRecord Record3D(int index)
        {
            if ((index < 0) || (index >= recordList3D.Count))
                return null;
            return recordList3D[index];
        }

        /// <summary>
        /// Gets the index of a record, profile or 3D record in the project.
        /// </summary>
        /// <param name="record">A record, profile or 3D record object.</param>
        /// <returns>The index of the given object or -1, if not included in the project.</returns>
        public int IndexOf(SonarRecord record)
        {
            if (record is Sonar3DRecord)
                return recordList3D.IndexOf(record as Sonar3DRecord);
            else if (record is SonarProfile)
                return profileList.IndexOf(record as SonarProfile);
            else if (record is SonarRecord)
                return recordList.IndexOf(record);
            else
                return -1;
        }

        /// <summary>
        /// Adds the given record to the project.
        /// </summary>
        /// <param name="record">A record object.</param>
        public void AddRecord(SonarRecord record)
        {
            if (record is Sonar3DRecord)
            {
                recordList3D.Add(record as Sonar3DRecord);
                Update3DOrigin();
            }
            else if (record is SonarProfile)
                profileList.Add(record as SonarProfile);
            else if (record is SonarRecord)
                recordList.Add(record);
            else
                return;

            GlobalNotifier.Invoke(this, record, GlobalNotifier.MsgTypes.NewRecord);
        }

        /// <summary>
        /// Removes the given record from the project.
        /// </summary>
        /// <param name="record">A record object.</param>
        public void RemoveRecord(SonarRecord record)
        {
            if ((record is Sonar3DRecord) && (recordList3D.Contains(record as Sonar3DRecord)))
            {
                recordList3D.Remove(record as Sonar3DRecord);
                Update3DOrigin();
            }
            else if ((record is SonarProfile) && (profileList.Contains(record as SonarProfile)))
                profileList.Remove(record as SonarProfile);
            else if (recordList.Contains(record))
                recordList.Remove(record);
        }
        #endregion

        #region Misc stuff
        public void ApplyArchAndVolume()
        {
            try
            {
                foreach (SonarRecord rec in recordList)
                    rec.ApplyArchAndVolume();

                foreach (Sonar3DRecord rec3D in recordList3D)
                    rec3D.ApplyArchAndVolume();
            }
            catch
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Arch::ApplyValues failed on parsing prj.");
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle of all record coordinates.
        /// </summary>
        /// <returns>The bounding rectangle of the whole project.</returns>
        public RectangleD CoordBounds()
        {
            RectangleD rc = new RectangleD();
            RectangleD rc2;

            // Normal records.
            foreach (SonarRecord record in recordList)
            {
                rc2 = record.CoordLimits;
                if (!record.ShowInTrace && !record.ShowManualPoints)
                    continue;
                if (rc2.IsEmpty)
                    continue;

                rc.Merge(rc2, true);
            }

            // 3D records.
            foreach (Sonar3DRecord record3D in recordList3D)
            {
                rc2 = record3D.BlankLine.Poly.BoundingBox;
                if (!record3D.ShowInTrace)
                    continue;
                if (rc2.IsEmpty)
                    continue;

                rc.Merge(rc2, true);
            }

            // Profiles.
            foreach (SonarProfile profile in profileList)
            {
                rc2 = profile.CoordLimits;
                if (!profile.ShowInTrace && !profile.ShowManualPoints)
                    continue;
                if (rc2.IsEmpty)
                    continue;

                rc.Merge(rc2, true);
            }

            // Buoys.
            foreach (Buoy b in buoyList)
                rc.Merge(b.GetPointRVHV(), true);

            // Tracking position.
            if (tracking && (lastCoord.Type != CoordinateType.TransverseMercator))
                rc.Merge(lastCoord.Point, true);

            // DXF files.
            foreach (DXFFile dxfFile in dxfFiles)
            {
                if (!dxfFile.DXF.Header.BoundingBox.IsEmpty)
                {
                    rc.Merge(dxfFile.DXF.Header.BoundingBox.XMin, dxfFile.DXF.Header.BoundingBox.YMin, true);
                    rc.Merge(dxfFile.DXF.Header.BoundingBox.XMax, dxfFile.DXF.Header.BoundingBox.YMax, true);
                }
            }

            // Blankline.
            foreach (BlankLine bl in blankLines)
            {
                rc.Merge(bl.Poly);
            }

            return rc;
        }

        /// <summary>
        /// Calculates the bounding rectangle of all 3D record coordinates.
        /// </summary>
        /// <returns>The bounding rectangle of the 3D part of the project.</returns>
        public RectangleD CoordBounds3D()
        {
            RectangleD rc = new RectangleD();

            foreach (Sonar3DRecord rec3D in recordList3D)
            {
                //if (!rec3D.ShowInTrace)
                //    continue;

                RectangleD rc2 = rec3D.BlankLine.Poly.BoundingBox;

                if ((rc2 == null) || (rc2.IsZero))
                    continue;

                if (rc.IsZero)
                    rc = new RectangleD(rc2);
                else
                    rc.Merge(rc2);
            }

            return rc;
        }

        public void Update3DOrigin()
        {
            PointD pt = CoordBounds3D().Center;

            foreach (Sonar3DRecord rec3D in recordList3D)
                rec3D.Origin = pt;
        }

        /// <summary>
        /// Removes all LineData elements that are invisible due to cut lines.
        /// </summary>
        public void DropInvisibleData()
        {
            foreach (SonarRecord record in recordList)
            {
                record.DropInvisibleData();
            }
        }

        /// <summary>
        /// Find a suitable SonarLine to each marker coordinate and add it as a tag.
        /// </summary>
        private void MatchMarkersToSonarLines()
        {
            foreach (SonarRecord record in recordList)
                record.MatchMarkersToSonarLines(buoyList);
        }

        public void RecalcVolume()
        {
            foreach (SonarRecord record in recordList)
                record.RecalcVolume();

            foreach (Sonar3DRecord record in recordList3D)
                record.RecalcVolume();
        }
        #endregion

        #region Corridors
        public bool GetNearestConnection(PointD pt, ref BuoyConnection connection, ref double distance, bool removeHighlighting)
        {
            double dNearest = double.MaxValue;
            double dNearestBuoy = double.MaxValue;
            double d, dBuoy, a;
            BuoyConnection c, cNearest = null;

            for (int i = 0; i < buoyConnectionList.Count; i++)
            {
                c = buoyConnectionList[i] as BuoyConnection;

                if (removeHighlighting)
                    c.Highlighted = false;

                d = c.GetCorridorDistance(pt);
                dBuoy = c.GetNearestBuoyDistance(pt);
                a = c.GetBiggestAngle(pt);

                if ((a > 100.0) && (a < 260.0))
                    continue;

                if (Math.Abs(d) > Math.Abs(dNearest))
                    continue;

                if (dBuoy > dNearestBuoy)
                    continue;

                dNearestBuoy = dBuoy;
                dNearest = d;
                cNearest = c;
            }

            if (cNearest != null)
            {
                connection = cNearest;
                distance = dNearest;

                return true;
            }

            return false;
        }
        #endregion

        #region Build 3D record meshes
        public void Build3DMesh()
        {
            foreach (Sonar3DRecord rec in recordList3D)
                rec.Build3DMesh();
        }

        public void Build3DMesh(SonarPanelType type, TopColorMode mode)
        {
            foreach (Sonar3DRecord rec in recordList3D)
                rec.Build3DMesh(type, mode);
        }
        #endregion

        #region GetRectangleLines
        /// <summary>
        /// Gets the lines inside the rectangle.
        /// </summary>
        /// <param name="rc">The rectangle.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        public void GetRectangleLines(RectangleD rc, List<SonarLine> listSL, List<ManualPoint> listMP)
        {
            foreach (SonarRecord record in recordList)
            {
                if (record.ShowInTrace)
                    record.GetRectangleLines(rc, listSL, listMP);
            }
        }
        #endregion

        #region GetCorridorLines
        /// <summary>
        /// Gets the lines inside the corridor.
        /// </summary>
        /// <param name="conn">The BuoyConnection.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        /// <param name="normalizeSL">Use this flag to normalize the SonarLine coordinates to the connection.</param>
        /// <param name="normalizeMP">Use this flag to normalize the ManualPoint coordinates to the connection.</param>
        /// <param name="linkObjects">This flag controls the linking of the SonarLine and ManualPoint objects instead of creating new ones.</param>
        public void GetCorridorLines(BuoyConnection conn, List<SonarLine> listSL, List<ManualPoint> listMP, bool normalizeSL, bool normalizeMP, bool linkObjects)
        {
            foreach (SonarRecord record in recordList)
                if (record.ShowInTrace || record.ShowManualPoints)
                    record.GetCorridorLines(conn, listSL, listMP, normalizeSL, normalizeMP, linkObjects);
        }

        #region Old code
        /*/// <summary>
        /// Gets the lines inside the corridor and sorts them.
        /// </summary>
        /// <param name="conn">The BuoyConnection.</param>
        /// <param name="sortedListSL">The destination list of SonarLine objects.</param>
        /// <param name="sortedListMP">The destination list of ManualPoint objects.</param>
        /// <param name="normalizeSL">Use this flag to normalize the SonarLine coordinates to the connection.</param>
        /// <param name="normalizeMP">Use this flag to normalize the ManualPoint coordinates to the connection.</param>
        /// <param name="samplingRes">This value enables the sampling of the lines along the direction of the connection.</param>
        /// <param name="mode"></param>
        /// <param name="colors"></param>
        /// <param name="mergeManualPoints"></param>
        public void GetSortedCorridorLines(BuoyConnection conn, List<SonarLine> sortedListSL, List<ManualPoint> sortedListMP, bool normalizeSL, bool normalizeMP, double samplingRes, LineData.MergeMode mode, bool[] colors, bool mergeManualPoints, bool manualPointsDominate, double depthResolution)
        {
            if (conn.Width <= 0)
                return;

            List<SonarLine> listSL = new List<SonarLine>();
            List<ManualPoint> listMP = new List<ManualPoint>();

            List<SonarLine>[] listSLField = null;
            List<ManualPoint>[] listMPField = null;

            SonarLine line;
            ManualPoint point;
            PointD ptStart = conn.StartBuoy.GetPointRVHV();
            PointD ptEnd = conn.EndBuoy.GetPointRVHV();
            double d, d2;
            int count, sortedCount;
            int i;
            int j;
            int n = 1;

            if (!normalizeSL)
                samplingRes = 0;

            if (samplingRes > 0)
            {
                n = (int)Math.Ceiling(ptEnd.Distance(ptStart) / samplingRes);
                listSLField = new List<SonarLine>[n];
                listMPField = new List<ManualPoint>[n];
            }

            // Clear the lists.
            sortedListSL.Clear();
            sortedListMP.Clear();

            // Get the lines, unsorted.
            GetCorridorLines(conn, listSL, listMP, normalizeSL, normalizeMP, samplingRes > 0);

            // Order the SonarLine objects.
            count = listSL.Count;

            for (i = 0; i < count; i++)
            {
                line = listSL[i];
                d = ptStart.Distance(line.CoordRvHv);

                if (samplingRes > 0)
                {
                    j = (int)(Math.Floor(d / samplingRes));

                    if (listSLField[j] == null)
                        listSLField[j] = new List<SonarLine>();

                    listSLField[j].Add(line);
                }
                else
                {
                    sortedCount = sortedListSL.Count;

                    for (j = 0; j < sortedCount; j++)
                    {
                        d2 = ptStart.Distance(sortedListSL[j].CoordRvHv);

                        if (d < d2)
                            break;
                    }

                    sortedListSL.Insert(j, line);
                }
            }

            // Order the ManualPoint objects.
            count = listMP.Count;

            for (i = 0; i < count; i++)
            {
                point = listMP[i];
                d = ptStart.Distance(point.GetPointRVHV());

                if ((samplingRes > 0) && mergeManualPoints)
                {
                    j = (int)(Math.Floor(d / samplingRes));

                    if (listMPField[j] == null)
                        listMPField[j] = new List<ManualPoint>();

                    listMPField[j].Add(point);
                }
                else
                {
                    for (j = 0; j < sortedListMP.Count; j++)
                    {
                        d2 = ptStart.Distance(sortedListMP[j].GetPointRVHV());

                        if (d < d2)
                            break;
                    }

                    if (normalizeMP)
                    {
                        PointD ptn = conn.NormalizeToCorridor(point.Coord.Point);
                        ManualPoint newPoint = new ManualPoint(point);
                        point = newPoint;
                        point.RV = ptn.X;
                        point.HV = ptn.Y;
                    }

                    sortedListMP.Insert(j, point);
                }
            }

            // Now, interpolate each SonarLine and ManualPoint list to one single line
            if (samplingRes > 0)
                for (i = 0; i < n; i++)
                {
                    line = InterpolateListToLine(listSLField[i], ((double)i + 0.5) * samplingRes, conn, mode, colors, depthResolution);
                    point = (mergeManualPoints ? InterpolateListToPoint(listMPField[i], ((double)i + 0.5) * samplingRes, conn) : null);

                    if (point != null)
                        sortedListMP.Add(point);

                    if ((point != null) && (line != null))
                        if (manualPointsDominate)
                            continue;
                        else
                            point.Tag = line;

                    if (line != null)
                        sortedListSL.Add(line);
                }
        }*/
        #endregion
        #endregion

        #region Buoys
        public void AppendBuoysToExport(StringCollection list, SonarExport cfg)
        {
            if (!cfg.ExpSettings.ExportWithBuoys)
                return;

            list.Add("");

            foreach (Buoy b in buoyList)
            {
                string s = cfg.ExportPoint(new ManualPoint(b), -1);
                if (s != "")
                    list.Add(s);
            }
        }
        #endregion
        #endregion

        #region XML r/w
        #region Write
        /// <summary>
        /// Writes the project into the same file.
        /// </summary>
        public void Write()
        {
            Write("");
        }

        /// <summary>
        /// Writes the project into the passed file.
        /// </summary>
        /// <param name="fileSaveAs">The name of the target file.</param>
        public void Write(string fileSaveAs)
        {
            //IsBinary |= dxfFiles.Count > 0;
            IsBinary &= GSC.Settings.Lic[Module.Modules.V22];

            if (fileName.Length == 0)
            {
                if (fileSaveAs.Length == 0)
                    throw new Exception("No file name specified.");

                WriteNew(Path.ChangeExtension(fileSaveAs, IsBinary ? "sonx" : "xml"), false);
            }
            else
            {
                if (fileSaveAs.Length == 0)
                    fileSaveAs = fileName;
                
                string tempFile = Path.ChangeExtension(fileSaveAs, "tmp");
                int i = 1;

                while (File.Exists(tempFile))
                    tempFile = Path.ChangeExtension(fileSaveAs, "tmp" + (i++).ToString());

                WriteNew(tempFile, false);

                if ((fileSaveAs.Length == 0) || (fileSaveAs == fileName))
                {
                    File.Delete(fileName);
                    fileSaveAs = fileName;
                }

                fileSaveAs = Path.ChangeExtension(fileSaveAs, IsBinary ? "sonx" : "xml");
                File.Delete(fileSaveAs);
                File.Move(tempFile, fileSaveAs);
            }

            fileName = fileSaveAs;
            GSC.Settings.Changed = false;
            string file = AppDomain.CurrentDomain.BaseDirectory + "AutoSave.xml";
            File.Delete(file);
        }
        #endregion

        #region Write Header & Footer
        /// <summary>
        /// Writes the project header.
        /// </summary>
        /// <param name="writer">The XmlTextWriter object.</param>
        private void WriteHeader(XmlTextWriter writer)
        {
            // Start project node.
            writer.WriteStartElement("project");

            // Write attributes.
            writer.WriteAttributeString("name", name);

            // Write blank line.
            writer.WriteStartElement("blanklines");
            foreach (BlankLine blankLine in blankLines)
                blankLine.WriteToXml(writer, false);
            writer.WriteEndElement();

            // Write buoy list
            if (buoyList.Count > 0)
                Buoy.WriteXml(writer, buoyList, buoyConnectionList, GSC.Settings.NFI);

            // Start sonar data.
            writer.WriteStartElement("sondata");
        }

        /// <summary>
        /// Writes the project footer.
        /// </summary>
        /// <param name="writer">The XmlTextWriter object.</param>
        private void WriteFooter(XmlTextWriter writer)
        {
            // End sonar data.
            writer.WriteEndElement();

            // Write Settings.Sets.
            GSC.WriteXml(writer);

            // End project node.
            writer.WriteEndElement();

            // Close document.
            writer.WriteEndDocument();
        }
        #endregion

        #region Write New
        /// <summary>
        /// This function writes a new file.
        /// </summary>
        /// <param name="fileSaveAs">The name of the target file.</param>
        /// <param name="autosave">This flag indicates an autosave process.</param>
        private void WriteNew(string fileSaveAs, bool autosave)
        {
            XmlTextWriter writerXML = null;
            BinaryWriter writerBin = null;
            string tempPath = SonarProject.TempPath + "\\";
            string tempRawPath = tempPath + "raw\\";
            string tempCfgPath = tempPath + "cfg\\";
            string tempDXFPath = tempPath + "dxf\\";
            ArrayList recordFiles = new ArrayList();
            SonarRecord rec;
            SonarProfile prof;
            Sonar3DRecord rec3D;
            int max, i;

            if (IsBinary)
            {
                // Binary file mode chosen.
                if (!SonarProject.TempPathExists)
                    CreateTempPath();

                // Get file list of the raw path.
                recordFiles.AddRange(Directory.GetFiles(tempRawPath));

                if (autosave)
                    writerXML = new XmlTextWriter(tempPath + autoSaveFileName, System.Text.Encoding.UTF8);
                else
                    writerXML = new XmlTextWriter(tempPath + defaultFileName, System.Text.Encoding.UTF8);
            }
            else
                writerXML = new XmlTextWriter(fileSaveAs, System.Text.Encoding.UTF8);

            writerXML.Formatting = Formatting.Indented;
            writerXML.WriteStartDocument(true);

            // Reset crypto engine.
            Crypto.ResetDES();

            try
            {
                // Header.
                WriteHeader(writerXML);

                // Record list.
                max = recordList.Count;

                for (i = 0; i < max; i++)
                {
                    rec = recordList[i];

                    if (IsBinary)
                        if (!recordFiles.Contains(tempRawPath + rec.RawDataGuid) || (rec == newRecord))
                        {
                            if (Guid.Empty == rec.RawDataGuid)
                                rec.RawDataGuid = Guid.NewGuid();
                            writerBin = new BinaryWriter(File.Create(tempRawPath + rec.RawDataGuid));
                        }
                        else
                            writerBin = null;

                    rec.Write(writerXML, writerBin, IsBinary);

                    if (writerBin != null)
                    {
                        writerBin.Close();
                        writerBin = null;
                    }
                }

                // Profile list.
                max = profileList.Count;

                for (i = 0; i < max; i++)
                {
                    prof = profileList[i];

                    if (IsBinary)
                        if (!recordFiles.Contains(tempRawPath + prof.RawDataGuid) || (prof == newRecord))
                            writerBin = new BinaryWriter(File.Create(tempRawPath + prof.RawDataGuid));
                        else
                            writerBin = null;

                    prof.Write(writerXML, writerBin, IsBinary);

                    if (writerBin != null)
                    {
                        writerBin.Close();
                        writerBin = null;
                    }
                }

                // Record 3D list.
                max = recordList3D.Count;

                for (i = 0; i < max; i++)
                {
                    rec3D = recordList3D[i];

                    if (IsBinary)
                        if (!recordFiles.Contains(tempRawPath + rec3D.RawDataGuid))
                            writerBin = new BinaryWriter(File.Create(tempRawPath + rec3D.RawDataGuid));
                        else
                            writerBin = null;

                    rec3D.Write(writerXML, writerBin, IsBinary);

                    if (writerBin != null)
                    {
                        writerBin.Close();
                        writerBin = null;
                    }
                }

                // Footer.
                WriteFooter(writerXML);

                writerXML.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An exception was thrown during save procedure.", ex.StackTrace + "\n" + ex.Message);
            }

            // Do binary data handling.
            if (IsBinary && !autosave)
            {
                // Remove the autosave file if existing. (not needed anymore)
                if (File.Exists(tempPath + autoSaveFileName))
                    File.Delete(tempPath + autoSaveFileName);

                try
                {
                    // Create ZIP file.
                    Sonx.Create(fileSaveAs, tempPath);
                }
                catch (Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Yellow, "SonarProject.WriteNew: " + ex.Message + " - RETRY");

                    try
                    {
                        // Start the garbage collector.
                        GC.Collect();

                        // Create ZIP file.
                        Sonx.Create(fileSaveAs, tempPath);
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(ex2.StackTrace + "\n" + ex2.Message, "An exception was thrown during ZIP operation after retry.\nPerforming backup copy.");

                        try
                        {
                            DateTime now = DateTime.Now;
                            string dstFile = (fileName != "") ? fileName : fileSaveAs;
                            string path = Path.GetDirectoryName(dstFile) + "\\" + Path.GetFileNameWithoutExtension(dstFile) + "_" + now.ToString("yyyyMMdd") + "_" + now.ToString("HHmmff");

                            Directory.CreateDirectory(path);
                            Directory.CreateDirectory(path + "\\raw");
                            Directory.CreateDirectory(path + "\\dxf");
                            Directory.CreateDirectory(path + "\\cfg");

                            string[] files = Directory.GetFiles(tempPath);

                            foreach (string file in files)
                                File.Copy(file, path + "\\" + Path.GetFileName(file));

                            files = Directory.GetFiles(tempRawPath);

                            foreach (string file in files)
                                File.Copy(file, path + "\\raw\\" + Path.GetFileName(file));

                            files = Directory.GetFiles(tempCfgPath);

                            foreach (string file in files)
                                File.Copy(file, path + "\\cfg\\" + Path.GetFileName(file));

                            files = Directory.GetFiles(tempDXFPath);

                            foreach (string file in files)
                                File.Copy(file, path + "\\dxf\\" + Path.GetFileName(file));

                            MessageBox.Show("Backup saved to:\n" + path + "\n\n" + "Please send the directory content to the support team for further analysis.", "Backup operation complete.");
                        }
                        catch (Exception ex3)
                        {
                            MessageBox.Show(ex3.StackTrace + "\n" + ex3.Message, "Backup operation failed.");
                        }
                    }
                }
            }
        }
        #endregion

        #region Write AutoSave
        /// <summary>
        /// Writes the changes since last save.
        /// </summary>
        public void WriteAutoSave()
        {
            IsBinary = true;
            WriteNew("", true);

            #region Old AutoSave
            /*try
            {
                if (IsBinary)
                {
                    WriteNew("", true);
                    return;
                }

                if ((recordList.Count == 0) || (newRecord == null))
                    return;
                
                string tempFile = Path.GetTempFileName();
                string file = AppDomain.CurrentDomain.BaseDirectory + "AutoSave.xml";

                // Create writer.
                XmlTextWriter writerXML = new XmlTextWriter(tempFile, System.Text.Encoding.UTF8);
                writerXML.Formatting = Formatting.Indented;
                writerXML.WriteStartDocument(true);

                // Test if autosave file already exists.
                if (File.Exists(file))
                {
                    // Create reader.
                    XmlTextReader reader = new XmlTextReader(file);

                    int recordIndex = -1;
                    bool changesWritten = false;

                    while (reader.Read())
                    {
                        if (reader.Name == "config")
                        {
                            reader.ReadOuterXml();
                            GSC.WriteXml(writerXML);
                            continue;
                        }

                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            writerXML.WriteStartElement(reader.Name);
                            writerXML.WriteAttributes(reader, false);

                            if (!reader.IsEmptyElement)
                            {
                                switch (reader.Name)
                                {
                                    case "HF":
                                        writerXML.WriteString(reader.ReadInnerXml());
                                        writerXML.WriteEndElement();
                                        break;

                                    case "NF":
                                        writerXML.WriteString(reader.ReadInnerXml());
                                        writerXML.WriteEndElement();
                                        break;

                                    case "pos":
                                        writerXML.WriteString(reader.ReadInnerXml());
                                        writerXML.WriteEndElement();
                                        break;

                                    case "record":
                                        recordIndex++;
                                        break;
                                }
                            }
                            else
                            {
                                writerXML.WriteEndElement();
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            // If the end element is a record AND if this record is the new record, OR
                            // if the end element is the whole bunch of sonar data AND the changes were not written (than the new record was not saved before)
                            // THAN write the new record.
                            if (((reader.Name == "record") && (recordIndex == recordList.IndexOf(newRecord))) || ((reader.Name == "sondata") && !changesWritten))
                            {
                                newRecord.WriteAutoSave(writerXML, null, IsBinary);
                                changesWritten = true;
                            }
                            else
                            {
                                writerXML.WriteEndElement();
                            }
                        }
                    }

                    // Close the reader.
                    reader.Close();
                }
                else
                {
                    // Write project information.
                    WriteHeader(writerXML);

                    // Record tree.
                    foreach (SonarRecord rec in recordList)
                    {
                        rec.Write(writerXML, null, IsBinary);
                    }
                }

                // Close the writer.
                writerXML.Close();

                // Move temporary file to application root directory.
                if (File.Exists(file))
                    File.Delete(file);
                File.Move(tempFile, file);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.WriteAutoSave: " + e.Message);
            }*/
            #endregion
        }
        #endregion

        #region Read AutoSave
        /// <summary>
        /// Tries to read from an autosave file and requests the permission to do so.
        /// In either case, any existing autosave file will be deleted.
        /// </summary>
        public bool ReadAutoSave()
        {
            string fileName = SonarProject.TempPath + "\\" + autoSaveFileName;
            bool ret = false;

            try
            {
                // Does an autosave backup exist, otherwise cancel.
                if (!File.Exists(fileName))
                    return false;

                // Ask the user.
                ret = (MessageBox.Show("Do you want to open the backup before deleting?", "AutoSave backup available.", MessageBoxButtons.YesNo) == DialogResult.Yes);

                // Read the file.
                if (ret)
                    ReadFromTempPath(true);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.ReadAutoSave: " + e.Message);
            }

            try
            {
                // Delete the file.
                File.Delete(fileName);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.ReadAutoSave: " + e.Message);
            }

            return ret;
        }
        #endregion

        #region Read
        public void Read(string fileName, bool noNotifications = false)
        {
            // First, let's see, if its the new ZIP format in camouflage...
            Stream testStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            byte[] buf = new byte[4];
            testStream.Read(buf, 0, 4);
            IsBinary = (buf[0] == 0x50) && (buf[1] == 0x4b) && (((buf[2] == 0x03) && (buf[3] == 0x04)) || ((buf[2] == 0x05) && (buf[3] == 0x06)));
            testStream.Close();

            // Start reading...
            XmlTextReader readerXML = null;

            try
            {
                if (IsBinary)
                {
                    // Set the XmlTextReader object.
                    SonarProject.ClearTempPath();
                    SonarProject.CreateTempPath();

                    string tempPath = TempPath + "\\";
                    
                    try
                    {
                        Sonx.Extract(fileName, tempPath);
                    }
                    catch(Exception ex)
                    {
                        UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
                    }

                    readerXML = new XmlTextReader(tempPath + defaultFileName);

                    // Clear the current DXF list and read the new one.
                    dxfFiles.Clear();

                    string[] files = Directory.GetFiles(tempPath + "dxf\\");

                    DXFFile dxf;

                    foreach (string file in files)
                    {
                        dxf = new DXFFile(this, file, false);
                        GlobalNotifier.Invoke(this, dxf, GlobalNotifier.MsgTypes.ToggleDXFFile);
                    }
                }
                else
                    readerXML = new XmlTextReader(fileName);

                this.fileName = fileName;

                Read(readerXML, noNotifications);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
            finally
            {
                if (readerXML != null)
                    readerXML.Close();
            }
        }

        public void ReadFromTempPath(bool autosave)
        {
            IsBinary = true;
            XmlTextReader readerXML = null;

            try
            {
                string tempPath = TempPath + "\\";
                readerXML = new XmlTextReader(tempPath + (autosave ? autoSaveFileName : defaultFileName));
                Read(readerXML);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
            finally
            {
                if (readerXML != null)
                    readerXML.Close();
            }
        }

        /// <summary>
        /// Reads the project from a XML and binary reader pair.
        /// </summary>
        /// <param name="readerXML">The XML reader.</param>
        /// <param name="sonx">A flag indicating the binary format.</param>
        public void Read(XmlTextReader readerXML, bool noNotifications = false)
        {
            BinaryReader readerBin = null;
            SonarRecord rec = null;
            BlankLine blankLine = null;
            string tempPath = Path.GetTempPath() + "sonOmeter\\";
            string tempRawPath = tempPath + "raw\\";
            string tempCfgPath = tempPath + "cfg\\";

            // Reset crypto engine.
            Crypto.ResetDES();

            // Clear old lists.
            blankLines.Clear();
            buoyList.Clear();

            try
            {
                // Load the reader with the data file and ignore all white space nodes.         
                readerXML.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the file and display each of the nodes.
                readerXML.Read();
                while (!readerXML.EOF)
                {
                    if (readerXML.NodeType != XmlNodeType.Element)
                    {
                        readerXML.Read();
                        continue;
                    }

                    switch (readerXML.Name)
                    {
                        case "config":
                            GSC.ReadOldXml(readerXML.ReadOuterXml());
                            break;
                        case "GlobalSettings":
                            GSC.ReadXmlString(readerXML.ReadOuterXml());
                            break;
                        case "blankline":
                            blankLine = new BlankLine();
                            blankLine.ReadFromXml(readerXML);
                            blankLines.Add(blankLine);
                            if (!noNotifications)
                                GlobalNotifier.Invoke(this, blankLine, GlobalNotifier.MsgTypes.NewBlankLine);
                            break;

                        case "buoylist":
                            Buoy.ReadXml(readerXML, buoyList, buoyConnectionList);
                            break;

                        case "project":
                            // The project description and Settings.Sets.
                            try
                            {
                                name = readerXML.GetAttribute("name");
                                readerXML.Read();
                            }
                            catch
                            {
                            }
                            break;

                        case "devicelist":
                            // Validate record state.
                            if (rec == null)
                                throw new ArgumentOutOfRangeException("rec", rec, "Invalid record (null).");

                            // Extract xml tag and send it to active record.
                            rec.ReadDeviceList(readerXML.ReadOuterXml());
                            break;

                        case "record":
                            // If there was already a record, refresh devices and raise event.
                            if (rec != null)
                            {
                                rec.RefreshDevices();
                                if (!rec.Interpolated)
                                    rec.UpdateAllCoordinates();
                                if (!noNotifications)
                                    GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.NewRecord);
                            }

                            // Add new record to list and store index.
                            rec = new SonarRecord(readerXML);
                            recordList.Add(rec);
                            if (!recordList.Contains(rec))
                                throw new ArgumentOutOfRangeException("rec", rec, "Adding new record to list failed.");

                            if (IsBinary && (rec.RawDataGuid != null))
                            {
                                try
                                {
                                    if (readerBin != null)
                                        readerBin.Close();
                                    readerBin = new BinaryReader(File.Open(tempRawPath + rec.RawDataGuid, FileMode.Open, FileAccess.Read));
                                }
                                catch
                                {
                                    readerBin = null;
                                }
                            }

                            // get next record
                            readerXML.Read();
                            break;

                        case "profile":
                            // If there was already a record, refresh devices and raise event.
                            if ((rec != null) && !(rec is Sonar3DRecord))
                            {
                                rec.RefreshDevices();
                                if (!rec.Interpolated)
                                    rec.UpdateAllCoordinates();
                                if (!noNotifications)
                                    GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.NewRecord);

                                rec = null;
                            }

                            // Create new profile and add it to the list.
                            SonarProfile profile = new SonarProfile(readerXML, buoyConnectionList);
                            profile.CreateProfile(this);
                            AddRecord(profile);

                            // get next record
                            readerXML.Read();
                            break;

                        case "record3D":
                            // If there was already a record, refresh devices and raise event.
                            if ((rec != null) && !(rec is Sonar3DRecord))
                            {
                                rec.RefreshDevices();
                                if (!rec.Interpolated)
                                    rec.UpdateAllCoordinates();
                                if (!noNotifications)
                                    GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.NewRecord);
                            }

                            // Add new record to list and store index.
                            rec = new Sonar3DRecord(this, readerXML, readerBin);
                            AddRecord(rec);
                            if (!recordList3D.Contains(rec as Sonar3DRecord))
                                throw new ArgumentOutOfRangeException("rec", rec, "Adding new 3D record to list failed.");
                            if (!noNotifications)
                                GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.Toggle3DRecord);

                            // get next record
                            readerXML.Read();
                            break;

                        case "line":
                            // Add new line to active record.
                            if (rec == null)
                                throw new ArgumentOutOfRangeException("rec", rec, "Invalid record (null).");

                            SonarLine line = new SonarLine(readerXML, readerBin, rec.TimeStart.Date, IsBinary);

                            if (rec is Sonar3DRecord)
                                break;

                            // Create new sonar line and add it to record.
                            rec.AddSonarLine(line, false);
                            break;

                        case "manualpoint":
                            // Add new point to the active record.
                            if (rec == null)
                                throw new ArgumentOutOfRangeException("rec", rec, "Invalid record (null).");

                            // Create new manual point and add it to record.
                            rec.AddManualPoint(new ManualPoint(readerXML));
                            readerXML.Read();
                            break;

                        default:
                            readerXML.Read();
                            break;
                    }
                }

                // Raise event for last record.
                if ((rec != null) && !(rec is Sonar3DRecord))
                {
                    rec.RefreshDevices();
                    if (!rec.Interpolated)
                        rec.UpdateAllCoordinates();
                    if (!noNotifications)
                        GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.NewRecord);
                }

                if (readerBin != null)
                    readerBin.Close();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
            finally
            {
                MatchMarkersToSonarLines();
                ApplyArchAndVolume();
            }
        }
        #endregion
        #endregion

        #region Recording
        /// <summary>
        /// Starts recording.
        /// </summary>
        /// <param name="tracking">Tracking mode switch.</param>
        public void StartRec(bool tracking, bool simulation)
        {
            try
            {
                if (!this.tracking & recording)
                    return;

                if (simulation)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.DefaultExt = "xml";
                    ofd.Filter = "Record files|*.xml;*.sonx|All files|*.*";
                    ofd.Title = "Open existing record";

                    if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return;

                    simulationThread = new SimulationThread();
                    simulationThread.SynchronizingObject = synchronizingObject;
                    simulationThread.NewLine += new TcpLineEventHandler(recordThread_NewLine);
                    simulationThread.Open(ofd.FileName);
                }
                else if (!recordThread.Connected)
                {
                    recordThread = new TcpClientThread();
                    recordThread.EOLTag = "\r\n";
                    recordThread.DisconnectTag = "quit\r\n";
                    recordThread.SynchronizingObject = synchronizingObject;
                    recordThread.NewLine += new TcpLineEventHandler(recordThread_NewLine);
                    recordThread.Connect(GSC.Settings.HostName, GSC.Settings.HostPort);
                }

                if (!tracking)
                {
                    newRecord = new SonarRecord();
                    newRecord.TimeStart = DateTime.Now;
                    newRecord.Recording = true;
                    AddRecord(newRecord);

                    if (StartRecording != null)
                        StartRecording(this, EventArgs.Empty);
                }

                recording = true;
                this.tracking = tracking;
                this.simulation = simulation;
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.StartRec: " + e.Message);
            }
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        public void StopRec()
        {
            try
            {
                recording = false;

                if (recordThread.Connected)
                    recordThread.Close();

                if (simulationThread.Connected)
                    simulationThread.Close();

                if (!tracking && (newRecord != null))
                {
                    newRecord.TimeEnd = DateTime.Now;
                    newRecord.Recording = false;
                    WriteAutoSave();
                    GlobalNotifier.Invoke(this, newRecord, GlobalNotifier.MsgTypes.UpdateRecord);
                    if (StopRecoding != null)
                        StopRecoding(this, EventArgs.Empty);
                }

                tracking = false;
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.StopRec: " + e.Message);
            }
        }

        private void recordThread_NewLine(object sender, TcpLineEventArgs e)
        {
            string prevValue = "";

            try
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(e.S));
                reader.WhitespaceHandling = WhitespaceHandling.None;

                SonarLine line;

                reader.Read();
                while (!reader.EOF)
                {
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        prevValue = reader.Value;
                        reader.Read();

                        if ((prevValue != "") && (reader.Value == prevValue))
                            return;

                        continue;
                    }

                    switch (reader.Name)
                    {
                        case "sondevice":
                        case "devicelist":
                            // Extract xml tag and send it to active record.
                            if (tracking)
                            {
                                reader.Read();
                            }
                            else
                            {
                                newRecord.ReadDeviceList(reader.ReadOuterXml());
                            }
                            break;

                        case "line":
                            // Create new sonar line and add it to record.
                            line = new SonarLine(reader, null, DateTime.Now, false);

                            if (line.ReadError)
                                break;

                            if (tracking)
                            {
                                line.ScanPositions(null);

                                if (line.CoordRvHv.Type == CoordinateType.TransverseMercator)
                                {
                                    lastCoord = line.CoordRvHv;
                                    double angle = line.CompassAngle;
                                    if (!double.IsNaN(angle))
                                        lastRotation.Yaw = angle;
                                    GlobalNotifier.Invoke(this, line.CoordRvHv, GlobalNotifier.MsgTypes.NewCoordinate);
                                }

                                GlobalNotifier.Invoke(this, line, GlobalNotifier.MsgTypes.NewSonarLine);
                            }
                            else
                            {
                                object dev = newRecord.AddSonarLine(line, true);
                                GlobalNotifier.Invoke(dev, line, GlobalNotifier.MsgTypes.NewSonarLine);

                                if (newRecord.UpdateCoordinates(line, ref lastRotation, ref lastCoord))
                                    GlobalNotifier.Invoke(this, line.CoordRvHv, GlobalNotifier.MsgTypes.NewCoordinate);
                            }
                            break;

                        case "manualpoint":
                            // Add new point to the active record.
                            if (simulation && newRecord != null)
                            {
                                // Create new manual point and add it to record.
                                newRecord.AddManualPoint(new ManualPoint(reader));
                                reader.Read();
                            }
                            break;

                        default:
                            reader.Read();
                            break;
                    }
                }

                if (!tracking)
                    GSC.Settings.Changed = true;
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProject.recordThread_NewLine: " + ex.Message + "|" + e.S);
            }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return name;
        }
        #endregion
    }
}
