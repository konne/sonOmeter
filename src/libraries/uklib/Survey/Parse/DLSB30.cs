using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.Survey.Parse
{
    public class DLSB30
    { 
        private static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        public Double Distance { get; private set; }

        public DLSB30(string input)
        {            
            if (input.Length == 13)
            {
               
                int s;
                if (int.TryParse(input.Substring(5),out s))
                {
                    Distance = s/10000.0;
                }
            }
            //  g0uh+00001409
        }

    }
}
