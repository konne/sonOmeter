using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UKLib.Hex
{
    #region IntelHexFile
    public class IntelHexFile
    {
        #region Variables & Properties
        public byte DefaultValue { get; set; }

        public byte this[int idx]
        {
            get
            {
                if (idx <= size)
                {
                    return data[idx];
                }
                else
                    return DefaultValue;
            }
        }

        private int size = 0;
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        private byte[] data;
        #endregion

        #region Constructor
        public IntelHexFile(string filename)
        {
            DefaultValue = 0xFF;
           
            data = new byte[32768];

            for (int i = 0; i < data.Length; i++) data[i] = DefaultValue;

            StreamReader sr = new StreamReader(File.OpenRead(filename));
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                int hiaddr = Int16.Parse((s[3] + "" + s[4]), System.Globalization.NumberStyles.HexNumber);
                int addr = hiaddr * 256 + Int16.Parse((s[5] + "" + s[6]), System.Globalization.NumberStyles.HexNumber);
                int cnt = Int16.Parse((s[1] + "" + s[2]), System.Globalization.NumberStyles.HexNumber);
                for (int i = 0; i < cnt; i++)
                {
                    data[addr] = Byte.Parse((s[i * 2 + 9] + "" + s[i * 2 + 10]), System.Globalization.NumberStyles.HexNumber);
                    addr++;
                    if (size < addr) size = addr;
                }
            }
            sr.Close();
        }
        #endregion
    }
    #endregion
}
