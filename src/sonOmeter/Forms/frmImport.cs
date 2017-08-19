using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for frmImport.
	/// </summary>
	public partial class frmImport : System.Windows.Forms.Form
	{

		public frmImport()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}


		private bool firstStart = true;

		private SonarProject project = new SonarProject();

		private SonarProject mainProject = null;
		public SonarProject MainProject
		{
			set { mainProject = value; }
		}		
		
		private string fileName = "";
		public string FileName
		{
			set { fileName = value; }
		}

		private void frmImport_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (this.Visible && firstStart)
			{
				firstStart = false;

				if (!File.Exists(fileName))
					Close();

				this.Cursor = Cursors.WaitCursor;
				Application.DoEvents();

				project.Read(fileName, true);
				
				label.Visible = false;
				checkedListBox.Visible = true;
				Application.DoEvents();

				for (int i=0; i<project.RecordCount; i++)
				{
					SonarRecord record = project.Record(i);

					if (record.Description == "") record.Description = "Record "+i;
					string str = record.Description+" - ("+record.TimeStart.ToString("T")+"-"+record.TimeEnd.ToString("T")+")";
					
					checkedListBox.Items.Add(str, false);
					Application.DoEvents();
				}

				btnAccept.Enabled = true;
				this.Cursor = Cursors.Default;
			}
		}

		private void btnAccept_Click(object sender, System.EventArgs e)
		{
			if (mainProject == null)
				return;
			if (project == null)
				return;

			foreach (int i in checkedListBox.CheckedIndices)
				mainProject.AddRecord(project.Record(i));

			project.Dispose();
		}
	}
}
