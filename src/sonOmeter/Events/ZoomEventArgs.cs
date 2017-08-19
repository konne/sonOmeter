using System;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for ZoomEventArgs.
	/// </summary>
	public class ZoomEventArgs : EventArgs
	{
		public enum ZoomEventTypes
		{
			None,
			Start,
			End,
			Position
		}

		#region Properties
		private ZoomEventTypes zoomEventType = ZoomEventTypes.None;
		public ZoomEventTypes ZoomEventType
		{
			get
			{
				return zoomEventType;
			}
			set
			{
				zoomEventType = value;
			}
		}

		private double zoomValue = 0.0;
		public double ZoomValue
		{
			get
			{
				return zoomValue;
			}
			set
			{
				zoomValue = value;
			}
		}
		#endregion

		public ZoomEventArgs(ZoomEventTypes zoomEventType, double zoomValue)
		{
			this.zoomEventType = zoomEventType;
			this.zoomValue = zoomValue;
		}
	}

	/// <summary>
	/// Zoom event callback.
	/// </summary>
	public delegate void ZoomEventHandler(object sender, ZoomEventArgs e);
}
