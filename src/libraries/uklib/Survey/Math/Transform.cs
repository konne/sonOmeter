using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace UKLib.Survey.Math
{
    /// <summary>
    /// Summary description for Transform.
    /// </summary>
    public class Transform : System.ComponentModel.Component
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Transform(System.ComponentModel.IContainer container)
        {
            if (container != null) container.Add(this);            
        }

        public Transform():this(null)
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Variables
        protected Ellipsoid srcEllipsoid;
        protected Ellipsoid dstEllipsoid;

        protected Datum2WGS srcDatum;
        protected Datum2WGS dstDatum;

        protected double param = -1;

        protected double k0 = 1;		// UTM-Coordinates standard: 0.9996
        #endregion

        #region Properties
        [Description("The source ellipsoid of the position."), Category("Conversion")]
        public Ellipsoid SrcEllipsoid
        {
            get { return srcEllipsoid; }
            set { srcEllipsoid = value; }
        }

        [Description("The destination ellipsoid of the position."), Category("Conversion")]
        public Ellipsoid DstEllipsoid
        {
            get { return dstEllipsoid; }
            set { dstEllipsoid = value; }
        }

        [Description("The source datum of the position."), Category("Conversion")]
        public Datum2WGS SrcDatum
        {
            get { return srcDatum; }
            set { srcDatum = value; }
        }

        [Description("The destination datum of the position."), Category("Conversion")]
        public Datum2WGS DstDatum
        {
            get { return dstDatum; }
            set { dstDatum = value; }
        }

        [Description("An optional conversion dependend parameter."), Category("Conversion")]
        public double Param
        {
            get { return param; }
            set { param = value; }
        }

        [Description("Magnification ratio for UTM coordinates."), Category("Parameters")]
        public double K0
        {
            get { return k0; }
        }
        #endregion

        #region Transformation
        public Coordinate Run(Coordinate src)
        {
            return Run(src, src.Type);
        }

        public Coordinate Run(Coordinate src, CoordinateType dstType)
        {
            if (src.Type == CoordinateType.TransverseMercator)
                src = FlatToElliptic(src);

            if (src.Type == CoordinateType.Elliptic)
                src = EllipticToCartesian(src);

            src = Helmert(src);
            src = InvHelmert(src);

            if (dstType != CoordinateType.Cartesian)
            {
                src = CartesianToElliptic(src);

                if (dstType != CoordinateType.Elliptic)
                    src = EllipticToFlat(src);
            }

            return src;
        }
        #endregion

        #region Flat <=> Elliptic
        private Coordinate FlatToElliptic(Coordinate src)
        {
            double la = 0;
            double lo = 0;

            try
            {
                int K;

                if (param < 0)
                    K = (int)(src.RV / 1e6);
                else
                    K = (int)param;

                double x = src.HV;
                double y = src.RV - K * 1e6 - 0.5 * 1e6;

                double temp = (1 + srcEllipsoid.N) / srcEllipsoid.A;
                double phi = temp / (srcEllipsoid.A0 * k0) * x;

                for (int i = 0; i < 100; i++)
                {
                    double G = (srcEllipsoid.A0 * phi - srcEllipsoid.A2 * System.Math.Sin(2 * phi) + srcEllipsoid.A4 * System.Math.Sin(4 * phi) -
                        srcEllipsoid.A6 * System.Math.Sin(6 * phi) + srcEllipsoid.A8 * System.Math.Sin(8 * phi)) / temp;

                    double dphi = 1 / srcEllipsoid.A * (x / k0 - G);

                    if (System.Math.Abs(dphi) < 1e-14)
                        break;

                    phi += dphi;
                }

                double s = System.Math.Sin(phi);
                double c = System.Math.Cos(phi);
                double t = System.Math.Tan(phi);
                double etha2 = srcEllipsoid.E_sqr / (1 - srcEllipsoid.E_sqr) * System.Math.Pow(c, 2);
                double N = srcEllipsoid.A / System.Math.Sqrt(1 - srcEllipsoid.E_sqr * System.Math.Pow(s, 2));
                double Y = y / (k0 * N);

                la = phi -
                     (t * (1 + etha2) * System.Math.Pow(Y, 2)) / 2 +
                     (t * (5 + 3 * System.Math.Pow(t, 2) + 6 * etha2 - 6 * etha2 * System.Math.Pow(t, 2) - 3 * System.Math.Pow(etha2, 2) - 9 * System.Math.Pow(etha2, 2) * System.Math.Pow(t, 2)) * System.Math.Pow(Y, 4)) / 24 -
                     (t * (61 + 90 * System.Math.Pow(t, 2) + 45 * System.Math.Pow(t, 4) + 107 * etha2 - 162 * System.Math.Pow(t, 2) * etha2 - 45 * etha2 * System.Math.Pow(t, 4)) * System.Math.Pow(Y, 6)) / 720;

                lo = K * 3 * System.Math.PI / 180 +
                     1 / c * (Y - ((1 + 2 * System.Math.Pow(t, 2) + etha2) * System.Math.Pow(Y, 3)) / 6 +
                     ((5 + 28 * System.Math.Pow(t, 2) + 24 * System.Math.Pow(t, 4) + 6 * etha2 + 8 * etha2 * System.Math.Pow(t, 2)) * System.Math.Pow(Y, 5)) / 120);
            }
            catch
            {
            }

            return new Coordinate(la, lo, src.AL, CoordinateType.Elliptic);
        }

        private Coordinate EllipticToFlat(Coordinate src)
        {
            double rv = 0;
            double hv = 0;

            try
            {
                int K;

                if (param < 0)
                    K = (int)System.Math.Round((src.LO * 180) / (3 * System.Math.PI));
                else
                    K = (int)param;

                double G = dstEllipsoid.A / (1 + dstEllipsoid.N) * (dstEllipsoid.A0 * src.LA - dstEllipsoid.A2 * System.Math.Sin(2 * src.LA) +
                    dstEllipsoid.A4 * System.Math.Sin(4 * src.LA) - dstEllipsoid.A6 * System.Math.Sin(6 * src.LA) + dstEllipsoid.A8 * System.Math.Sin(8 * src.LA));

                double s = System.Math.Sin(src.LA);
                double c = System.Math.Cos(src.LA);
                double t = System.Math.Tan(src.LA);
                double etha2 = dstEllipsoid.E_sqr / (1 - dstEllipsoid.E_sqr) * System.Math.Pow(c, 2);
                double N = dstEllipsoid.A / System.Math.Sqrt(1 - dstEllipsoid.E_sqr * System.Math.Pow(s, 2));

                double l = src.LO - K * 3 * System.Math.PI / 180;

                double A = G / N +
                           (System.Math.Pow(l, 2) * s * c) / 2 +
                           (System.Math.Pow(l, 4) * s * System.Math.Pow(c, 3) * (5 - System.Math.Pow(t, 2) + 9 * etha2 + 4 * System.Math.Pow(etha2, 2))) / 24 +
                           (System.Math.Pow(l, 6) * s * System.Math.Pow(c, 5) * (61 - 58 * System.Math.Pow(t, 2) + System.Math.Pow(t, 4) + 270 * etha2 - 330 * etha2 * System.Math.Pow(t, 2) + 445 * System.Math.Pow(etha2, 2))) / 720;
                double B = l * c +
                           (System.Math.Pow(l, 3) * System.Math.Pow(c, 3) * (1 - System.Math.Pow(t, 2) + etha2)) / 6 +
                           (System.Math.Pow(l, 5) * System.Math.Pow(c, 5) * (5 - 18 * System.Math.Pow(t, 2) + System.Math.Pow(t, 4) + 14 * etha2 - 58 * System.Math.Pow(t, 2) * etha2 + 13 * System.Math.Pow(etha2, 2))) / 120;

                hv = k0 * N * A;
                rv = k0 * N * B + 0.5e6 + K * 1e6;
            }
            catch
            {
            }

            return new Coordinate(rv, hv, src.AL, CoordinateType.TransverseMercator);
        }
        #endregion

        #region Cartesian <=> Elliptic
        private Coordinate CartesianToElliptic(Coordinate src)
        {
            double la = 0;
            double lo = 0;
            double al = 0;

            try
            {
                double p = System.Math.Sqrt(System.Math.Pow(src.X, 2) + System.Math.Pow(src.Y, 2));
                double Bi1 = System.Math.Atan(src.Z / (p * (1 - dstEllipsoid.E_sqr)));
                double Ni = 0;

                for (int i = 0; i < 100; i++)
                {
                    double Bi = Bi1;
                    Ni = dstEllipsoid.A / System.Math.Sqrt(1 - dstEllipsoid.E_sqr * System.Math.Pow(System.Math.Sin(Bi), 2));
                    Bi1 = System.Math.Atan((src.Z + dstEllipsoid.E_sqr * Ni * System.Math.Sin(Bi)) / p);

                    if (System.Math.Abs(Bi1 - Bi) < 1e-14)
                        break;
                }

                la = Bi1;
                lo = 2 * System.Math.Atan(src.Y / (src.X + p));
                al = p * System.Math.Cos(la) + src.Z * System.Math.Sin(la) - System.Math.Pow(dstEllipsoid.A, 2) / Ni;
            }
            catch
            {
            }

            return new Coordinate(la, lo, al, CoordinateType.Elliptic);
        }

        private Coordinate EllipticToCartesian(Coordinate src)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            try
            {
                double s = System.Math.Sin(src.LA);
                double c = System.Math.Cos(src.LA);
                double N = srcEllipsoid.A / System.Math.Sqrt(1 - srcEllipsoid.E_sqr * s * s);

                x = (N + src.AL) * c * System.Math.Cos(src.LO);
                y = (N + src.AL) * c * System.Math.Sin(src.LO);
                z = (N * (1 - srcEllipsoid.E_sqr) + src.AL) * s;
            }
            catch
            {
            }

            return new Coordinate(x, y, z, CoordinateType.Cartesian);
        }
        #endregion

        #region Helmert
        private Coordinate Helmert(Coordinate src)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            try
            {
                x = srcDatum.DX + srcDatum.Sc * (src.X + srcDatum.RZ * src.Y - srcDatum.RY * src.Z);
                y = srcDatum.DY + srcDatum.Sc * (-srcDatum.RZ * src.X + src.Y + srcDatum.RX * src.Z);
                z = srcDatum.DZ + srcDatum.Sc * (srcDatum.RY * src.X - srcDatum.RX * src.Y + src.Z);
            }
            catch
            {
            }

            return new Coordinate(x, y, z, CoordinateType.Cartesian);
        }

        private Coordinate InvHelmert(Coordinate src)
        {
            double x = 0;
            double y = 0;
            double z = 0;

            try
            {
                x = -dstDatum.DX + 1 / dstDatum.Sc * (src.X - dstDatum.RZ * src.Y + dstDatum.RY * src.Z);
                y = -dstDatum.DY + 1 / dstDatum.Sc * (dstDatum.RZ * src.X + src.Y - dstDatum.RX * src.Z);
                z = -dstDatum.DZ + 1 / dstDatum.Sc * (-dstDatum.RY * src.X + dstDatum.RX * src.Y + src.Z);
            }
            catch
            {
            }

            return new Coordinate(x, y, z, CoordinateType.Cartesian);
        }
        #endregion
    }
}
