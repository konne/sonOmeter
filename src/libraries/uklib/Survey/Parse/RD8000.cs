using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.Survey.Parse
{
    public class RD8000Message
    {
        #region ModeEnum
        public enum ModeEnum
        {
            Notdefined = 0,
            Active = 1,
            Radio = 2,
            Power = 3,
            Fault_Find_8K = 4,
            Fault_Find_CD = 5,
            CD = 6,
            ACD = 7,
            Passive_Avoidance = 8,
            CPS = 9,
            CATV = 10,
            ELF = 11,
            Sonde = 12
        }
        #endregion

        #region Properties
        public UInt32 EepromFileIndex { get; private set; }
        public ModeEnum Mode { get; private set; }
        public Single LocatorFrequency { get; private set; }
        public string LogId { get; private set; }
        public UInt32 Reserved { get; private set; }
        public Single Depth { get; private set; }
        public Single FaultFindSignal { get; private set; }
        public Single LocateCurrent { get; private set; }
        public Single Phase { get; private set; }
        public Single SignalStrength { get; private set; }
        public Single Gain { get; private set; }
        public bool isValid { get; private set; }
        #endregion

        #region Constructor
        public RD8000Message(byte[] buf)
        {
            isValid = false;

            if (buf == null)
                return;
            if (buf.Length == 56)
            {
                if ((buf[0] == buf[55]) && (buf[55] == 0x7E))
                {
                    byte[] nbuf = new byte[54];
                    for (int i = 0; i < 54; i++)
                        nbuf[i] = buf[i+1];
                    buf = nbuf;
                }
                else
                    return;             
            }

            if (buf.Length != 54)
                return;
            try
            {
                EepromFileIndex = BitConverter.ToUInt32(buf, 8);
                UInt32 tmpMode = BitConverter.ToUInt32(buf, 12);
                if ((tmpMode >= 0) & (tmpMode < 13))
                    Mode = (ModeEnum)tmpMode;
                else
                    Mode = ModeEnum.Notdefined;
                LocatorFrequency = BitConverter.ToSingle(buf, 16);

                string s = Encoding.ASCII.GetString(buf, 20, 4);
                LogId = "";
                foreach (char c in s)
                    LogId = c + LogId;                

                Reserved = BitConverter.ToUInt32(buf, 24);
                Depth = BitConverter.ToSingle(buf, 28);
                FaultFindSignal = BitConverter.ToSingle(buf, 32);
                LocateCurrent = BitConverter.ToSingle(buf, 36);
                Phase = BitConverter.ToSingle(buf, 40);
                SignalStrength = BitConverter.ToSingle(buf, 44);
                Gain = BitConverter.ToSingle(buf, 48);
                isValid = true;
            }
            catch
            {
            }

        }
        #endregion

        #region RD8000ResponseMessages
        public static byte[] RD8000ACKMessage()
        {
            return new byte[] { 0x7E, 0xFF, 0x03, 0x00, 0x21, 0x01, 0x02, 0x44, 0x74, 0x7E };

        }
        public static byte[] RD8000NACKMessage()
        {
            return new byte[] { 0x7E, 0xFF, 0x03, 0x00, 0x21, 0x01, 0x02, 0xCD, 0x65, 0x7E };
        }
        #endregion
    }

    public class RD4000Message
    {
        private static NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        public Single Depth { get; private set; }

        public RD4000Message(string input)
        {
            string[] lines = input.Split(new char[] { ',' });
            if (lines.Length > 10)
            {
                Single s;
                
                if (Single.TryParse(lines[9],System.Globalization.NumberStyles.Float, nfi,out s))
                {
                    Depth = s/100F;
                }
            }
            //  F03,0015,ACTIVE,640Hz,0,---,L,P,cm,8,mA,15,dB,23,,,,
        }

    }

}
