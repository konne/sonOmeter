using System;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using UKLib.MathEx;
using UKLib.Survey.Math;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Summary description for SonarDevice.
    /// </summary>
    public class SonarDevice
    {
        #region Variables
        int sonID = 0;

        string desc = "No information available.";

        double dX = 0.0;
        double dY = 0.0;
        double dZ = 0.0;
        double dP = 0.0;
        double dR = 0.0;

        protected float maxDepth = Single.NaN;
        protected double alMax = Double.MinValue;
        protected double alMin = Double.MaxValue;

        bool showInTrace = true;
        bool isOpen = false;

        string licencedFor = "";
        string licenceNr = "";
        bool hf = true;
        bool nf = true;
        int sonicSpeed = 0;
        double depth = 0.0;
        string gpsMask = "";

        CutLineSet clSetHF = new CutLineSet(SonarPanelType.HF);
        CutLineSet clSetNF = new CutLineSet(SonarPanelType.NF);

        List<SonarLine> sonarLines = new List<SonarLine>();

        Form hostForm = null;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if the device is visible in the trace window.
        /// </summary>
        [Browsable(false)]
        public bool ShowInTrace
        {
            get { return showInTrace; }
            set { showInTrace = value; }
        }

        /// <summary>
        /// The set of cut lines for the HF sonar data.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public CutLineSet ClSetHF
        {
            get { return clSetHF; }
        }

        /// <summary>
        /// The set of cut lines for the NF sonar data.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public CutLineSet ClSetNF
        {
            get { return clSetNF; }
        }

        /// <summary>
        /// Gets or sets the opened state of a corresponding view.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        /// <summary>
        /// Gets or sets the ID of the device.
        /// </summary>
        [Browsable(false)]
        public int SonID
        {
            get { return sonID; }
            set { sonID = value; }
        }

        /// <summary>
        /// Gets or sets the sonar line list.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public virtual List<SonarLine> SonarLines
        {
            get { return sonarLines; }
            set { sonarLines = value; }
        }

        /// <summary>
        /// Gets the minimum altitude of all sonar lines.
        /// </summary>
        public virtual double ALMin
        {
            get { return alMin; }
        }

        /// <summary>
        /// Gets the maximum altitude of all sonar lines.
        /// </summary>
        public virtual double ALMax
        {
            get { return alMax; }
        }

        /// <summary>
        /// Gets or sets the device description.
        /// </summary>
        [Description("The device description."), Category("Device")]
        public virtual string Description
        {
            get { return desc; }
            set { desc = value; }
        }

        /// <summary>
        /// Gets or sets the device displacement of the x-coordinate.
        /// </summary>
        [Description("The x-coordinate of the displacement vector."), Category("Device"), DefaultValue(0.0)]
        public double DX
        {
            get { return dX; }
            set { dX = value; }
        }

        /// <summary>
        /// Gets or sets the device displacement of the y-coordinate.
        /// </summary>
        [Description("The y-coordinate of the displacement vector."), Category("Device"), DefaultValue(0.0)]
        public double DY
        {
            get { return dY; }
            set { dY = value; }
        }

        /// <summary>
        /// Gets or sets the device displacement of the z-coordinate.
        /// </summary>
        [Description("The z-coordinate of the displacement vector."), Category("Device"), DefaultValue(0.0)]
        public double DZ
        {
            get { return dZ; }
            set { dZ = value; }
        }

        /// <summary>
        /// Gets or sets the device pitch component.
        /// </summary>
        [Description("The pitch component of the displacement vector."), Category("Device"), DefaultValue(0.0)]
        public double DP
        {
            get { return dP; }
            set { dP = value; }
        }

        /// <summary>
        /// Gets or sets the device roll component.
        /// </summary>
        [Description("The roll component of the displacement vector."), Category("Device"), DefaultValue(0.0)]
        public double DR
        {
            get { return dR; }
            set { dR = value; }
        }

        /// <summary>
        /// Gets the location of the device in relation to the defined origin.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public Point3D Point
        {
            get { return new Point3D(dX, dY, dZ); }
        }

        [Browsable(false)]
        [XmlIgnore]
        public RotD Rotation
        {
            get { return new RotD(0.0, dP, dR); }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Form HostForm
        {
            get { return hostForm; }
            set { hostForm = value; }
        }

        public virtual string LicencedFor
        {
            get { return licencedFor; }
        }

        public virtual string LicenceNr
        {
            get { return licenceNr; }
        }

        public virtual bool HF
        {
            get { return hf; }
        }

        public virtual bool NF
        {
            get { return nf; }
        }

        public virtual int SonicSpeed
        {
            get { return sonicSpeed; }
        }

        public virtual double Depth
        {
            get { return depth; }
        }

        public virtual string GPSMask
        {
            get { return gpsMask; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="sonID">The device id.</param>
        /// <param name="desc">The device description.</param>
        /// <param name="dX">The device displacement of the x-coordinate.</param>
        /// <param name="dY">The device displacement of the y-coordinate.</param>
        /// <param name="dZ">The device displacement of the z-coordinate.</param>
        /// <param name="dP">The pitch component.</param>
        /// <param name="dR">The roll component.</param>
        /// <param name="show">The show flag.</param>
        /// <param name="licencedFor">The name of the person / company that owns the sonar device.</param>
        /// <param name="licenceNr">The licence number.</param>
        /// <param name="hf">Contains HF data.</param>
        /// <param name="nf">Contains NF data.</param>
        /// <param name="sonicSpeed">The sonic speed of the device.</param>
        /// <param name="depth">???</param>
        /// <param name="GPSmask">The GPS mask.</param>
        public SonarDevice(int sonID, string desc, double dX, double dY, double dZ, double dP, double dR, bool show, string licencedFor, string licenceNr, bool hf, bool nf, int sonicSpeed, double depth, string GPSmask)
        {
            if (desc == null)
                desc = "No information available.";

            if (desc.Length == 0)
                desc = "No information available.";

            this.sonID = sonID;
            this.desc = desc;
            this.dX = dX;
            this.dY = dY;
            this.dZ = dZ;
            this.showInTrace = show;

            this.licencedFor = licencedFor;
            this.licenceNr = licenceNr;
            this.hf = hf;
            this.nf = nf;
            this.sonicSpeed = sonicSpeed;
            this.depth = depth;
            this.gpsMask = GPSmask;
        }

        public SonarDevice()
        {
            // Do nothing.
        }

        public override string ToString()
        {
            return this.desc;
        }
        #endregion

        #region Line management
        public List<SonarLine> UpdateCoordinates(int start, int end, Point3D ptStart, Point3D ptDelta, RotD rotStart, RotD rotDelta, Transform tr)
        {
            // go alway the short way !!! change between for example .5° and 359.4°
            if (rotDelta.Yaw > 180)
            {
                rotDelta.Yaw = rotDelta.Yaw - 360;
            }

            if (rotDelta.Yaw < -180)
            {
                rotDelta.Yaw = rotDelta.Yaw + 360;
            }
            List<SonarLine> lines = new List<SonarLine>();

            if ((start < 0) || (sonarLines.Count == 0))
                return lines;
            else if (end < 0)
                end = start;

            int delta = end - start + 2;
            if (delta <= 0)
                return lines;

            for (int n = start; n <= end; n++)
            {
                double factor = (double)(n - start + 1) / (double)delta;
                Point3D ptNew = ptDelta * factor + ptStart;
                RotD rotNew = rotDelta * factor + rotStart;

                if (double.IsInfinity(ptNew.X) || double.IsInfinity(ptNew.Y) || double.IsInfinity(ptNew.Z))
                    return lines;

                UpdateCoordinateOfLine(sonarLines[n], ptNew, rotNew, tr);

                lines.Add(sonarLines[n]);
            }

            return lines;
        }

        /// <summary>
        /// Updates the coordinate of a specific line of the device.
        /// </summary>
        /// <param name="n">Index of the line in the device line list.</param>
        /// <param name="ptNew">New interpolated point or already given coordinate when NULL is passed.</param>
        /// <param name="rotNew">New interpolated rotation.</param>
        /// <param name="tr">Coordinate transformation object.</param>
        public void UpdateCoordinateOfLine(SonarLine line, Point3D? ptNew, RotD rotNew, Transform tr)
        {
            QuaternionD qDev = new QuaternionD(rotNew);
            QuaternionD qLine = new QuaternionD(new RotD(rotNew + this.Rotation));
            Point3D ptDev = new Point3D(dX, dY, dZ);

            ptDev = qDev.Rotate(ptDev);

            line.HF.CalculateRotCorrection(qLine);
            line.NF.CalculateRotCorrection(qLine);

            // If a value was passed, an interpolated point will be used.
            // Otherwise, the original source data will be used.
            if (!ptNew.HasValue)
                ptNew = line.CoordRvHv.Point3D;
            else
                line.SrcCoordType = CoordinateType.Empty;

            line.CoordRvHv = new Coordinate(ptNew.Value + ptDev.NEDtoENU, CoordinateType.TransverseMercator);
            line.CoordLaLo = tr.Run(line.CoordRvHv, CoordinateType.Elliptic);
            line.Rotation = new RotD(rotNew + this.Rotation);
        }

        /// <summary>
        /// Adds a new line to the device list.
        /// </summary>
        /// <param name="line">The sonar line to be added.</param>
        public void AddLine(SonarLine line)
        {
            sonarLines.Add(line);

            if (line.CoordRvHv.Type == CoordinateType.TransverseMercator)
            {
                double al = line.CoordRvHv.AL;

                if (al < alMin)
                    alMin = al;
                if (al > alMax)
                    alMax = al;
            }
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
        /// Returns the index of a specific sonar line.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <returns>The index of the given line.</returns>
        public int IndexOf(SonarLine line)
        {
            return sonarLines.IndexOf(line);
        }

        /// <summary>
        /// Rasterizes the surface.
        /// </summary>
        /// <param name="surface">The sonar line surface field.</param>
        /// <param name="gridX">The X grid.</param>
        /// <param name="gridY">The Y grid.</param>
        public void RasterizeSurface(object[,] surface, int xMin, int xMax, int yMin, int yMax, double gridX, double gridY)
        {
            int i = 0;
            int max = sonarLines.Count;
            SonarLine line;

            for (i = 0; i < max; i++)
            {
                line = sonarLines[i];
                Coordinate coord = line.CoordRvHv;

                // Filter lines without coordinates.
                if (coord.Type != CoordinateType.TransverseMercator)
                    continue;

                // Calculate the corresponding normalized point.
                int x = (int)Math.Floor(coord.RV / gridX);
                int y = (int)Math.Floor(coord.HV / gridY);

                if ((x < xMin) || (x > xMax) || (y < yMin) || (y > yMax))
                    continue;

                x -= xMin;
                y -= yMin;

                // Check limits of the Array, because of rounding errors
                if ((x < 0) | (x >= surface.GetLength(0)))
                    continue;

                if ((y < 0) | (y >= surface.GetLength(1)))
                    continue;


                // If this particular list is not present yet, create it.

                if (surface[x, y] == null)
                    surface[x, y] = new List<SonarLine>();



                List<SonarLine> list = surface[x, y] as List<SonarLine>;

                // Add the line to this list.
                // Sort it with highest lines first.
                int maxList = list.Count, j = 0;
                double al = line.CoordRvHv.AL;

                for (j = 0; j < maxList; j++)
                    if (list[j].CoordRvHv.AL < al)
                        break;

                list.Insert(j, line);
            }
        }

        /// <summary>
        /// Gets the lines inside the rectangle.
        /// </summary>
        /// <param name="rc">The rectangle.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        internal void GetRectangleLines(RectangleD rc, List<SonarLine> listSL, List<ManualPoint> listMP)
        {
            int i = 0;
            int max = sonarLines.Count;
            SonarLine line;

            for (i = 0; i < max; i++)
            {
                line = sonarLines[i];

                // Filter lines without coordinates.
                if (line.CoordRvHv.Type != CoordinateType.TransverseMercator)
                    continue;

                // Filter lines that are not inside the rectangle.
                if (rc.Contains(line.CoordRvHv.Point))
                    listSL.Add(line);
            }
        }

        /// <summary>
        /// Gets the lines inside the corridor.
        /// </summary>
        /// <param name="conn">The BuoyConnection.</param>
        /// <param name="listSL">The destination list of SonarLine objects.</param>
        /// <param name="listMP">The destination list of ManualPoint objects.</param>
        /// <param name="normalize">Use this flag to normalize the coordinates to the connection.</param>
        /// <param name="linkObjects">This flag controls the linking of the SonarLine and ManualPoint objects instead of creating new ones.</param>
        internal void GetCorridorLines(BuoyConnection conn, List<SonarLine> listSL, List<ManualPoint> listMP, bool normalize, bool linkObjects)
        {
            int i = 0;
            int max = sonarLines.Count;
            SonarLine line, newLine;
            PointD pt, ptn;
            double al;

            for (i = 0; i < max; i++)
            {
                line = sonarLines[i];

                // Filter lines without coordinates.
                if (line.CoordRvHv.Type != CoordinateType.TransverseMercator)
                    continue;

                pt = line.CoordRvHv.Point;
                al = line.CoordRvHv.AL;

                // Filter lines that are not inside the corridor.
                if (!conn.IsInCorridor(pt))
                    continue;

                // Add the object directly?
                if (linkObjects)
                {
                    listSL.Add(line);
                    continue;
                }

                // Normalize coordinate to the corridor connection and save it to the sonar line.
                if (normalize)
                    ptn = conn.NormalizeToCorridor(pt);
                else
                    ptn = pt;
                newLine = new SonarLine(line, new Coordinate(ptn.X, ptn.Y, al, CoordinateType.TransverseMercator));
                newLine.SonID = 0;
                listSL.Add(newLine);
            }
        }
        #endregion

        #region XML r/w
        /// <summary>
        /// Writes the device data to an XML stream.
        /// </summary>
        /// <param name="writer">The writer object.</param>
        public void Write(XmlTextWriter writer)
        {
            NumberFormatInfo nfi = GSC.Settings.NFI;

            int n = nfi.NumberDecimalDigits;
            nfi.NumberDecimalDigits = 1;

            writer.WriteStartElement("sondevice");
            writer.WriteAttributeString("sonid", sonID.ToString());
            writer.WriteAttributeString("dx", dX.ToString(nfi));
            writer.WriteAttributeString("dy", dY.ToString(nfi));
            writer.WriteAttributeString("dz", dZ.ToString(nfi));
            writer.WriteAttributeString("dp", dP.ToString(nfi));
            writer.WriteAttributeString("dr", dR.ToString(nfi));
            writer.WriteAttributeString("desc", desc);
            writer.WriteAttributeString("show", showInTrace.ToString());
            writer.WriteAttributeString("LicencedFor", licencedFor);
            writer.WriteAttributeString("LicenceNr", licenceNr);
            writer.WriteAttributeString("HF", hf.ToString());
            writer.WriteAttributeString("NF", nf.ToString());
            writer.WriteAttributeString("SonicSpeed", sonicSpeed.ToString(nfi));
            writer.WriteAttributeString("Depth", depth.ToString(nfi));
            writer.WriteAttributeString("GPSmask", gpsMask);
            writer.WriteEndElement();

            nfi.NumberDecimalDigits = n;
        }
        #endregion

        #region Cut lines
        /// <summary>
        /// Recalculates the cut lines of the device.
        /// </summary>
        public void RefreshCutLines()
        {
            SonarDevice.RefreshCutLine(clSetHF, sonarLines);
            SonarDevice.RefreshCutLine(clSetNF, sonarLines);
            SonarDevice.RefreshCutLine(ClSetHF, CutMode.CDepth, sonarLines, GSC.Settings.CalcdLinearisation);
            SonarDevice.RefreshCutLine(ClSetNF, CutMode.CDepth, sonarLines, GSC.Settings.CalcdLinearisation);
        }

        /// <summary>
        /// Recalculates the begin and end cut lines for the given set and sonar line array.
        /// </summary>
        /// <param name="clSet">The target cut line set.</param>
        /// <param name="lines">The sonar line array.</param>
        static public void RefreshCutLine(CutLineSet clSet, List<SonarLine> lines)
        {
            SonarDevice.RefreshCutLine(clSet, CutMode.Top, lines);
            SonarDevice.RefreshCutLine(clSet, CutMode.Bottom, lines);
            SonarDevice.RefreshCutLine(clSet, CutMode.CDepth, lines);
        }

        /// <summary>
        /// Recalculates the cut lines for a specific set with the given parameters and sonar line array.
        /// </summary>
        /// <param name="clSet">The target cut line set.</param>
        /// <param name="cutMode">The cut mode (Begin or End).</param>
        /// <param name="lines">The sonar line array.</param>
        static public void RefreshCutLine(CutLineSet clSet, CutMode cutMode, List<SonarLine> lines)
        {
            SonarDevice.RefreshCutLine(clSet, cutMode, lines, 0.0001F);
        }

        /// <summary>
        /// Recalculates the cut lines for a specific set with the given parameters and sonar line array.
        /// </summary>
        /// <param name="clSet">The target cut line set.</param>
        /// <param name="cutMode">The cut mode (Begin or End).</param>
        /// <param name="lines">The sonar line array.</param>
        /// <param name="linearization">The linearization of the lines</param>
        static public void RefreshCutLine(CutLineSet clSet, CutMode cutMode, List<SonarLine> lines, float linearization)
        {
            if ((clSet == null) || (cutMode == CutMode.Nothing))
                return;

            SonarLine prev = null;
            int x = 0, xStart = 0, xEnd = 0;
            float y = 0, yStart = 0, yEnd = 0;
            CutLine cl;
            LineData data;

            if (cutMode == CutMode.CDepth)
                cl = clSet.CutCalcDepth;
            else if (cutMode == CutMode.Top)
                cl = clSet.CutTop;
            else
                cl = clSet.CutBottom;

            cl.Clear();

            if (cutMode == CutMode.Bottom)
                y = (float)GSC.Settings.DepthBottom;
            else
                y = (float)GSC.Settings.DepthTop;

            int count = lines.Count;

            for (x = 0; x < count; x++)
            {
                SonarLine line = lines[x];

                if (clSet.PanelType == SonarPanelType.HF)
                    data = line.HF;
                else
                    data = line.NF;

                if (data == null)
                    continue;

                if (data.isNullEntries)
                    continue;

                if (cutMode == CutMode.Top)
                    y = data.tCut;
                else if (cutMode == CutMode.Bottom)
                    y = data.bCut;
                else if (!float.IsNaN(data.CDepth))
                    y = data.cDepth;
                else
                    continue;

                if (prev == null)
                {
                    cl.Add(0, y);

                    xEnd = xStart = 0;
                    yEnd = yStart = y;

                    if (x > 0)
                        xEnd = x;
                }
                else
                {
                    if (xEnd > xStart)
                    {
                        if (Math.Abs((yEnd - yStart) / (xEnd - xStart) - (y - yEnd) / (x - xEnd)) >= linearization)
                        {
                            cl.Add(xEnd, yEnd);

                            xStart = xEnd;
                            yStart = yEnd;
                        }
                    }

                    xEnd = x;
                    yEnd = y;
                }

                // Set prev. line for next iteration.
                prev = line;
            }

            cl.Add(xEnd, yEnd);
        }

        public void ApplyArchToCutLine(SonarPanelType type, CutMode mode)
        {
            LineData data;
            int max = sonarLines.Count;

            for (int i = 0; i < max; i++)
            {
                if (type == SonarPanelType.HF)
                    data = sonarLines[i].HF;
                else
                    data = sonarLines[i].NF;

                if (data == null)
                    return;

                if (mode == CutMode.Top)
                    data.tCut = data.Depth;
                else
                    data.bCut = data.Depth;
            }

            if (type == SonarPanelType.HF)
                RefreshCutLine(clSetHF, mode, sonarLines);
            else
                RefreshCutLine(clSetNF, mode, sonarLines);
        }
        #endregion

        #region CalcDeep
        private void Average()
        {
            int cnt = sonarLines.Count;
            int avg = GSC.Settings.CalcdAverage;

            for (int i = 0; i < cnt; i++)
            {
                float averageHF = 0F;
                float averageNF = 0F;
                int anzHF = 0;
                int anzNF = 0;
                for (int j = -avg; j <= avg; j++)
                {
                    if ((i + j >= 0) & (i + j < cnt))
                    {
                        if (!float.IsNaN(sonarLines[i + j].HF.CDepth))
                        {
                            anzHF++;
                            averageHF += sonarLines[i + j].HF.CDepth;
                        }
                        if (!float.IsNaN(sonarLines[i + j].NF.CDepth))
                        {
                            anzNF++;
                            averageNF += sonarLines[i + j].NF.CDepth;
                        }
                    }
                }
                averageHF = averageHF / anzHF;
                averageNF = averageNF / anzNF;
                sonarLines[i].HF.CDepth = averageHF;
            }
        }

        public void CalcD(bool isCut)
        {
            if (GSC.Settings.Lic[Module.Modules.CalcDepth])
            {

                float? lastdepthHF = null;
                float? lastdepthNF = null;
                int i;
                for (i = 0; i < sonarLines.Count; i++)
                {
                    SonarLine line = sonarLines[i];

                    lastdepthHF = line.HF.CalcD(isCut, lastdepthHF, line.SubDepth);
                    lastdepthNF = line.NF.CalcD(isCut, lastdepthNF, line.SubDepth);
                }
                Average();

                SonarDevice.RefreshCutLine(clSetHF, CutMode.CDepth, sonarLines, GSC.Settings.CalcdLinearisation);
                SonarDevice.RefreshCutLine(clSetNF, CutMode.CDepth, sonarLines, GSC.Settings.CalcdLinearisation);
            }
        }
        #endregion

        public void CorrectLineTimestamps(DateTime start, DateTime end)
        {
            if (sonarLines == null || sonarLines.Count == 0)
                return;

            bool invalidTimestamps = true;
            int i;

            for (i = 1; i < sonarLines.Count; i++)
            {
                if (sonarLines[i].Time - sonarLines[i - 1].Time != TimeSpan.Zero)
                {
                    invalidTimestamps = false;
                    break;
                }
            }

            if (invalidTimestamps)
            {
                var secondsPerLine = (end - start).TotalSeconds / sonarLines.Count;

                for (i = 0; i < sonarLines.Count; i++)
                    sonarLines[i].Time = start.AddSeconds(i * secondsPerLine);
            }
        }

        public SonarLine GetLastLineWithCoord()
        {
            int max = sonarLines.Count - 1;

            for (int i = max; i >= 0; i--)
            {
                if (sonarLines[i].CoordRvHv.Type == CoordinateType.TransverseMercator)
                    return sonarLines[i];
            }

            return null;
        }

        public SonarLine GetFirstLineWithCoord()
        {
            int max = sonarLines.Count;

            for (int i = 0; i < max; i++)
            {
                if (sonarLines[i].CoordRvHv.Type == CoordinateType.TransverseMercator)
                    return sonarLines[i];
            }

            return null;
        }

        public ArchBounds ApplyArchAndVolume(bool? isCut, float? clipTop = null)
        {
            ArchBounds bounds = new ArchBounds() { MaxHF = (float)GSC.Settings.DepthBottom, MinHF = (float)GSC.Settings.DepthTop, MaxNF = (float)GSC.Settings.DepthBottom, MinNF = (float)GSC.Settings.DepthTop };

            if ((sonarLines == null) || (sonarLines.Count == 0))
                return bounds;

            int max = sonarLines.Count;
            SonarLine line;

            for (int i = 0; i < max; i++)
            {
                line = sonarLines[i];
                if (isCut.HasValue)
                    line.IsCut = isCut.Value;
                else
                    isCut = line.IsCut;

                line.ApplyArchAndVolume(isCut.Value, clipTop);

                if (bounds.MaxHF < line.HF.Depth)
                    bounds.MaxHF = line.HF.Depth;
                if (bounds.MinHF > line.HF.Depth)
                    bounds.MinHF = line.HF.Depth;

                if (bounds.MaxNF < line.NF.Depth)
                    bounds.MaxNF = line.NF.Depth;
                if (bounds.MinNF > line.NF.Depth)
                    bounds.MinNF = line.NF.Depth;
            }

            return bounds;
        }
    }

    public enum VirtualSonarDeviceIDs
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }

    public class VirtualSonarDevice : SonarDevice
    {
        [Description("The ID of the virtual device. Use alphanumeric identifiers A, B, C etc."), Category("Device"), DefaultValue(VirtualSonarDeviceIDs.A)]
        public VirtualSonarDeviceIDs ID { get; set; }

        #region Hidden Attributes
        [Browsable(false)]
        [XmlIgnore]
        public override List<SonarLine> SonarLines
        {
            get { return base.SonarLines; }
            set { base.SonarLines = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override string Description
        {
            get { return base.Description; }
            set { base.Description = value; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override double ALMax
        {
            get { return base.ALMax; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override double ALMin
        {
            get { return base.ALMin; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override double Depth
        {
            get { return base.Depth; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override string GPSMask
        {
            get { return base.GPSMask; }
        }

        [Browsable(false)]
        public override bool HF
        {
            get { return base.HF; }
        }

        [Browsable(false)]
        public override bool NF
        {
            get { return base.NF; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override string LicencedFor
        {
            get { return base.LicencedFor; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override string LicenceNr
        {
            get { return base.LicenceNr; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public override int SonicSpeed
        {
            get { return base.SonicSpeed; }
        }
        #endregion

        [XmlIgnore]
        public int RefDeviceID = -1;
        [XmlIgnore]
        public int CoDeviceID = -1;
        [XmlIgnore]
        public SonarLine LastCoLine = null;
        [XmlIgnore]
        public SonarLine IntLine = null;
        [XmlIgnore]
        public int ColorID = 0;
        [XmlIgnore]
        public double IntDepth = 0.0;

        public VirtualSonarDevice()
        {
            ID = VirtualSonarDeviceIDs.A;
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public bool InterpolateLine(SonarRecord rec, SonarLine line, SonarPanelType type, ExportSettings cfg, double depth, int colorid)
        {
            if ((CoDeviceID == -1) || (RefDeviceID == -1))
                return false;

            if (CoDeviceID == line.SonID)
            {
                LastCoLine = line;
            }
            else if (RefDeviceID == line.SonID)
            {
                // Find next coDev line in record and substitute previous one if it is nearer.
                SonarLine nextLine = null;
                int max = rec.SonarLines().Count;

                for (int i = rec.IndexOf(line) + 1; i < max; i++)
                {
                    nextLine = rec.SonarLine(i);

                    if (nextLine.SonID == CoDeviceID)
                    {
                        if ((LastCoLine == null) || (LastCoLine.CoordRvHv.Point.Distance(line.CoordRvHv) > nextLine.CoordRvHv.Point.Distance(line.CoordRvHv)))
                        {
                            LastCoLine = nextLine;
                        }
                        break;
                    }
                }

                // No coDev line found.
                if (LastCoLine == null)
                    return false;

                // Now, interpolate!
                if (double.IsNaN(line.Rotation.Yaw))
                    return false;
                PointD ptRef = rec.Devices[RefDeviceID].Point.PointXY.Rotate(line.Rotation.Yaw);
                PointD pt = this.Point.PointXY.Rotate(line.Rotation.Yaw) - ptRef;

                Coordinate c = new Coordinate(line.CoordRvHv.RV + pt.X, line.CoordRvHv.HV + pt.Y, line.CoordRvHv.AL, CoordinateType.TransverseMercator);

                double dRef = line.CoordRvHv.Point.Distance(c);
                double dCo = LastCoLine.CoordRvHv.Point.Distance(c);
                double? prepDepth = null;

                if ((LastCoLine.PrepareExportData(out prepDepth, out ColorID, type, cfg) == null) && !prepDepth.HasValue)
                {
                    return false;
                }
                else
                {
                    IntDepth = (dCo * prepDepth.Value + dRef * depth) / (dCo + dRef);
                    ColorID = Math.Max(ColorID, colorid);
                }

                IntLine = new SonarLine();
                IntLine.CoordRvHv = c;

                return true;
            }

            return false;
        }
    }
}
