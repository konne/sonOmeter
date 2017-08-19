using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UKLib.Binary
{
    #region EndianBinaryReader
    public class EndianBinaryReader : BinaryReader
    {
        #region Variables
        private bool isBigEndian = false;
        private byte[] buffer = new byte[16];
        #endregion

        #region Functions
        new private void FillBuffer(int count)
        {
            this.Read(buffer, 0, count);
        }
        #endregion

        #region FloatingPoint
        public override double ReadDouble()
        {
            if (!isBigEndian)
                return base.ReadDouble();
            else
                throw new Exception("Not implemented yet");
        }

        public override float ReadSingle()
        {
            if (!isBigEndian)
                return base.ReadSingle();
            else
                throw new Exception("Not implemented yet");
        }

        public override decimal ReadDecimal()
        {
            if (!isBigEndian)
                return base.ReadDecimal();
            else
                throw new Exception("Not implemented yet");
        }
        #endregion

        #region IntTypes
        public override short ReadInt16()
        {
            if (!isBigEndian)
                return base.ReadInt16();
            else
            {
                FillBuffer(2);
                return (short)(buffer[1] | buffer[0] << 8);
            }
        }

        public override ushort ReadUInt16()
        {
            if (!isBigEndian)
                return base.ReadUInt16();
            else
            {
                FillBuffer(2);
                return (ushort)(buffer[1] | buffer[0] << 8);
            }
        }

        public override int ReadInt32()
        {
            if (!isBigEndian)
                return base.ReadInt32();
            else
            {
                FillBuffer(4);
                return (int)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
            }

        }

        public override uint ReadUInt32()
        {
            if (!isBigEndian)
                return base.ReadUInt32();
            else
            {
                FillBuffer(4);
                return (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
            }
        }

        public override long ReadInt64()
        {
            if (!isBigEndian)
                return base.ReadInt64();
            else
                throw new Exception("Not implemented yet");

        }

        public override ulong ReadUInt64()
        {
            if (!isBigEndian)
                return base.ReadUInt64();
            else
                throw new Exception("Not implemented yet");

        }
        #endregion

        #region Constructor
        public EndianBinaryReader(Stream s, bool BigEndian)
            : base(s)
        {
            isBigEndian = true;
        }
        #endregion
    }
    #endregion

    #region EndianBinaryWriter
    public class EndianBinaryWriter : BinaryWriter
    {
        #region Variables
        private bool isBigEndian = false;

        private byte[] buffer = new byte[16];
        #endregion

        #region Functions
        private void WriteBuffer(int count)
        {
            this.Write(buffer, 0, count);
        }
        #endregion

        #region Floatingpoint
        public override void Write(decimal value)
        {
            if (!isBigEndian)
                base.Write(value);
            else
            {
                throw new Exception("Not implemented yet");
            }
        }

        public override void Write(float value)
        {
            if (!isBigEndian)
                base.Write(value);
            else
            {
                throw new Exception("Not implemented yet");
            }
        }

        public override void Write(double value)
        {
            if (!isBigEndian)
                base.Write(value);
            else
            {
                throw new Exception("Not implemented yet");
            }
        }
        #endregion

        #region Int Types
        public override void Write(short value)
        {
            if (!isBigEndian)
            {
                base.Write(value);
            }
            else
            {
                buffer[1] = (byte)value;
                buffer[0] = (byte)(value >> 8);
                WriteBuffer(2);
            }
        }

        public override void Write(ushort value)
        {
            if (!isBigEndian)
            {
                base.Write(value);
            }
            else
            {
                buffer[1] = (byte)value;
                buffer[0] = (byte)(value >> 8);
                WriteBuffer(2);
            }
        }

        public override void Write(int value)
        {
            if (!isBigEndian)
            {
                base.Write(value);
            }
            else
            {
                buffer[3] = (byte)value;
                buffer[2] = (byte)(value >> 8);
                buffer[1] = (byte)(value >> 16);
                buffer[0] = (byte)(value >> 24);
                WriteBuffer(4);
            }
        }

        public override void Write(uint value)
        {
            if (!isBigEndian)
            {
                base.Write(value);
            }
            else
            {
                buffer[3] = (byte)value;
                buffer[2] = (byte)(value >> 8);
                buffer[1] = (byte)(value >> 16);
                buffer[0] = (byte)(value >> 24);
                WriteBuffer(4);
            }
        }

        public override void Write(long value)
        {
            if (!isBigEndian)
                base.Write(value);
            else
            {
                throw new Exception("Not implemented yet");
            }
        }

        public override void Write(ulong value)
        {
            if (!isBigEndian)
                base.Write(value);
            else
            {
                throw new Exception("Not implemented yet");
            }

        }
        #endregion

        #region Constructor
        public EndianBinaryWriter(Stream s, bool BigEndian)
            : base(s)
        {
            isBigEndian = true;
        }
        #endregion
    }
    #endregion
}
