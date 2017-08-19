using System;
using System.Globalization;
using UKLib.MathEx;

namespace UKLib.Survey.Math
{
    /// <summary>
    /// Enumerates all usable coordinate types.
    /// </summary>
    public enum CoordinateType
    {
        Empty = 0,
        Elliptic = 1,
        Cartesian = 2,
        TransverseMercator = 3
    }

    /// <summary>
    /// General coordinate structure with support for all types specified in the CoordinateType enumeration.
    /// </summary>
    public struct Coordinate
    {
        #region Variables
        internal double d1;	// X, LA, RV
        internal double d2;	// Y, LO, HV
        internal double d3;	// Z, AL, AL

        CoordinateType coordType;

        public static Coordinate Empty = new Coordinate(0, 0, 0, CoordinateType.Empty);
        #endregion

        #region Properties
        /// <summary>
        /// Latitude (only usable in elliptic coordinates).
        /// </summary>
        public double LA
        {
            get
            {
                if (coordType != CoordinateType.Elliptic)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d1;
            }
            set
            {
                if (coordType != CoordinateType.Elliptic)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d1 = value;
            }
        }

        /// <summary>
        /// Longitude (only usable in elliptic coordinates).
        /// </summary>
        public double LO
        {
            get
            {
                if (coordType != CoordinateType.Elliptic)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d2;
            }
            set
            {
                if (coordType != CoordinateType.Elliptic)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d2 = value;
            }
        }

        /// <summary>
        /// Altitude (only usable in elliptic and transverse mercator coordinates).
        /// </summary>
        public double AL
        {
            get
            {
                if ((coordType != CoordinateType.Elliptic) && (coordType != CoordinateType.TransverseMercator))
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d3;
            }
            set
            {
                if ((coordType != CoordinateType.Elliptic) && (coordType != CoordinateType.TransverseMercator))
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d3 = value;
            }
        }

        /// <summary>
        /// Right value (only usable in transverse mercator coordinates).
        /// </summary>
        public double RV
        {
            get
            {
                if (coordType != CoordinateType.TransverseMercator)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d1;
            }
            set
            {
                if (coordType != CoordinateType.TransverseMercator)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d1 = value;
            }
        }

        /// <summary>
        /// High value (only usable in transverse mercator coordinates).
        /// </summary>
        public double HV
        {
            get
            {
                if (coordType != CoordinateType.TransverseMercator)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d2;
            }
            set
            {
                if (coordType != CoordinateType.TransverseMercator)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d2 = value;
            }
        }

        /// <summary>
        /// X (only usable in cartesian coordinates).
        /// </summary>
        public double X
        {
            get
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d1;
            }
            set
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d1 = value;
            }
        }

        /// <summary>
        /// Y (only usable in cartesian coordinates).
        /// </summary>
        public double Y
        {
            get
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d2;
            }
            set
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d2 = value;
            }
        }

        /// <summary>
        /// Z (only usable in cartesian coordinates).
        /// </summary>
        public double Z
        {
            get
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                return d3;
            }
            set
            {
                if (coordType != CoordinateType.Cartesian)
                    throw new InvalidOperationException("This property is not accessible for the current coordinate type: " + coordType.ToString());
                d3 = value;
            }
        }

        /// <summary>
        /// Gets a point based on the first two coordinates.
        /// </summary>
        public PointD Point
        {
            get { return new PointD(d1, d2); }
        }

        /// <summary>
        /// Gets a vector based on the coordinates.
        /// </summary>
        public Point3D Point3D
        {
            get { return new Point3D(d1, d2, d3); }
        }

        /// <summary>
        /// Gets the type of the coordinate.
        /// </summary>
        public CoordinateType Type
        {
            get { return coordType; }
        }

        public bool IsEmpty
        {
            get { return coordType == CoordinateType.Empty; }
        }

        public bool IsZero
        {
            get { return ((d1 == 0) && (d2 == 0) && (d3 == 0)); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a coordinate struct for different applications.
        /// </summary>
        /// <param name="d1">The 1st coordinate (X, LA, RV).</param>
        /// <param name="d2">The 2nd coordinate (Y, LO, HV).</param>
        /// <param name="d3">The 3rd coordinate (Z, AL, AL).</param>
        /// <param name="coordType">The type of the coordinate.</param>
        public Coordinate(double d1, double d2, double d3, CoordinateType coordType)
        {
            this.coordType = coordType;
        
            this.d1 = d1;
            this.d2 = d2;
            this.d3 = d3;
        }

        /// <summary>
        /// Initializes a coordinate struct for different applications.
        /// </summary>
        /// <param name="pt">The point of the coordinate.</param>
        /// <param name="coordType">The type of the coordinate.</param>
        public Coordinate(Point3D pt, CoordinateType coordType)
        {
            this.coordType = coordType;

            this.d1 = pt.X;
            this.d2 = pt.Y;
            this.d3 = pt.Z;
        }

        /// <summary>
        /// Initializes a coordinate struct based on an other struct.
        /// </summary>
        /// <param name="coord">The source coordinate to copy from.</param>
        public Coordinate(Coordinate coord)
        {
            this.coordType = coord.coordType;
            this.d1 = coord.d1;
            this.d2 = coord.d2;
            this.d3 = coord.d3;
        }

        /// <summary>
        /// Initializes a coordinate struct based on the mid of two other ones.
        /// </summary>
        /// <param name="coord1">The first one.</param>
        /// <param name="coord2">The second one.</param>
        public static Coordinate Mid(Coordinate coord1, Coordinate coord2)
        {
            if ((coord1.Type != CoordinateType.TransverseMercator) || (coord2.Type != CoordinateType.TransverseMercator))
                return Coordinate.Empty;

            Coordinate c = new Coordinate(coord1);

            c.d1 = (c.d1 + coord2.d1) / 2.0;
            c.d2 = (c.d2 + coord2.d2) / 2.0;
            c.d3 = (c.d3 + coord2.d3) / 2.0;

            return c;
        }
        #endregion

        #region ToString function
        public string[] ToString(CoordinateType dstType, Transform t, NumberFormatInfo nfi)
        {
            Coordinate c = this;
            string[] s = new string[3];
            s[0] = "";
            s[1] = "";
            s[2] = "";

            if (dstType != coordType)
            {
                try
                {
                    c = t.Run(this, dstType);
                }
                catch
                {
                    throw new ArgumentException("The passed argument is invalid.");
                }
            }

            try
            {
                s[0] = c.d1.ToString(nfi);
                s[1] = c.d2.ToString(nfi);
                s[2] = c.d3.ToString(nfi);
                        
                if (dstType == CoordinateType.Elliptic)
                {
                    s[0] = Trigonometry.Rad2DMS(c.d1, nfi, true);
                    s[1] = Trigonometry.Rad2DMS(c.d2, nfi, false);
                }
            }
            catch (Exception e)
            {
                throw new ArithmeticException("The (transformed) coordinate was invalid.", e);
            }

            return s;
        }

        public override string ToString()
        {
            return "Coordinate";
        }

        public string ToString(bool insertNewLine, NumberFormatInfo nfi)
        {
            string s = "";

            switch (coordType)
            {
                case CoordinateType.TransverseMercator:
                    s = "RV: " + d1.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "HV: " + d2.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "AL: " + d3.ToString("F", nfi);
                    break;
                case CoordinateType.Elliptic:
                    s = "LA: " + d1.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "LO: " + d2.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "AL: " + d3.ToString("F", nfi);
                    break;
                case CoordinateType.Cartesian:
                    s = "X: " + d1.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "Y: " + d2.ToString("F", nfi);
                    s += (insertNewLine) ? "\n" : "; ";
                    s += "Z: " + d3.ToString("F", nfi);
                    break;
            }

            return s;
        }
        #endregion

        public Vector3D DeltaVector(Coordinate cOrigin)
        {
            return new Vector3D(cOrigin.d1 - d1, cOrigin.d2 - d2, cOrigin.d3 - d3);
        }

        public bool IsSame(Coordinate coord)
        {
            if (coord.coordType != this.coordType)
                return false;

            if (coord.d1 != this.d1)
                return false;

            if (coord.d2 != this.d2)
                return false;

            if (coord.d3 != this.d3)
                return false;

            return true;
        }
    }
}