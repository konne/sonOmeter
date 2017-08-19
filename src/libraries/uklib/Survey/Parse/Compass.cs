using System;
using System.Text;
using System.Globalization;
using UKLib.MathEx;

namespace UKLib.Survey.Parse
{
    /// <summary>
    /// Summary description for Compass.
    /// </summary>
    [Serializable]
    public class Compass : RotD
    {
        public Compass()
        {
            this.Yaw = 0;
            this.Pitch = 0;
            this.Roll = 0;
        }

        public Compass(string data)
        {
            if ((data.Length == 14) && (data.Substring(12, 2) == "FF"))
            {
                try
                {
                    int ai = int.Parse(data.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                    int ei = int.Parse(data.Substring(4, 4), System.Globalization.NumberStyles.HexNumber);
                    int bi = int.Parse(data.Substring(8, 4), System.Globalization.NumberStyles.HexNumber);

                    this.Yaw = ai / 10.0;

                    if (ei > 3600) ei = ei - 65536;
                    if (bi > 3600) bi = bi - 65536;

                    this.Pitch = ei / 10.0;
                    this.Roll = -bi / 10.0;
                }
                catch
                {
                    this.Yaw = 0;
                    this.Pitch = 0;
                    this.Roll = 0;
                }
            }
        }
    }


    /// <summary>
    /// Summary description for Compass.
    /// </summary>
    [Serializable]
    public class HMR3300 : RotD
    {
        private static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        public HMR3300()
        {
            this.Yaw = 0;
            this.Pitch = 0;
            this.Roll = 0;
        }

        public HMR3300(string data)
        {
            var dl = data.Split(new char[] { ',' });

            if (dl.Length == 3)
            {
                try
                {
                    this.Yaw = double.Parse(dl[0], nfi);
                    this.Pitch = double.Parse(dl[1], nfi);
                    this.Roll = double.Parse(dl[2], nfi);
                }
                catch
                {
                    this.Yaw = 0;
                    this.Pitch = 0;
                    this.Roll = 0;
                }
            }
        }
    }
}
