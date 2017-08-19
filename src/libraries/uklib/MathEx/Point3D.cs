using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.MathEx
{
    public struct Point3D
    {
        private double x;
        private double y;
        private double z;

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

        public double Z
        {
            get { return z; }
            set { z = value; }
        }

        public bool IsEmpty
        {
            get { return ((x == 0) & (y == 0) & (z == 0)); }
        }

        public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public PointD PointXY
        {
            get { return new PointD(x, y); }
        }

        public Point3D NEDtoENU
        {
            get { return new Point3D(y, x, -z); }
        }

        public static bool operator ==(Point3D a, Point3D b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Point3D a, Point3D b)
        {
            return !(a == b);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Point3D p = (Point3D)obj;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return x == p.x && y == p.y && z == p.z;
        }

        public void Offset(double dx, double dy, double dz)
        {
            x += dx;
            y += dy;
            z += dz;
        }

        public double Distance(Point3D pt)
        {
            return System.Math.Sqrt((pt.x - x) * (pt.x - x) + (pt.y - y) * (pt.y - y) + (pt.z - z) * (pt.z - z));
        }

        public static Point3D operator -(Point3D pt1, Point3D pt2)
        {
            return new Point3D(pt1.x - pt2.x, pt1.y - pt2.y, pt1.z - pt2.z);
        }

        public static Point3D operator -(Point3D pt1, double d)
        {
            return new Point3D(pt1.x - d, pt1.y - d, pt1.z - d);
        }

        public static Point3D operator -(Point3D pt1)
        {
            return new Point3D(-pt1.x, -pt1.y, -pt1.z);
        }

        public static Point3D operator +(Point3D pt1, Point3D pt2)
        {
            return new Point3D(pt1.x + pt2.x, pt1.y + pt2.y, pt1.z + pt2.z);
        }

        public static Point3D operator +(Point3D pt1, double d)
        {
            return new Point3D(pt1.x + d, pt1.y + d, pt1.z + d);
        }

        public static Point3D operator *(Point3D pt, double d)
        {
            return new Point3D(pt.x * d, pt.y * d, pt.z * d);
        }

        public static Point3D operator /(Point3D pt, double d)
        {
            return new Point3D(pt.x / d, pt.y / d, pt.z / d);
        }

        #region ToString Variants
        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString() + ", " + z.ToString();
        }

        public string ToString(NumberFormatInfo nfi)
        {
            return x.ToString("f", nfi) + ", " + y.ToString("f", nfi) + ", " + z.ToString("f", nfi);
        }
        #endregion
    }

    public struct Point3F
    {
        private float x;
        private float y;
        private float z;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public Point3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
