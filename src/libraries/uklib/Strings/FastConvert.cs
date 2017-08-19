using System;

namespace UKLib.Strings
{
    /// <summary>
    /// Summary description for FastConvert.
    /// </summary>
    public class FastConvert
    {
        #region String convert functions
        static public float ToSingle(string s)
        {
            float ret = 0;
            int max = s.Length;
            int pow = 0;
            int i;
            char c;
            bool sign = false;
            bool exponent = false;
            bool expsign = false;
            bool dot = false;
            int exp = 0;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '-':
                        if (exponent)
                            expsign = true;
                        else
                            sign = true;
                        break;

                    case '+':
                        if (exponent)
                            expsign = false;
                        else
                            sign = false;
                        break;

                    case '.':
                        if (!exponent)
                            dot = true;
                        break;

                    case 'E':
                    case 'e':
                        exponent = true;
                        break;

                    default:
                        if (!exponent)
                        {
                            ret *= 10;
                            ret += (int)c - 0x30;

                            if (dot)
                                pow++;
                        }
                        else
                        {
                            exp *= 10;
                            exp += (int)c - 0x30;
                        }
                        break;
                }
            }

            if (expsign)
                pow += exp;
            else if (exponent)
                pow -= exp;

            if (pow > 0)
                for (i = 0; i < pow; i++)
                    ret /= 10;
            else
                for (i = pow; i < 0; i++)
                    ret += 10;

            if (sign)
                ret = -ret;

            return ret;
        }

        static public int ToInt32(string s)
        {
            int ret = 0;
            int max = s.Length;
            int i;
            char c;
            bool sign = false;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '-':
                        sign = true;
                        break;
                    case '0':
                        if (ret > 0)
                            ret *= 10;
                        //ret += 0;
                        break;
                    case '1':
                        if (ret > 0)
                            ret *= 10;
                        ret += 1;
                        break;
                    case '2':
                        if (ret > 0)
                            ret *= 10;
                        ret += 2;
                        break;
                    case '3':
                        if (ret > 0)
                            ret *= 10;
                        ret += 3;
                        break;
                    case '4':
                        if (ret > 0)
                            ret *= 10;
                        ret += 4;
                        break;
                    case '5':
                        if (ret > 0)
                            ret *= 10;
                        ret += 5;
                        break;
                    case '6':
                        if (ret > 0)
                            ret *= 10;
                        ret += 6;
                        break;
                    case '7':
                        if (ret > 0)
                            ret *= 10;
                        ret += 7;
                        break;
                    case '8':
                        if (ret > 0)
                            ret *= 10;
                        ret += 8;
                        break;
                    case '9':
                        if (ret > 0)
                            ret *= 10;
                        ret += 9;
                        break;
                }
            }

            if (sign)
                ret = -ret;

            return ret;
        }

        static public long ToInt64(string s)
        {
            long ret = 0;
            int max = s.Length;
            int i;
            char c;
            bool sign = false;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '-':
                        sign = true;
                        break;
                    case '0':
                        if (ret > 0)
                            ret *= 10;
                        //ret += 0;
                        break;
                    case '1':
                        if (ret > 0)
                            ret *= 10;
                        ret += 1;
                        break;
                    case '2':
                        if (ret > 0)
                            ret *= 10;
                        ret += 2;
                        break;
                    case '3':
                        if (ret > 0)
                            ret *= 10;
                        ret += 3;
                        break;
                    case '4':
                        if (ret > 0)
                            ret *= 10;
                        ret += 4;
                        break;
                    case '5':
                        if (ret > 0)
                            ret *= 10;
                        ret += 5;
                        break;
                    case '6':
                        if (ret > 0)
                            ret *= 10;
                        ret += 6;
                        break;
                    case '7':
                        if (ret > 0)
                            ret *= 10;
                        ret += 7;
                        break;
                    case '8':
                        if (ret > 0)
                            ret *= 10;
                        ret += 8;
                        break;
                    case '9':
                        if (ret > 0)
                            ret *= 10;
                        ret += 9;
                        break;
                }
            }

            if (sign)
                ret = -ret;

            return ret;
        }

        static public double ToDouble(string s)
        {
            double ret = 0;
            int max = s.Length;
            int pow = 0;
            int i;
            char c;
            bool sign = false;
            bool exponent = false;
            bool expsign = false;
            bool dot = false;
            int exp = 0;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                if (c == '-')
                    if (exponent)
                        expsign = true;
                    else
                        sign = true;
                else if (c == '+')
                    if (exponent)
                        expsign = false;
                    else
                        sign = false;
                else if ((c == '.') & !exponent)
                    dot = true;
                else if ((c == 'E') | (c == 'e'))
                    exponent = true;
                else
                {
                    if (exponent)
                    {
                        if (exp > 0)
                            exp *= 10;

                        exp += (int)c - 0x30;
                    }
                    else
                    {
                        if (ret > 0.0)
                            ret = (double)(ret * 10.0);

                        ret = (double)(ret + (double)((int)c - 0x30));

                        if (dot)
                            pow++;
                    }
                }
            }

            if (expsign)
                pow += exp;
            else if (exponent)
                pow -= exp;

            if (pow > 0)
                for (i = 0; i < pow; i++)
                    ret /= 10.0;
            else
                for (i = pow; i < 0; i++)
                    ret *= 10.0;

            if (sign)
                ret = -ret;

            return ret;
        }

        static public decimal ToDecimal(string s)
        {
            decimal ret = 0;
            int max = s.Length;
            int pow = 0;
            int i;
            char c;
            bool sign = false;
            bool exponent = false;
            bool expsign = false;
            bool dot = false;
            int exp = 0;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                if (c == '-')
                    if (exponent)
                        expsign = true;
                    else
                        sign = true;
                else if (c == '+')
                    if (exponent)
                        expsign = false;
                    else
                        sign = false;
                else if ((c == '.') & !exponent)
                    dot = true;
                else if ((c == 'E') | (c == 'e'))
                    exponent = true;
                else
                {
                    if (exponent)
                    {
                        if (exp > 0)
                            exp *= 10;

                        exp += (int)c - 0x30;
                    }
                    else
                    {
                        if (ret > 0)
                            ret = ret * (decimal)10;

                        ret = ret + (decimal)((int)c - 0x30);

                        if (dot)
                            pow++;
                    }
                }
            }

            if (expsign)
                pow += exp;
            else if (exponent)
                pow -= exp;

            if (pow > 0)
                for (i = 0; i < pow; i++)
                    ret /= (decimal)10;
            else
                for (i = pow; i < 0; i++)
                    ret *= (decimal)10;

            if (sign)
                ret = -ret;

            return ret;
        }

        static public short ToInt16(string s)
        {
            short ret = 0;
            int max = s.Length;
            int i;
            char c;
            bool sign = false;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '-':
                        sign = true;
                        break;
                    case '0':
                        if (ret > 0)
                            ret *= 10;
                        //ret += 0;
                        break;
                    case '1':
                        if (ret > 0)
                            ret *= 10;
                        ret += 1;
                        break;
                    case '2':
                        if (ret > 0)
                            ret *= 10;
                        ret += 2;
                        break;
                    case '3':
                        if (ret > 0)
                            ret *= 10;
                        ret += 3;
                        break;
                    case '4':
                        if (ret > 0)
                            ret *= 10;
                        ret += 4;
                        break;
                    case '5':
                        if (ret > 0)
                            ret *= 10;
                        ret += 5;
                        break;
                    case '6':
                        if (ret > 0)
                            ret *= 10;
                        ret += 6;
                        break;
                    case '7':
                        if (ret > 0)
                            ret *= 10;
                        ret += 7;
                        break;
                    case '8':
                        if (ret > 0)
                            ret *= 10;
                        ret += 8;
                        break;
                    case '9':
                        if (ret > 0)
                            ret *= 10;
                        ret += 9;
                        break;
                }
            }

            if (sign)
                ret = (short)-ret;

            return ret;
        }

        public static byte ToByte(string s)
        {
            byte ret = 0;
            int max = s.Length;
            int i;
            char c;
            bool sign = false;

            for (i = 0; i < max; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '-':
                        sign = true;
                        break;
                    case '0':
                        if (ret > 0)
                            ret *= 10;
                        //ret += 0;
                        break;
                    case '1':
                        if (ret > 0)
                            ret *= 10;
                        ret += 1;
                        break;
                    case '2':
                        if (ret > 0)
                            ret *= 10;
                        ret += 2;
                        break;
                    case '3':
                        if (ret > 0)
                            ret *= 10;
                        ret += 3;
                        break;
                    case '4':
                        if (ret > 0)
                            ret *= 10;
                        ret += 4;
                        break;
                    case '5':
                        if (ret > 0)
                            ret *= 10;
                        ret += 5;
                        break;
                    case '6':
                        if (ret > 0)
                            ret *= 10;
                        ret += 6;
                        break;
                    case '7':
                        if (ret > 0)
                            ret *= 10;
                        ret += 7;
                        break;
                    case '8':
                        if (ret > 0)
                            ret *= 10;
                        ret += 8;
                        break;
                    case '9':
                        if (ret > 0)
                            ret *= 10;
                        ret += 9;
                        break;
                }
            }

            if (sign)
                ret = (byte)-ret;

            return ret;
        }

        static public bool ToBoolean(string s)
        {
            if (string.Compare(s, "true", true) == 0)
                return true;

            return false;
        }

        static public DateTime ToDateTime(string s)
        {
            // tbd Errors!!!
            string[] parts = s.Split(':');
            DateTime d = new DateTime(2005, 1, 1, ToInt32(parts[0]), ToInt32(parts[1]), ToInt32(parts[2]));

            return d;
        }
        #endregion

        #region To string convert functions
        static int StringRecursion(ref string s, int value)
        {
            int mod = value % 10;
            int depth = 0;

            if (value > 0)
            {
                value /= 10;

                depth = StringRecursion(ref s, value) + 1;

                s += (char)(mod + 0x30);
            }

            return depth;
        }

        static public string ToString(int value)
        {
            string s = "";

            if (value < 0)
            {
                s += '-';
                value = -value;
            }

            if (value > 0)
                StringRecursion(ref s, value);
            else
                return "0";

            return s;
        }

        static public string ToString(DateTime value)
        {
            string[] parts = new string[3];

            parts[0] = ToString(value.Hour);
            parts[1] = ToString(value.Minute);
            parts[2] = ToString(value.Second);

            return string.Join(":", parts);
        }
        #endregion
    }
}
