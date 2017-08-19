using System;
using System.Globalization;
using System.Collections.Specialized;

namespace UKLib.Survey.Parse
{
	/// <summary>
	/// Summary description for Geodimeter.
	/// </summary>
	public class Geodimeter : FixedRVHV
	{
		public Geodimeter(string data)
		{
			NumberFormatInfo nfi = new CultureInfo( "en-US", false ).NumberFormat;

			try 
			{
				if (data.IndexOf("37=") > -1) 
				{
					string s = data.Substring(data.IndexOf("37="));
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
				if (data.IndexOf("38=") > -1) 
				{
					string s = data.Substring(data.IndexOf("38="));
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
				if (data.IndexOf("39=") > -1) 
				{
					string s = data.Substring(data.IndexOf("39="));
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
