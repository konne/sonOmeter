using System;
using System.Xml;
using System.Drawing;
using System.Globalization;
using UKLib.Strings;

namespace UKLib.Xml
{
	/// <summary>
	/// Summary description for XmlReadConvert.
	/// </summary>
	public class XmlReadConvert
	{
		#region Xml read functions
		static public string Read(XmlTextReader reader, string attribute, string defaultValue)
		{
			string value = reader.GetAttribute(attribute);

			if (value == null)
				value = defaultValue;

			return value;
		}

		static public bool Read(XmlTextReader reader, string attribute, bool defaultValue)
		{
			bool value = defaultValue;

			string s = reader.GetAttribute(attribute);

			if (s != null)
				value = FastConvert.ToBoolean(s);

			return value;
		}

		static public double Read(XmlTextReader reader, string attribute, double defaultValue)
		{
			double value = defaultValue;

			string s = reader.GetAttribute(attribute);

			if (s != null)
				value = FastConvert.ToDouble(s);
				//double.TryParse(s, NumberStyles.Float, nfi, out value);

			return value;
		}

		static public float Read(XmlTextReader reader, string attribute, float defaultValue)
		{
			float value = defaultValue;

			string s = reader.GetAttribute(attribute);

			if (s != null)
				value = FastConvert.ToSingle(s);

			return value;
		}

		static public Int16 Read(XmlTextReader reader, string attribute, Int16 defaultValue)
		{
			Int16 value = defaultValue;

			string s = reader.GetAttribute(attribute);

			if (s != null)
				value = FastConvert.ToInt16(s);

			return value;
		}

		static public Int32 Read(XmlTextReader reader, string attribute, Int32 defaultValue)
		{
			Int32 value = defaultValue;

			string s = reader.GetAttribute(attribute);

			if (s != null)
				value = FastConvert.ToInt32(s);

			return value;
		}

		static public Color Read(XmlTextReader reader, Color defaultValue, string prefix, NumberFormatInfo nfi)
		{
			Color value;

			try
			{
				string s = reader.GetAttribute(prefix + "colorname");
				if (s != null)
				{
					value = Color.FromName(s);
				}
				else
				{
					s = reader.GetAttribute(prefix + "argb");
					int col = 0;
					int.TryParse(s, NumberStyles.HexNumber, nfi, out col);
					value = Color.FromArgb(col);
				}
			}
			catch
			{
				value = defaultValue;
			}

			return value;
		}

		static public Color Read(XmlTextReader reader, Color defaultValue, NumberFormatInfo nfi)
		{
			return Read(reader, defaultValue, "", nfi);
		}
		#endregion
	}
}
