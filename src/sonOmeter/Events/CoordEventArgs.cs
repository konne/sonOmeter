using System;
using UKLib.Survey.Math;

namespace sonOmeter
{
	
	/// <summary>
	/// Summary description for CoordEventArgs.
	/// </summary>
	public class CoordEventArgs : EventArgs
	{
		Coordinate coord;

		#region Properties
		public Coordinate Coord
		{
			get { return coord;}
		}
		#endregion

		public CoordEventArgs(Coordinate coord)
		{
			this.coord = coord;
			//
			// TODO: Add constructor logic here
			//
		}
	
	}

	public delegate void CoordEventHandler(object sender, CoordEventArgs e);
}
