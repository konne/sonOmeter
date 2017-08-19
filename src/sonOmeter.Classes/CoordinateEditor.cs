using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Math;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;

namespace sonOmeter.Classes
{
	public class CoordinateEditor : UITypeEditor
	{
		#region Variables
		private IWindowsFormsEditorService edSvc = null;
		private frmCoordEditor editorForm = new frmCoordEditor();
		#endregion

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (!(value is Coordinate))
					return value;

				if (edSvc != null)
				{
					editorForm.Coord = (Coordinate)value;
					if (edSvc.ShowDialog(editorForm) == DialogResult.OK)
						value = editorForm.Coord;
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
				return UITypeEditorEditStyle.Modal;

			return base.GetEditStyle(context);
		}
	}
}
