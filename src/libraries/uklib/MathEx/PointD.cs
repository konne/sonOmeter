using System;
using System.Xml;
using System.Drawing;
using System.Globalization;
using UKLib.Xml;
using System.ComponentModel;
using UKLib.Survey.Math;

namespace UKLib.MathEx
{
    /// <summary>
    /// Summary description for PointD.
    /// </summary>
    public struct PointD
    {
        public static PointD Origin = new PointD(0, 0);

        #region Variables
        double x;
        double y;

        const double Rad2Deg = 180.0 / System.Math.PI;
        #endregion

        #region Properties
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public bool IsZero
        {
            get { return ((X == 0) && (Y == 0)); }
        }

        public Point Point
        {
            get { return new Point((int)x, (int)y); }
        }

        public PointF PointF
        {
            get { return new PointF((float)x, (float)y); }
        }

        public double Length
        {
            get
            {
                return System.Math.Sqrt(x * x + y * y);
            }
            set
            {
                double l = value / this.Length;

                x *= l;
                y *= l;
            }
        }
        #endregion

        #region XML r/w
        /// <summary>
        /// Writes the object to an XML stream.
        /// </summary>
        /// <param name="writer">XML writer object</param>
        /// <param name="nfi">The number format info of the application.</param>
        public void ToXml(XmlTextWriter writer, NumberFormatInfo nfi)
        {
            try
            {
                writer.WriteStartElement("pointD");
                writer.WriteAttributeString("x", x.ToString(nfi));
                writer.WriteAttributeString("y", y.ToString(nfi));
                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "PointD.ToXml: " + e.Message);
            }
        }

        /// <summary>
        /// Reads the object from an XML stream.
        /// </summary>
        /// <param name="reader">XML reader object.</param>
        /// <param name="nfi">The number format info of the application.</param>
        public void FromXml(XmlTextReader reader)
        {
            try
            {
                x = XmlReadConvert.Read(reader, "x", 0.0);
                y = XmlReadConvert.Read(reader, "y", 0.0);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "PointD.FromXml: " + e.Message);
            }
        }
        #endregion

        #region Constructor
        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public PointD(Point pt)
        {
            this.x = pt.X;
            this.y = pt.Y;
        }

        public PointD(PointF pt)
        {
            this.x = pt.X;
            this.y = pt.Y;
        }

        public PointD(XmlTextReader reader)
        {
            this.x = 0.0;
            this.y = 0.0;

            FromXml(reader);
        } 
        #endregion

        #region ToString Variants
        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString();
        }

        public string ToString(NumberFormatInfo nfi)
        {
            return x.ToString("f", nfi) + ", " + y.ToString("f", nfi);
        } 
        #endregion

        #region Maths
        public double Distance(PointD pt)
        {
            return System.Math.Sqrt((pt.x - x) * (pt.x - x) + (pt.y - y) * (pt.y - y));
        }

        public double Distance(double x2, double y2)
        {
            return System.Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));
        }

        public double Distance(Coordinate c)
        {
            return System.Math.Sqrt((c.d1 - x) * (c.d1 - x) + (c.d2 - y) * (c.d2 - y));
        }

        public PointD Normalize()
        {
            double l = this.Length;

            return new PointD(x / l, y / l);
        }

        public double AngleRad(PointD pt)
        {
            return System.Math.Atan2(pt.y - this.y, pt.x - this.x);
        }

        public double AngleDeg(PointD pt)
        {
            return AngleRad(pt) * Rad2Deg;
        }

        public RectangleD BuildRectD(double width, double height)
        {
            return new RectangleD(x - width / 2.0, y - height / 2.0, x + width / 2.0, y + height / 2.0);
        }

        public RectangleF BuildRectF(float width, float height)
        {
            return new RectangleF((float)x - width / 2.0F, (float)y - height / 2.0F, width, height);
        }

        public PointD Offset(double x, double y)
        {
            return new PointD(this.x + x, this.y + y);
        }

        public PointD Rotate(double angleDeg)
        {
            double angle = angleDeg / Rad2Deg;

            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);

            return new PointD(x * cos - y * sin, x * sin + y * cos);
        }

        public PointD Scale(double dX, double dY)
        {
            return new PointD(this.x * dX, this.y * dY);
        }
        #endregion

        #region Operators
        public static PointD operator -(PointD pt1, PointD pt2)
        {
            return new PointD(pt1.x - pt2.x, pt1.y - pt2.y);
        }

        public static PointD operator -(Point pt1, PointD pt2)
        {
            return new PointD((double)pt1.X - pt2.x, (double)pt1.Y - pt2.y);
        }

        public static PointD operator -(PointD pt1, double d)
        {
            return new PointD(pt1.x - d, pt1.y - d);
        }

        public static PointD operator -(PointD pt1)
        {
            return new PointD(-pt1.x, -pt1.y);
        }

        public static PointD operator +(PointD pt1, PointD pt2)
        {
            return new PointD(pt1.x + pt2.x, pt1.y + pt2.y);
        }

        public static PointD operator +(PointD pt1, double d)
        {
            return new PointD(pt1.x + d, pt1.y + d);
        }

        public static PointD operator *(PointD pt, double d)
        {
            return new PointD(pt.x * d, pt.y * d);
        }

        public static PointD operator /(PointD pt, double d)
        {
            return new PointD(pt.x / d, pt.y / d);
        }

        public bool Equals(PointD pt)
        {
            return (pt.x == this.x) && (pt.y == this.y);
        }
        #endregion
    }
}
