using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace sonOmeter.Server.Classes
{
    #region DataEntry
    /// <summary>
    /// One area of sonar information having the same colour
    /// </summary>
    public struct DataEntry
    {
        #region Variables
        public float high;
        public float low;

        public int colorID;
        #endregion

        #region ToString
        public override string ToString()
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return ToString(nfi);
        }

        public string ToString(NumberFormatInfo nfi)
        {
            return (string.Format(nfi, "|{0:0.##},{1:0.##},{2:d}", -low, -high, colorID));
        }
        #endregion
    }
    #endregion

    #region LineData
    /// <summary>
    /// Summary description for LineData.
    /// </summary>
    public class LineData
    {
        #region Variables
        internal float bCut = 0.0F;
        internal float eCut = 0.0F;
        internal float cDepth = float.NaN;

        internal Queue<DataEntry> entries = null;
        #endregion

        #region Properties
        public float BCut
        {
            get { return bCut; }
            set { bCut = value; }
        }

        public float ECut
        {
            get { return eCut; }
            set { eCut = value; }
        }

        public float CDepth
        {
            get { return cDepth; }
            set { cDepth = value; }
        }

        public Queue<DataEntry>Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        #endregion

        #region Constructor
        public LineData()
        {
            entries = new Queue<DataEntry>();
        }
        #endregion

        private static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        #region ToString
        public string ToString(string prefix)
        {
            string result = "<"+prefix;
            if (!Double.IsNaN(CDepth)) string.Format(nfi, " cdepth=\"{0:0.##}\"", this.CDepth);
            result += ">";
            foreach (DataEntry e in this.Entries)
            {
                result += e.ToString(nfi);
            }
            result += string.Format("</{0}>", prefix);
            return (result);
        }
        #endregion
    }
    #endregion

    #region SonarLine

    /// <summary>
    /// One Sonar Line
    /// </summary>
    public class SonarLine
    {
        #region Variables
        internal int sonID = 0;

        internal LineData dataHF = new LineData();
        internal LineData dataNF = new LineData();

        internal DateTime time;

        internal List<PositionData> posList = new List<PositionData>();

        #endregion

        #region Properties
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

        public List<PositionData> PosList
        {
            get { return posList; }
        }

        public DateTime Time
        {
            get { return time; }
        }

       
        #endregion

        #region Constructors
        public SonarLine()
        {
            sonID = 0;
            time = DateTime.Now;
        }
        public SonarLine(int sonid)
        {
            this.sonID = sonid;
            time = DateTime.Now;
        }

        #endregion

        #region ToString
        public override string ToString()
        {
            string res = string.Format("<line sonid=\"{0}\"", SonID);
            if (time != null) res += " time=\"" + time.ToString("HH:mm:ss.fff") + "\"";
            res += ">";
            if (HF != null) res += HF.ToString("HF");
            if (NF != null) res += NF.ToString("NF");
            foreach (PositionData pos in this.PosList)
                res += pos.ToString();
            res += "</line>";
            return (res);
        }
        #endregion
    }
    #endregion

}
