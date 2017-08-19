using System;
using System.Collections.Generic;
using System.Text;

namespace UKLib.MathEx
{
    public struct QuaternionD
    {
        public double X, Y, Z, W;

        public QuaternionD(double w, double x, double y, double z)
        {
            W = w; X = x; Y = y; Z = z;
        }

        public QuaternionD(double w, Vector3D v)
        {
            W = w; X = v.X; Y = v.Y; Z = v.Z;
        }

        public QuaternionD(RotD rot)
        {
            RotD r = new RotD(rot * (Math.PI / 360.0)); // PI/180 / 2, refer to definition of quaternion!

            // Yaw / Heading / Ψ
            double cy = Math.Cos(r.Yaw);
            double sy = Math.Sin(r.Yaw);
            // Roll / Bank / Φ
            double cr = Math.Cos(r.Roll);
            double sr = Math.Sin(r.Roll);
            // Pitch / Attitude / θ
            double cp = Math.Cos(r.Pitch);
            double sp = Math.Sin(r.Pitch);

            // Assemble from single rotation quaternions in the order yaw -> pitch -> roll
            QuaternionD qx = new QuaternionD(cr, sr, 0, 0);
            QuaternionD qy = new QuaternionD(cp, 0, sp, 0);
            QuaternionD qz = new QuaternionD(cy, 0, 0, sy);
            QuaternionD q = qz * qy;
            q *= qx;

            W = q.W;
            X = q.X;
            Y = q.Y;
            Z = q.Z;
        }

        public Vector3D V
        {
            set { X = value.X; Y = value.Y; Z = value.Z; }
            get { return new Vector3D(X, Y, Z); }
        }

        public void Normalise()
        {
            double m = W * W + X * X + Y * Y + Z * Z;
            if (m > 0.001)
            {
                m = Math.Sqrt(m);
                W /= m;
                X /= m;
                Y /= m;
                Z /= m;
            }
            else
            {
                W = 1; X = 0; Y = 0; Z = 0;
            }
        }

        public void Conjugate()
        {
            X = -X; Y = -Y; Z = -Z;
        }

        public void FromAxisAngle(Vector3D axis, double angleRadian)
        {
            double m = axis.Magnitude;
            if (m > 0.0001)
            {
                double ca = Math.Cos(angleRadian / 2);
                double sa = Math.Sin(angleRadian / 2);
                X = axis.X / m * sa;
                Y = axis.Y / m * sa;
                Z = axis.Z / m * sa;
                W = ca;
            }
            else
            {
                W = 1; X = 0; Y = 0; Z = 0;
            }
        }

        public QuaternionD Copy()
        {
            return new QuaternionD(W, X, Y, Z);
        }

        public void Multiply(QuaternionD q)
        {
            this *= q;
        }

        public Point3D Rotate(Point3D pt)
        {
            this.Normalise();
            QuaternionD q1 = this.Copy();
            q1.Conjugate();

            QuaternionD qNode = new QuaternionD(0, pt.X, pt.Y, pt.Z);
            qNode = this * qNode * q1;
            pt.X = qNode.X;
            pt.Y = qNode.Y;
            pt.Z = qNode.Z;

            return pt;
        }

        public QuaternionD Rotate(double w, double x, double y, double z)
        {
            this.Normalise();
            QuaternionD q1 = this.Copy();
            q1.Conjugate();

            QuaternionD qNode = new QuaternionD(w, x, y, z);
            qNode = this * qNode;
            qNode *= q1;

            return qNode;
        }

        public void Rotate(Point3D[] nodes)
        {
            this.Normalise();
            QuaternionD q1 = this.Copy();
            q1.Conjugate();
            for (int i = 0; i < nodes.Length; i++)
            {
                QuaternionD qNode = new QuaternionD(0, nodes[i].X, nodes[i].Y, nodes[i].Z);
                qNode = this * qNode;
                qNode *= q1;
                nodes[i].X = qNode.X;
                nodes[i].Y = qNode.Y;
                nodes[i].Z = qNode.Z;
            }
        }

        public static QuaternionD operator *(QuaternionD q1, QuaternionD q2)
        {
            double nw = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;
            double nx = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            double ny = q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X;
            double nz = q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W;
            return new QuaternionD(nw, nx, ny, nz);
        }

        public override string ToString()
        {
            return W.ToString() + "; " + X.ToString() + "; " + Y.ToString() + "; " + Z.ToString();
        }
    }
}
