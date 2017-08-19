using System;
using System.Collections.Generic;
using System.Text;

namespace UKLib.MathEx
{
    public struct Vector3D
    {
        public static Vector3D XAxis = new Vector3D(1.0, 0, 0);
        public static Vector3D YAxis = new Vector3D(0, 1.0, 0);
        public static Vector3D ZAxis = new Vector3D(0, 0, 1.0);

        public double X, Y, Z;

        public Vector3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public Vector3D(Point3D pt)
        {
            X = pt.X; Y = pt.Y; Z = pt.Z;
        }

        public Vector3D(Point3D startPoint, Point3D endPoint)
        {
            X = endPoint.X - startPoint.X;
            Y = endPoint.Y - startPoint.Y;
            Z = endPoint.Z - startPoint.Z;
        }

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public void Normalise()
        {
            double m = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (m > 0.001)
            {
                X /= m; Y /= m; Z /= m;
            }
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.X, -v.Y, -v.Z);
        }

        // A x B = |A|*|B|*sin(angle), direction follow right-hand rule
        public static Vector3D CrossProduct(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        public static double DotProduct(Vector3D v1, Vector3D v2) // A . B = |A|*|B|*cos(angle)
        {
            return (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
        }

        public Vector3D CrossProduct(Vector3D v)
        {
            return CrossProduct(this, v);
        }

        public double DotProduct(Vector3D v)
        {
            return DotProduct(this, v);
        }
    }
}
