#region CVS info
// $Id: ProjectEventArgs.cs,v 1.3 2005/03/10 16:15:44 konne Exp $
// $Log: ProjectEventArgs.cs,v $
// Revision 1.3  2005/03/10 16:15:44  konne
// frmMain:
//
// big changes
//
// Revision 1.2  2005/02/16 23:43:44  konne
// new frmPositions add
//
// Revision 1.1  2005/02/15 23:33:28  beezle
// First check in.
//
#endregion

using System;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for ProjectEventType.
	/// </summary>
	public enum ProjectEventType
	{
		ShowSonar,
		Show2D,
		ToggleRecord,
		AddRecord,
		AddDevice,
		UpdateRecord,
		ShowPositions,
		DeleteRecord,
		DeleteAll
	}

	/// <summary>
	/// Summary description for ProjectEventArgs.
	/// </summary>
	public class ProjectEventArgs : EventArgs
	{
		#region Properties
		private string id;
		public string Id
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
		
		public ProjectEventArgs(string id, SonarRecord record, ProjectEventType type)
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
