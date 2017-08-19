using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;
using System.Windows.Forms;
using System.Drawing;

namespace sonOmeter.Classes
{
    public class CoordinateFactory : ControlFactory
    {
        public override System.Windows.Forms.Control GetControl(Cell cell)
        {
            if (cell.Data == null)
                return null;

            Label lab = new Label();
            lab.Click += new EventHandler(OnEditCoordinate);
            lab.BackColor = cell.Selected ? SystemColors.Highlight : cell.BackColor;
            lab.Text = cell.Data.ToString();
            lab.Height = cell.Row.TableModel.Table.RowHeight;
            
            return lab;
        }

        public override Control UpdateControl(Cell cell, Control control)
        {
            control.BackColor = cell.Row.AnyCellsSelected ? SystemColors.Highlight : cell.BackColor;
            //control.Height = cell.Row.Height;
            
            return control;
        }

        void OnEditCoordinate(object sender, EventArgs e)
        {
            MessageBox.Show("Test");
        }
    }
}
