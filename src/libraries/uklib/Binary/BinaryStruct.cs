using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UKLib.Binary
{
    #region BinaryStructReader Attributes
    public class BinaryStructIgnore : Attribute
    {

    }

    public class BinaryStructArray : Attribute
    {
        public int Count { get; set; }
        public string CountProperty { get; set; }

        public BinaryStructArray()
        {
            Count = 0;
            CountProperty = null;
        }
    }

    public class BinaryStructSize : Attribute
    {
        /// <summary>
        /// If size (-1) then read to end
        /// </summary>
        public int Size { get; set; }

        public BinaryStructSize(int Size)
        {
            this.Size = Size;
        }
    }
    #endregion

    #region BinaryStruct
    public class BinaryStruct
    {
        #region Write
        private static void WriteValue(BinaryWriter br, Type type, object value, BinaryStructSize abBinaryStructSize)
        {
            #region Attribute Parse
            if (abBinaryStructSize == null)
            {
                foreach (object mi in type.GetCustomAttributes(false))
                {
                    if (mi is BinaryStructSize)
                        abBinaryStructSize = mi as BinaryStructSize;

                }
            }
            #endregion

            switch (type.Name)
            {
                case "Byte":
                    br.Write((byte)value);
                    break;
                case "UInt16":
                    br.Write((UInt16)value);
                    break;
                case "UInt32":
                    br.Write((UInt32)value);
                    break;

                case "String":
                default:
                    if (type.IsClass)
                    {
                        if (abBinaryStructSize != null)
                        {
                            byte[] buffer = null;
                            if (type.Name == "String")
                            {
                                buffer = Encoding.ASCII.GetBytes((string)value);
                            }
                            else
                            {
                                if (value != null)
                                    Write(out buffer, value);
                            }
                            if (buffer != null)
                                br.Write(buffer);
                        }
                        else
                        {
                            throw new Exception("BinaryStruct classes need an BinaryStructSize Attribute");
                        }
                    }
                    break;
            }

        }

        public static void Write(out byte[] buffer, object value)
        {
            var bw = new EndianBinaryWriter(new MemoryStream(), true);

            Type structtype = value.GetType();

            if (structtype.IsClass)
            {
                foreach (FieldInfo fi in structtype.GetFields())
                {
                    Type type = fi.FieldType;

                    #region Parse Attributes
                    BinaryStructSize abBinaryStructSize = null;
                    BinaryStructArray abBinaryStructArray = null;
                    BinaryStructIgnore abBinaryStructIgnore = null;
                    FieldOffsetAttribute abFieldOffsetAttribute = null;
                    foreach (object mi in fi.GetCustomAttributes(false))
                    {
                        if (mi is BinaryStructSize)
                            abBinaryStructSize = mi as BinaryStructSize;
                        if (mi is BinaryStructArray)
                            abBinaryStructArray = mi as BinaryStructArray;
                        if (mi is BinaryStructIgnore)
                            abBinaryStructIgnore = mi as BinaryStructIgnore;
                        if (mi is FieldOffsetAttribute)
                            abFieldOffsetAttribute = mi as FieldOffsetAttribute;
                    }
                    #endregion

                    if (abBinaryStructIgnore == null)
                    {
                        if (type.IsArray)
                        {
                            #region FillArray

                            if (abBinaryStructArray != null)
                            {
                                int count = abBinaryStructArray.Count;

                                if ((abBinaryStructArray.CountProperty != null) & (count == 0))
                                {
                                    FieldInfo fiCount = structtype.GetField(abBinaryStructArray.CountProperty);
                                    if (fiCount != null)
                                    {
                                        try
                                        {
                                            count = Convert.ToInt32(fiCount.GetValue(value));
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }

                                // Array of Type
                                Type ItemType = Type.GetType(type.FullName.Replace("[]", ""));

                                // Create Array
                                Array arrayO = (Array)fi.GetValue(value);

                                // Write Array
                                for (int i = 0; i < count; i++)
                                {

                                    WriteValue(bw, ItemType, arrayO.GetValue(i), abBinaryStructSize);
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            WriteValue(bw, type, fi.GetValue(value), abBinaryStructSize);
                        }
                    }
                }
            }
            buffer = new byte[bw.BaseStream.Length];
            Array.Copy((bw.BaseStream as MemoryStream).GetBuffer(), buffer, buffer.Length);
        }

        #endregion

        #region Read
        private static object ReadValue(BinaryReader br, Type type, BinaryStructSize abBinaryStructSize)
        {
            #region Attribute Parse
            if (abBinaryStructSize == null)
            {
                foreach (object mi in type.GetCustomAttributes(false))
                {
                    if (mi is BinaryStructSize)
                        abBinaryStructSize = mi as BinaryStructSize;

                }
            }
            #endregion

            switch (type.Name)
            {
                case "Byte":
                    return br.ReadByte();
                case "UInt16":
                    return br.ReadUInt16();
                case "UInt32":
                    return br.ReadUInt32();

                case "String":
                default:
                    if (type.IsClass)
                    {
                        if (abBinaryStructSize != null)
                        {
                            int size = abBinaryStructSize.Size;
                            if (size == -1) size = (int)(br.BaseStream.Length - br.BaseStream.Position);
                            byte[] buffer = new byte[size];
                            br.Read(buffer, 0, size);
                            if (type.Name == "String")
                            {
                                return Encoding.ASCII.GetString(buffer);
                            }

                            return Parse(buffer, type);

                        }
                        else
                        {
                            throw new Exception("BinaryStruct classes need an BinaryStructSize Attribute");
                        }
                        //byte[] buffer = br.BaseStream.Length
                        //    return Parse(br, type);
                    }
                    return null;
            }

        }

        public static object Parse(byte[] buffer, Type structtype)
        {

            var br = new EndianBinaryReader(new MemoryStream(buffer), true);

            object result = null;

            ConstructorInfo ci = structtype.GetConstructor(Type.EmptyTypes);
            if (ci != null)
            {
                // Call Empty Constructor
                result = ci.Invoke(Type.EmptyTypes);

                foreach (FieldInfo fi in structtype.GetFields())
                {
                    Type type = fi.FieldType;

                    #region Parse Attributes
                    BinaryStructSize abBinaryStructSize = null;
                    BinaryStructArray abBinaryStructArray = null;
                    BinaryStructIgnore abBinaryStructIgnore = null;
                    FieldOffsetAttribute abFieldOffsetAttribute = null;
                    foreach (object mi in fi.GetCustomAttributes(false))
                    {
                        if (mi is BinaryStructSize)
                            abBinaryStructSize = mi as BinaryStructSize;
                        if (mi is BinaryStructArray)
                            abBinaryStructArray = mi as BinaryStructArray;
                        if (mi is BinaryStructIgnore)
                            abBinaryStructIgnore = mi as BinaryStructIgnore;
                        if (mi is FieldOffsetAttribute)
                            abFieldOffsetAttribute = mi as FieldOffsetAttribute;
                    }
                    #endregion

                    object o = null;

                    if (abBinaryStructIgnore == null)
                    {
                        if (type.IsArray)
                        {
                            #region FillArray

                            if (abBinaryStructArray != null)
                            {
                                int count = abBinaryStructArray.Count;

                                if ((abBinaryStructArray.CountProperty != null) & (count == 0))
                                {
                                    FieldInfo fiCount = structtype.GetField(abBinaryStructArray.CountProperty);
                                    if (fiCount != null)
                                    {
                                        try
                                        {
                                            count = Convert.ToInt32(fiCount.GetValue(result));
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }

                                // Array of Type
                                Type ItemType = Type.GetType(type.FullName.Replace("[]", ""));

                                // Create Array
                                Array arrayO = Array.CreateInstance(ItemType, count);

                                // Fill Array
                                for (int i = 0; i < count; i++)
                                {
                                    object val = ReadValue(br, ItemType, abBinaryStructSize);
                                    arrayO.SetValue(val, i);
                                }
                                o = arrayO;

                            }
                            #endregion
                        }
                        else
                        {
                            o = ReadValue(br, type, abBinaryStructSize);
                        }

                        if (o != null)
                            fi.SetValue(result, o);
                    }
                }
            }
            return result;
        }
        #endregion
    }
    #endregion
}
