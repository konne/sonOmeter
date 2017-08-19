using System;
using System.Collections.Generic;
using System.Text;
using UKLib.Survey.Math;

namespace sonOmeter.Classes
{
    public class NewCoordsEventArgs
    {
        #region Variables
        Coordinate coord;
		#endregion

		#region Properties
        public Coordinate Coord 
		{
            get { return coord; }
		}
		#endregion

        public NewCoordsEventArgs(Coordinate coord)
		{           
			this.coord = coord;
		}
	}

	public delegate void NewCoordsHandler(object sender, NewCoordsEventArgs e);
}
