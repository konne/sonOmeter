using System;
using System.Collections.Generic;
using System.Text;

namespace UKLib.MathEx
{
    public class Matrix3D
    {
        private double[,] m = new double[4, 4];

        public double this[int i, int j]
        {
            get { return m[i, j]; }
            set { m[i, j] = value; }
        }

        /// <summary>
        /// Returns an identity matrix.
        /// </summary>
        public Matrix3D()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (i == j)
                        m[i, j] = 1;
                    else
                        m[i, j] = 0;
        }

        /// <summary>
        /// Multiplies m1 and m2 (matrix dot product).
        /// </summary>
        /// <param name="m1">Matrix m1.</param>
        /// <param name="m2">Matrix m2.</param>
        /// <returns>The matrix dot product of m1 and m2.</returns>
        public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D ret = new Matrix3D();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        ret[i, j] += m1[k, j] * m2[i, k];
            return ret;
        }

        /// <summary>
        /// Returns a translation matrix.
        /// </summary>
        /// <param name="dx">The x offset.</param>
        /// <param name="dy">The y offset.</param>
        /// <param name="dz">The z offset.</param>
        /// <returns>A translation matrix.</returns>
        public static Matrix3D Translate(double dx, double dy, double dz)
        {
            Matrix3D ret = new Matrix3D();
            ret[3, 0] = dx;
            ret[3, 1] = dy;
            ret[3, 2] = dz;
            return ret;
        }

        /// <summary>
        /// Returns a scale matrix.
        /// </summary>
        /// <param name="sx">The x scale factor.</param>
        /// <param name="sy">The y scale factor.</param>
        /// <param name="sz">The z scale factor.</param>
        /// <returns>A scale matrix.</returns>
        public static Matrix3D Scale(double sx, double sy, double sz)
        {
            Matrix3D ret = new Matrix3D();
            ret[0, 0] = sx;
            ret[1, 1] = sy;
            ret[2, 2] = sz;
            return ret;
        }

        public static Matrix3D RotateX(double rads)
        {
            double c = System.Math.Cos(rads);
            double s = System.Math.Sin(rads);
            Matrix3D ret = new Matrix3D();
            ret[1, 1] = c;
            ret[2, 2] = c;
            ret[1, 2] = -s;
            ret[2, 1] = s;
            return ret;
        }

        public static Matrix3D RotateY(double rads)
        {
            double c = System.Math.Cos(rads);
            double s = System.Math.Sin(rads);
            Matrix3D ret = new Matrix3D();
            ret[0, 0] = c;
            ret[2, 2] = c;
            ret[0, 2] = s;
            ret[2, 0] = -s;
            return ret;
        }

        public static Matrix3D RotateZ(double rads)
        {
            double c = System.Math.Cos(rads);
            double s = System.Math.Sin(rads);
            Matrix3D ret = new Matrix3D();
            ret[0, 0] = c;
            ret[1, 1] = c;
            ret[0, 1] = -s;
            ret[1, 0] = s;
            return ret;
        }

        public static Matrix3D Rotate(double rx, double ry, double rz)
        {
            Matrix3D matX = Matrix3D.RotateX(rx);
            Matrix3D matY = Matrix3D.RotateY(ry);
            Matrix3D matZ = Matrix3D.RotateZ(rz);
            
            return matX * matY * matZ;
        }

        public static Matrix3D Rotate(RotD rot)
        {
            Matrix3D matX = Matrix3D.RotateX(rot.Roll);
            Matrix3D matY = Matrix3D.RotateY(rot.Pitch);
            Matrix3D matZ = Matrix3D.RotateZ(rot.Yaw);

            return matX * matY * matZ;
        }
    }
}
