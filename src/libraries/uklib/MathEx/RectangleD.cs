using System;
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UKLib.MathEx
{
	public struct RectArrayF
	{
		public Collection<RectangleF> List;
		public SolidBrush Brush;
        public Pen Pen;

        public RectArrayF(Color color)
        {
            Brush = new SolidBrush(color);
            Pen = new Pen(color, 1.0F);
            List = new Collection<RectangleF>();
        }

		public RectArrayF(Color color, float width)
		{
			Brush = new SolidBrush(color);
			Pen = new Pen(color, width);
			List = new Collection<RectangleF>();
		}

		public RectangleF[] Array
		{
			get
			{
				RectangleF[] ar = new RectangleF[List.Count];
				List.CopyTo(ar, 0);
				return ar;
			}
		}

		public bool Empty
		{
			get { return List.Count == 0; }
		}

		public bool Contains(RectangleF rcTest)
		{
			RectangleF rc;
			int i = 0;
			int count = List.Count;

			for (i = 0; i < count; i++)
			{
				rc = List[i];

				if (rc.X != rcTest.X)
					continue;

				if (rc.Y != rcTest.Y)
					continue;

				if (rc.Width != rcTest.Width)
					continue;

				if (rc.Height != rcTest.Height)
					continue;

				return false;
			}

			return false;
		}
	}

    public struct RectArrayD
    {
        public List<RectangleD> List;
        public SolidBrush Brush;
        public Pen Pen;

        public RectArrayD(Color color)
        {
            Brush = new SolidBrush(color);
            Pen = new Pen(color, 1.0F);
            List = new List<RectangleD>();
        }

        public RectArrayD(Color color, float width)
        {
            Brush = new SolidBrush(color);
            Pen = new Pen(color, width);
            List = new List<RectangleD>();
        }

        public RectangleD[] Array
        {
            get
            {
                RectangleD[] ar = new RectangleD[List.Count];
                List.CopyTo(ar, 0);
                return ar;
            }
        }

        public RectangleF[] OffsetArrayF(PointD ptOffset)
        {
            int max = List.Count;
            RectangleF[] ar = new RectangleF[max];
            PointF pt;

            for (int i = 0; i < max; i++)
            {
                pt = (List[i].BL + ptOffset).PointF;
                ar[i] = new RectangleF(pt.X, pt.Y, (float)(List[i].Width), (float)(List[i].Height));
            }

            return ar;
        }

        public bool Empty
        {
            get { return List.Count == 0; }
        }
    }

	/// <summary>
	/// Summary description for RectangleD.
	/// </summary>
	public class RectangleD
	{
		#region Variables
		double left = 0;
		double right = 0;
		double top = 0;
		double bottom = 0;
		#endregion

		#region Properties
		public double Left
		{
			get { return left; }
			set { left = value; }
		}

		public double Right
		{
			get { return right; }
			set { right = value; }
		}

		public double Top
		{
			get { return top; }
			set { top = value; }
		}

		public double Bottom
		{
			get { return bottom; }
			set { bottom = value; }
		}

		public double Width
		{
			get { return right - left; }
		}

		public double Height
		{
			get { return top - bottom; }
		}

		public double Area
		{
			get { return Width * Height; }
		}

		public double Diag
		{
			get { return System.Math.Sqrt(Width * Width + Height * Height) / 2.0; }
		}

		public bool IsEmpty
		{
			get { return ((right == left) & (top == bottom)); }
		}

		public bool IsInverted
		{
			get { return ((Width < 0) || (Height < 0)); }
		}

		public bool IsZero
		{
			get { return ((left == 0) && (right == 0) && (top == 0) && (bottom == 0)); }
		}

		public PointD BL
		{
			get { return new PointD(left, bottom); }
			set { left = value.X; bottom = value.Y; }
		}

        public PointD BR
        {
            get { return new PointD(right, bottom); }
            set { right = value.X; bottom = value.Y; }
        }

        public PointD TL
        {
            get { return new PointD(left, top); }
            set { left = value.X; top = value.Y; }
        }

		public PointD TR
		{
			get { return new PointD(right, top); }
			set { right = value.X; top = value.Y; }
		}

		public PointD Center
		{
			get { return new PointD((left + right) / 2.0, (bottom + top) / 2.0); }
		}

        public RectangleF RectF
        {
            get { return new RectangleF((float)left, (float)bottom, (float)System.Math.Abs(this.Width), (float)System.Math.Abs(this.Height)); }
        }
		#endregion

		#region Constructors
		public RectangleD()
		{
			this.left = 0;
			this.right = 0;
			this.top = 0;
			this.bottom = 0;
		}

		public RectangleD(double left, double top)
		{
			this.left = this.right = left;
			this.top = this.bottom = top;
		}

		public RectangleD(PointD pt)
		{
			this.left = this.right = pt.X;
			this.top = this.bottom = pt.Y;
		}

		public RectangleD(PointD bl, PointD tr)
		{
            if (bl.X < tr.X)
            {
                this.left = bl.X;
                this.right = tr.X;
            }
            else
            {
                this.left = tr.X;
                this.right = bl.X;
            }

            if (bl.Y < tr.Y)
            {
                this.bottom = bl.Y;
                this.top = tr.Y;
            }
            else
            {
                this.bottom = tr.Y;
                this.top = bl.Y;
            }
		}

		public RectangleD(RectangleD rc)
		{
			this.left = rc.left;
			this.right = rc.right;
			this.top = rc.top;
			this.bottom = rc.bottom;
		}

		public RectangleD(double left, double bottom, double right, double top)
		{
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		#endregion

        #region ToString
        public override string ToString()
        {
            return left.ToString() + ", " + top.ToString() + ", " + Width.ToString() + ", " + Height.ToString();
        } 
        #endregion

        #region Operators
        public static bool operator ==(RectangleD a, RectangleD b)
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
            return (a.left == b.left) && (a.right == b.right) && (a.top == b.top) && (a.bottom == b.bottom);
        }

        public static bool operator !=(RectangleD a, RectangleD b)
        {
            return !(a == b);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            RectangleD p = (RectangleD)obj;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (left == p.left) && (right == p.right) && (top == p.top) && (bottom == p.bottom);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Merge
        public void Merge(double x, double y)
        {
            if (left > x)
                left = x;
            else if (right < x)
                right = x;

            if (bottom > y)
                bottom = y;
            else if (top < y)
                top = y;
        }

        public void Merge(PointD pt)
        {
            Merge(pt.X, pt.Y);
        }

        public void Merge(RectangleD rc)
        {
            Merge(rc.BL);
            Merge(rc.TR);
        }

        public void Merge(double x, double y, bool newIfZero)
        {
            if (newIfZero && this.IsZero)
            {
                this.left = this.right = x;
                this.top = this.bottom = y;
            }
            else
                Merge(x, y);
        }

        public void Merge(PointD pt, bool newIfZero)
        {
            if (newIfZero && this.IsZero)
            {
                this.left = this.right = pt.X;
                this.top = this.bottom = pt.Y;
            }
            else
                Merge(pt);
        }

        public void Merge(RectangleD rc, bool newIfZero)
        {
            if (newIfZero && this.IsZero)
            {
                this.left = rc.Left;
                this.right = rc.right;
                this.top = rc.top;
                this.bottom = rc.bottom;
            }
            else
                Merge(rc);
        }

        public void Merge(PolygonD polygonD)
        {
            foreach (PointD pt in polygonD.Points)
                Merge(pt);
        }
        #endregion

        #region Offset
        public void Offset(double x, double y)
        {
            left += x;
            right += x;
            top += y;
            bottom += y;
        }

        public void Offset(PointD pt)
        {
            Offset(pt.X, pt.Y);
        } 
        #endregion

        #region Hit tests (contains)
        public bool Contains(double x, double y)
        {
            bool retX = false;
            bool retY = false;

            retX = ((x >= left) && (x <= right));
            retY = ((y >= bottom) && (y <= top));

            return (retX & retY);
        }

        public bool Contains(PointD pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(RectangleD rc)
        {
            bool retX = false;
            bool retY = false;

            retX = ((rc.left <= right) && (rc.right >= left));
            retY = ((rc.bottom <= top) && (rc.top >= bottom));

            return (retX & retY);
        }
        #endregion

        #region Inflate
        public void Inflate(double x, double y)
        {
            double w = Width;
            double h = Height;

            if (Width >= 0)
            {
                left -= w * (x - 1) / 2;
                right += w * (x - 1) / 2;
            }
            else
            {
                left += w * (x - 1) / 2;
                right -= w * (x - 1) / 2;
            }

            if (Height >= 0)
            {
                bottom -= h * (y - 1) / 2;
                top += h * (y - 1) / 2;
            }
            else
            {
                bottom += h * (y - 1) / 2;
                top -= h * (y - 1) / 2;
            }
        } 
        #endregion

        #region Transformations
        public PointD Transform(PointD pt, double width, double height, bool invertX, bool invertY)
        {
            double x = 0;
            double y = 0;

            if (this.IsEmpty)
                return pt;

            x = left;
            y = bottom;

            x = (pt.X - x) * width / this.Width;

            if (invertX)
                x = width - x;

            y = (pt.Y - y) * height / this.Height;

            if (invertY)
                y = height - y;

            return new PointD(x, y);
        }

        public Point Transform(Point pt, double width, double height, bool invertX, bool invertY)
        {
            PointD point = Transform(new PointD(pt.X, pt.Y), width, height, invertX, invertY);
            return new Point((int)point.X, (int)point.Y);
        }

        public double Transform(double d, double reference, bool invert, bool x0y1)
        {
            PointD pt;

            if (x0y1)
            {
                pt = Transform(new PointD(0, d), reference, reference, invert, invert);
                return pt.Y;
            }
            else
            {
                pt = Transform(new PointD(d, 0), reference, reference, invert, invert);
                return pt.X;
            }
        } 
        #endregion

        public void Mirror(bool xAxis, bool yAxis)
        {
            if (xAxis)
            {
                this.top = -this.top;
                this.bottom = -this.bottom;
            }

            if (yAxis)
            {
                this.left = -this.left;
                this.right = -this.right;
            }
        }
    }
}
