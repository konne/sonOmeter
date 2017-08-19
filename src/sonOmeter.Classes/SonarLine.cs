using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Globalization;
using UKLib.Survey.Math;
using UKLib.MathEx;
using UKLib.Xml;
using UKLib.Strings;
using UKLib.Survey.Parse;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace sonOmeter.Classes
{
    public enum TopColorMode
    {
        Top = 0,
        Vol = 1,
        Dep = 2
    }

    public struct DataEntry
    {
        #region Public Fields
        public float high;
        public float uncutHigh;
        public float low;

        public bool visible;

        public byte colorID;
        #endregion

        #region Constructor
        public DataEntry(float high, float low, byte colorID)
        {
            this.high = this.uncutHigh = high;
            this.low = low;
            this.colorID = colorID;
            this.visible = true;
        }

        public void InitFF(float high, float low)
        {
            this.high = high;
            this.uncutHigh = high;
            this.low = low;
            this.colorID = 0xff;
            this.visible = true;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return high.ToString() + " - " + low.ToString() + " : " + colorID.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Summary description for LineData.
    /// </summary>
    public class LineData
    {
        public enum MergeMode
        {
            Strongest,
            Occurance
        }

        #region Variables
        /// <summary>
        /// Top cut value.
        /// </summary>
        internal float tCut = 0.0F;
        /// <summary>
        /// Bottom cut value;
        /// </summary>
        internal float bCut = 0.0F;
        /// <summary>
        /// Calculated depth value.
        /// </summary>
        internal float cDepth = float.NaN;
        /// <summary>
        /// The maximum depth (uncut).
        /// </summary>
        internal float mDepth = float.NaN;
        /// <summary>
        /// The maximum depth (cut).
        /// </summary>
        internal float mDepthCut = float.NaN;

        internal Point3D rotCorrection = new Point3D();

        /// <summary>
        /// Volume value.
        /// </summary>
        float vol = -1;

        int lastCol = -1;
        public float Depth = 0;

        internal int topColor = -1;

        public DataEntry[] Entries = null;

        private long binfilepos = 0;
        #endregion      

        #region Properties
        public int TopColor
        {
            get { return topColor; }
            set { topColor = value; }
        }

        public float TCut
        {
            get { return tCut; }
            set { tCut = value; }
        }

        public float BCut
        {
            get { return bCut; }
            set { bCut = value; }
        }

        public float CDepth
        {
            get { return cDepth; }
            set { cDepth = value; }
        }

        public bool isNullEntries
        {
            get
            {
                return (Entries == null);
            }
        }

        public int LastCol
        {
            get { return lastCol; }
            set { lastCol = value; }
        }

        public float Volume
        {
            get { return vol; }
            set { vol = value; }
        }

        public Point3D RotCorrection
        {
            get { return rotCorrection; }
            set { rotCorrection = value; }
        }
        #endregion

        #region Volume calculation
        public float GetVolume()
        {
            if (vol < 0)
                return 0;
            else
                return vol;
        }

        public float GetVolume(bool reCalc)
        {
            /*if (reCalc || (vol < 0))
            {
                // Calculate new volume value.
                if (Entries == null)
                    return 0;

                float[] volCol = new float[7];
                bool active = GSC.Settings.ArchActive;
                int count = Entries.Length;
                DataEntry e;
                float low, high;

                for (int i = 0; i < count; i++)
                {
                    e = Entries[i];
                    low = e.low;
                    high = e.high;

                    if ((low > tCut) | (high < bCut))
                        continue;

                    if (low < bCut)
                        low = bCut;
                    if (high > tCut)
                        high = tCut;

                    if ((e.colorID < 0) | (e.colorID > 6))
                        continue;
                    if (active & (low > Depth))
                        continue;

                    try
                    {
                        if (active & (high > Depth))
                            volCol[e.colorID] += Depth - low;
                        else
                            volCol[e.colorID] += high - low;
                    }
                    catch
                    {
                        // Filter out all unwanted events. :-) 
                    }
                }

                vol = 0;

                for (int j = 0; j < 7; j++)
                {
                    if (volCol[j] >= 0.1)
                        vol += (float)System.Math.Log10(volCol[j] * 10) * GSC.Settings.SECL[j].VolumeWeight;
                }
            }*/

            return vol;
        }
        #endregion

        #region XML r/w
        public void Read(XmlTextReader readerXML, BinaryReader readerBin)
        {
            try
            {
                // Convert cutting data.
                bCut = XmlReadConvert.Read(readerXML, "ecut", -100.0F);
                tCut = XmlReadConvert.Read(readerXML, "bcut", 0.0F);

                // Calculated depth.
                cDepth = XmlReadConvert.Read(readerXML, "cdepth", float.NaN);

                // Read content.
                if (readerBin != null)
                {
                    #region Read binary data.
                    binfilepos = XmlReadConvert.Read(readerXML, "binfilepos", 0);

                    // wenn in den nachfolgenden Operationen einen Exception auftritt muss
                    // er auf jeden Fall eine "Zeile" weitergehen.
                    readerXML.Read();

                    if (readerBin.BaseStream.Position != binfilepos)
                        readerBin.BaseStream.Seek(binfilepos, SeekOrigin.Begin);
                
                    int max = readerBin.ReadInt32();

                    Entries = new DataEntry[max];
                    bool setTopCol = true;
                    for (int i = 0; i < max; i++)
                    {
                        Entries[i] = new DataEntry(readerBin.ReadSingle(), readerBin.ReadSingle(), readerBin.ReadByte());

                        if (Entries[i].uncutHigh > this.tCut)
                            Entries[i].high = Math.Max(Entries[i].low, this.tCut);
                        else
                            Entries[i].high = Entries[i].uncutHigh;

                        Entries[i].visible = (Entries[i].high > Entries[i].low) & (Entries[i].high > this.bCut);

                        if (setTopCol & Entries[i].visible)
                        {
                            this.topColor = Entries[i].colorID;
                            setTopCol = false;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Read XML data.
                    string encrypted = readerXML.GetAttribute("encrypted");
                    string line = readerXML.ReadString();
#if DEBUG
                    if (encrypted != null)
                        if (Convert.ToBoolean(encrypted))
                            line = Crypto.Decrypt(line);
#endif                  
                    try
                    {
                        // Parse through the string and get number of Entries.
                        string[] lines = line.Split('|');
                        int i = -1;
                        int n = lines.Length;

                        // Create entry array.
                        this.Entries = new DataEntry[n - 1];

                        // Fill it with data.
                        DataEntry entry;
                        bool setTopCol = true;
                        string currentLine;
                        int posDelimiter1, posDelimiter2;
                        for (i = 1; i < n; i++)
                        {
                            currentLine = lines[i];
                            posDelimiter1 = currentLine.IndexOf(',');
                            posDelimiter2 = currentLine.LastIndexOf(',');
                            entry.uncutHigh = FastConvert.ToSingle(currentLine.Substring(0, posDelimiter1));
                            entry.low = FastConvert.ToSingle(currentLine.Substring(posDelimiter1 + 1, posDelimiter2 - posDelimiter1 - 1));
                            entry.colorID = FastConvert.ToByte(currentLine.Substring(posDelimiter2 + 1));
                            entry.colorID--;

                            if (entry.uncutHigh > this.tCut)
                                entry.high = Math.Max(entry.low, this.tCut);
                            else
                                entry.high = entry.uncutHigh;

                            entry.visible = entry.high > entry.low;

                            if (entry.visible)
                            {
                                mDepth = entry.low;
                                entry.visible &= (entry.high > this.bCut);
                                if (entry.visible)
                                    mDepthCut = mDepth;
                            }

                            this.Entries[i - 1] = entry;

                            if (setTopCol & entry.visible)
                            {
                                this.topColor = entry.colorID;
                                setTopCol = false;
                            }
                        }                       
                    }
                    catch
                    {

                    }

                    bool b = (this.Entries == null); 
                    #endregion
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarLine.ReadData: " + e.Message);
            }
        }

        public void Write(XmlTextWriter writerXML, BinaryWriter writerBin, string type, bool binary)
        {
            if (Entries == null)
                return;

            try
            {
                NumberFormatInfo nfi = GSC.Settings.NFI;

                int n = nfi.NumberDecimalDigits;
                nfi.NumberDecimalDigits = 2;

                // Write start node.
                writerXML.WriteStartElement(type);

                // Write attributes.
                writerXML.WriteAttributeString("bcut", tCut.ToString(nfi));
                writerXML.WriteAttributeString("ecut", bCut.ToString(nfi));

                if (float.IsNaN(cDepth))
                    writerXML.WriteAttributeString("cdepth", "NaN");
                else
                    writerXML.WriteAttributeString("cdepth", (-cDepth).ToString(nfi));

                int max = Entries.Length;
                DataEntry entry;
                int i;

                if (binary)
                {
                    #region Write binary data.
                    if (writerBin != null)
                    {
                        binfilepos = writerBin.BaseStream.Position;

                        writerBin.Write(max);

                        for (i = 0; i < max; i++)
                        {
                            entry = Entries[i];

                            writerBin.Write(entry.uncutHigh);
                            writerBin.Write(entry.low);
                            writerBin.Write(entry.colorID);
                        }
                    }

                    writerXML.WriteAttributeString("binfilepos", binfilepos.ToString());
                    #endregion
                }
                else
                {
                    #region Write XML data.
                    int count = 1;
                    string s = "";

                    string[] elements = new string[3];
                    string[] block = new string[7];

                    block[0] = "\n          ";

                    for (i = 0; i < max; i++)
                    {
                        entry = Entries[i];

                        if (count == 7)
                        {
                            s += string.Join("|", block);
                            count = 1;
                        }

                        elements[0] = entry.uncutHigh.ToString("f", nfi);
                        elements[1] = entry.low.ToString("f", nfi);
                        elements[2] = FastConvert.ToString(entry.colorID + 1);

                        block[count] = string.Join(",", elements);

                        count++;
                    }

                    if (count > 1)
                        s += string.Join("|", block, 0, count);

                    s += "\n        ";

                    writerXML.WriteString(s);
                    #endregion
                }

                // Write end node.
                writerXML.WriteEndElement();

                nfi.NumberDecimalDigits = n;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Data management
        /// <summary>
        /// Creates a shallow copy of the object.
        /// </summary>
        /// <returns>A copy of the object.</returns>
        internal LineData Copy()
        {
            return this.MemberwiseClone() as LineData;
        }

        /// <summary>
        /// Removes all LineData elements that are invisible due to cut lines.
        /// </summary>
        internal void DropInvisibleData()
        {
            if (Entries == null)
                return;

            Collection<DataEntry> entryBuf = new Collection<DataEntry>();
            DataEntry entry;
            int count = Entries.Length;

            for (int i = 0; i < Entries.Length; i++)
            {
                entry = Entries[i];

                if (entry.low < bCut)
                {
                    if (entry.high > bCut)
                        entry.low = bCut;
                    else
                        entry.visible = false;
                }

                if (entry.visible)
                {
                    entry.uncutHigh = entry.high;
                    entryBuf.Add(entry);
                }
            }

            Entries = new DataEntry[entryBuf.Count];
            entryBuf.CopyTo(Entries, 0);
        }

        public float GetSurfaceHeight()
        {
            if (Entries == null)
            {
                return 0;
            }

            DataEntry entry = new DataEntry();
            int count = Entries.Length;

            for (int i = 0; i < count; i++)
            {
                entry = Entries[i];

                if (!entry.visible)
                    continue;

                if ((tCut <= entry.high) & (bCut >= entry.low))
                    entry.high = tCut;

                if (bCut >= entry.high)
                    return 0;
                else if (tCut < entry.high)
                    return tCut;
                else
                    return entry.high;
            }

            return float.NaN;
        }

        /// <summary>
        /// Merges the given array with the own data Entries.
        /// </summary>
        /// <param name="array">An array of other Entries (another SonarLine).</param>
        /// <param name="mode">The mode how to merge.</param>
        /// <param name="colors">An array of color toggle flags. Only color IDs passed with a true will be considered.</param>
        /// <param name="colorCounter">An array of integers that holds the numbers of each color at each height step.</param>
        /// <param name="depthResolution">The depth resolution.</param>
        /// <param name="offset">The depth offset.</param>
        public void MergeData(DataEntry[] array, MergeMode mode, bool[] colors, int[] colorCounter, double depthResolution, double offset, float? prevHigh = null, bool topDepthOnly = false)
        {
            if (array == null)
                return;

            DataEntry entry;
            int start = 0;
            int stop = 0;
            int max = array.Length;
            int i, j;
            byte colID, colID2;
            int entryCount = (mode == MergeMode.Occurance) ? colorCounter.Length / 7 : Entries.Length;
            bool first = true;

            for (i = 0; i < max; i++)
            {
                entry = array[i];

                // filter invisible Entries
                if (!entry.visible)
                    continue;

                // filter hidden colors
                colID = entry.colorID;
                if (!colors[colID])
                    continue;

                // calculate offset from previous depth interpolation step.
                if (first)
                {
                    if (prevHigh.HasValue)
                        offset += (double)(entry.high - prevHigh);
                }

                // calculate start and stop index in the array
                start = (int)Math.Round((offset - entry.high) / depthResolution);
                stop = (int)Math.Round((offset - entry.low) / depthResolution);

                if (start < 0)
                    start = 0;
                if (stop > entryCount)
                    stop = entryCount;

                for (j = start; j < stop; j++)
                    if (mode == MergeMode.Strongest)
                    {
                        colID2 = Entries[j].colorID;
                        if ((colID2 == 0xff) || (colID2 < colID))
                            Entries[j].colorID = colID;
                    }
                    else
                    {
                        colorCounter[j + colID * entryCount]++;
                    }

                // Break if only top depth has to be determined.
                if (first)
                {
                    if (topDepthOnly)
                        break;
                    first = false;
                }
            }
        }

        public void InitializeEntries(int count, float depthResolution)
        {            
            Entries = new DataEntry[count];

            float high = 0, low = 0;

            for (int i = 0; i < count; i++)
            {
                high = low;
                low = -(i + 1) * depthResolution;
                
                Entries[i].InitFF(high, low);
            }
        }

        public void InitializeEntries(int[] colorCounter, double depthResolution)
        {
            int count = colorCounter.Length / 7;
            
          
            Entries = new DataEntry[count];

            for (int i = 0; i < count; i++)
            {
                int max = 0;
                byte color = 0xff;

                for (byte j = 6; j != 0xff; j--)
                {
                    if (colorCounter[i + j * count] > max)
                    {
                        color = j;
                        max = colorCounter[i + j * count];
                    }
                }

                Entries[i] = new DataEntry(-(float)(i * depthResolution), -(float)((i - 1) * depthResolution), color);
            }
        }

        public void CompressEntries(float off)
        {
            Collection<DataEntry> compressed = new Collection<DataEntry>();

            DataEntry entry = new DataEntry();
            float high = off;
            float low = 0;
            byte lastColor = 0xff;
            int max = Entries.Length - 1;
            int i;

            // check on empty list
            if (max < 0)
                return;

            // compress the Entries
            for (i = 0; i <= max; i++)
            {
                entry = Entries[i];

                if ((entry.colorID == lastColor) & (i < max))
                {
                    low = entry.low + off;
                    continue;
                }

                if (lastColor != 0xff)
                {
                    if (compressed.Count == 0)
                    {
                        this.Depth = high;
                        this.cDepth = high;
                        this.tCut = high;
                        this.topColor = lastColor;
                    }

                    if (high > low)
                        compressed.Add(new DataEntry(high, low, lastColor));
                }

                high = entry.high + off;
                low = entry.low + off;
                lastColor = entry.colorID;
            }

            this.bCut = low;

            // copy the new list over the old one
            Entries = new DataEntry[compressed.Count];
            compressed.CopyTo(Entries, 0);
        }

        public void CalculateRotCorrection(QuaternionD q)
        {
            rotCorrection = new Point3D(0, 0, -this.Depth);
            rotCorrection = q.Rotate(rotCorrection).NEDtoENU;
        }
        #endregion

        #region Automatic cut module
        internal float? CalcD(bool isCut, float? lastdepth, float subDepth)
        {
            float max1 = float.NaN;
            int max1c = -1;
            
            if (this.Entries != null)
            {
                for (int j = 0; j < this.Entries.Length; j++)
                {
                    if ((isCut & this.Entries[j].visible) | !isCut)
                    {
                        float high = ((isCut) ? this.Entries[j].high : this.Entries[j].uncutHigh) - subDepth;
                        float low = this.Entries[j].low - subDepth;

                        // Clipped by upper boundary - continue searching.
                        if (low > GSC.Settings.CalcdTop)
                            continue;

                        // Clipped by lower boundary - break loop.
                        if (high < GSC.Settings.CalcdBottom)
                            break;
                        
                        int dl = this.Entries[j].colorID;

                        if (j + 1 < this.Entries.Length)
                        {
                            float high1 = ((isCut) ? this.Entries[j + 1].high : this.Entries[j + 1].uncutHigh) - subDepth;

                            if (low - high1 > GSC.Settings.CalcdMaxSpace)
                                dl = -1;
                        }
                        if (lastdepth.HasValue)
                        {
                            if (Math.Abs(lastdepth.Value - high) > GSC.Settings.CalcdMaxChange)
                            {
                                dl = -1;
                            }
                        }

                        if (dl > max1c)
                        {
                            max1c = dl;
                            max1 = high;
                        }
                    }
                }

                this.CDepth = max1;
                if (max1 <= 0)
                    lastdepth = max1;
            }

            return lastdepth;
        }
        #endregion
    }

    /// <summary>
    /// Summary description for SonarLine.
    /// </summary>
    public class SonarLine
    {
        #region Variables
        int sonID = 0;

        LineData dataHF = new LineData();
        LineData dataNF = new LineData();

        internal DateTime time = DateTime.MinValue;
        
        Collection<SonarPos> posList = new Collection<SonarPos>();

        Coordinate coordRvHv;
        Coordinate coordLaLo;

        CoordinateType srcCoordType = CoordinateType.Empty;

        bool isMarked = false;
        bool isCut = true;
        bool isProfile = false;
        bool readError = false;

        static float lastSubDepth = 0;
        float subDepth = float.NaN;

        double compassAngle = double.NaN;

        RotD rotation = new RotD(double.NaN, double.NaN, double.NaN);

        double antDepth = double.NaN;
        #endregion      

        #region Properties
        public double CompassAngle
        {
            get { return compassAngle; }
        }

        public RotD Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public double AntDepth
        {
            get { return antDepth; }
            set { antDepth = value; }
        }

        public PointD LockOffset
        {
            get
            {
                Point3D pt = new Point3D(GSC.Settings.LockOffsetX, GSC.Settings.LockOffsetY, 0);

                if (!double.IsNaN(rotation.Yaw))
                {
                    QuaternionD qDev = new QuaternionD(rotation);
                    pt = qDev.Rotate(pt);
                }

                return pt.NEDtoENU.PointXY;
            }
        }

        public float SubDepth
        {
            get
            {
                if (float.IsNaN(subDepth))
                {
                    subDepth = lastSubDepth;

                    foreach (SonarPos pos in posList)
                    {
                        if (pos.Disabled)
                            continue;

                        if (pos.type != PosType.Depth)
                            continue;

                        Nullable<float> f = SosoDepthGauge.ToFloat(pos.strPos);
                        if (f.HasValue)
                        {
                            subDepth = f.Value;
                            break;
                        }                       
                    }
                }

                lastSubDepth = subDepth;
                return subDepth;
            }
        }

        public int SonID
        {
            get { return sonID; }
            set { sonID = value; }
        }

        public LineData HF
        {
            get { return dataHF; }
        }

        public LineData NF
        {
            get { return dataNF; }
        }

        public Collection<SonarPos> PosList
        {
            get { return posList; }
        }

        public bool IsMarked
        {
            get { return isMarked; }
            set { isMarked = value; }
        }

        public bool IsCut
        {
            get { return isCut; }
            set { isCut = value; }
        }

        public bool IsProfile
        {
            get { return isProfile; }
            set { isProfile = value; }
        }

        public bool ReadError
        {
            get { return readError; }
        }

        public bool IsInterpolated
        {
            get { return srcCoordType == CoordinateType.Empty; }
        }

        public bool IsEmpty
        {
            get { return (dataHF.Entries == null) && (dataNF.Entries == null); }
        }

        public Coordinate CoordRvHv
        {
            get { return coordRvHv; }
            set { coordRvHv = value; }
        }

        public Coordinate CoordLaLo
        {
            get { return coordLaLo; }
            set { coordLaLo = value; }
        }

        public Coordinate SrcCoord
        {
            get
            {
                if (srcCoordType == CoordinateType.Elliptic)
                    return coordLaLo;
                else if (srcCoordType == CoordinateType.TransverseMercator)
                    return coordRvHv;
                else
                    return new Coordinate(0, 0, 0, srcCoordType);
            }
        }

        public CoordinateType SrcCoordType
        {
            set { srcCoordType = value; }
            get { return srcCoordType; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        #endregion

        #region Constructors
        public SonarLine()
        {
        }

        public SonarLine(SonarLine line, Coordinate coordRvHv)
        {
            this.coordLaLo = Coordinate.Empty;
            this.coordRvHv = coordRvHv;
            this.srcCoordType = CoordinateType.TransverseMercator;
            this.dataHF = line.dataHF;
            this.dataNF = line.dataNF;
            this.isCut = line.isCut;
            this.isProfile = line.isProfile;
            this.isMarked = line.isMarked;
            this.sonID = line.sonID;
            this.time = line.time;
        }

        public SonarLine(XmlTextReader readerXML, BinaryReader readerBin, DateTime date, bool isBinary)
        {
            readError = Read(readerXML, readerBin, date, isBinary);
        }

        public SonarLine(SonarLine line)
        {
            this.coordLaLo = line.coordLaLo;
            this.coordRvHv = line.coordRvHv;
            this.srcCoordType = line.srcCoordType;
            this.dataHF = line.dataHF.Copy();
            this.dataNF = line.dataNF.Copy();
            this.isCut = line.isCut;
            this.isProfile = line.isProfile;
            this.isMarked = line.isMarked;
            this.sonID = line.sonID;
            this.time = line.time;
        }

        internal void SetCoordinate(Coordinate coordinate)
        {
            srcCoordType = coordinate.Type;

            if (srcCoordType == CoordinateType.TransverseMercator)
                coordRvHv = coordinate;
            else if (srcCoordType == CoordinateType.Elliptic)
                coordLaLo = coordinate;
        }
        #endregion

        #region XML r/w
        public bool Read(XmlTextReader readerXML, BinaryReader readerBin, DateTime date, bool isBinary)
        {
            SonarPos pos;
            string prevValue = "";

            // Parse the line.
            while (!readerXML.EOF)
            {
                if ((readerXML.Name == "line") && (readerXML.NodeType == XmlNodeType.EndElement))
                    break;

                if (readerXML.NodeType != XmlNodeType.Element)
                {
                    prevValue = readerXML.Value;
                    readerXML.Read();

                    if ((prevValue != "") && (readerXML.Value == prevValue))
                        return true;
                    
                    continue;
                }

                switch (readerXML.Name)
                {
                    case "line":
                        // Set line ID.
                        sonID = XmlReadConvert.Read(readerXML, "sonid", 0);

                        time = date;

                        if (readerXML.GetAttribute("time") != null)
                            time = time.Add(DateTime.Parse(readerXML.GetAttribute("time")).TimeOfDay);

                        if (readerXML.GetAttribute("profile") != null)
                            isProfile = bool.Parse(readerXML.GetAttribute("profile"));

                        try
                        {
                            string rv = readerXML.GetAttribute("rv");
                            string hv = readerXML.GetAttribute("hv");
                            string alRVHV = readerXML.GetAttribute("alRVHV");

                            string la = readerXML.GetAttribute("la");
                            string lo = readerXML.GetAttribute("lo");
                            string alLALO = readerXML.GetAttribute("alLALO");

                            string srctype = readerXML.GetAttribute("srctype");

                            rotation.Yaw = XmlReadConvert.Read(readerXML, "angle", double.NaN);
                            rotation.Roll = XmlReadConvert.Read(readerXML, "roll", double.NaN);
                            rotation.Pitch = XmlReadConvert.Read(readerXML, "pitch", double.NaN);

                            srcCoordType = CoordinateType.Empty;

                            if (rv != null)
                            {
                                coordRvHv = new Coordinate(FastConvert.ToDouble(rv), FastConvert.ToDouble(hv), FastConvert.ToDouble(alRVHV), CoordinateType.TransverseMercator);
                                srcCoordType = CoordinateType.TransverseMercator;
                            }

                            if (la != null)
                            {
                                coordLaLo = new Coordinate(FastConvert.ToDouble(la), FastConvert.ToDouble(lo), FastConvert.ToDouble(alLALO), CoordinateType.Elliptic);
                                if (srcCoordType == CoordinateType.Empty)
                                    srcCoordType = CoordinateType.Elliptic;
                            }

                            if (srctype != null)
                                srcCoordType = (CoordinateType)FastConvert.ToInt16(srctype);

                            if (readerXML.IsEmptyElement)
                            {
                                readerXML.Read();
                                return true;
                            }
                        }
                        catch
                        {
                        }
                        readerXML.Read();
                        break;

                    case "HF":
                        // Fill HF data.
                        if (dataHF == null)
                            dataHF = new LineData();

                        if (isBinary && (readerBin == null))
                            readerXML.Read();
                        else
                            dataHF.Read(readerXML, readerBin);
                        break;

                    case "NF":
                        // Fill NF data.
                        if (dataNF == null)
                            dataNF = new LineData();

                        if (isBinary && (readerBin == null))
                            readerXML.Read();
                        else
                            dataNF.Read(readerXML, readerBin);
                        break;

                    case "pos":
                        // Insert original pos data.
                        pos = new SonarPos(time.Date, readerXML.GetAttribute("time"), readerXML.GetAttribute("type"), XmlReadConvert.Read(readerXML, "disabled", false), XmlReadConvert.Read(readerXML, "used", false), XmlReadConvert.Read(readerXML, "realpos", false), readerXML.ReadString());
                        if (pos.strPos.Length > 0)
                            posList.Add(pos);
                        break;
                    default:
                        readerXML.Read();
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Writes the SonarLine into an XML stream combined with a binary LineData stream.
        /// </summary>
        /// <param name="writerXML">The XML stream writer.</param>
        /// <param name="writerBin">The binary stream writer.</param>
        /// <param name="interpolatedTime">The interpolated time.</param>
        /// <param name="binary">A flag which indicates a binary record.</param>
        public void Write(XmlTextWriter writerXML, BinaryWriter writerBin, DateTime interpolatedTime, bool binary)
        {
            try
            {
                if (time != interpolatedTime)
                    time = interpolatedTime;

                NumberFormatInfo nfi = GSC.Settings.NFI;

                // Write start node.
                writerXML.WriteStartElement("line");

                // Write attributes.
                writerXML.WriteAttributeString("sonid", sonID.ToString());
                writerXML.WriteAttributeString("time", time.ToLongTimeString());

                if (coordRvHv.Type == CoordinateType.TransverseMercator)
                {
                    writerXML.WriteAttributeString("rv", coordRvHv.RV.ToString(nfi));
                    writerXML.WriteAttributeString("hv", coordRvHv.HV.ToString(nfi));
                    writerXML.WriteAttributeString("alRVHV", coordRvHv.AL.ToString(nfi));
                }
                if (coordLaLo.Type == CoordinateType.Elliptic)
                {
                    writerXML.WriteAttributeString("la", coordLaLo.LA.ToString(nfi));
                    writerXML.WriteAttributeString("lo", coordLaLo.LO.ToString(nfi));
                    writerXML.WriteAttributeString("alLALO", coordLaLo.AL.ToString(nfi));
                }

                if (!double.IsNaN(rotation.Yaw))
                    writerXML.WriteAttributeString("angle", rotation.Yaw.ToString(nfi));
                if (!double.IsNaN(rotation.Roll))
                    writerXML.WriteAttributeString("roll", rotation.Roll.ToString(nfi));
                if (!double.IsNaN(rotation.Pitch))
                    writerXML.WriteAttributeString("pitch", rotation.Pitch.ToString(nfi));

                writerXML.WriteAttributeString("srctype", FastConvert.ToString((int)srcCoordType));

                // Write HF and NF data.
                dataHF.Write(writerXML, writerBin, "HF", binary);
                dataNF.Write(writerXML, writerBin, "NF", binary);

                // Write pos strings.
                SonarPos pos;
                int max = posList.Count;
                int i;

                for (i = 0; i < max; i++)
                {
                    pos = posList[i] as SonarPos;
                    writerXML.WriteStartElement("pos");
                    writerXML.WriteAttributeString("time", FastConvert.ToString(pos.time));
                    writerXML.WriteAttributeString("type", FastConvert.ToString((int)pos.type));
                    writerXML.WriteAttributeString("disabled", pos.Disabled.ToString());
                    writerXML.WriteAttributeString("realpos", pos.RealPos.ToString());
                    writerXML.WriteAttributeString("used", pos.Used.ToString());
                    writerXML.WriteString(pos.strPos);
                    writerXML.WriteEndElement();
                }

                // Write end node.
                writerXML.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "SonarLine.Write: " + e.Message);
            }
        }
        #endregion

        #region ScanPositions
        /// <summary>
        /// Evaluates the entries of the position list for usable coordinates and auxiliary information.
        /// </summary>
        /// <param name="record">A record, whose bounding box shall be updated.</param>
        internal void ScanPositions(SonarRecord record)
        {
            Transform tr;
            SonarPos emptyPos = new SonarPos(time.Date, "00:00:00", "0", false, "");
            SonarPos lastPos = emptyPos;
            string lastType = "";

            compassAngle = double.NaN;
            rotation.Pitch = double.NaN;
            rotation.Roll = double.NaN;

            int max = posList.Count;

            for (int i = 0; i < max; i++)
            {
                SonarPos pos = posList[i] as SonarPos;
                PosType type = pos.type;

                if (type == PosType.Fixed)
                    continue;

                if (pos.Disabled)
                {
                    pos.Used = false;
                    continue;
                }

                if (GSC.Settings.CompassVector)
                {
                    // Other position types.
                    switch (pos.type)
                    {
                        case PosType.Compass:
                                rotation = new Compass(pos.strPos) - GSC.Settings.CompassZero;
                                if (!GSC.Settings.UsePitchRoll)
                                    rotation = new RotD(rotation.Yaw, 0, 0);                                
                                compassAngle = rotation.Yaw;
                            break;
                        case PosType.HMR3300:
                                rotation = new HMR3300(pos.strPos) - GSC.Settings.CompassZero;
                                if (!GSC.Settings.UsePitchRoll)
                                    rotation = new RotD(rotation.Yaw, 0, 0);
                                compassAngle = rotation.Yaw;
                            break;
                        case PosType.NMEA:
                            NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);

                            if (nmea.Type == "$SBG01")
                            {
                                rotation = new RotD(nmea.Yaw, nmea.Pitch, nmea.Roll) - GSC.Settings.CompassZero;
                                if (!GSC.Settings.UsePitchRoll)
                                    rotation = new RotD(rotation.Yaw, 0, 0);
                                compassAngle = rotation.Yaw;
                            }
                            break;
                    }

                    if (compassAngle > 360) 
                        compassAngle = compassAngle - 360;
                    if (compassAngle < 0) 
                        compassAngle = compassAngle + 360;
                }

                

                if ((type == GSC.Settings.ScanPosType) | (lastPos.type != GSC.Settings.ScanPosType))
                {
                    pos.Used = false;

                    switch (type)
                    {
                        #region NMEA
                        case PosType.NMEA:
                            NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);

                            string nmeaType = nmea.Type;
                            if (nmeaType.StartsWith("$GN"))
                                nmeaType = nmeaType.Replace("$GN", "$GP");

                            if ((nmeaType != "$GPLLQ") & (nmeaType != "$GPLLK") & (nmeaType != "$GPGGA") & (nmeaType != "$PTNL"))
                                continue;
                            pos.RealPos = true;

                            bool thisTypeFound = false;

                            foreach (string s in GSC.Settings.ScanPosStrings)
                            {
                                if (!thisTypeFound && (s.ToUpper() == nmea.Type.ToUpper()))
                                    thisTypeFound = true;
                            }

                            if (!thisTypeFound)
                                continue;

                            if ((nmea.Quality < GSC.Settings.MinQuality) ||
                                (nmea.Satellites < GSC.Settings.MinSatellites) ||
                                (nmea.QualityMeter > GSC.Settings.MinQualityMeter))
                                continue;                           

                            switch (nmeaType)
                            {
                                case "$GPGGA":
                                    tr = GSC.Settings.InversTransform;

                                    srcCoordType = CoordinateType.Elliptic;
                                    coordLaLo = new Coordinate(Trigonometry.Grad2Rad(nmea.Latitude.Value), Trigonometry.Grad2Rad(nmea.Longitude.Value), (GSC.Settings.UseWGSAltitude ? nmea.AltitudeWGS84 : nmea.Altitude), CoordinateType.Elliptic);
                                    coordRvHv = tr.Run(coordLaLo, CoordinateType.TransverseMercator);

                                    lastPos = pos;
                                    lastType = nmea.Type;
                                    break;
                                case "$GPLLK":
                                    tr = GSC.Settings.ForwardTransform;

                                    srcCoordType = CoordinateType.TransverseMercator;
                                    coordRvHv = new Coordinate(nmea.RightValue, nmea.HighValue, nmea.Altitude, CoordinateType.TransverseMercator);
                                    coordLaLo = tr.Run(coordRvHv, CoordinateType.Elliptic);

                                    lastPos = pos;
                                    lastType = nmea.Type;
                                    break;
                                case "$PTNL":
                                    tr = GSC.Settings.ForwardTransform;

                                    srcCoordType = CoordinateType.TransverseMercator;
                                    coordRvHv = new Coordinate(nmea.RightValue, nmea.HighValue, nmea.Altitude, CoordinateType.TransverseMercator);
                                    coordLaLo = tr.Run(coordRvHv, CoordinateType.Elliptic);

                                    lastPos = pos;
                                    lastType = nmea.Type;
                                    break;
                                case "$GPLLQ":
                                    tr = GSC.Settings.ForwardTransform;

                                    srcCoordType = CoordinateType.TransverseMercator;
                                    coordRvHv = new Coordinate(nmea.RightValue, nmea.HighValue, nmea.Altitude, CoordinateType.TransverseMercator);
                                    coordLaLo = tr.Run(coordRvHv, CoordinateType.Elliptic);

                                    lastPos = pos;
                                    lastType = nmea.Type;
                                    break;

                                default:
                                    continue;
                            }
                            break;
                        #endregion

                        #region LeicaTPS
                        case PosType.LeicaTPS:
                            LeicaTPS tps = new LeicaTPS(pos.strPos);
                            pos.RealPos = true;

                            tr = GSC.Settings.ForwardTransform;

                            srcCoordType = CoordinateType.TransverseMercator;
                            coordRvHv = new Coordinate(tps.RightValue, tps.HighValue, tps.Altitude, CoordinateType.TransverseMercator);
                            coordLaLo = tr.Run(coordRvHv, CoordinateType.Elliptic);

                            lastPos = pos;
                            break;
                        #endregion

                        #region Geodimeter
                        case PosType.Geodimeter:
                            Geodimeter geo = new Geodimeter(pos.strPos);
                            pos.RealPos = true;

                            tr = GSC.Settings.ForwardTransform;

                            srcCoordType = CoordinateType.TransverseMercator;
                            coordRvHv = new Coordinate(geo.RightValue, geo.HighValue, geo.Altitude, CoordinateType.TransverseMercator);
                            coordLaLo = tr.Run(this.CoordRvHv, CoordinateType.Elliptic);

                            lastPos = pos;
                            break;
                        #endregion

                        default:
                            break;
                    }

                    pos.Used = true;

                    if (lastPos == pos)
                    {
                        if ((record != null) && !record.RefreshCoordLimits(coordRvHv, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold))
                        {
                            pos.Used = false;
                            lastPos = emptyPos;
                            coordRvHv = Coordinate.Empty;
                            coordLaLo = Coordinate.Empty;
                        }
                    }

                    for (int j = 0; j < i; j++)
                    {
                        pos = posList[j] as SonarPos;
                        if (pos.type == type)
                            pos.Used = false;
                    }
                }           
            }
        }
        #endregion

        #region Data handling
        public float GetMaxDepth(bool isCut)
        {
            float dHF = (isCut ? dataHF.mDepthCut : dataHF.mDepth);
            float dNF = (isCut ? dataNF.mDepthCut : dataNF.mDepth);

            if (float.IsNaN(dHF))
                dHF = dNF;
            else if (!float.IsNaN(dNF))
                dHF = Math.Min(dHF, dNF);

            return dHF;
        }

        public LineData PrepareExportData(out double? depth, out int colorid, SonarPanelType type, ExportSettings cfg)
        {
            depth = null;
            colorid = 0;

            #region Select line data.
            LineData data = null;

            switch (type)
            {
                case SonarPanelType.HF:
                    data = this.HF;
                    break;
                case SonarPanelType.NF:
                    data = this.NF;
                    break;
                default:
                    data = null;
                    break;
            }

            if (data == null)
                return null;

            if (data.Entries == null)
                return null;
            #endregion

            try
            {
                #region Select appropriate color and depth depending on the cut state.
                if (!cfg.ExportWithCut)
                {
                    // Export plain depth (uncut)
                    foreach (DataEntry entry in data.Entries)
                    {
                        if ((entry.uncutHigh > cfg.ExportMinDepth) | (entry.uncutHigh < cfg.ExportMaxDepth))
                            continue;

                        depth = entry.uncutHigh;
                        colorid = entry.colorID;
                        break;
                    }
                }
                else
                {
                    // Export cut depth
                    foreach (DataEntry entry in data.Entries)
                    {
                        if ((entry.high > cfg.ExportMinDepth) | (entry.high < cfg.ExportMaxDepth))
                            continue;

                        if (entry.visible)
                        {
                            depth = entry.high;
                            colorid = entry.colorID;
                            break;
                        }

                    }
                }
                #endregion

                // From Ticket #136:
                // If arch is enabled, then export the arch depth
                // If no depth was found in the previous step, the line ist empty and shall therefore be ignored.
                if (cfg.ExportWithArch & depth.HasValue)
                {
                    if ((data.Depth < depth.Value) & (data.Depth <= cfg.ExportMinDepth) & (data.Depth >= cfg.ExportMaxDepth))
                    {
                        depth = data.Depth;
                        colorid = data.TopColor;
                    }
                }
                
            }
            catch
            {
                return null;
            }

            return data;
        }

        public LineData GetData(SonarPanelType type)
        {
            return (type == SonarPanelType.HF) ? dataHF : dataNF;
        }

        public Coordinate ExtrapolateCoord(double depth)
        {
            Coordinate c = Coordinate.Empty;

            if ((rotation != null) && (coordRvHv.Type == CoordinateType.TransverseMercator))
            {
                QuaternionD q = new QuaternionD(rotation);
                c = new Coordinate(q.Rotate(new Point3D(0, 0, -depth)).NEDtoENU + coordRvHv.Point3D, CoordinateType.TransverseMercator);
            }

            return c;
        }
        #endregion

        #region Arch and Volume
        /// <summary>
        /// Applies the virtual archeology and volume settings to the SonarLine data.
        /// </summary>
        /// <param name="cut">The cut flag.</param>
        public void ApplyArchAndVolume(bool cut, float? clipTop = null)
        {
            float archDepth = GSC.Settings.ArchDepth;
            float archTopColorDepth = GSC.Settings.ArchDepthsIndependent ? GSC.Settings.ArchTopColorDepth : archDepth;

            if (!clipTop.HasValue)
                clipTop = (float)GSC.Settings.DepthTop;

            bool archActive = GSC.Settings.ArchActive;
            
            ApplyArchAndVolume(SonarPanelType.HF, cut, archActive, archDepth, archTopColorDepth, (float)clipTop);
            ApplyArchAndVolume(SonarPanelType.NF, cut, archActive, archDepth, archTopColorDepth, (float)clipTop);
        }

        /// <summary>
        /// Applies the virtual archeology and volume settings to the SonarLine data.
        /// </summary>
        /// <param name="data">The data element type (HF or NF).</param>
        /// <param name="cut">The cut flag.</param>
        /// <param name="archActive">The arch flag.</param>
        /// <param name="archDepth">The arch depth.</param>
        /// <param name="archTopColorDepth">The depth of the top color.</param>
        public void ApplyArchAndVolume(SonarPanelType type, bool cut, bool archActive, float archDepth, float archTopColorDepth, float clipTop)
        {
            LineData data = GetData(type);
            float dSub = this.SubDepth;
            
            if (data == null)
                return;

            bool start = false;
            float dStart = 0;
            int lastCol = -1;
            int i = 0, j = 0;
            int max = 0;
            int colCount = GSC.Settings.SECL.Count;

            DataEntry entry;
            DataEntry nextEntry = new DataEntry();

            // Speed up things by prefetching arch settings.
            bool archDeptshIndependent = GSC.Settings.ArchDepthsIndependent;
            bool archStopAtStrongLayer = GSC.Settings.ArchStopAtStrongLayer;
            BindingList<SonarEntryConfig> SECL = GSC.Settings.SECL;
            float TCut = data.TCut;
            float BCut = data.BCut;

            if (TCut > clipTop)
                TCut = clipTop;
            
            int dLastCol = -1;
            int dTopColor = -1;
            float dDepth = dStart - archDepth;

            double[] volCol = new double[colCount];

            bool breakDepth = false;
            bool breakColor = false;

            if (data.Entries != null)
            {
                try
                {
                    data.TopColor = -1;
                    max = data.Entries.Length;

                    if (max > 0)
                        nextEntry = data.Entries[0];

                    // UBOOT
                    nextEntry.high -= dSub;
                    nextEntry.low -= dSub;
                    nextEntry.uncutHigh -= dSub;
                    // UBOOT
                        
                    for (i = 1; i <= max; i++)
                    {
                        entry = nextEntry;
                        if (i != max)
                            nextEntry = data.Entries[i];

                        // UBOOT
                        nextEntry.high -= dSub;
                        nextEntry.low -= dSub;
                        nextEntry.uncutHigh -= dSub;
                        // UBOOT

                        if ((entry.low < BCut) & cut)
                            entry.low = BCut;

                        if (!start)
                        {
                            if ((entry.low > TCut) & cut)
                                continue;

                            start = true;

                            if ((entry.high > TCut) & cut)
                                dStart = TCut;          // Use cut line border as start.
                            else
                                dStart = entry.high;    // Use top of the element.
                            
                            if (!archActive)
                            {
                                // Arch is not active - take the first element.
                                dLastCol = entry.colorID;
                                dTopColor = entry.colorID;
                                dDepth = dStart;
                                break;
                            }
                        }
                        else if ((entry.high < BCut) & cut)
                        {
                            // No more valid data available - take -1 color and standard depth.
                            dLastCol = -1;
                            dTopColor = -1;
                            dDepth = dStart - archDepth;
                            break;
                        }
                        else
                        {
                            if ((entry.high < dStart - archTopColorDepth) && !breakColor)
                            {
                                // Element is out of range - take previous color.
                                dLastCol = lastCol;
                                dTopColor = lastCol;
                                breakColor = true;
                            }

                            if ((entry.high < dStart - archDepth) && !breakDepth)
                            {
                                // Element is out of range - take standard depth.
                                // removed by archTopColorDepth: dLastCol = lastCol;
                                // removed by archTopColorDepth: dTopColor = lastCol;
                                dDepth = dStart - archDepth;
                                breakDepth = true;
                            }

                            if (breakColor & breakDepth)
                                break;
                        }

                        bool b = (SECL[entry.colorID].ArchStopColor && archDeptshIndependent);

                        if (SECL[entry.colorID].ArchStop || b)
                        {
                            // Element stops at a layer change - save special depth.
                            if (!breakColor && (!archDeptshIndependent || b))
                            {
                                dLastCol = -2;
                                dTopColor = entry.colorID;
                                breakColor = true;
                            }
                            if (!breakDepth)
                            {
                                dDepth = entry.high;
                                breakDepth = true;
                            }
                        }
                        else if (archStopAtStrongLayer && (i < max - 1) && (nextEntry.colorID < entry.colorID))
                        {
                            // Element stops at a layer change - save special depth.
                            if (!breakColor)
                            {
                                dLastCol = -2;
                                dTopColor = entry.colorID;
                            }
                            if (!breakDepth)
                                dDepth = entry.high;
                            break;
                        }
                        else if (i == max)
                        {
                            // No more valid data available - take -1 color and standard depth.
                            if (!breakColor)
                            {
                                dLastCol = -1;
                                dTopColor = -1;
                            }
                            if (!breakDepth)
                                dDepth = dStart - archDepth;
                            break;
                        }
                        else
                        {
                            if ((entry.low < dStart - archTopColorDepth) && !breakColor)
                            {
                                // Element is splitted by range.
                                dLastCol = -2;
                                dTopColor = entry.colorID;
                                breakColor = true;
                            }

                            if ((entry.low < dStart - archDepth) && !breakDepth)
                            {
                                // Element is splitted by range - save standard depth.
                                // removed by archTopColorDepth: dLastCol = -2;
                                // removed by archTopColorDepth: dTopColor = entry.colorID;
                                dDepth = dStart - archDepth;
                                breakDepth = true;
                            }
                        }

                        if (breakColor & breakDepth)
                            break;

                        lastCol = entry.colorID;
                        if (!breakDepth)
                            dDepth = entry.high;
                    }
                }
                catch
                {
                }
            }

            data.LastCol = dLastCol;
            data.TopColor = dTopColor;
            data.Depth = dDepth;
            data.Volume = 0;

            for (j = i; j < max; j++)
            {
                entry = data.Entries[j];
                float low = entry.low;
                float high = entry.high;

                if ((low > TCut) | (high < BCut))
                    continue;

                if (low < BCut)
                    low = BCut;
                if (high > TCut)
                    high = TCut;

                if ((entry.colorID < 0) | (entry.colorID > colCount - 1))
                    continue;
                if (archActive & (low > dDepth))
                    continue;

                try
                {
                    if (archActive & (high > dDepth))
                        volCol[entry.colorID] += dDepth - low;
                    else
                        volCol[entry.colorID] += high - low;
                }
                catch
                {
                    // Filter out all unwanted events. :-) 
                }
            }

            for (j = 0; j < colCount; j++)
            {
                if (volCol[j] >= 0.1)
                    data.Volume += (float)System.Math.Log10(volCol[j] * 10) * GSC.Settings.SECL[j].VolumeWeight;
            }
        }
        #endregion
    }
}
