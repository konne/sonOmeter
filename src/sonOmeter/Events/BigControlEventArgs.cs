using System;

namespace sonOmeter
{
	public delegate void BigControlerHandler(object sender, BigControlEventArgs e);


	public enum BigControlEventType
	{
		RecordStart,
		RecordStop,
		ProjectSave,
		TrackingStart,
		TrackingStop,
        RestartServer,
		ManualInput,
        Simulate
	}

	/// <summary>
	/// Summary description for BigControlEventArgs.
	/// </summary>
	public class BigControlEventArgs : EventArgs
	{
		#region Variables
		private BigControlEventType bigControlEvent;
		#endregion

		#region Properties
		public BigControlEventType BigControlEvent
		{
			get { return bigControlEvent; }
		}
		#endregion

		public BigControlEventArgs(BigControlEventType bgtype)
		{
			bigControlEvent = bgtype;
		}
	}
	
}
