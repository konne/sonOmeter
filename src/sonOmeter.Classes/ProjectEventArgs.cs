using System;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Summary description for ProjectEventType.
	/// </summary>
	public enum ProjectEventType
	{
        ShowSonar,
        ShowManualPoints,
        ShowPositions,
		Show2D,
        Show3D
	}

	/// <summary>
	/// Summary description for ProjectEventArgs.
	/// </summary>
	public class ProjectEventArgs : EventArgs
	{
		#region Properties
		private int id;
		public int Id
		{
			get { return id; }
		}
		
		private SonarRecord record;
		public SonarRecord Record
		{
			get { return record; }
		}

		private ProjectEventType type;
		public ProjectEventType Type
		{
			get { return type; }
		}
		#endregion
		
		public ProjectEventArgs(int id, SonarRecord record, ProjectEventType type)
		{
			this.id = id;
			this.record = record;
			this.type = type;
		}
	}

	/// <summary>
	/// Record event callback.
	/// </summary>
	public delegate void ProjectEventHandler(object sender, ProjectEventArgs e);	
}
