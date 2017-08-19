using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UKLib.MathEx
{
	/// <summary>
	/// Summary description for MatrixD.
	/// </summary>
	public class MatrixD
	{
		#region Variables
		protected double[] elements = new double[6];
		#endregion

		#region Properties
		public double[] Elements
		{
			get { return elements; }
		}
		#endregion

		#region Constructors
		public MatrixD()
		{
			Reset();
		}

		public MatrixD(double m11, double m12, double m21, double m22, double dx, double dy)
		{
			elements[0] = m11;
			elements[1] = m12;
			elements[2] = m21;
			elements[3] = m22;
			elements[4] = dx;
			elements[5] = dy;
		}
		#endregion

		#region Misc functions
		public void Reset()
		{
			for (int i=0; i<6; i++)
			{
				elements[i] = 0.0;
			}
			elements[0] = 1.0;
			elements[3] = 1.0;
		}
		#endregion

		#region Scale, Translate and Rotate
		public void Scale(double scaleX, double scaleY)
		{
			Scale(scaleX, scaleY, MatrixOrder.Prepend);
		}

		public void Scale(double scaleX, double scaleY, MatrixOrder order)
		{
			if (MatrixOrder.Append == order)
			{
				elements[0] *= scaleX;
				elements[2] *= scaleX;
				elements[4] *= scaleX;

				elements[1] *= scaleY;
				elements[3] *= scaleY;
				elements[5] *= scaleY;
			}
			else
			{
			}
		}

		public void Translate(double offsetX, double offsetY)
		{
			Translate(offsetX, offsetY, MatrixOrder.Prepend);
		}
		
		public void Translate(double offsetX, double offsetY, MatrixOrder order)
		{
			if (MatrixOrder.Append == order)
			{
				elements[4] += offsetX;
				elements[5] += offsetY;
			}
			else
			{
			}
		}

		public void Rotate(double angle)
		{
			Rotate(angle, MatrixOrder.Prepend);
		}
		
		public void Rotate(double angle, MatrixOrder order)
		{
			double cos = System.Math.Cos((angle*System.Math.PI)/180.0);
			double sin = System.Math.Sin((angle*System.Math.PI)/180.0);

			if (MatrixOrder.Append == order)
			{
				double m11 = elements[0];
				double m12 = elements[1];
				double m21 = elements[2];
				double m22 = elements[3];
				double dx = elements[4];
				double dy = elements[5];
							
				elements[0] = cos*m11-sin*m12;
				elements[1] = sin*m11+cos*m12;
				elements[2] = cos*m21-sin*m22;
				elements[3] = sin*m21+cos*m22;
				elements[4] = cos*dx-sin*dy;
				elements[5] = sin*dx+cos*dy;
			}
			else
			{
			}
		}

		public void RotateAt(double angle, PointD point)
		{
			RotateAt(angle, point, MatrixOrder.Prepend);
		}
		
		public void RotateAt(double angle, PointD point, MatrixOrder order)
		{
			double cos = System.Math.Cos((angle*System.Math.PI)/180.0);
			double sin = System.Math.Sin((angle*System.Math.PI)/180.0);

			if (MatrixOrder.Append == order)
			{
				elements[4] -= point.X;
				elements[5] -= point.Y;

				double m11 = elements[0];
				double m12 = elements[1];
				double m21 = elements[2];
				double m22 = elements[3];
				double dx = elements[4];
				double dy = elements[5];
							
				elements[0] = cos*m11-sin*m12;
				elements[1] = sin*m11+cos*m12;
				elements[2] = cos*m21-sin*m22;
				elements[3] = sin*m21+cos*m22;
				elements[4] = cos*dx-sin*dy;
				elements[5] = sin*dx+cos*dy;

				elements[4] += point.X;
				elements[5] += point.Y;
			}
			else
			{
			}
		}
		#endregion

		#region Transformation
		public void TransformPoints(Point[] pts)
		{
			for (int i=0; i<pts.Length; i++)
			{
				pts[i] = TransformPoint(pts[i]);
			}
		}

		public void TransformPoints(PointF[] pts)
		{
			for (int i=0; i<pts.Length; i++)
			{
				pts[i] = TransformPoint(pts[i]);
			}
		}

		public void TransformPoints(PointD[] pts)
		{
			for (int i=0; i<pts.Length; i++)
			{
				pts[i] = TransformPoint(pts[i]);
			}
		}

		public Point TransformPoint(Point pt)
		{
			return TransformPoint(new PointD(pt.X, pt.Y)).Point;
		}

		public PointF TransformPoint(PointF pt)
		{
			return TransformPoint(new PointD(pt.X, pt.Y)).PointF;
		}

		public PointD TransformPoint(PointD pt)
		{
			double x = pt.X;
			double y = pt.Y;

			return new PointD(elements[0]*x +elements[2]*y + elements[4], elements[1]*x +elements[3]*y + elements[5]);
		}
		#endregion
	}
}
