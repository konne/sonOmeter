using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using UKLib.MathEx;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace UKLib.DXF
{
    public struct DXFGroup
    {
        public string Code;
        public string Data;

        public DXFGroup(string code, string data)
        {
            Code = code;
            Data = data;
        }
    }

    public class DXFAtom
    {
        [Browsable(false)]
        public string Type { get; protected set; }
        [Browsable(false)]
        public string Handle { get; protected set; }
        [Browsable(false)]
        public string Pointer { get; protected set; }

        public DXFAtom()
        {
            //Init();
        }

        public DXFAtom(string type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return Type;
        }

        protected virtual void Init()
        {
            Type = "";
            Handle = "";
            Pointer = "";
        }

        public virtual DXFGroup Read(StreamReader reader)
        {
            DXFGroup group;

            // Call begin read function for preparations.
            BeginRead();

            // Read next lines.
            group = ReadLines(reader);

            while ((group.Code != "0") && !reader.EndOfStream)
            {
                // Call group code interpreter.
                NewGroupCode(group);

                // Read next lines.
                group = ReadLines(reader);
            }

            // Call end read function for custom final steps.
            EndRead();

            // Return next type specifier
            return group;
        }

        internal virtual bool NewGroupCode(DXFGroup group)
        {
            switch (group.Code)
            {
                case "5":
                    Handle = group.Data;
                    break;
                case "330":
                    if ((Pointer == null) || (Pointer.Length == 0))
                        Pointer = group.Data;
                    else
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal virtual void BeginRead()
        {
        }

        internal virtual void EndRead()
        {
        }

        public virtual void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
        }

        public virtual void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
        }

        public virtual void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
        }

        public static DXFGroup ReadLines(StreamReader reader)
        {
            string code = reader.ReadLine();
            if (code != null)
                code = code.Trim();

            string data = reader.ReadLine();
            if (data != null)
                data = data.Trim();

            return new DXFGroup(code, data);
        }
    }
}
