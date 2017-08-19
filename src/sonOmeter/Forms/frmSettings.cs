using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
	[Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
	public partial class frmSettings : DockDotNET.DockWindow
	{
		#region Construct and dispose
		public frmSettings()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
					
			GlobalNotifier.SignIn(new GlobalEventHandler(OnSwitchProperties), GlobalNotifier.MsgTypes.SwitchProperties);
		}

		#endregion

		#region Variables
		protected object selectedObject = null;
		#endregion

		#region Properties
		public object SelectedObject
		{
			get { return selectedObject; }
			set
			{
				selectedObject = value;
				propGrid.SelectedObject = selectedObject;

				if (selectedObject is GlobalSettings)
				{
					propGrid.Height = propGrid.Parent.Height - 40;
					btnLoad.Visible = true;
					btnLoad.Enabled = true;
					btnSave.Visible = true;
					btnSave.Enabled = true;
				}
				else
				{
					propGrid.Height = propGrid.Parent.Height;
					btnLoad.Visible = false;
					btnLoad.Enabled = false;
					btnSave.Visible = false;
					btnSave.Enabled = false;
				}
			}
		}
		#endregion

		#region Buttons
		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			if (!(selectedObject is GlobalSettings))
				return;
			
			if (openConfigDlg.ShowDialog() == DialogResult.OK)
			{
                GSC.ReadXmlFile(openConfigDlg.FileName);
                propGrid.SelectedObject = GSC.Settings;
				propGrid.Refresh();
			}           
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{            
			if (!(selectedObject is GlobalSettings))
				return;

            if (MessageBox.Show("Save as Defaults?","Question", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                sonOmeter.Properties.Settings.Default.Setting = GSC.WriteXmlGetString();
                sonOmeter.Properties.Settings.Default.Save();
            }
            else
            {                                
                if (saveConfigDlg.ShowDialog() == DialogResult.OK)
                {
                    GSC.WriteXml(saveConfigDlg.FileName);
                }
            }		           
		}
		#endregion

        private void OnSwitchProperties(object sender, object args, GlobalNotifier.MsgTypes state)
		{
			if (args != null)
				SelectedObject = args;
		}
	}
}

