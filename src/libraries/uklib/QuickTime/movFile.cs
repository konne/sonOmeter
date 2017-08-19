using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using UKLib.Binary;
using System.Windows.Forms;

namespace UKLib.QuickTime
{
    #region QuickTime Atoms
    #region QTTimeConversion
    public class QTTime
    {
        private static DateTime QTRefDateTime = new DateTime(1904, 1, 1);

        public static DateTime ToDateTime(uint qticks)
        {
            return QTRefDateTime.AddSeconds(qticks);
        }

        public static uint ToQTTicks(DateTime time)
        {
            return (uint)System.Math.Round((time - QTRefDateTime).TotalSeconds);
        }
    }
    #endregion

    #region QTAtomTypes
    public enum QTAtomTypes
    {
        moov, prfl, mvhd, clip, crgn, udta,
        trak, tkhd, matt, kmat, edts, elst,
        mdia, mdhd, hdlr, mhlr,
        minf, vmhd, dinf, dref,
        stbl, stts, stss, stsd, stsz, stsc, stco,
        mdat, wide, free, ftyp, pnot, skip,
        none
    }
    #endregion

    #region QTMatrixStucture
    [BinaryStructSize(36)]
    public class QTMatrixStucture
    {
        public UInt32 a;
        public UInt32 b;
        public UInt32 u;
        public UInt32 c;
        public UInt32 d;
        public UInt32 v;
        public UInt32 x;
        public UInt32 y;
        public UInt32 w;

        public QTMatrixStucture()
        {
        }
    }
    #endregion

    #region QTEditListTable
    [BinaryStructSize(12)]
    public class QTEditListTable
    {
        public UInt32 TrackDuration;
        public UInt32 MediaTime;
        public UInt32 MediaRate;

        public QTEditListTable()
        { }
    }
    #endregion

    #region QTSample2ChunkTable
    [BinaryStructSize(12)]
    public class QTSample2ChunkTable
    {
        public UInt32 FirstChunk;
        public UInt32 SamplesPerChunk;
        public UInt32 SampleDescriptionID;

        public QTSample2ChunkTable()
        { }
    }
    #endregion

    #region  QTTime2SampleTable
    [BinaryStructSize(8)]
    public class QTTime2SampleTable
    {
        public UInt32 SampleCount;
        public UInt32 SampleDuration;

        public QTTime2SampleTable()
        { }
    }
    #endregion

    #region QTmvhd
    public class QTmvhd
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 Creationtime;
        public UInt32 Modificationtime;
        public UInt32 Timescale;
        public UInt32 Duration;
        public UInt32 Preferredrate;
        public UInt16 Preferredvolume;
        [BinaryStructArray(Count = 10)]
        public byte[] Reserved = new byte[10];
        public QTMatrixStucture Matrixstructure = new QTMatrixStucture();
        public UInt32 Previewtime;
        public UInt32 Previewduration;
        public UInt32 Postertime;
        public UInt32 Selectiontime;
        public UInt32 Selectionduration;
        public UInt32 Currenttime;
        public UInt32 NexttrackID;
    }
    #endregion

    #region QTtkhd
    public class QTtkhd
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 Creationtime;
        public UInt32 Modificationtime;
        public UInt32 TrackID;
        public UInt32 Reserved;
        public UInt32 Duration;
        [BinaryStructArray(Count = 8)]
        public byte[] Reserved1 = new byte[8];
        public UInt16 Layer;
        public UInt16 Alternategroup;
        public UInt16 Volume;
        public UInt16 Reserved2;
        public QTMatrixStucture Matrixstructure = new QTMatrixStucture();
        public UInt32 Trackwidth;
        public UInt32 Trackheight;

        public QTtkhd()
        { }
    }
    #endregion

    #region QTmdhd
    public class QTmdhd
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 Creationtime;
        public UInt32 Modificationtime;
        public UInt32 Timescale;
        public UInt32 Duration;
        public UInt16 Language;
        public UInt16 Quality;

        public QTmdhd()
        { }
    }
    #endregion

    #region QThdlr
    public class QThdlr
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 Componenttype;
        public UInt32 Componentsubtype;
        public UInt32 Componentmanufacturer;
        public UInt32 Componentflags;
        public UInt32 Componentflagsmask;
        public byte ComponentNameSize;
        [BinaryStructSize(-1)]
        public string Componentname;

        public QThdlr()
        { }
    }
    #endregion

    #region QTelst
    public class QTelst
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 NumberOfEntries;
        [BinaryStructArray(CountProperty = "NumberOfEntries")]
        public QTEditListTable[] EditListTable;

        public QTelst()
        { }
    }
    #endregion

    #region QTvmhd
    public class QTvmhd
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt16 Graphicsmode;
        [BinaryStructArray(Count = 6)]
        public byte[] Opcolor = new byte[6];

        public QTvmhd()
        { }
    }
    #endregion

    #region QTstts
    public class QTstts
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 NumberOfEntries;
        [BinaryStructArray(CountProperty = "NumberOfEntries")]
        public QTTime2SampleTable[] Time2SampleTable;

        public QTstts()
        { }
    }
    #endregion

    #region QTstsc
    public class QTstsc
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 NumberOfEntries;
        [BinaryStructArray(CountProperty = "NumberOfEntries")]
        public QTSample2ChunkTable[] Sample2ChunkTable;

        public QTstsc()
        { }
    }
    #endregion

    #region QTstsz
    public class QTstsz
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 SampleSize;
        public UInt32 NumberOfEntries;
        [BinaryStructArray(CountProperty = "NumberOfEntries")]
        public UInt32[] SampleSizeTable;

        public QTstsz()
        { }
    }
    #endregion

    #region QTstco
    public class QTstco
    {
        public byte Version;
        [BinaryStructArray(Count = 3)]
        public byte[] Flags = new byte[3];
        public UInt32 NumberOfEntries;
        [BinaryStructArray(CountProperty = "NumberOfEntries")]
        public UInt32[] ChunkOffsetTable;

        public QTstco()
        { }
    }
    #endregion
    #endregion

    #region QTMovFile
    public class QTMovFile
    {
        #region Variables
        public QTSimpleAtom moovAtom = null;
        public QTSimpleAtom freeAtom = null;
        public QTSimpleAtom mdatAtom = null;
       
        public Dictionary<QTAtomTypes, QTSimpleAtom> AtomsDict = new Dictionary<QTAtomTypes, QTSimpleAtom>();

        FileStream fs = null;
        EndianBinaryReader br;
        EndianBinaryWriter bw;

        bool fileOpen = false;
        bool appendingFrame = false;
        #endregion

        #region Functions
        public void RefreshDictionary(QTSimpleAtom node)
        {
            // tbd !!!
            AtomsDict.Add(node.QType, node);
            foreach (var child in node.Childs)
                RefreshDictionary(child);
        }
        #endregion

        #region Constructor
        public QTMovFile(string filename)
        {
            fs = new FileStream(filename, FileMode.Open);
            br = new EndianBinaryReader(fs, true);
            bw = new EndianBinaryWriter(fs, true);

            int idx = 0;
            while (br.BaseStream.Position < (br.BaseStream.Length))
            {
                QTSimpleAtom cnode = new QTSimpleAtom(br);

                switch (idx)
                {
                    case 0:
                        if (cnode.QType != QTAtomTypes.moov) throw new Exception("moov has to be the first Atom");
                        moovAtom = cnode;
                        RefreshDictionary(cnode);
                        break;
                    case 1:
                        if (cnode.QType != QTAtomTypes.free) throw new Exception("free has to be the second Atom");
                        freeAtom = cnode;
                        RefreshDictionary(cnode);
                        break;
                    case 2:
                        if (cnode.QType != QTAtomTypes.mdat) throw new Exception("mdat has to be the second Atom");
                        mdatAtom = cnode;
                        RefreshDictionary(cnode);
                        break;
                }                                             
                idx++;
            }
            fileOpen = true;
        }
        #endregion

        public int FrameCount
        {
            get
            {
                try
                {
                    UInt32 Samples = 0;
                    QTstts stts = (AtomsDict[QTAtomTypes.stts].Data as QTstts);                    
                    for (int i = 0; i < stts.NumberOfEntries; i++)
                    {                        
                        Samples += stts.Time2SampleTable[i].SampleCount;
                    }
                    return (int)Samples;
                }
                catch
                {
                    return -1;
                }
            }
        }

        #region FrameFunctions
        public void AppendFrame(byte[] frame, double time)
        {
            if ((!fileOpen) & (fs != null)) return;

            appendingFrame = true;
            try
            {
                bw.BaseStream.Position = bw.BaseStream.Length;
                bw.Write(frame);

                QTstts stts = (AtomsDict[QTAtomTypes.stts].Data as QTstts);
                if (stts.NumberOfEntries >= stts.Time2SampleTable.Length)
                {
                    QTTime2SampleTable[] newT = new QTTime2SampleTable[stts.NumberOfEntries + 100];
                    Array.Copy(stts.Time2SampleTable, newT, stts.NumberOfEntries);
                    stts.Time2SampleTable = newT;
                }

                QTmdhd mdhd = (AtomsDict[QTAtomTypes.mdhd].Data as QTmdhd);

                stts.Time2SampleTable[stts.NumberOfEntries] = new QTTime2SampleTable() { SampleCount = 1, SampleDuration = (uint)(time * mdhd.Timescale) };
                stts.NumberOfEntries++;

                QTstsz stsz = (AtomsDict[QTAtomTypes.stsz].Data as QTstsz);
                if (stsz.NumberOfEntries >= stsz.SampleSizeTable.Length)
                {
                    uint[] newT = new uint[stsz.NumberOfEntries + 100];
                    Array.Copy(stsz.SampleSizeTable, newT, stsz.NumberOfEntries);
                    stsz.SampleSizeTable = newT;
                }
                stsz.SampleSizeTable[stsz.NumberOfEntries] = (uint)frame.Length;
                stsz.NumberOfEntries++;
            }
            finally
            {
                appendingFrame = false;
            }
        }

        public byte[] GetFrame(int idx)
        {
            if ((!fileOpen) & (fs != null)) return null;
            
            QTstsz stsz = (AtomsDict[QTAtomTypes.stsz].Data as QTstsz);
            long framePos = AtomsDict[QTAtomTypes.mdat].DataPos;

            for (int i = 0; i < idx; i++)
                framePos+= (long)stsz.SampleSizeTable[i];

            br.BaseStream.Position = framePos;
            int size = (int)stsz.SampleSizeTable[idx];
            
            return br.ReadBytes(size) ;
        }
        #endregion

        #region Close
        public void Close()
        {
            Close(false);
        }

        public void Close(bool write)
        {
            if (fs != null)
            {
                fileOpen = false;
                if (write)
                {
                    try
                    {
                        int id = 0;
                        while ((appendingFrame) && (id++ < 1000)) Application.DoEvents();

                        br.BaseStream.Position = 0;

                        #region Recal TotalTime
                        QTstts stts = (AtomsDict[QTAtomTypes.stts].Data as QTstts);
                        UInt32 Duration = 0;
                        UInt32 Samples = 0;
                        for (int i = 0; i < stts.NumberOfEntries; i++)
                        {
                            Duration += stts.Time2SampleTable[i].SampleDuration;
                            Samples += stts.Time2SampleTable[i].SampleCount;
                        }
                        QTmdhd mdhd = (AtomsDict[QTAtomTypes.mdhd].Data as QTmdhd);
                        if (mdhd != null) mdhd.Duration = Duration;
                        QTmvhd mvhd = (AtomsDict[QTAtomTypes.mvhd].Data as QTmvhd);
                        if (mvhd != null) mvhd.Duration = Duration;
                        QTtkhd tkhd = (AtomsDict[QTAtomTypes.tkhd].Data as QTtkhd);
                        if (tkhd != null) tkhd.Duration = Duration;


                        QTstsc stsc = (AtomsDict[QTAtomTypes.stsc].Data as QTstsc);
                        if ((stsc != null) && (stsc.Sample2ChunkTable != null) && (stsc.Sample2ChunkTable.Length > 0))
                            stsc.Sample2ChunkTable[0].SamplesPerChunk = Samples;
                        #endregion

                        moovAtom.Write(bw);

                        InsertFreeAtPosition(bw);

                        br.BaseStream.Position = AtomsDict[QTAtomTypes.mdat].DataPos - 8;
                        long diff = br.BaseStream.Length - br.BaseStream.Position;
                        bw.Write((UInt32)diff);
                    }
                    catch
                    {
                    }

                }
                fs.Close();
            }
        }
        #endregion

        #region EmptyMPJEGVideoFile
        public static QTMovFile EmptyMPJEGVideoFile(string filename)
        {
            EndianBinaryWriter bw = new EndianBinaryWriter(new FileStream(filename, FileMode.Create), true);

            UInt32 qtTimeNow = QTTime.ToQTTicks(DateTime.Now);

            #region Predefined
            QTSimpleAtom moovTree = new QTSimpleAtom()
             {
                 Name = "moov",
                 Childs = new List<QTSimpleAtom>() {
                     new QTSimpleAtom () {Name = "mvhd", Data = new QTmvhd() {Creationtime = qtTimeNow, Modificationtime = qtTimeNow, Timescale = 600, Duration = 0, Preferredrate = 65536, Preferredvolume = 256, NexttrackID = 2}, Childs = new List<QTSimpleAtom>() {} },
                     new QTSimpleAtom () {Name = "trak", Childs = new List<QTSimpleAtom>() {
                       new QTSimpleAtom () {Name = "tkhd", Data = new QTtkhd() {Creationtime = qtTimeNow, Modificationtime = qtTimeNow, TrackID = 1, Duration = 0}, Childs = new List<QTSimpleAtom>() {} },
                       new QTSimpleAtom () {Name = "edts", Childs = new List<QTSimpleAtom>() {
                         new QTSimpleAtom () {Name = "elst", Data = new QTelst() {NumberOfEntries = 1, EditListTable = new QTEditListTable[] {new QTEditListTable() {TrackDuration= 0, MediaTime = 0, MediaRate = 65536}}}, Childs = new List<QTSimpleAtom>() {} }} },
                       new QTSimpleAtom () {Name = "mdia", Childs = new List<QTSimpleAtom>() {
                         new QTSimpleAtom () {Name = "mdhd", Data = new QTmdhd() {Creationtime = qtTimeNow, Modificationtime = qtTimeNow, Timescale = 600, Duration = 0}, Childs = new List<QTSimpleAtom>() {} },
                         new QTSimpleAtom () {Name = "hdlr", Data = new QThdlr() {Componenttype = 1835560050, Componentsubtype = 1986618469, ComponentNameSize = 12, Componentname = "VideoHandler"}, Childs = new List<QTSimpleAtom>() {} },
                         new QTSimpleAtom () {Name = "minf", Childs = new List<QTSimpleAtom>() {
                           new QTSimpleAtom () {Name = "vmhd", Data = new QTvmhd() {}, Childs = new List<QTSimpleAtom>() {} },
                           new QTSimpleAtom () {Name = "dinf", Childs = new List<QTSimpleAtom>() {
                             new QTSimpleAtom () {Name = "dref", Data = new byte[20] {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 12, 117, 114, 108, 32, 0, 0, 0, 1}, Childs = new List<QTSimpleAtom>() {} }} },
                           new QTSimpleAtom () {Name = "stbl", Childs = new List<QTSimpleAtom>() {
                             new QTSimpleAtom () {Name = "stsd", Data = new byte[94] {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 86, 109, 106, 112, 97, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 72, 0, 0, 0, 72, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24, 255, 255}, Childs = new List<QTSimpleAtom>() {} },
                             new QTSimpleAtom () {Name = "stts", Data = new QTstts() {NumberOfEntries = 0}, Childs = new List<QTSimpleAtom>() {} },
                             new QTSimpleAtom () {Name = "stsc", Data = new QTstsc() {NumberOfEntries = 1, Sample2ChunkTable = new QTSample2ChunkTable[] { new QTSample2ChunkTable(){FirstChunk = 1, SamplesPerChunk = 0x00, SampleDescriptionID = 0x01 }}}, Childs = new List<QTSimpleAtom>() {} },
                             new QTSimpleAtom () {Name = "stsz", Data = new QTstsz() {NumberOfEntries = 0}, Childs = new List<QTSimpleAtom>() {} },
                             new QTSimpleAtom () {Name = "stco", Data = new QTstco() {NumberOfEntries = 1, ChunkOffsetTable = new UInt32[1] {0x070008} }, Childs = new List<QTSimpleAtom>() {} }} }} }} }} }}
             };
            #endregion

            moovTree.Write(bw);

            InsertFreeAtPosition(bw);

            #region mdat
            bw.Write(8);
            bw.Write(new char[4] { 'm', 'd', 'a', 't' });
            #endregion

            bw.Close();

            return new QTMovFile(filename);
        }
        #endregion

        #region Free Space
        private static void InsertFreeAtPosition(EndianBinaryWriter bw)
        {
           
            long free = 0x70000 - bw.BaseStream.Position;
            bw.Write((UInt32)free);
            bw.Write(new char[4] { 'f', 'r', 'e', 'e' });
            free -= 8;
            byte[] buf = new byte[1024];
            bw.Write(buf, 0, (int)(free % 1024));
            while (bw.BaseStream.Length < 0x70000) bw.Write(buf, 0, buf.Length);

        }
        #endregion
    }

    #endregion

    #region QTSimpleAtom
    public class QTSimpleAtom
    {
        private string MakeSource(QTSimpleAtom node, int level)
        {
            string schild = "";
            foreach (QTSimpleAtom child in node.Childs)
            {
                if (schild != "") schild += ",";
                schild += "\r\n".PadRight(3 + level * 2); ;
                schild += MakeSource(child, level + 1);
            }

            string sdata = "";
            if (node.Data != null)
            {
                if (node.Data.GetType().FullName == "System.Byte[]")
                {
                    byte[] buf = (byte[])(node.Data);
                    sdata = "new byte[" + buf.Length + "] {";
                    for (int i = 0; i < buf.Length; i++)
                    {
                        sdata += buf[i].ToString();
                        if (i < (buf.Length - 1)) sdata += ", ";
                    }
                    sdata += "}";
                }
                else
                {
                    Type type = node.Data.GetType();
                    sdata += "new " + type.Name + "() {";
                    string sfi = "";
                    foreach (FieldInfo fi in type.GetFields())
                    {
                        if (!fi.FieldType.IsArray)
                        {
                            object value = fi.GetValue(node.Data);

                            try
                            {
                                Int64 i64 = Convert.ToInt64(value);
                                if (i64 == 0) value = null;
                            }
                            catch
                            {

                            }
                            if (value != null)
                            {
                                if (sfi != "") sfi += ", ";
                                sfi += fi.Name + " = " + fi.GetValue(node.Data);
                            }
                        }
                    }
                    sdata += sfi + "}";

                }
                sdata = ", Data = " + sdata;
            }
            return "new QTSimpleAtom () {Name = \"" + node.QType.ToString() + "\"" + sdata + ", Childs = new List<QTSimpleAtom>() {" + schild + "} }";
        }

        #region Vars & Props
        public UInt32 Size { get; private set; }

        public string Name
        {
            get
            {
                return QType.ToString();
            }
            set
            {
                try
                {
                    QType = (QTAtomTypes)Enum.Parse(typeof(QTAtomTypes), value, true);
                }
                catch
                {
                    Console.WriteLine("!!Achtung erweitern um" + value);
                    QType = QTAtomTypes.none;
                }
            }
        }
        public long DataPos { get; private set; }
        public QTAtomTypes QType { get; set; }

        public object Data { get; set; }

        public List<QTSimpleAtom> Childs { get; set; }
        #endregion

        public bool IsContainer
        {
            get
            {
                switch (QType)
                {
                    case QTAtomTypes.moov:
                    case QTAtomTypes.trak:
                    case QTAtomTypes.edts:
                    case QTAtomTypes.clip:
                    case QTAtomTypes.matt:
                    case QTAtomTypes.mdia:
                    case QTAtomTypes.dinf:
                    case QTAtomTypes.stbl:
                    case QTAtomTypes.minf:
                        return true;
                    default:
                        return false;
                }
            }
        }


        public QTSimpleAtom()
        {
        }


        public QTSimpleAtom(BinaryReader br)
        {
            Childs = new List<QTSimpleAtom>();
            Size = br.ReadUInt32() - 8;
            char[] name = br.ReadChars(4);
            Name = "" + name[0] + name[1] + name[2] + name[3];

            DataPos = br.BaseStream.Position;

            if (IsContainer)
            {
                while (br.BaseStream.Position < (DataPos + Size))
                {
                    QTSimpleAtom cnode = new QTSimpleAtom(br);
                    Childs.Add(cnode);
                }
            }
            else
            {
                if ((Name != "mdat") & (Name != "free"))
                {
                    byte[] buf = new byte[Size];
                    br.Read(buf, 0, (int)Size);
                    Type type = Type.GetType("UKLib.QuickTime.QT" + Name);
                    if (Name == "stts")
                    {
                        Name = Name;
                    }
                    if (type != null)
                    {
                        Data = BinaryStruct.Parse(buf, type);

                    }
                    else
                    {
                        Data = buf;
                        Console.WriteLine(Name);
                    }
                }
            }
            br.BaseStream.Position = DataPos + Size;
        }

        public void Write(BinaryWriter bw)
        {

            byte[] buffer = null;

            if (IsContainer)
            {
                EndianBinaryWriter bwc = new EndianBinaryWriter(new MemoryStream(), true);
                foreach (var item in Childs)
                {
                    item.Write(bwc);
                }
                buffer = new byte[bwc.BaseStream.Length];
                Array.Copy((bwc.BaseStream as MemoryStream).GetBuffer(), buffer, buffer.Length);
            }
            else
            {
                if (Data != null)
                {
                    if (Data.GetType().FullName == "System.Byte[]")
                    {
                        buffer = (byte[])Data;
                    }
                    else
                    {
                        BinaryStruct.Write(out buffer, Data);
                    }
                }
                else
                {
                    Data = null;
                }
            }

            UInt32 size = 8;
            if (buffer != null)
                size += (UInt32)buffer.Length;

            bw.Write((UInt32)size);
            bw.Write(Encoding.ASCII.GetBytes(Name));
            if (buffer != null) bw.Write(buffer);
        }

        public override string ToString()
        {
            return Name + " " + Size.ToString();
        }
    }
    #endregion
}
