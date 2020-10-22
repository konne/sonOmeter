using System;
using System.Globalization;

namespace UKLib.Survey.Parse
{

    public enum LatLongTyp
    {
        Latitude,
        Longitude
    }

    /// <summary>
    /// Summary description for LatLong.
    /// </summary>
    public class LatLong
    {
        #region Variables
        private NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        private double deg;
        private LatLongTyp type;
        #endregion

        #region Properties
        public double Value
        {
            get { return deg; }
        }

        public LatLongTyp Type
        {
            get { return type; }
        }
        #endregion

        public LatLong(string strDeg, string q)
        {
            if (string.IsNullOrWhiteSpace(strDeg))
                return;

            string grad;
            string min;

            grad = strDeg.Substring(0, strDeg.IndexOf(".") - 2);
            min = strDeg.Substring(strDeg.IndexOf(".") - 2);
            deg = Double.Parse(grad, nfi) + Double.Parse(min, nfi) / 60;
            switch (q.ToUpper())
            {
                case "N": type = LatLongTyp.Longitude; break;
                case "S": deg = -deg; type = LatLongTyp.Longitude; break;
                case "W": deg = 360 - deg; type = LatLongTyp.Latitude; break;
                case "E": type = LatLongTyp.Latitude; break;
            }
        }


        public override string ToString()
        {
            string data = "";
            #region N/S W/E
            switch (type)
            {
                case LatLongTyp.Longitude:
                    if (deg < 0)
                    {
                        data = "S";
                        deg = -deg;
                    }
                    else
                    {
                        data = "N";
                    }
                    break;
                case LatLongTyp.Latitude:

                    if (deg > 180)
                    {
                        data = "W";
                        deg = 360 - deg;

                    }
                    else
                    {
                        data = "E";
                    }
                    break;
            }
            #endregion

            if (data.Length > 0)
            {
                int ideg = (int)deg;
                int min = (int)((deg - ideg) * 60);
                double sec = (deg - ideg - min / 60.0) * 3600;
                data = ideg.ToString() + "° " + min.ToString() + "' " + sec.ToString("f", nfi) + "'' " + data;
            }
            return data;
        }
    }
}
