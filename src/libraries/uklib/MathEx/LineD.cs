using System;
using System.Collections.Generic;
using System.Text;
using M = System.Math;
using UKLib.Survey.Math;

namespace UKLib.MathEx
{
	/// <summary>
	/// A struct that describes a line in the general form.
	/// </summary>
	public struct LineD
	{
		public PointD pt1;
		public PointD pt2;

		public double A;
		public double B;
		public double C;

		public LineD(PointD ptStart, PointD ptEnd)
		{
			this.pt1 = ptStart;
			this.pt2 = ptEnd;

			this.A = 0;
			this.B = 0;
			this.C = 0;

			UpdateABC();
		}

		public void UpdateABC()
		{
			double x0 = pt1.X;
			double y0 = pt1.Y;
			double dx = pt2.X - x0;
			double dy = pt2.Y - y0;

			this.A = dy;
			this.B = -dx;
			this.C = dx * y0 - dy * x0;
		}

		public double OriginDistance()
		{
			return C / M.Sqrt(A * A + B * B);
		}

		public double OriginAngleRad()
		{
			return M.Atan2(B, A);
		}

		public double OriginAngleDeg()
		{
			return OriginAngleRad() / M.PI * 180.0;
		}

		public double PointDistance(PointD pt)
		{
			return (A * pt.X + B * pt.Y + C) / M.Sqrt(A * A + B * B);
		}

		public bool Inside(PointD pt)
		{
			double angle = GetBiggestAngle(pt);

			return (angle <= 90.0) | (angle >= 270);
		}

		public double GetBiggestAngle(PointD pt)
		{
			double a1 = GetAngle(pt1, pt, pt2);
			double a2 = GetAngle(pt2, pt1, pt);

			return (a1 > a2) ? a1 : a2;
		}

		private double GetAngle(PointD ptRef, PointD pt1, PointD pt2)
		{
			return System.Math.Abs(ptRef.AngleRad(pt1) - ptRef.AngleRad(pt2)) / System.Math.PI * 180.0;
		}
	}
}
