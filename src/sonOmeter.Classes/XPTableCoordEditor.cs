using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Editors;
using System.Drawing;
using XPTable.Win32;
using UKLib.Survey.Math;
using System.Windows.Forms;

namespace sonOmeter.Classes
{
    public class XPTableCoordEditor : CellEditor
    {
        protected frmCoordEditor editor;

        public frmCoordEditor Editor
        {
            get { return editor; }
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TextCellEditor class with default settings
        /// </summary>
        public XPTableCoordEditor()
            : base()
        {
            editor = new frmCoordEditor();

            this.Control = editor;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Sets the location and size of the CellEditor
        /// </summary>
        /// <param name="cellRect">A Rectangle that represents the size and location 
        /// of the Cell being edited</param>
        protected override void SetEditLocation(Rectangle cellRect)
        {
            editor.Location = cellRect.Location;
        }


        /// <summary>
        /// Sets the initial value of the editor based on the contents of 
        /// the Cell being edited
        /// </summary>
        protected override void SetEditValue()
        {
            editor.Coord = (Coordinate)this.EditingCell.Data;
        }


        /// <summary>
        /// Sets the contents of the Cell being edited based on the value 
        /// in the editor
        /// </summary>
        protected override void SetCellValue()
        {
            this.EditingCell.Data = editor.Coord;
            this.EditingCell.Text = editor.Coord.ToString(false, GSC.Settings.NFI);
        }


        /// <summary>
        /// Starts editing the Cell
        /// </summary>
        public override void StartEditing()
        {
            if (editor.ShowDialog() == DialogResult.OK)
                this.EditingTable.StopEditing();
            else
                this.EditingTable.CancelEditing();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handler for the editors TextBox.KeyPress event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">A KeyPressEventArgs that contains the event data</param>
        protected virtual void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == AsciiChars.CarriageReturn /*Enter*/)
            {
                if (this.EditingTable != null)
                {
                    this.EditingTable.StopEditing();
                }
            }
            else if (e.KeyChar == AsciiChars.Escape)
            {
                if (this.EditingTable != null)
                {
                    this.EditingTable.CancelEditing();
                }
            }
        }


        /// <summary>
        /// Handler for the editors TextBox.LostFocus event
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">An EventArgs that contains the event data</param>
        protected virtual void OnLostFocus(object sender, EventArgs e)
        {
            if (this.EditingTable != null)
            {
                this.EditingTable.StopEditing();
            }
        }

        #endregion
    }
}
