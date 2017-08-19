using System;
using System.Collections.Specialized;
using System.Globalization;
using UKLib.Strings;

namespace UKLib.Survey.Parse
{
    /// <summary>
    /// Summary description for NMEA.
    /// </summary>
    /// 
    public enum SatStatus
    {
        NotUsed,
        TryUse,
        InUse
    }

    public struct SatSNR
    {
        public double Elevation;
        public double Azimuth;
        public int SNR;
        public SatStatus Status;

        public override string ToString()
        {
            return Elevation.ToString("0.0") + " " + Azimuth.ToString("0.0") + " " + SNR.ToString("0.0") + " " + Status.ToString();
        }
    }

    public class NMEA
    {
        #region Variables
        private NumberFormatInfo nfi;

        private StringCollection items=new StringCollection();
        #endregion

        #region SBG Compass
        public double Roll
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$SBG01":
                            return Double.Parse(items[2], nfi);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        public double Pitch
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$SBG01":
                            return Double.Parse(items[3], nfi);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        public double Yaw
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$SBG01":
                            double d =Double.Parse(items[4], nfi);
                            if (d < 0) d+=360;
                            return d;
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        } 
        #endregion

      
        #region Properties
        public string Type
        {
            get
            {
                if (items.Count > 0)
                    return items[0].ToString();
                else
                    return "";
            }
        }

        public double Speed
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPVTG":
                            {
                                return Double.Parse(items[7], nfi);
                            }
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public LatLong Latitude
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPGGA":
                        case "$GNGGA":
                            return (new LatLong(items[2], items[3]));

                        case "$GPGLL":
                        case "$GNGLL":
                            return (new LatLong(items[1], items[2]));
                        case "$PNTL": return null;
                        default: return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        public LatLong Longitude
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPGGA":
                        case "$GNGGA":
                            return new LatLong(items[4], items[5]);
                        case "$GPGLL":
                        case "$GNGLL":
                            return new LatLong(items[3], items[4]);
                        case "$PNTL": return null;
                        default: return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public double Altitude
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPGGA":
                        case "$GNGGA":
                            return Double.Parse(items[9], nfi);
                        case "$GPGLL":
                        case "$GNGLL":
                            return 0;
                        case "$PNTL": return Double.Parse(items[11], nfi);
                        case "$GPLLK":
                        case "$GNLLK":
                            return Double.Parse(items[10], nfi);
                        case "$GPLLQ":
                        case "$GNLLQ":
                            return Double.Parse(items[10], nfi);
                        case "$PTNL": return Double.Parse(items[11].Substring(4), nfi);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }	
            }
        }

        public double AltitudeWGS84
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPGGA":
                        case "$GNGGA":
                            return Double.Parse(items[11], nfi);
                        case "$GPGLL":
                        case "$GNGLL":
                            return 0;
                        case "$PNTL": return 0;
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }


        public double RightValue
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPLLK":
                        case "$GNLLK": 
                            return Double.Parse(items[3], nfi);
                        case "$GNLLQ": 
                        case "$GPLLQ": 
                            return Double.Parse(items[3], nfi);
                        case "$PNTL": return Double.Parse(items[4], nfi);
                        case "$PTNL": return Double.Parse(items[6], nfi);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double HighValue
        {
            get 
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPLLK":
                        case "$GNLLK": 
                            return Double.Parse(items[5], nfi);
                        case "$GPLLQ":
                        case "$GNLLQ": 
                            return Double.Parse(items[5], nfi);
                        case "$PNTL": return Double.Parse(items[6], nfi);
                        case "$PTNL": return Double.Parse(items[4], nfi);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int Quality
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPLLK":
                        case "$GNLLK":
                            return FastConvert.ToInt32(items[7]);
                        case "$GPLLQ":
                        case "$GNLLQ": 
                            return FastConvert.ToInt32(items[7]);
                        case "$GPGGA":
                        case "$GNGGA": 
                            return FastConvert.ToInt32(items[6]);
                        case "$PTNL": 
                            return FastConvert.ToInt32(items[8]);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public float QualityMeter
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPLLQ":
                        case "$GNLLQ": 
                            return FastConvert.ToSingle(items[9]);
                        default: return 0.0f;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public int Satellites
        {
            get
            {
                try
                {
                    switch (items[0].ToString())
                    {
                        case "$GPLLK":
                        case "$GNLLK":
                            return FastConvert.ToInt32(items[8]);
                        case "$GPLLQ":
                        case "$GNLLQ":
                            return FastConvert.ToInt32(items[8]);
                        case "$GPGGA":
                        case "$GNGGA":
                            return FastConvert.ToInt32(items[7]);
                        case "$PTNL": return FastConvert.ToInt32(items[9]);
                        default: return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion



        #region Functions
        public SatSNR SatSNRs(int index, ref int nr)
        {
            SatSNR satsnr;
            satsnr.Azimuth=0;
            satsnr.Elevation=0;
            satsnr.SNR=0;
            satsnr.Status = SatStatus.NotUsed;
            nr = -1;
            if ((index > 3) | (index < 0) | (items[0].ToString().Substring(3) != "GSV")) return satsnr;
            nr= FastConvert.ToInt32(items[index*4+4]);
            satsnr.Elevation= FastConvert.ToInt32(items[index*4+5]);
            satsnr.Azimuth= FastConvert.ToInt32(items[index*4+6]);
            if (items[index*4+7] != "")
            {
                satsnr.SNR= FastConvert.ToInt32(items[index*4+7]);
                satsnr.Status = SatStatus.TryUse;
            }

            return satsnr;
        }

        public int SatNrInUse(int index)
        {
            if ((index > 0) && (index < 13) && (items[0].ToString().Substring(3) == "GSA"))
            {
                if (items[index+2] != "")
                    return FastConvert.ToInt32(items[index+2]);
                else
                    return -1;
            }
            else
            {
                return -1;
            }
        }


        private bool CheckSum(string strLine) 
        {
            int i;
            byte b;
            char c;
            int indexOfStar;
    
            indexOfStar = strLine.IndexOf('*');

            if (indexOfStar == -1)
            {
                return true;
            } 
            else
            {
                b = 0;
                for (i=1;i < indexOfStar ; i++) 
                {
                    c = strLine[i];
                    b = (byte)(b ^ c);
                }

                try
                {
                    return (strLine.Substring(indexOfStar+1, 2)) == b.ToString("X").PadLeft(2, '0');
                }
                catch
                {
                    return false;
                }
            }
        }

        #endregion

        #region Constructor
        public NMEA(string NMEALine, NumberFormatInfo nfi)
        {
            this.nfi = nfi;
            string strCheckSum="";

            char [] seperator = new char[1];
            seperator[0] = '*';

            string[] mainSplit = NMEALine.Split(seperator);

            if (mainSplit.Length == 2)
                strCheckSum = mainSplit[1];
        
            seperator[0] = ',';
            items.AddRange(mainSplit[0].Split(seperator));

            if (strCheckSum == "")
                return;

            if (!CheckSum(NMEALine))
                items.Clear();
        }
        #endregion
    }
}
