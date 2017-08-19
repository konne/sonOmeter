using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Design;

namespace UKLib.Xml
{     
    public class SerializeColor
    {                
        public static string Serialize(Color color)
        {
            if (color.IsNamedColor)
                return color.Name;
            else
                return string.Format("{0}; {1}; {2}; {3}",
                    color.A, color.R, color.G, color.B);
        }

        public static Color Deserialize(string color)
        {
            byte a, r, g, b;

            try
            {
                bool isNamed = (color.IndexOf(";") == -1);
                if (isNamed)
                {
                    return Color.FromName(color);
                }
                else
                {
                    string[] pieces = color.Split(new char[] { ':' });
                    a = byte.Parse(pieces[0]);
                    r = byte.Parse(pieces[1]);
                    g = byte.Parse(pieces[2]);
                    b = byte.Parse(pieces[3]);
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch
            {
                return Color.Empty;
            }                                          
        }
    }
}
