using System;
using System.Globalization;

namespace UKLib.Survey.Math
{
	/// <summary>
	/// This class provides useful conversion functions for trigonometric purposes.
	/// </summary>
	public struct Trigonometry
	{
		/// <summary>
		/// Builds a radian expression out of degree, minute and second of a coordinate.
		/// </summary>
		/// <param name="degree">The degree part of the coordinate.</param>
		/// <param name="minute">The minute part of the coordinate.</param>
		/// <param name="second">The second part of the coordinate.</param>
		/// <returns>The coordinate in radians.</returns>
		static public double DMS2Rad(double degree, double minute, double second)
		{
			return Grad2Rad(degree + minute/60.0 + second/3600.0);
		}

		static public double DMS2Rad(string s, NumberFormatInfo nfi)
		{
			double d1 = 0, d2 = 0, d3 = 0;

			string sNew = s.Replace("°", "").Replace(" ", "").ToLower();
			char[] directions = { 'w', 'e', 'n', 's' };

			int pos1 = sNew.IndexOfAny(directions);
			if (pos1 == -1)
			{
				sNew = s.Replace(" ", "").ToLower();
				pos1 = sNew.IndexOf("°");
			}
			if (pos1 != -1)
				double.TryParse(sNew.Substring(0, pos1), out d1);
			pos1++;

			int pos2 = sNew.IndexOf("'");
			if (pos2 != -1)
				double.TryParse(sNew.Substring(pos1, pos2 - pos1), out d2);
			pos2++;

			int pos3 = sNew.IndexOf("\"");
			if (pos3 != -1)
				double.TryParse(sNew.Substring(pos2, pos3 - pos2), NumberStyles.Float | NumberStyles.AllowThousands, nfi, out d3);

			if (sNew.IndexOf('w') != -1)
				d1 = 360.0 - d1;
			else if (sNew.IndexOf('s') != -1)
				d1 = -d1;

			return DMS2Rad(d1, d2, d3);
		}

		/// <summary>
		/// Builds a DMS string out of a radian expression.
		/// </summary>
		/// <param name="d">The coordinate in radians.</param>
		/// <param name="nfi">The number format info provider.</param>
		/// <param name="lat">True for latitude, False for longitude.</param>
		/// <returns>A string expression with degree, minutes and seconds.</returns>
		static public string Rad2DMS(double d, NumberFormatInfo nfi, bool lat)
		{
			d = Rad2Grad(d);

			string direction = "° ";

			if (!lat)
				if ((d >= 0) & (d < 180.0))
					direction = "°E ";
				else
				{
					d = 360.0 - d;
					direction = "°W ";
				}
			else
				if (d >= 0)
					direction = "°N ";
				else
				{
					d = -d;
					direction = "°S ";
				}

			int deg = (int)d;
			d -= deg;
			d = System.Math.Abs(d*60.0);
			int min = (int)d;
			d -= min;
			d *= 60.0;

			return deg.ToString("#0") + direction + min.ToString("#0") + "' " + d.ToString("#0.000", nfi) + "\"";
		}

		/// <summary>
		/// Converts a grad expression into a radian expression.
		/// </summary>
		/// <param name="d">The expression in grad.</param>
		/// <returns>The expression in radians.</returns>
		static public double Grad2Rad(double d)
		{
			return d / 180 * System.Math.PI;
		}

		/// <summary>
		/// Converts a radian expression into a grad expression.
		/// </summary>
		/// <param name="d">The expression in radians.</param>
		/// <returns>The expression in grad.</returns>
		static public double Rad2Grad(double d)
		{
			return d * 180 / System.Math.PI;
		}
	}
}
