using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace UKLib.MathEx
{
	public class PolygonD
	{
		#region Variables
		protected Collection<PointD> points = new Collection<PointD>();
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the point list.
		/// </summary>
		public Collection<PointD> Points
		{
			get { return points; }
			set { points = value; }
		}

		/// <summary>
		/// Gets the corresponding bounding box (e.g. for coarse contain checks).
		/// </summary>
		public RectangleD BoundingBox
		{
			get
			{
				RectangleD rc = null;
				int i, max = points.Count;

				// No points in the list? - Abort!
				if (max == 0)
					return rc;

				// Create starting condition.
				rc = new RectangleD(points[0]);

				// Browse through point list and merge each point into the rectangle.
				for (i = 1; i < max; i++)
					rc.Merge(points[i]);

				// The bounding box is ready!
				return rc;
			}
		}

		/// <summary>
		/// Gets the number of point entries.
		/// </summary>
		public int Count
		{
			get { return points.Count; }
		}
		#endregion

		#region Contains
		/// <summary>
		/// Is the given point inside the polygon trace?
		/// </summary>
		/// <param name="pt">The point that is to be checked.</param>
		/// <returns>True, if inside the polygon trace.</returns>
		public bool Contains(PointD pt)
		{
			PointD pt1, pt2;
			double sum = 0;
			int i, max = points.Count;

			// No points in the list? - Abort!
			if (max == 0)
				return false;

			// Browse through the list and build the sum of all angles.
			// Source: http://rw7.de/ralf/inffaq/polygon.html
			for (i = 0; i < max; i++)
			{
				pt1 = points[i] - pt;
				pt2 = ((i == max - 1) ? points[0] : points[i + 1]) - pt;

				sum += System.Math.Atan2(pt2.Y * pt1.X - pt2.X * pt1.Y, pt1.X * pt2.X + pt1.Y * pt2.Y);
			}
			
			// If the sum is zero, the point is not inside the polygon.
			return (System.Math.Round(sum, 3) != 0);
		}
		#endregion

		#region List handling
		public void Clear()
		{
			points.Clear();
		}
		#endregion
	}
}