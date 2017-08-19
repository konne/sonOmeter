using System;
using System.Xml;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using sonOmeter.Classes.Sonar2D;
using UKLib.Xml;
using System.Collections.ObjectModel;
using UKLib.Controls;
using System.Collections.Generic;
using UKLib.Arrays;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Summary description for SurveyEditorForm.
	/// </summary>
	public partial class frmBuoyEditor : System.Windows.Forms.Form
	{
		#region Construct and dispose
		public frmBuoyEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            btnSetStation.Visible = GSC.Settings.Lic[Module.Modules.V22];
		}

		#endregion

		#region Variables
        private IDList<Sonar2DElement> buoyList = null;
        private IDList<Sonar2DElement> buoyConnectionsList = null;

		#endregion

		#region Properties
        public IDList<Sonar2DElement> BuoyList
		{
			get { return buoyList; }
			set { buoyList = value; }
		}

        public IDList<Sonar2DElement> BuoyConnectionsList
		{
			get { return buoyConnectionsList; }
			set { buoyConnectionsList = value; }
		}
		#endregion

		#region Lists and property grid
		private void BuildList()
		{
			lbList.BeginUpdate();
			lbList.Items.Clear();

			foreach (Buoy b in buoyList)
				lbList.Items.Add(b);
			
			lbList.EndUpdate();
		}

		private void BuildConnectionsList()
		{
			lbConnections.BeginUpdate();
			lbConnections.Items.Clear();

			foreach (BuoyConnection c in buoyConnectionsList)
				lbConnections.Items.Add(c);
			
			lbConnections.EndUpdate();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BuildList();
			BuildConnectionsList();
		}

		private void lbList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			propertyGrid.SelectedObject = lbList.SelectedItem;
		}

		private void lbConnections_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			propertyGrid.SelectedObject = lbConnections.SelectedItem;
		}
		
		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "ID")
			{
				if (propertyGrid.SelectedObject is Buoy)
				{
					buoyList.UpdateID(propertyGrid.SelectedObject as Buoy);
                    BuildList();
				}
				else
				{
                    buoyConnectionsList.UpdateID(propertyGrid.SelectedObject as Sonar2DElement);
					BuildConnectionsList();
				}
			}
		}
		#endregion

		#region Buttons
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;

			Buoy b = new Buoy();
			buoyList.Add(b);
			
			lbList.Items.Add(b);
			lbList.SelectedItem = b;

			this.Cursor = Cursors.Default;
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Remove");

            Buoy[] selectedBuoys = new Buoy[lbList.SelectedItems.Count];
            lbList.SelectedItems.CopyTo(selectedBuoys, 0);

            foreach (Buoy b in selectedBuoys)
            {
                buoyList.Remove(b);
                lbList.Items.Remove(b);

                int count = buoyConnectionsList.Count;

                for (int i = 0; i < count; i++)
                {
                    BuoyConnection c = buoyConnectionsList[i] as BuoyConnection;

                    if ((c.StartBuoy == b) | (c.EndBuoy == b))
                    {
                        i--;
                        count--;

                        buoyConnectionsList.Remove(c);
                        lbConnections.Items.Remove(c);
                    }
                }
            }
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Please confirm the clear list command.", "Clear entire list?", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				buoyConnectionsList.Clear();
				buoyList.Clear();
				propertyGrid.SelectedObject = null;
				BuildList();
				BuildConnectionsList();
			}
		}

		private void btnLoad_Click(object sender, System.EventArgs e)
		{
			if (dlgOpen.ShowDialog() != DialogResult.OK)
				return;

			Buoy.ReadXml(dlgOpen.FileName, buoyList, buoyConnectionsList);
			BuildList();
			BuildConnectionsList();
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			if (dlgSave.ShowDialog() != DialogResult.OK)
				return;

			Buoy.WriteXml(dlgSave.FileName, buoyList, buoyConnectionsList, new CultureInfo("en-US", false).NumberFormat);
		}

		private void btnAddConn_Click(object sender, System.EventArgs e)
		{
			frmBuoySelector frm = new frmBuoySelector();
			frm.BuoyList = buoyList;

			if (frm.ShowDialog() == DialogResult.OK)
			{
				if ((frm.FirstBuoy == null) | (frm.SecondBuoy == null))
					return;

				this.Cursor = Cursors.WaitCursor;

				BuoyConnection c = new BuoyConnection();
				c.StartBuoy = frm.FirstBuoy;
				c.EndBuoy = frm.SecondBuoy;
				c.Description = c.StartBuoy.ID + " - " + c.EndBuoy.ID;

                List<Buoy> buoysDone = new List<Buoy>();
                buoysDone.Add(c.StartBuoy);
                
                // Create copy of connection list for fast search.
                List<BuoyConnection> tmp = new List<BuoyConnection>();

                foreach (BuoyConnection con in buoyConnectionsList)
                    tmp.Add(con);

                // Start the update.
                Buoy.UpdateStation(c.EndBuoy, c.StartBuoy.Station + c.Length, tmp, buoysDone);

				buoyConnectionsList.Add(c);
			
				lbConnections.Items.Add(c);
				lbConnections.SelectedItem = c;

				this.Cursor = Cursors.Default;
			}
		}

		private void btnRemoveConn_Click(object sender, System.EventArgs e)
		{
            BuoyConnection[] selectedConns = new BuoyConnection[lbConnections.SelectedItems.Count];
            lbConnections.SelectedItems.CopyTo(selectedConns, 0);

			foreach (BuoyConnection c in selectedConns)
            {
				buoyConnectionsList.Remove(c);
				lbConnections.Items.Remove(c);
			}
        }

        private void btnSetStation_Click(object sender, EventArgs e)
        {
            if ((lbList.SelectedItems.Count != 1) && (lbConnections.SelectedItems.Count != 1))
            {
                MessageBox.Show("Please select one buoy or connection.", "Action aborted.");
                return;
            }

            Buoy b;

            if (lbList.SelectedItems.Count != 1)
                b = (lbConnections.SelectedItems[0] as BuoyConnection).StartBuoy;
            else
                b = lbList.SelectedItems[0] as Buoy;

            // Request user entry of new value and check it.
            InputBoxResult res = InputBox.Show(b.Station.ToString(), "Enter new station value:", "Edit station");

            if (res.DialogResult != DialogResult.OK)
                return;

            double station = 0;

            if (!double.TryParse(res.InputText, out station))
            {
                MessageBox.Show("Please enter a valid number.", "Action aborted.");
                return;
            }

            // Create copy of connection list for fast search.
            List<BuoyConnection> tmp = new List<BuoyConnection>();

            foreach (BuoyConnection c in buoyConnectionsList)
                tmp.Add(c);

            // Start the update.
            Buoy.UpdateStation(b, station, tmp, new List<Buoy>());
        }
        #endregion
    }
}
