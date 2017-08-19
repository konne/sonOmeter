using System;
using System.Collections.Generic;
using System.Text;
using XPTable.Models;
using System.ComponentModel;
using System.Drawing;
using XPTable.Renderers;
using XPTable.Editors;
using XPTable.Sorting;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Represents a Column whose Cells are displayed as strings
    /// </summary>
    [DesignTimeVisible(false),
    ToolboxItem(false)]
    public class XPTableCoordColumn : Column
    {
        #region Constructor

        /// <summary>
        /// Creates a new TextColumn with default values
        /// </summary>
        public XPTableCoordColumn()
            : base()
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        public XPTableCoordColumn(string text)
            : base(text)
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        public XPTableCoordColumn(string text, int width)
            : base(text, width)
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public XPTableCoordColumn(string text, int width, bool visible)
            : base(text, width, visible)
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text and image
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        public XPTableCoordColumn(string text, Image image)
            : base(text, image)
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text, image and width
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        public XPTableCoordColumn(string text, Image image, int width)
            : base(text, image, width)
        {

        }


        /// <summary>
        /// Creates a new TextColumn with the specified header text, image, width and visibility
        /// </summary>
        /// <param name="text">The text displayed in the column's header</param>
        /// <param name="image">The image displayed on the column's header</param>
        /// <param name="width">The column's width</param>
        /// <param name="visible">Specifies whether the column is visible</param>
        public XPTableCoordColumn(string text, Image image, int width, bool visible)
            : base(text, image, width, visible)
        {

        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellRenderer
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellRenderer</returns>
        public override string GetDefaultRendererName()
        {
            return "TEXT";
        }


        /// <summary>
        /// Gets the Column's default CellRenderer
        /// </summary>
        /// <returns>The Column's default CellRenderer</returns>
        public override ICellRenderer CreateDefaultRenderer()
        {
            return new TextCellRenderer();
        }


        /// <summary>
        /// Gets a string that specifies the name of the Column's default CellEditor
        /// </summary>
        /// <returns>A string that specifies the name of the Column's default 
        /// CellEditor</returns>
        public override string GetDefaultEditorName()
        {
            return "COORD";
        }


        /// <summary>
        /// Gets the Column's default CellEditor
        /// </summary>
        /// <returns>The Column's default CellEditor</returns>
        public override ICellEditor CreateDefaultEditor()
        {
            return new XPTableCoordEditor();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the Type of the Comparer used to compare the Column's Cells when 
        /// the Column is sorting
        /// </summary>
        public override Type DefaultComparerType
        {
            get
            {
                return typeof(TextComparer);
            }
        }

        #endregion
    }
}
