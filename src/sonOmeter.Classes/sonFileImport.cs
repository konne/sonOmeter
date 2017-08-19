using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace sonOmeter.Classes
{
    public class PrjFile
    {
        #region Structs
        public struct Boje
        {
            public string name;
            public double hw;
            public double rw;
            public double al;
        }
        #endregion

        #region Variables
        public readonly List<string> SonFiles = new List<string>();
        public readonly string Name = "";
        public readonly byte Stb = 0;
        public readonly byte Asf = 0;
        public readonly List<Boje> Bojen = new List<Boje>();

        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        #region Functions
        private bool ValueName(string s, out string v, out string n)
        {
            string[] s2 = s.Split(new char[] { '=' });
            if (s2.Length == 2)
            {
                n = s2[0];
                v = s2[1];
                return true;
            }
            n = "";
            v = "";
            return false;
        }
        #endregion

        #region Constructor
        public PrjFile(string fileName)
        {
            StreamReader rPrj = new StreamReader(fileName, Encoding.GetEncoding(850));
            string dir = Path.GetDirectoryName(fileName);
            string section = "";
            while (!rPrj.EndOfStream)
            {
                string s = rPrj.ReadLine();
                if ((s.IndexOf("[") == 0) & (s.IndexOf("]") == (s.Length - 1)))
                {
                    section = s;
                }
                else
                {
                    string name;
                    string value;
                    if (ValueName(s, out value, out name))
                    {
                        switch (section)
                        {
                            case "[Son-Files]":
                                string path = Path.GetDirectoryName(name);
                                if (path == "")
                                    SonFiles.Add(dir + @"\" + name);
                                else
                                    SonFiles.Add(name);
                                break;

                            case "[Bojen]":
                                string[] values = value.Split(new char[] { ',' });
                                if (values.Length == 3)
                                {
                                    try
                                    {
                                        Boje boje;
                                        boje.name = name;
                                        boje.rw = Double.Parse(values[0], nfi);
                                        boje.hw = Double.Parse(values[1], nfi);
                                        boje.al = Double.Parse(values[2], nfi);
                                        Bojen.Add(boje);
                                    }
                                    catch
                                    {
                                    }
                                }
                                break;
                            #region Config
                            case "[Config]":
                                switch (name)
                                {
                                    case "name":
                                        Name = value;
                                        break;
                                    case "stb":
                                        if (!Byte.TryParse(value, out Stb)) Stb = 0;
                                        break;
                                    case "asf":
                                        if (!Byte.TryParse(value, out Asf)) Asf = 0;
                                        break;
                                }
                                break;
                            #endregion
                        }
                    }
                }
            }
        }
        #endregion
    }

    public class sonFile
    {
        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        private struct TFK
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04, ArraySubType = UnmanagedType.U1)]
            internal byte[] k;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x05, ArraySubType = UnmanagedType.U1)]
            internal byte[] rest1;
            internal UInt16 kl;
            internal byte stb;
            internal Int32 maxr, maxl;
            internal Int32 minr, minl;
            internal Int32 btime;
            internal Int32 etime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xE6, ArraySubType = UnmanagedType.U1)]
            internal string kommentar;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xFF, ArraySubType = UnmanagedType.U1)]
            internal string ort;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct TSK
        {
            [FieldOffset(0)]
            internal byte stb;		// b.7 1:NF,0:HF
            // b.6 1:echte Postion 0:interpol. Pos.
            // b.3,4,5 Reserviert
            // b.0-b.2 härteste Farbe
            [FieldOffset(1)]
            internal Int32 lw;
            [FieldOffset(5)]
            internal Int32 rw;	// *100
            [FieldOffset(9)]
            internal Int16 hw;		// *10
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct TSZ
        {
            [FieldOffset(0)]
            internal TSK kopf;
            [FieldOffset(12), MarshalAs(UnmanagedType.ByValArray, SizeConst = 513, ArraySubType = UnmanagedType.U2)]
            internal UInt16[] t;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct TRSZ
        {
            [FieldOffset(0)]
            internal byte stb;		// b.7 1:NF,0:HF
            // b.6 1:echte Postion 0:interpol. Pos.
            // b.3,4,5 Reserviert
            // b.0-b.2 härteste Farbe
            [FieldOffset(1)]
            internal Int32 lw;
            [FieldOffset(5)]
            internal Int32 rw;	// *100
            [FieldOffset(9)]
            internal Int16 hw;		// *10
            [FieldOffset(11)]
            internal byte b;
            [FieldOffset(12), MarshalAs(UnmanagedType.ByValArray, SizeConst = 1026, ArraySubType = UnmanagedType.U2)]
            internal Byte[] t;
        }
        #endregion

        #region Variables
        private byte[] kennung = new byte[] { (byte)'S', (byte)'K', (byte)'M', 26 };
        //private string posstr = "";

        private NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        private string data = "";

        public SonarRecord record = new SonarRecord();
        #endregion

        #region Properties
        public string Data
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><project><sondata>" + data + "</sondata></project>";
            }
        }
        #endregion

        #region Functions
        private static object ReadStructure(BinaryReader br, Type t)
        {
            //Read byte array
            byte[] buff = br.ReadBytes(Marshal.SizeOf(t));
            //Make sure that the Garbage Collector doesn't move our buffer 
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
            //Marshal the bytes
            object s = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
            handle.Free();//Give control of the buffer back to the GC 
            return s;
        }

        private static TFK ReadTFK(BinaryReader reader)
        {
            TFK fk = new TFK();

            fk.k = reader.ReadBytes(4);
            fk.rest1 = reader.ReadBytes(5);
            fk.kl = reader.ReadUInt16();
            fk.stb = reader.ReadByte();
            fk.maxr = reader.ReadInt32();
            fk.maxl = reader.ReadInt32();
            fk.minr = reader.ReadInt32();
            fk.minl = reader.ReadInt32();
            fk.btime = reader.ReadInt32();
            fk.etime = reader.ReadInt32();
            fk.kommentar = reader.ReadString();
            fk.ort = reader.ReadString();

            return fk;
        }

        private static void upktime2s(Int32 l, byte stb, out string uhrz, out string datum)
        {
            string s;

            s = ((l >> 23) & 31).ToString("00") + '.';
            datum = s + ((l >> 28) & 15).ToString("00");

            s = ((l >> 18) & 31).ToString("00") + ':' + ((l >> 12) & 63).ToString("00");

            if ((stb & 4) == 4)
                s += ':' + ((l >> 6) & 63).ToString("00");
            else if ((stb & 8) == 8)
                s += '.' + ((l >> 1) & 127).ToString("00");

            uhrz = s;
        }

        private static bool CheckPosLine(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if ("³abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:;.,$*= >-".IndexOf(s[i]) == -1)
                    return false;

            return true;
        }

        private static int CalcTiefe(int t)
        {
            if ((t >= 0) & (t < 800))
                t = (int)Math.Truncate((double)t * 10.0 / 8.0);
            else if ((t >= 800) & (t < 1400))
                t = (int)Math.Truncate((double)t * 5.0 / 2.0 - 1000.0);
            else if ((t >= 1400) & (t < 2040))
                t = (int)Math.Truncate((double)t * 15.0 / 4.0 - 2750.0);
            else if ((t >= 2040) & (t < 2856))
                t = (int)Math.Truncate((double)t * 25.0 / 4.0 - 7850.0);
            else if ((t >= 2856) & (t < 4073))
                t = 10000;

            return t;
        }

        private static int CalcTiefeOld(int t)
        {
            if ((t >= 0) & (t < 100))
                return t;
            else if ((t >= 100) & (t < 175))
                t = t * 2 - 100;
            else if ((t >= 175) & (t < 255))
                t = t * 3 - 275;
            else if ((t >= 255) & (t < 357))
                t = t * 5 - 785;
            else if ((t >= 357) & (t < 510))
                t = 1000;

            return t;
        }

        private static void ReadSZ(BinaryReader reader, out TSZ sz, out int blockCount, int w)
        {
            Int32 z1 = 0, z2 = 0;
            int i;
            int dz;
            int size = Marshal.SizeOf(typeof(TSK));

            reader.BaseStream.Seek(523 + 3 * w, SeekOrigin.Begin);

            byte[] b1 = reader.ReadBytes(3);
            byte[] b2 = reader.ReadBytes(3);

            z1 = ((Int32)b1[2] << 16) + ((Int32)b1[1] << 8) + (Int32)b1[0];
            z2 = ((Int32)b2[2] << 16) + ((Int32)b2[1] << 8) + (Int32)b2[0];

            dz = z2 - z1;

            if (dz > (1025 + size))
                dz = 0;

            reader.BaseStream.Seek(z1, SeekOrigin.Begin);
            TRSZ rsz;
            rsz = (TRSZ)ReadStructure(reader, typeof(TRSZ));
            
            // Very important! Represents the number of visible blocks.
            blockCount = (dz - size) >> 1;
            
            sz = new TSZ();
            sz.t = new UInt16[513];

            sz.kopf.hw = rsz.hw;
            sz.kopf.lw = rsz.lw;
            sz.kopf.rw = rsz.rw;
            sz.kopf.stb = rsz.stb;
            sz.t = new UInt16[513];

            sz.t[0] = (UInt16)(rsz.t[0] * 256 + rsz.b);
            for (i = 1; i <= 512; i++)
            {
                sz.t[i] = (UInt16)(rsz.t[i * 2] * 256 + rsz.t[i * 2 - 1]);
            }

            if (dz == 0)
                blockCount = 1;
        }
        #endregion

        #region WriteSonarLine
        private void WriteSonarLine(TSZ sz, SonarLine line, StreamReader readerPos, int blockCount, bool writeCut)
        {
            int i, i2, co, ch, ct, oldct, oldco, anft, bcut, ecut;
            string date, typ;
            byte[] zc = new byte[4169];
            string s, datum, uhrzeit;

            datum = "";
            uhrzeit = "";
            date = "";
            typ = "";
            bcut = 0;
            ecut = 0;

            for (i = 0; i <= 4168; i++)
                zc[i] = 0;

            if ((sz.kopf.rw == 0) && ((sz.kopf.hw & 1) == 1))
                upktime2s(sz.kopf.lw, 4, out uhrzeit, out datum);

            if (readerPos != null)
            {
                if (((sz.kopf.stb & 64) == 64) || ((sz.kopf.rw != 0) && (sz.kopf.hw == 1)))
                {
                    s = readerPos.ReadLine();
                    if (CheckPosLine(s))
                    {
                        if (s.IndexOf('³') != -1)
                        {
                            date = s.Substring(0, s.IndexOf('³') - 1);
                            date = date.Remove(date.IndexOf("."));
                            s = s.Remove(0, s.IndexOf('³') + 1);
                            typ = s.Substring(0, s.IndexOf('>'));
                            s = s.Remove(0, s.IndexOf('>') + 1);
                        }
                        else
                        {
                            date = s.Substring(0, s.IndexOf('>') - 1);
                            date = date.Remove(date.IndexOf("."));
                            s = s.Remove(0, s.IndexOf('>') + 1);
                            typ = "0";
                            if (s.IndexOf("38=") != -1) typ = "5";
                            if (s.IndexOf("$GP") != -1) typ = "4";
                        }

                        if (typ == "5")
                        {
                            s = s.Replace(',', ';');
                            while (s.IndexOf('$') != -1)
                                s = s.Remove(s.IndexOf('$'), 1);
                            s += ";";
                        }

                        line.PosList.Add(new SonarPos(line.time.Date, date, typ, false, s));
                    }
                    else
                    {
                        UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Pos String error:" + s);
                    }
                }
            }
            else
            {
                if ((sz.kopf.stb & 64) == 64)
                {
                    string s2 = "import;37=" + ((double)sz.kopf.lw / 100.0).ToString("0.00", nfi) + ";38=" + ((double)sz.kopf.rw / 100.0).ToString("0.00", nfi) + ";39=" + ((float)sz.kopf.hw / 100.0F).ToString(nfi) + ";";
                    line.PosList.Add(new SonarPos(line.time.Date, "00:00:00", "5", false, s2));
                }
            }

            oldct = -1;
            ct = 0;

            for (i = 0; i <= blockCount; i++)
            {
                co = ((sz.t[i] >> 13) & 7) + 1;
                if (co == 8)
                    co = 7;
                ct = sz.t[i] & 0xff;
                if ((sz.t[i] & 0x0100) != 0)
                    ct += 255;

                if (oldct >= ct)
                {
                    ct = oldct;
                    break;
                }
                oldct = ct;

                ch = (sz.t[i] >> 10) & 7;
                if ((sz.t[i] & 0x0200) == 0)
                {
                    if (bcut == 0)
                        bcut = ct;
                }
                else
                {
                    if ((ecut == 0) && (bcut > 0))
                        ecut = ct;
                }

                for (i2 = 7 - ch; i2 <= 7; i2++)
                    zc[ct * 8 + i2] = (byte)co;
            }

            if (ecut == 0)
                ecut = ct;

            oldco = 0;
            anft = 0;

            LineData data;
            if (sz.kopf.stb > 127)
                data = line.NF;
            else
                data = line.HF;

            if (writeCut)
            {
                data.TCut = (-(float)CalcTiefeOld(bcut) / 10.0F);
                data.BCut = (-(float)CalcTiefeOld(ecut) / 10.0F);
            }
            else
            {
                data.BCut = -100;
                data.TCut = 0;
            }

            if (uhrzeit != "")
            {
                line.time = DateTime.Parse(uhrzeit);
            }

            List<DataEntry> entries = new List<DataEntry>();
            
            for (i = 0; i <= 521 * 8; i++)
            {
                if ((oldco == 0) && (zc[i] != 0))
                {
                    oldco = zc[i];
                    anft = i;
                }
                else if (oldco != zc[i])
                {
                    if (CalcTiefe(anft) != 10000)
                        entries.Add(NewEntry(i, oldco, anft, data));
                    oldco = zc[i];
                    anft = i;
                }
            }            
            data.Entries = entries.ToArray();
        }
        #endregion

        #region NewEntry
        private DataEntry NewEntry(int i, int oldco, int anft, LineData data)
        {
            DataEntry entry = new DataEntry();
            entry.colorID = (byte)oldco;
            entry.colorID--;
            entry.uncutHigh = (-(float)CalcTiefe(anft) / 100.0F) - 0.1F;
            entry.low = (-(float)CalcTiefe(i) / 100.0F) - 0.1F;
            entry.visible = true;

            if (entry.uncutHigh > data.tCut)
                entry.high = Math.Max(entry.low, data.tCut);
            else
                entry.high = entry.uncutHigh;

            entry.visible = (entry.high > entry.low) & (entry.high > data.bCut);

            if ((data.topColor != -1) && entry.visible)
                data.topColor = entry.colorID;

            return entry;
        }
        #endregion

        #region ConvertSonFile
        public sonFile(string fileName, bool writeCut, string year, string dx, string dy, string dz, string dp, string dr)
        {
            BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open), Encoding.Default);
            StreamReader readerPos = null;
            string sbtm = "", setm = "", sd = "";
            TFK fk = ReadTFK(reader);
            TSZ sz, szHF = new TSZ();
            int blockCount = 0, blockCountHF = 0;
            bool hf = false;
            data = "";

            string posFile = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".POS";
            if (File.Exists(posFile))
                readerPos = new StreamReader(posFile, Encoding.Default);

            if ((fk.k[0] != kennung[0]) | (fk.k[1] != kennung[1]) | (fk.k[2] != kennung[2]) | (fk.k[3] != kennung[3]))
                throw new Exception("Wrong SonFile Format.");

            upktime2s(fk.btime, 4, out sbtm, out sd);
            upktime2s(fk.etime, 4, out setm, out sd);
            record.Description = Path.GetFileName(fileName);

            DateTime date = DateTime.Parse(sd + "." + year);
            record.TimeStart = date.Add(DateTime.Parse(sbtm).TimeOfDay);
            record.TimeEnd = date.Add(DateTime.Parse(setm).TimeOfDay);

            record.Devices.Add(new SonarDevice(0, "Main-Sonar", Double.Parse(dx), Double.Parse(dy), Double.Parse(dz), 0.0, 0.0, true, "", "", true, true, 1460, 100, ""));

            for (int i = 0; i < fk.kl; i++)
            {
                // Read header data and block count.
                ReadSZ(reader, out sz, out blockCount, i);

                if (sz.kopf.stb < 127)
                {
                    // New HF section detected.
                    if (hf)
                    {
                        // There was a previous HF section without NF.
                        // Complete this before we can proceed.
                        SonarLine line = new SonarLine();
                        WriteSonarLine(szHF, line, readerPos, blockCountHF, writeCut);
                        record.AddSonarLine(line, false);
                    }

                    // Before writing the section to a new line, store all data (header and block count) and wait for next section.
                    szHF = sz;
                    blockCountHF = blockCount;
                    hf = true;
                }
                else
                {
                    // New NF section detected.
                    SonarLine line = new SonarLine();
                    if (hf)
                    {
                        // There was a previous HF section.
                        // Store both of them.
                        WriteSonarLine(szHF, line, readerPos, blockCountHF, writeCut);
                        WriteSonarLine(sz, line, readerPos, blockCount, writeCut);
                    }
                    else
                        // This NF section does not come along with HF.
                        // Store only NF.
                        WriteSonarLine(sz, line, readerPos, blockCount, writeCut);

                    record.AddSonarLine(line, false);
                    hf = false;
                }
            }

            reader.Close();
            record.RefreshDevices();
            record.UpdateAllCoordinates();

            if (readerPos != null)
                readerPos.Close();
        }
        #endregion
    }
}
