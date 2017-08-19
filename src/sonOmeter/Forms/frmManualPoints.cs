using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes.Sonar2D;
using XPTable.Models;
using sonOmeter.Classes;
using UKLib.Arrays;
using XPTable.Editors;

namespace sonOmeter
{
    public partial class frmManualPoints : DockDotNET.DockWindow
    {
        #region Variables
        private SonarProject project = null;
        private SonarRecord record = null;
        private IDList<ManualPoint> baseList = null;
        #endregion

        public frmManualPoints(SonarProject prj, SonarRecord rec)
		{
			InitializeComponent();

            if ((prj == null) || (rec == null))
                return;

            // Hide columns that are version dependent.
            cbColShowDepth.Visible = GSC.Settings.Lic[Module.Modules.V22];
            cbColShowDesc.Visible = GSC.Settings.Lic[Module.Modules.V22];
            cbColShowType.Visible = GSC.Settings.Lic[Module.Modules.V22];

            project = prj;
            record = rec;

            if (record is SonarProfile)
                this.Text = "[" + record.Description + "]";
            else
                this.Text = "[" + (project.IndexOf(record) + 1).ToString() + "]";

            this.Text += " - Manual Points";

			baseList = record.ManualPoints;

			xpTable.BeginUpdate();

            foreach (ManualPoint pt in baseList)
                AddManualPoint(pt);

            xpTable.EndUpdate();

            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceManualPoint);

            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalNotifier), filterlist);
        }

        private void AddManualPoint(ManualPoint pt)
        {
            int numDecDig = GSC.Settings.NFI.NumberDecimalDigits;
            GSC.Settings.NFI.NumberDecimalDigits = 3;

            Row row = new Row();
            row.Tag = pt;

            row.Cells.Add(new Cell(pt.ID.ToString()));
            row.Cells.Add(new Cell(pt.Coord.ToString(false, GSC.Settings.NFI), pt.Coord));
            row.Cells.Add(new Cell(pt.Depth.ToString("F", GSC.Settings.NFI)));
            row.Cells.Add(new Cell((pt.Coord.AL + pt.Depth).ToString("F", GSC.Settings.NFI)));
            row.Cells.Add(new Cell(pt.Color.ToKnownColor().ToString(), pt.Color.ToKnownColor()));
            row.Cells.Add(new Cell(pt.Description));
            row.Cells.Add(new Cell(pt.Time));
            row.Cells.Add(new Cell(pt.Type.ToString()));
            row.Cells.Add(new Cell("", pt.ShowTextDepth));
            row.Cells.Add(new Cell("", pt.ShowTextDesc));
            row.Cells.Add(new Cell("", pt.ShowTextType));

            foreach (Cell c in row.Cells)
                c.WordWrap = false;

            tm.Rows.Add(row);

            GSC.Settings.NFI.NumberDecimalDigits = numDecDig;
        }

        private void OnGlobalNotifier(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.PlaceManualPoint:
                    if (sender == record)
                        AddManualPoint(args as ManualPoint);
                    break;
            }
        }

        private void xpTable_EditingStopped(object sender, XPTable.Events.CellEditEventArgs e)
        {
            ManualPoint pt = e.Cell.Row.Tag as ManualPoint;

            switch (e.CellPos.Column)
            {
                case 0:
                    int id = 0;
                    if (int.TryParse((e.Editor as XPTable.Editors.TextCellEditor).TextBox.Text, out id))
                        pt.ID = id;

                    baseList.UpdateID(pt);
                        
                    e.Cell.Text = pt.ID.ToString();
                    break;
                case 1:
                    pt.Coord = (e.Editor as XPTableCoordEditor).Editor.Coord;
                    break;
                case 2:
                    float depth = 0;
                    if (float.TryParse((e.Editor as XPTable.Editors.TextCellEditor).TextBox.Text, System.Globalization.NumberStyles.Any, GSC.Settings.NFI, out depth))
                    {
                        pt.Depth = depth;
                        e.Cell.Row.Cells[e.Column + 1].Text = (pt.Coord.AL + pt.Depth).ToString("F3", GSC.Settings.NFI);
                    }
                    else
                        e.Cell.Text = pt.Depth.ToString(GSC.Settings.NFI);
                    break;
                case 3:
                    break;
                case 4:
                    if ((e.Editor as ComboBoxCellEditor).SelectedItem != null)
                        pt.Color = Color.FromKnownColor((KnownColor)(e.Editor as ComboBoxCellEditor).SelectedItem);
                    break;
                case 5:
                    pt.Description = (e.Editor as XPTable.Editors.TextCellEditor).TextBox.Text;
                    break;
                case 6:
                    break;
                case 7:
                    int type = 0;
                    if (int.TryParse((e.Editor as XPTable.Editors.TextCellEditor).TextBox.Text, out type))
                        pt.Type = type;
                    else
                        e.Cell.Text = pt.Type.ToString();
                    break;
            }
        }

        private void xpTable_BeginEditing(object sender, XPTable.Events.CellEditEventArgs e)
        {
            switch (e.Column)
            {
                case 1:
                    break;

                case 4:
                    ((ComboBoxCellEditor)e.Editor).Items.Clear();

                    foreach (SonarEntryConfig secl in GSC.Settings.SECL)
                        ((ComboBoxCellEditor)e.Editor).Items.Add(secl.SonarColor.ToKnownColor());

                    ((ComboBoxCellEditor)e.Editor).Items.Add(Color.White.ToKnownColor());
                    ((ComboBoxCellEditor)e.Editor).Items.Add(Color.Gray.ToKnownColor());
                    break;
            }
        }

        private void xpTable_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Delete:
                    if (xpTable.SelectedItems.Length > 0)
                    {
                        int[] deleted = xpTable.SelectedIndicies;
                        for (int i = deleted.Length - 1; i >= 0; i--)
                        {
                            baseList.RemoveAt(deleted[i]);
                            xpTable.TableModel.Rows.RemoveAt(deleted[i]);
                        }
                    }
                    break;
            }
        }

        private void xpTable_CellCheckChanged(object sender, XPTable.Events.CellCheckBoxEventArgs e)
        {
            ManualPoint pt = e.Cell.Row.Tag as ManualPoint;

            switch (e.Column)
            {
                case 8:
                    pt.ShowTextDepth = e.Cell.Checked;
                    break;

                case 9:
                    pt.ShowTextDesc = e.Cell.Checked;
                    break;

                case 10:
                    pt.ShowTextType = e.Cell.Checked;
                    break;
            }
        }
    }
}
