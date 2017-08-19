using System;
using System.Collections.Generic;
using System.Text;

namespace UKLib.Hex
{
    public class HexStrings
    {
        public static string ToHexString(byte[] buf)
        {
            if (buf != null)
            {
                string s = "";
                for (int i = 0; i < buf.Length; i++)
                    s += buf[i].ToString("X2");
                return s; 
            }
            else
                return null;
        }

        public static byte[] ToByteArray(string s)
        {
            if ((s != null) && (s.Length % 2 == 0))
            {
                byte[] buf = new byte[s.Length / 2];
                for (int i = 0; i < s.Length / 2; i++)
                {
                    buf[i] = byte.Parse(s.Substring(i * 2, 2),System.Globalization.NumberStyles.HexNumber);
                }
                return buf;
            }
            return null;
        }

    }
}