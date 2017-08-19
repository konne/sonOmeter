using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using sonOmeter.Classes.Sonar2D;
using UKLib.MathEx;
using UKLib.Survey.Math;
using UKLib.Controls;

namespace sonOmeter
{
	public partial class frmManualInput : DockDotNET.DockWindow
	{
		#region Helping struct
		public struct BuoyConn
		{
			public BuoyConnection conn;
			public bool startBuoy;

			public Buoy Buoy
			{
				get { return (startBuoy) ? conn.StartBuoy : conn.EndBuoy; }
			}

			public BuoyConn(BuoyConnection conn, bool startBuoy)
			{
				this.conn = conn;
				this.startBuoy = startBuoy;
			}

			public override string ToString()
			{
				return conn.ToString() + " : " + ((startBuoy) ? conn.StartBuoy.ToString() : conn.EndBuoy.ToString());
			}
		}
		#endregion

		#region Variables
		private SonarProject project = null;

		private GlobalEventHandler globalEventHandler;

		private Coordinate lastCoord = Coordinate.Empty;

		private bool onlyDirect = false;
		#endregion

		#region Properties
		public SonarProject Project
		{
			get { return project; }
			set { project = value; }
		}
		#endregion

		#region Constructor
		public frmManualInput()
		{
			Init();

			cbMode.SelectedIndex = 0;
			
			this.DockType = DockDotNET.DockContainerType.ToolWindow;
		}

		public frmManualInput(bool onlyDirect)
		{
			Init();

			this.onlyDirect = onlyDirect;

			if (!onlyDirect)
			{
				cbMode.SelectedIndex = 0;
			
				this.DockType = DockDotNET.DockContainerType.ToolWindow;
			}
			else
			{
				cbMode.SelectedIndex = 2;

				int h = panFullGroup.Height;

				this.Height -= h;
				panDirectOnlyGroup.Location = panFullGroup.Location;
				panDirectOnlyGroup.Height += h;
				panFullGroup.Visible = false;

				this.DockType = DockDotNET.DockContainerType.None;
			}
		}

		private void Init()
		{
			InitializeComponent();

			globalEventHandler = new GlobalEventHandler(OnGlobalEvent);			

            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.BuoyListChanged);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceBuoy);
            filterlist.Add(GlobalNotifier.MsgTypes.NewRecord);
            filterlist.Add(GlobalNotifier.MsgTypes.NewCoordinate);

			GlobalNotifier.SignIn(globalEventHandler, filterlist);
		}
		#endregion

        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
		{
			switch (state)
			{
				case GlobalNotifier.MsgTypes.BuoyListChanged:
				case GlobalNotifier.MsgTypes.PlaceBuoy:
				case GlobalNotifier.MsgTypes.NewRecord:
					RebuildList();
					break;

				case GlobalNotifier.MsgTypes.NewCoordinate:
					lastCoord = (Coordinate)args;
					break;
			}
		}

		private void frmManualInput_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (globalEventHandler != null)
			{
                var filterlist = new List<GlobalNotifier.MsgTypes>();
                filterlist.Add(GlobalNotifier.MsgTypes.BuoyListChanged);
                filterlist.Add(GlobalNotifier.MsgTypes.PlaceBuoy);
                filterlist.Add(GlobalNotifier.MsgTypes.NewRecord);
                filterlist.Add(GlobalNotifier.MsgTypes.NewCoordinate);
				
				GlobalNotifier.SignOut(globalEventHandler, filterlist);
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if ((lbBuoyList.SelectedItem == null) & (lbBuoyList2.SelectedItem == null))
			{
				MessageBox.Show("No buoy / record or connection selected!");
				return;
			}

			if (!project.Recording & (cbMode.SelectedIndex != 2))
			{
				MessageBox.Show("This feature is only available during recording.");
				return;
			}

			if (lastCoord.IsSame(Coordinate.Empty) & (cbMode.SelectedIndex == 2))
			{
				MessageBox.Show("This feature is only available during recording or tracking.");
				return;
			}

			SonarRecord rec = project.NewRecord;
			Buoy b = null;

			if (cbMode.SelectedIndex == 0)
				b = lbBuoyList.SelectedItem as Buoy;
			if (cbMode.SelectedIndex == 1)
				b = GetBuoyConn().Buoy;
			else if (cbMode.SelectedIndex == 2)
				rec = lbBuoyList.SelectedItem as SonarRecord;

			double dist = 0;
			float depth = 0;
			int type = 0;

			float.TryParse(tbDepth.Text, out depth);
			double.TryParse(tbDistance.Text, out dist);
			int.TryParse(tbType.Text, out type);

			if (cbMode.SelectedIndex == 2)
			{
				rec.AddManualPoint(new ManualPoint(lastCoord, depth, type));

				if (onlyDirect)
				{
					this.DialogResult = DialogResult.OK;
					Close();
				}

				return;
			}

			if (lastCoord.Type != CoordinateType.TransverseMercator)
				return;

			PointD ptStart = lastCoord.Point;

			if (cbMode.SelectedIndex == 1)
				ptStart = GetBuoyConn().conn.NormalizeToCorridor(ptStart);

			PointD ptEnd = b.Coord.Point;
			PointD ptDir = ptEnd - ptStart;

			if (tbDistance.Text == "")
			{
				UKLib.Controls.frmInputBox form = new UKLib.Controls.frmInputBox("Please enter the distance:", "Add manual point");
				if (form.ShowDialog() != DialogResult.OK)
					return;
				double.TryParse(form.InputText, out dist);
				if (dist == 0)
					return;
			}

			ptDir.Length = dist;
			PointD ptNew = ptDir + ptStart;

			// Höheninterpolation nicht verwenden
			//double al = lastCoord.AL + dist * (b.Coord.AL - lastCoord.AL);

			rec.AddManualPoint(new ManualPoint(new Coordinate(ptNew.X, ptNew.Y, lastCoord.AL, CoordinateType.TransverseMercator), depth, type));
		}

		private BuoyConn GetBuoyConn()
		{
			return (lbBuoyList.SelectedItem != null) ? ((BuoyConn)lbBuoyList.SelectedItem) : ((BuoyConn)lbBuoyList2.SelectedItem);
		}

		private void lbBuoyList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbBuoyList.SelectedItem != null)
				lbBuoyList2.SelectedItem = null;
		}

		private void lbBuoyList2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lbBuoyList2.SelectedItem != null)
				lbBuoyList.SelectedItem = null;
		}

		public void RebuildList()
		{

			lbBuoyList.BeginUpdate();
			lbBuoyList2.BeginUpdate();
			lbBuoyList.Items.Clear();
			lbBuoyList2.Items.Clear();

			if (project != null)
			{
				if (cbMode.SelectedIndex == 0)
				{
					foreach (Buoy b in project.BuoyList)
						lbBuoyList.Items.Add(b);
				}
				else if (cbMode.SelectedIndex == 1)
				{
					foreach (BuoyConnection c in project.BuoyConnectionList)
					{
						lbBuoyList.Items.Add(new BuoyConn(c, true));
						lbBuoyList2.Items.Add(new BuoyConn(c, false));
					}
				}
				else if (cbMode.SelectedIndex == 2)
				{
					foreach (SonarRecord rec in project.Records)
						lbBuoyList.Items.Add(rec);
				}
			}

			lbBuoyList.EndUpdate();
			lbBuoyList2.EndUpdate();
		}

		private void ModeChanged(object sender, EventArgs e)
		{
			if (cbMode.SelectedIndex == 0)
			{
				lbBuoyList.Width = (lbBuoyList2.Location.X - lbBuoyList.Location.X) + lbBuoyList2.Width;
				lbBuoyList2.Visible = false;
				labDistance.Text = "Distance to boat location (m):";
				labDescription.Text = "Select the destination buoy:";
				btnAdd.Text = "Add to current record";
			}
			else if (cbMode.SelectedIndex == 1)
			{
				lbBuoyList.Width = lbBuoyList2.Location.X - lbBuoyList.Location.X;
				lbBuoyList2.Visible = true;
				labDistance.Text = "Distance to boat projection (m):";
				labDescription.Text = "Select the connection and destination buoy:";
				btnAdd.Text = "Add to current record";
			}
			else if (cbMode.SelectedIndex == 2)
			{
				lbBuoyList.Width = (lbBuoyList2.Location.X - lbBuoyList.Location.X) + lbBuoyList2.Width;
				lbBuoyList2.Visible = false;
				labDistance.Text = "N/A";
				labDescription.Text = "Select the target record:";
				btnAdd.Text = "Add to selected record";
			}

			lbBuoyList.Height = lbBuoyList2.Height = btnAdd.Location.Y - 6 - lbBuoyList.Location.Y;

			//bool en = !(cbMode.SelectedIndex == 2);
			bool en = true;

			tbDistance.Enabled = en;
			lbBuoyList.Enabled = en;
			lbBuoyList2.Enabled = en;

			RebuildList();
		}
	}
}

