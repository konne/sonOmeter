using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.Survey.Parse
{
    public class LeicaTPS : FixedRVHV
    {
        public LeicaTPS(string data)
        {           
			NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;

            char[] seperator = new char[1];
            seperator[0] = ',';
            string[] mainSplit = data.Split(seperator);

            if (mainSplit.Length != 6)
                return;

            try
            {
                rightValue = Double.Parse(mainSplit[1], nfi);
                highValue = Double.Parse(mainSplit[2], nfi);
                altitude = Double.Parse(mainSplit[3], nfi);
            }
            catch
            {
                altitude = 0;
                rightValue = 0;
                highValue = 0;
            }
		}
    }
}
