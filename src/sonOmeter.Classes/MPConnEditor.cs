using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UKLib.Survey.Math;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Drawing;
using UKLib.Arrays;
using System.Xml.Serialization;
using UKLib.Xml;
using sonOmeter.Classes.Sonar2D;
using UKLib.Controls;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Description class for the style of manual points.
	/// </summary>
	public class MPConnStyle
	{
		[DisplayName("Maximum Distance"), Description("The maximum distance (in m) of two manual points."), DefaultValue(5.0)]
		public double MaxDistance { get; set; }

		[DisplayName("Width"), Description("The width of the line."), DefaultValue(1.0F)]
		public float Width { get; set; }

		[DisplayName("Color"), Description("The color of the line."), DefaultValue(KnownColor.Black), XmlIgnore]
		public Color Color { get; set; }
		[Browsable(false), DefaultValue("Black")]
		public string XmlForeColor
		{
			get { return SerializeColor.Serialize(Color); }
			set { Color = SerializeColor.Deserialize(value); }
		}

		[DisplayName("Transparency"), Description("The transparency of the line."), DefaultValue(255)]
		public int Alpha { get; set; }

		[DisplayName("Show Points"), Description("Toggles the display of the manual points."), DefaultValue(false)]
		public bool ShowPoints { get; set; }

		[DisplayName("Use Point Template"), Description("Toggles a template for the manual points."), DefaultValue(false)]
		public bool UseTemplate { get; set; }

		[DisplayName("Point Template"), Description("Defines the template for the manual points.")]
		public ManualPointTemplate Template { get; set; }

		public MPConnStyle()
		{
			this.MaxDistance = 5.0;
			this.Width = 1.0F;
			this.Color = Color.FromKnownColor(KnownColor.Black);
			this.Alpha = 255;
			this.ShowPoints = false;
			this.UseTemplate = false;
			this.Template = new ManualPointTemplate();
		}

		public MPConnStyle(MPConnStyle style)
		{
			this.MaxDistance = style.MaxDistance;
			this.Width = style.Width;
			this.Color = style.Color;
			this.Alpha = style.Alpha;
			this.ShowPoints = style.ShowPoints;
			this.UseTemplate = style.UseTemplate;
			this.Template = new ManualPointTemplate(style.Template);
		}

		public Pen CreatePen()
		{
			return new Pen(Color.FromArgb(this.Alpha, this.Color), this.Width);
		}
	}

	public class MPConnEditor : UITypeEditor
	{
		#region Variables
		private IWindowsFormsEditorService edSvc = null;
		private frmMPConnEditor editorForm = new frmMPConnEditor();
		#endregion

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (!(value is SerializeDictionary<int, MPConnStyle>))
					return value;

				if (edSvc != null)
				{
					editorForm.StyleList = new SerializeDictionary<int, MPConnStyle>();

					foreach (KeyValuePair<int, MPConnStyle> kvp in (SerializeDictionary<int, MPConnStyle>)value)
						editorForm.StyleList.Add(kvp.Key, new MPConnStyle(kvp.Value));

					if (edSvc.ShowDialog(editorForm) == DialogResult.OK)
						value = editorForm.StyleList;
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
