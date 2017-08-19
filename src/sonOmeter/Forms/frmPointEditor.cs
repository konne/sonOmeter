using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;

namespace sonOmeter
{
	public partial class frmPointEditor : Form
	{
        private List<ManualPoint> baseList = null;
        private List<ManualPoint> editList = null;

		public frmPointEditor()
		{
			InitializeComponent();
		}

        public frmPointEditor(List<ManualPoint> list)
		{
			InitializeComponent();

			baseList = list;
            editList = new List<ManualPoint>();

			lbPoints.BeginUpdate();

			foreach (ManualPoint pt in baseList)
			{
				ManualPoint ptNew = new ManualPoint(pt);
				editList.Add(ptNew);
				lbPoints.Items.Add(ptNew);
			}

			lbPoints.EndUpdate();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			baseList.Clear();

			foreach (ManualPoint pt in editList)
				baseList.Add(pt);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			editList.Clear();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			ManualPoint ptNew = new ManualPoint();
			editList.Add(ptNew);
			lbPoints.Items.Add(ptNew);
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			int i = lbPoints.SelectedIndex;

			if (i == -1)
				return;

			ManualPoint pt = lbPoints.SelectedItem as ManualPoint;
			lbPoints.Items.Remove(pt);
			editList.Remove(pt);

			if (lbPoints.Items.Count < i + 1)
				i--;

			lbPoints.SelectedIndex = i;
		}

		private void lbPoints_SelectedIndexChanged(object sender, EventArgs e)
		{
			pgPoint.SelectedObject = lbPoints.SelectedItem;
		}

		private void pgPoint_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (lbPoints.SelectedItem != pgPoint.SelectedObject)
				return;

			ManualPoint pt = lbPoints.SelectedItem as ManualPoint;
			int i = lbPoints.SelectedIndex;

			lbPoints.BeginUpdate();
			lbPoints.Items.RemoveAt(i);
			lbPoints.Items.Insert(i, pt);
			lbPoints.SelectedItem = pt;
			lbPoints.EndUpdate();
		}

		private void frmPointEditor_Load(object sender, EventArgs e)
		{
		}
	}
}