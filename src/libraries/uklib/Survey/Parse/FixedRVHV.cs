using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UKLib.Survey.Parse
{
	public class FixedRVHV
	{
		#region Variables
		protected double rightValue = 0;
		protected double highValue = 0;
		protected double altitude = 0;
		#endregion

		#region Properties
		public double RightValue
		{
			get
			{
				return rightValue;
			}
		}

		public double HighValue
		{
			get
			{
				return highValue;
			}
		}

		public double Altitude
		{
			get
			{
				return altitude;
			}
		}
		#endregion

		public FixedRVHV()
		{
			highValue = -1;
			rightValue = -1;
			altitude = -1;
		}

		public FixedRVHV(string data)
		{
			NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;

			try 
			{
				if (data.IndexOf("rv=") > -1) 
				{
					string s = data.Substring(data.IndexOf("rv="));
					s = s.Substring(3,s.IndexOf(";")-3);
					highValue = Double.Parse(s,nfi);
				}
			}
			catch
			{
                highValue = -1;
			}
			
			try 
			{
				if (data.IndexOf("hv=") > -1) 
				{
					string s = data.Substring(data.IndexOf("hv="));
					s = s.Substring(3,s.IndexOf(";")-3);
                    rightValue = Double.Parse(s, nfi);
				}
			}
			catch
			{
				rightValue = -1;
			}		

			try 
			{
				if (data.IndexOf("al=") > -1) 
				{
					string s = data.Substring(data.IndexOf("al="));
					s = s.Substring(3,s.IndexOf(";")-3);
					altitude = Double.Parse(s,nfi);
				}
			}
			catch
			{
                altitude = -1;
			}
		}
	}
}
