using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UKLib.MathEx
{
	/// <summary>
	/// This enumeration holds all corners of a box. The min values are adressed by small letters and the max values by big ones.
	/// </summary>
	public enum BoxCorners
	{
		xyz = 0,
		Xyz = 1,
		xYz = 2,
		XYz = 3,
		xyZ = 4,
		XyZ = 5,
		xYZ = 6,
		XYZ = 7
	}

	public struct BoxD
	{
		#region Variables
		private double xMin;
		private double xMax;
		private double yMin;
		private double yMax;
		private double zMin;
		private double zMax; 
		#endregion

		#region Properties
		public double XMin
		{
			get { return xMin; }
			set { xMin = value; }
		}

		public double XMax
		{
			get { return xMax; }
			set { xMax = value; }
		}

		public double YMin
		{
			get { return yMin; }
			set { yMin = value; }
		}

		public double YMax
		{
			get { return yMax; }
			set { yMax = value; }
		}

		public double ZMin
		{
			get { return zMin; }
			set { zMin = value; }
		}

		public double ZMax
		{
			get { return zMax; }
			set { zMax = value; }
		}

		public Point3D[] Corners
		{
			get
			{
				Point3D[] corners = new Point3D[8];

				corners[(int)BoxCorners.xyz] = new Point3D(xMin, yMin, zMin);
				corners[(int)BoxCorners.Xyz] = new Point3D(xMax, yMin, zMin);
				corners[(int)BoxCorners.xYz] = new Point3D(xMin, yMax, zMin);
				corners[(int)BoxCorners.XYz] = new Point3D(xMax, yMax, zMin);
				corners[(int)BoxCorners.xyZ] = new Point3D(xMin, yMin, zMax);
				corners[(int)BoxCorners.XyZ] = new Point3D(xMax, yMin, zMax);
				corners[(int)BoxCorners.xYZ] = new Point3D(xMin, yMax, zMax);
				corners[(int)BoxCorners.XYZ] = new Point3D(xMax, yMax, zMax);

				return corners;
			}
		}

        public bool IsEmpty
        {
            get
            {
                return ((xMin == 0) && (xMax == 0) && (yMin == 0) && (yMax == 0) && (zMin == 0) && (zMax == 0));
            }
        }
		#endregion

		#region Constructors
		public BoxD(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
			this.zMin = zMin;
			this.zMax = zMax;
		}

		public BoxD(RectangleD rcXY, double zMin, double zMax)
		{
			this.xMin = rcXY.Left;
			this.xMax = rcXY.Right;
			this.yMin = rcXY.Bottom;
			this.yMax = rcXY.Top;
			this.zMin = zMin;
			this.zMax = zMax;
		}

        public BoxD(Point3D ptMin, Point3D ptMax)
        {
            this.xMin = ptMin.X;
            this.xMax = ptMax.X;
            this.yMin = ptMin.Y;
            this.yMax = ptMax.Y;
            this.zMin = ptMin.Z;
            this.zMax = ptMax.Z;
        }
		#endregion
	}

	public struct BoxF
	{
		#region Variables
		private float xMin;
		private float xMax;
		private float yMin;
		private float yMax;
		private float zMin;
		private float zMax;
		#endregion

		#region Properties
		public float XMin
		{
			get { return xMin; }
			set { xMin = value; }
		}

		public float XMax
		{
			get { return xMax; }
			set { xMax = value; }
		}

		public float YMin
		{
			get { return yMin; }
			set { yMin = value; }
		}

		public float YMax
		{
			get { return yMax; }
			set { yMax = value; }
		}

		public float ZMin
		{
			get { return zMin; }
			set { zMin = value; }
		}

		public float ZMax
		{
			get { return zMax; }
			set { zMax = value; }
		}

		public Point3F[] Corners
		{
			get
			{
				Point3F[] corners = new Point3F[8];

				corners[(int)BoxCorners.xyz] = new Point3F(xMin, yMin, zMin);
				corners[(int)BoxCorners.Xyz] = new Point3F(xMax, yMin, zMin);
				corners[(int)BoxCorners.xYz] = new Point3F(xMin, yMax, zMin);
				corners[(int)BoxCorners.XYz] = new Point3F(xMax, yMax, zMin);
				corners[(int)BoxCorners.xyZ] = new Point3F(xMin, yMin, zMax);
				corners[(int)BoxCorners.XyZ] = new Point3F(xMax, yMin, zMax);
				corners[(int)BoxCorners.xYZ] = new Point3F(xMin, yMax, zMax);
				corners[(int)BoxCorners.XYZ] = new Point3F(xMax, yMax, zMax);

				return corners;
			}
		}
		#endregion

		#region Constructors
		public BoxF(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
		{
			this.xMin = xMin;
			this.xMax = xMax;
			this.yMin = yMin;
			this.yMax = yMax;
			this.zMin = zMin;
			this.zMax = zMax;
		}

		public BoxF(RectangleF rcXY, float zMin, float zMax)
		{
			this.xMin = rcXY.Left;
			this.xMax = rcXY.Right;
			this.yMin = rcXY.Bottom;
			this.yMax = rcXY.Top;
			this.zMin = zMin;
			this.zMax = zMax;
		}

		public BoxF(RectangleD rcXY, float zMin, float zMax)
		{
			this.xMin = (float)rcXY.Left;
			this.xMax = (float)rcXY.Right;
			this.yMin = (float)rcXY.Bottom;
			this.yMax = (float)rcXY.Top;
			this.zMin = zMin;
			this.zMax = zMax;
		}
		#endregion
	}
}
