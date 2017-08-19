using System;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class NewSonarLineEventArgs
	{
		#region Variables
		SonarLine line;
		#endregion

		#region Properties
		public SonarLine Line 
		{
			get { return line;}
		}
		#endregion
		
		public NewSonarLineEventArgs(SonarLine line)
		{
			this.line = line;
		}
	}

	public delegate void NewSonarLineHandler(object sender, NewSonarLineEventArgs e);
}
