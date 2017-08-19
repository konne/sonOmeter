using System;
using System.Globalization;

namespace UKLib.Survey.Parse
{
	/// <summary>
	/// Summary description for SoSoZeiss.
	/// </summary>
	public class SoSoZeiss
	{
		#region  Variables
		private NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;

		private double azimut=0;
		private double elevation=0;
		private double distance=0;
		#endregion

		#region Properties
		public double Azimut
		{
			get { return azimut; }
		}
		public double Elevation
		{
			get { return elevation; }
		}
		public double Distance
		{
			get { return distance; }
		}

		#endregion

		public SoSoZeiss(string data)
		{
			String delim = "0 ";
			try
			{ azimut = Double.Parse(data.Substring(0,data.IndexOf(";")).Trim(delim.ToCharArray()),nfi); }
			catch
			{ azimut = Double.NaN; }
			
			data = data.Remove(0,data.IndexOf(";")+1);

			try
			{ elevation = Double.Parse(data.Substring(0,data.IndexOf(";")).Trim(),nfi); }
			catch
			{ elevation = Double.NaN; }

			data = data.Remove(0,data.IndexOf(";")+1);

			try
			{ distance = Double.Parse(data.Trim(),nfi); }
			catch
			{ distance = Double.NaN; }
		}
		
	}
}
