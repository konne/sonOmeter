using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.ComponentModel;
using sonOmeter.Classes.Sonar2D;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UKLib.Arrays;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Summary description for EllipsoidEditor.
	/// </summary>
	public class BuoyEditor : UITypeEditor
	{
		#region Variables
		private IWindowsFormsEditorService edSvc = null;
		private frmBuoyEditor editorForm = new frmBuoyEditor();
		#endregion

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null) 
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (edSvc != null) 
				{
                    editorForm.BuoyList = value as IDList<Sonar2DElement>;
					edSvc.ShowDialog(editorForm);
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}

			return base.GetEditStyle(context);
		}
	}
}
