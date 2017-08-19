using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sonOmeter
{
	public partial class frmPropertyViewer : Form
	{
		public frmPropertyViewer()
		{
			InitializeComponent();
		}

		public static DialogResult ShowDialog(object obj)
		{
			frmPropertyViewer form = new frmPropertyViewer();
			form.pg.SelectedObject = obj;

			return form.ShowDialog();
        }

        public static DialogResult ShowDialog(object obj, string caption)
        {
            frmPropertyViewer form = new frmPropertyViewer();
            form.Text = caption;
            form.pg.SelectedObject = obj;

            return form.ShowDialog();
        }
	}
}