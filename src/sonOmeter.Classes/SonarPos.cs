using System;
using UKLib.Strings;

namespace sonOmeter.Classes
{
    public enum PosType
    {
        Unknown = 0,
        SoSoZeiss = 1,
        NMEA = 4,
        Geodimeter = 5,
        LeicaTPS = 7,
        HMR3300 = 31,
        Compass = 32,
        Depth = 33,
        RD8000 = 34,
        RD4000 = 35,
        DLSB30 = 37,
        VideoMarker = 40,
        Fixed = 42
    }

    /// <summary>
    /// Summary description for SonarPos.
    /// </summary>
    public class SonarPos
    {
        #region Variables
        public DateTime time;
        public PosType type;
        public bool Disabled;
        public bool Used = false;
        public bool RealPos = false;
        public string strPos;
        #endregion

        public SonarPos(DateTime baseDate, string strTime, string strType, bool disabled, bool used, bool realpos, string strPos)
        {
            this.time = baseDate.Add(DateTime.Parse(strTime).TimeOfDay);
            this.type = (PosType)FastConvert.ToInt16(strType);
            this.RealPos = realpos;
            this.Used = used;
            this.Disabled = disabled;
            this.strPos = strPos;
        }

        public SonarPos(DateTime baseDate, string strTime, string strType, bool disabled, string strPos)
        {
            this.time = baseDate.Add(FastConvert.ToDateTime(strTime).TimeOfDay);
            this.type = (PosType)FastConvert.ToInt16(strType);

            this.Disabled = disabled;
            this.strPos = strPos;
        }

        public SonarPos(DateTime time, string strType, bool disabled, string strPos)
        {
            this.time = time;
            this.type = (PosType)FastConvert.ToInt16(strType);

            this.Disabled = disabled;
            this.strPos = strPos;
        }

        public SonarPos(DateTime time, PosType type, bool disabled, string strPos)
        {
            this.time = time;
            this.type = type;

            this.Disabled = disabled;
            this.strPos = strPos;
        }

        public override string ToString()
        {
            return strPos;
        }
    }
}
