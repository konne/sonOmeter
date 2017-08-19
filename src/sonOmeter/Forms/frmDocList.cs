using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;
using DockDotNET;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public partial class frmDocList : System.Windows.Forms.Form
	{

		public frmDocList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		protected DockManager manager = null;
		public DockManager Manager
		{
			set
			{
				manager = value;

				RebuildList();
			}
		}

		private void RebuildList()
		{
			try
			{
				if (manager != null)
				{
					listBox.Items.Clear();

					foreach (DockPanel p in DockManager.ListDocument)
					{
						if (p.Form.Visible)
							listBox.Items.Add(p);
					}
				}
			}
			catch (Exception e)
			{
				UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmDocList.RebuildList: "+e.Message);
			}
		}

		private void btnBringToFront_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (listBox.SelectedItem != null)
				{
					DockWindow frm = (listBox.SelectedItem as DockPanel).Form;

					if (!frm.Visible)
						return;
					
					if (frm.IsDocked)
						frm.HostContainer.SelectTab(frm.ControlContainer);
					else
						frm.BringToFront();

					Close();
				}
			}
			catch (Exception ex)
			{
				UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmDocList.btnBringToFront_Click: "+ex.Message);
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (listBox.SelectedItem != null)
				{
					DockWindow frm = (listBox.SelectedItem as DockPanel).Form;

					if (!frm.Visible)
						return;

					frm.Close();

					Close();
				}
			}
			catch (Exception ex)
			{
				UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmDocList.btnBringToFront_Click: "+ex.Message);
			}
		}

		private void frmDocList_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			listBox.Items.Clear();
		}
	}
}
