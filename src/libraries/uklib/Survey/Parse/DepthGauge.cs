using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.Survey.Parse
{
    public class SosoDepthGauge
    {
        public static Nullable<float> ToFloat(string s)
        {          
            if ((s.Length > 4) && (s[0] == '1'))
            {
                s = s.Substring(1, 4);
                int idepth;
                if (Int32.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out idepth))
                {
                    return ((idepth * 250F) / 4095F);
                }               
            }
            return null;
        }
    }
}
