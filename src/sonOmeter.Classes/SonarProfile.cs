using System;
using System.Collections.Generic;
using System.Text;
using sonOmeter.Classes.Sonar2D;
using System.Xml;
using System.IO;
using System.Globalization;
using UKLib.MathEx;
using UKLib.Survey.Math;
using System.ComponentModel;
using UKLib.Arrays;
using UKLib.Xml;

namespace sonOmeter.Classes
{
    public class SonarProfile : SonarRecord
    {
        #region Variables
        protected SonarLine lastUpdatedLine = null;

        protected GlobalEventHandler globalEventHandler;
        protected BuoyConnection profile = null;

        protected List<SonarLine>[] listSLField = null;
        protected List<ManualPoint>[] listMPField = null;

        [Browsable(false), Category("Profile"), DisplayName("Normalize sonar lines"), DefaultValue(true)]
        public bool NormalizeSL { get; set; }

        [Browsable(false), Category("Profile"), DisplayName("Normalize manual points"), DefaultValue(true)]
        public bool NormalizeMP { get; set; }

        [Browsable(false), Category("Profile"), DisplayName("Sampling resolution (m)"), DefaultValue(0.0)]
        public double SamplingRes { get; set; }       

        [Browsable(false), Category("Profile"), DisplayName("Interpolation mode"), DefaultValue(LineData.MergeMode.Strongest)]
        public LineData.MergeMode Mode { get; set; }

        [Browsable(false), Category("Profile"), DisplayName("Interpolation colors")]
        public bool[] Colors { get; set; }

        [Browsable(false), Category("Profile"), DisplayName("Merge manual points"), DefaultValue(false)]
        public bool MergeManualPoints { get; set; }

        [Browsable(false), Category("Profile"), DisplayName("Manual points dominate"), DefaultValue(false), Obsolete("Remove from profile and add them to export!")]
        public bool ManualPointsDominate { get; set; }
        #endregion

        #region Properties
        public new bool IsProfile
        {
            get { return isProfile; }
        }

        public SonarLine LastUpdatedLine
        {
            get { return lastUpdatedLine; }
        }

        public BuoyConnection Profile
        {
            get { return profile; }
        }
        #endregion

        #region Constructor and Dispose
        public SonarProfile(BuoyConnection conn, bool dynamic)
        {
            this.isProfile = true;
            this.profile = conn;
            this.desc = conn.ToString();
            this.sonarDevices.Add(new SonarDevice(0, "Profile data", 0, 0, 0, 0, 0, true, "", "", true, true, 0, 0, ""));
            this.ShowInTrace = false;
            this.ShowManualPoints = false;

            timeStart = DateTime.MinValue;
            timeEnd = DateTime.MinValue;

            NormalizeSL = true;
            NormalizeMP = true;
            SamplingRes = 0.0;
            Mode = LineData.MergeMode.Strongest;
            MergeManualPoints = false;
            
            Colors = new bool[GSC.Settings.SECL.Count];

            for (int i = 0; i < Colors.Length; i++)
                Colors[i] = true;

            if (dynamic)
            {
                globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
                GlobalNotifier.SignIn(globalEventHandler, GetFilterList());
            }
        }

        public SonarProfile(XmlTextReader reader, IDList<Sonar2DElement> buoyConnectionList)
        {
            int id = XmlReadConvert.Read(reader, "buoyconnectionID", -1);
            
            BuoyConnection conn = null;

            foreach (BuoyConnection c in buoyConnectionList)
                if (c.ID == id)
                    conn = c;

            if (conn == null)
                return;

            this.isProfile = true;
            this.profile = conn;
            this.desc = XmlReadConvert.Read(reader, "desc", conn.ToString());
            this.sonarDevices.Add(new SonarDevice(0, "Profile data", 0, 0, 0, 0, 0, true, "", "", true, true, 0, 0, ""));
            this.ShowInTrace = false;
            this.ShowManualPoints = false;

            timeStart = DateTime.MinValue;
            timeEnd = DateTime.MinValue;

            NormalizeSL = XmlReadConvert.Read(reader, "normalizeSL", true);
            NormalizeMP = XmlReadConvert.Read(reader, "normalizeMP", true);
            SamplingRes = XmlReadConvert.Read(reader, "samplingRes", 0.0);
                        
            Mode = (LineData.MergeMode)Enum.Parse(LineData.MergeMode.Strongest.GetType(), XmlReadConvert.Read(reader, "mode", "Strongest"));
            MergeManualPoints = XmlReadConvert.Read(reader, "mergeMP", false);
            
            Colors = new bool[GSC.Settings.SECL.Count];

            for (int i = 0; i < Colors.Length; i++)
                Colors[i] = XmlReadConvert.Read(reader, "color" + i.ToString(), true);

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
            GlobalNotifier.SignIn(globalEventHandler, GetFilterList());
        }

        private List<GlobalNotifier.MsgTypes> GetFilterList()
        {
            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateCoordinates);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceManualPoint);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateProfiles);

            return filterlist;
        }

        public override void Dispose()
        {
            if (globalEventHandler != null)
                GlobalNotifier.SignOut(globalEventHandler, GetFilterList());

            base.Dispose();
        }
        #endregion

        #region Global event handler
        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    AddSonarLine(args as SonarLine, false);
                    break;

                case GlobalNotifier.MsgTypes.UpdateCoordinates:
                    List<SonarLine> lines = args as List<SonarLine>;
                    int count = lines.Count;

                    for (int i = 0; i < count; i++)
                        AddSonarLine(lines[i], false);
                    break;

                case GlobalNotifier.MsgTypes.PlaceManualPoint:
                    AddManualPoint(args as ManualPoint);
                    break;

                case GlobalNotifier.MsgTypes.UpdateProfiles:
                    CreateProfile(args as SonarProject);
                    break;
            }
        }
        #endregion

        #region New lines and points
        /// <summary>
        /// Adds a new line to the list and recalculates the bounding rectangle.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <param name="scanPos">Scan for positions in the sonar line - ignored for profiles.</param>
        public override object AddSonarLine(SonarLine line, bool scanPos)
        {
            // Filter lines without coordinates.
            if (line.CoordRvHv.Type != CoordinateType.TransverseMercator)
                return null;

            // Filter lines that are already contained.
            if (sonarLines.Contains(line))
                return null;

            PointD pt = line.CoordRvHv.Point;
            PointD ptn = pt;

            // Filter lines that are not inside the corridor.
            if (!profile.IsInCorridor(pt))
                return null;

            double antDepth = line.AntDepth;
            if (double.IsNaN(antDepth) && (lastUpdatedLine != null))
                antDepth = lastUpdatedLine.AntDepth;

            // Normalize coordinate to the corridor connection and save it to the sonar line.
            if (NormalizeSL && (SamplingRes == 0))
            {
                ptn = profile.NormalizeToCorridor(pt);
                line = new SonarLine(line, new Coordinate(ptn.X, ptn.Y, line.CoordRvHv.AL, CoordinateType.TransverseMercator));
                line.SonID = 0;
            }

            // Sort the SonarLine object.
            PointD ptStart = profile.StartBuoy.GetPointRVHV();
            int bin = SortSonarLine(line, ptStart);

            // Interpolate, if needed.
            if ((SamplingRes > 0) && (bin >= 0) && (sonarLines.Count > bin))
            {
                SonarLine newLine = InterpolateListToLine(listSLField[bin], ((double)bin + 0.5) * SamplingRes, profile, Mode, Colors, DepthRes, GSC.Settings.MergeWithAbsoluteDepths);

                line = sonarLines[bin];

                int max = manualPoints.Count;
                for (int i = 0; i < max; i++)
                    if (manualPoints[i].Tag == line)
                        manualPoints[i].Tag = newLine;

                lastUpdatedLine = newLine;
                lastUpdatedLine.AntDepth = antDepth;
                sonarLines[bin] = newLine;
                sonarDevices[0].SonarLines[bin] = newLine;

                UpdateMaxDepth(newLine);

                // Update profile bounding box.
                RefreshCoordLimits(newLine.CoordRvHv, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);
            }

            return null;
        }

        /// <summary>
        /// Adds a new manual point to the list.
        /// </summary>
        /// <param name="point">The manual point.</param>
        /// <returns>The index of the line.</returns>
        public override int AddManualPoint(ManualPoint point)
        {
            PointD pt = point.GetPointRVHV();
            PointD ptn = pt;

            if (!profile.IsInCorridor(pt))
                return -1;

            // Normalize coordinate to the corridor connection and save it to the manual point.
            if (NormalizeMP && (SamplingRes == 0))
            {
                ptn = profile.NormalizeToCorridor(pt);
                point = new ManualPoint(point);
                point.RV = ptn.X;
                point.HV = ptn.Y;
            }

            // Sort the ManualPoint object.
            PointD ptStart = profile.StartBuoy.GetPointRVHV();
            int bin = SortManualPoint(point, ptStart);

            // Interpolate, if needed.
            if ((SamplingRes > 0) && (bin >= 0))
            {
                if (MergeManualPoints)
                    point = InterpolateListToPoint(listMPField[bin], ((double)bin + 0.5) * SamplingRes, profile);

                if (point != null)
                {
                    if (sonarLines.Count > bin)
                    {
                        SonarLine line = sonarLines[bin];

                        if ((line.HF.Entries == null) && (line.NF.Entries == null))
                        {
                            PointD ptNew = line.CoordRvHv.Point;
                            double al = ((alMax == double.MinValue) || (alMin == double.MaxValue)) ? 0 : ((alMax + alMin) / 2.0);
                            line.CoordRvHv = new Coordinate(ptNew.X, ptNew.Y, al, CoordinateType.TransverseMercator);
                        }

                        point.Tag = line;
                    }

                    if (MergeManualPoints)
                    {
                        int i = 0, max = manualPoints.Count;
                        for (i = 0; i < max; i++)
                            if (manualPoints[i].GetPointRVHV().Equals(ptn))
                                break;

                        if (i < max)
                            manualPoints[i] = point;
                        else
                            InsertManualPoint(point, ptStart);
                    }
                    else
                        InsertManualPoint(point, ptStart);
                }
            }

            return manualPoints.IndexOf(point);
        }
        #endregion

        #region Profile management
        public void CreateProfile(SonarProject project)
        {
            if (project == null)
                return;

            if (profile.Width <= 0)
                return;

            if (sonarDevices.Count != 1)
                return;

            manualPoints.Clear();
            sonarLines.Clear();

            alMax = Double.MinValue;
            alMin = Double.MaxValue;

            List<SonarLine> listSL = new List<SonarLine>();
            List<ManualPoint> listMP = new List<ManualPoint>();

            SonarLine line;
            ManualPoint point;
            PointD ptStart = profile.StartBuoy.GetPointRVHV();
            PointD ptEnd = profile.EndBuoy.GetPointRVHV();
            PointD ptNorm = (ptEnd - ptStart).Normalize();
            int count;
            int i;
            int n = 1;

            if (!NormalizeSL)
                SamplingRes = 0;

            if (SamplingRes > 0)
            {
                n = (int)Math.Ceiling(ptEnd.Distance(ptStart) / SamplingRes);
                listSLField = new List<SonarLine>[n];
                listMPField = new List<ManualPoint>[n];
            }

            // Clear the own lists.
            sonarLines.Clear();
            sonarDevices[0].SonarLines.Clear();
            manualPoints.Clear();

            // Get the lines, unsorted.
            project.GetCorridorLines(profile, listSL, listMP, NormalizeSL, NormalizeMP, SamplingRes > 0);

            // Sort the SonarLine objects.
            count = listSL.Count;
            for (i = 0; i < count; i++)
                SortSonarLine(listSL[i], ptStart);

            // Sort the ManualPoint objects.
            count = listMP.Count;
            for (i = 0; i < count; i++)
                SortManualPoint(listMP[i], ptStart);
            
            // Now, interpolate each SonarLine and ManualPoint list to one single line
            if (SamplingRes > 0)
                for (i = 0; i < n; i++)
                {
                    // calculate the new coordinate
                    PointD ptNew = ptStart + ptNorm * ((double)i + 0.5) * SamplingRes;

                    line = InterpolateListToLine(listSLField[i], ptNew, profile.Corridor, Mode, Colors, DepthRes, GSC.Settings.MergeWithAbsoluteDepths);
                    point = InterpolateListToPoint(listMPField[i], ptNew, profile.Corridor);

                    // Empty line? Set mean altitude of the profile.
                    if ((line.HF.Entries == null) && (line.NF.Entries == null))
                    {
                        double al = ((alMax == double.MinValue) || (alMin == double.MaxValue)) ? 0 : ((alMax + alMin) / 2.0);
                        line.CoordRvHv = new Coordinate(ptNew.X, ptNew.Y, (alMax + alMin) / 2.0, CoordinateType.TransverseMercator);
                    }
                    
                    if (MergeManualPoints)
                    {
                        if (point != null)
                        {
                            point.Tag = line;
                            manualPoints.Add(point);
                        }
                    }
                    else if (listMPField[i] != null)
                    {
                        foreach (ManualPoint p in listMPField[i])
                        {
                            p.Tag = line;
                            manualPoints.Add(p);
                        }
                    }

                    sonarLines.Add(line);
                    sonarDevices[0].AddLine(line);

                    UpdateMaxDepth(line);

                    // Update profile bounding box.
                    RefreshCoordLimits(line.CoordRvHv , GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);
                }

            // Reset the antenna depth line.
            lastUpdatedLine = null;
        }

        /// <summary>
        /// Creates a snapshot of the current state.
        /// </summary>
        /// <returns>A record representing the current state of the profile.</returns>
        public SonarRecord CreateSnapshot()
        {
            SonarRecord record = new SonarRecord();

            record.Description = this.Description;
            record.Devices.Add(new SonarDevice(0, "Profile", 0, 0, 0, 0, 0, true, "", "", true, true, 0, 0, ""));
            record.TimeStart = timeStart;
            record.TimeEnd = timeEnd;
            record.ShowInTrace = false;
            record.ShowManualPoints = false;
            record.IsProfile = true;

            int i, max = sonarLines.Count;

            for (i = 0; i < max; i++)
                record.AddSonarLine(new SonarLine(sonarLines[i]), false);

            max = manualPoints.Count;

            for (i = 0; i < max; i++)
            {
                ManualPoint point = new ManualPoint(manualPoints[i]);

                if (manualPoints[i].Tag is SonarLine)
                    point.Tag = record.SonarLines()[sonarLines.IndexOf(manualPoints[i].Tag as SonarLine)];

                record.AddManualPoint(point);
            }

            record.RefreshDevices();

            return record;
        }

        #region Sort functions
        private int SortManualPoint(ManualPoint point, PointD ptStart)
        {
            int i = -1;

            if (!MergeManualPoints && NormalizeMP)
            {
                PointD ptn = profile.NormalizeToCorridor(point.Coord.Point);
                ManualPoint newPoint = new ManualPoint(point);
                point = newPoint;
                point.RV = ptn.X;
                point.HV = ptn.Y;
            }

            if (SamplingRes > 0)
            {
                i = (int)(Math.Floor(ptStart.Distance(point.GetPointRVHV()) / SamplingRes));

                if (listMPField[i] == null)
                    listMPField[i] = new List<ManualPoint>();

                listMPField[i].Add(point);
            }
            else
            {
                i = InsertManualPoint(point, ptStart);
            }

            return i;
        }

        private int InsertManualPoint(ManualPoint point, PointD ptStart)
        {
            double d = ptStart.Distance(point.GetPointRVHV());
            double d2 = 0.0;
            int i = 0;

            for (i = 0; i < manualPoints.Count; i++)
            {
                d2 = ptStart.Distance(manualPoints[i].GetPointRVHV());

                if (d < d2)
                    break;
            }

            manualPoints.Insert(i, point);

            return i;
        }

        private int SortSonarLine(SonarLine line, PointD ptStart)
        {
            double d, d2;
            int sortedCount;
            int i = -1;
            Coordinate coord = line.CoordRvHv;
            double al = coord.AL;

            if (al < alMin)
                alMin = al;
            if (al > alMax)
                alMax = al;

            d = ptStart.Distance(coord);

            if (SamplingRes > 0)
            {
                if (listSLField == null)
                    return -1;

                i = (int)(Math.Floor(d / SamplingRes));

                if (listSLField[i] == null)
                    listSLField[i] = new List<SonarLine>();

                int j = 0;

                for (j = 0; j < listSLField[i].Count; j++)
                    if (listSLField[i][j].CoordRvHv.AL < al)
                        break;

                listSLField[i].Insert(j, line);
            }
            else
            {
                sortedCount = sonarLines.Count;

                for (i = 0; i < sortedCount; i++)
                {
                    d2 = ptStart.Distance(sonarLines[i].CoordRvHv);

                    if (d < d2)
                        break;
                }

                lastUpdatedLine = line;
                sonarLines.Insert(i, line);
                sonarDevices[0].SonarLines.Insert(i, line);

                UpdateMaxDepth(line);
                
                // Update profile bounding box.
                RefreshCoordLimits(coord, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);
            }

            DateTime time = line.Time;

            if (((time.CompareTo(timeStart) < 0) || (timeStart == DateTime.MinValue)) && (time != DateTime.MinValue))
                timeStart = time;

            if (((time.CompareTo(timeEnd) > 0) || (timeEnd == DateTime.MinValue)) && (time != DateTime.MinValue))
                timeEnd = time;

            return i;
        } 
        #endregion
        #endregion

        #region XML r/w
        #region Read
        // Refer to special constructor.
        #endregion

        #region Write
        public override void Write(XmlTextWriter writerXML, BinaryWriter writerBin, bool binary)
        {
            // Write Header.
            WriteHeader(writerXML);

            // Omit sonar lines etc. as they will be regenerated when loaded.

            // Write footer.
            WriteFooter(writerXML);
        }

        protected override void WriteHeader(XmlTextWriter writer)
        {
            try
            {
                NumberFormatInfo nfi = GSC.Settings.NFI;

                // Start record node.
                writer.WriteStartElement("profile");

                // Write attributes.
                writer.WriteAttributeString("desc", desc);
                writer.WriteAttributeString("buoyconnectionID", profile.ID.ToString());
                writer.WriteAttributeString("normalizeSL", NormalizeSL.ToString());
                writer.WriteAttributeString("normalizeMP", NormalizeMP.ToString());
                writer.WriteAttributeString("samplingRes", SamplingRes.ToString(GSC.Settings.NFI));                
                writer.WriteAttributeString("mode", Mode.ToString());

                for (int i = 0; i < Colors.Length; i++)
                    writer.WriteAttributeString("color" + i.ToString(), Colors[i].ToString());

                writer.WriteAttributeString("mergeMP", MergeManualPoints.ToString());
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarProfile.WriteHeader: " + e.Message);
            }
        } 
        #endregion
        #endregion
    }
}
