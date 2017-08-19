using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Math;
using UKLib.Strings;

namespace sonOmeter.Classes
{
	public partial class frmCoordEditor : Form
	{
		public frmCoordEditor()
		{
			InitializeComponent();
		}

		#region Variables
		private Coordinate coord = Coordinate.Empty;
		#endregion

		#region Properties
		public Coordinate Coord
		{
			get { return coord; }
			set
			{
				coord = value;

				bool elliptic = value.Type == CoordinateType.Elliptic;

				tbD3.Text = coord.AL.ToString();

				if (elliptic)
				{
					tbD1.Text = Trigonometry.Rad2DMS(coord.LA, GSC.Settings.NFI, true);
					tbD2.Text = Trigonometry.Rad2DMS(coord.LO, GSC.Settings.NFI, false);

					rbElliptic.Checked = true;
				}
				else
				{
					tbD1.Text = coord.RV.ToString();
					tbD2.Text = coord.HV.ToString();

					rbTransvers.Checked = true;
				}
			}
		}
		#endregion

		private void CoordTypeChanged(object sender, EventArgs e)
		{
			double d1 = 0, d2 = 0, d3 = 0;
			double.TryParse(tbD3.Text, out d3);

			if (rbElliptic.Checked)
			{
				double.TryParse(tbD1.Text, out d1);
				double.TryParse(tbD2.Text, out d2);

				labD1.Text = "Latitude";
				labD2.Text = "Longitude";

				if (coord.Type == CoordinateType.TransverseMercator)
				{
					coord = new Coordinate(d1, d2, d3, CoordinateType.TransverseMercator);
					coord = GSC.Settings.ForwardTransform.Run(coord, CoordinateType.Elliptic);

					tbD1.Text = Trigonometry.Rad2DMS(coord.LA, GSC.Settings.NFI, true);
					tbD2.Text = Trigonometry.Rad2DMS(coord.LO, GSC.Settings.NFI, false);
				}
			}
			else
			{
				d1 = Trigonometry.DMS2Rad(tbD1.Text, GSC.Settings.NFI);
				d2 = Trigonometry.DMS2Rad(tbD2.Text, GSC.Settings.NFI);

				labD1.Text = "Right Value";
				labD2.Text = "High Value";

				if (coord.Type == CoordinateType.Elliptic)
				{
					coord = new Coordinate(d1, d2, d3, CoordinateType.Elliptic);
					coord = GSC.Settings.InversTransform.Run(coord, CoordinateType.TransverseMercator);

					tbD1.Text = coord.RV.ToString();
					tbD2.Text = coord.HV.ToString();
				}
			}

			tbD3.Text = coord.AL.ToString();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			double d1 = 0, d2 = 0, d3 = 0;
			double.TryParse(tbD3.Text, out d3);

            if (coord.Type == CoordinateType.TransverseMercator)
            {
                double.TryParse(tbD1.Text, out d1);
                double.TryParse(tbD2.Text, out d2);
            }
            else
            {
                d1 = Trigonometry.DMS2Rad(tbD1.Text, GSC.Settings.NFI);
                d2 = Trigonometry.DMS2Rad(tbD2.Text, GSC.Settings.NFI);
            }

			coord = new Coordinate(d1, d2, d3, coord.Type);
		}
	}
}