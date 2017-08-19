using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes.Sonar2D;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UKLib.Arrays;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Summary description for frmBuoySelector.
	/// </summary>
	public partial class frmBuoySelector : System.Windows.Forms.Form
	{

		public frmBuoySelector()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}


        private IDList<Sonar2DElement> buoyList = null;
        public IDList<Sonar2DElement> BuoyList
		{
			get { return buoyList; }
			set { buoyList = value; }
		}

		private Buoy firstBuoy = null;
		public Buoy FirstBuoy
		{
			get { return firstBuoy; }
			set { firstBuoy = value; }
		}
		
		private Buoy secondBuoy = null;
		public Buoy SecondBuoy
		{
			get { return secondBuoy; }
			set { secondBuoy = value; }
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			firstBuoy = lbFirstBuoy.SelectedItem as Buoy;
			secondBuoy = lbSecondBuoy.SelectedItem as Buoy;
		}

		private void BuildList(ListBox lb)
		{
			lb.BeginUpdate();
			lb.Items.Clear();

			foreach (Buoy b in buoyList)
				lb.Items.Add(b);
			
			lb.EndUpdate();
		}

		private void frmBuoySelector_Load(object sender, System.EventArgs e)
		{
			BuildList(lbFirstBuoy);
			BuildList(lbSecondBuoy);
		}

		private void lbFirstBuoy_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lbFirstBuoy.SelectedItem != firstBuoy)
			{
				firstBuoy = lbFirstBuoy.SelectedItem as Buoy;

				BuildList(lbSecondBuoy);

				if (firstBuoy != null)
				{
					lbSecondBuoy.Items.Remove(firstBuoy);
					lbSecondBuoy.SelectedItem = secondBuoy;
				}
			}
		}

		private void lbSecondBuoy_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lbSecondBuoy.SelectedItem != secondBuoy)
			{
				secondBuoy = lbSecondBuoy.SelectedItem as Buoy;

				BuildList(lbFirstBuoy);

				if (secondBuoy != null)
				{
					lbFirstBuoy.Items.Remove(secondBuoy);
					lbFirstBuoy.SelectedItem = firstBuoy;
				}
			}
		}
	}
}
