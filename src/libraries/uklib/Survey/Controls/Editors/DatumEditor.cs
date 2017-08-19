using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using UKLib.Survey.Math;

namespace UKLib.Survey.Editors
{
	/// <summary>
	/// Summary description for DatumEditor.
	/// </summary>
	public class DatumEditor : UITypeEditor
	{
		#region Variables
		private IWindowsFormsEditorService edSvc = null;
		private frmSurveyEditor editorForm = new frmSurveyEditor(typeof(Datum2WGS));
		#endregion

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null) 
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (edSvc != null) 
				{
					editorForm.EditingInstance = value;
					if (edSvc.ShowDialog(editorForm) == DialogResult.OK)
						value = editorForm.EditingInstance;
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
