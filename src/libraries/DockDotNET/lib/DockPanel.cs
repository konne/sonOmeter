using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Security.Permissions;
using System.Drawing.Drawing2D;

namespace DockDotNET
{
	/// <summary>
	/// This class is derived from the standard framework class <see cref="System.Windows.Forms.Panel"/>.
	/// It is used as final container of the window's controls and is transferred between <see cref="DockContainer"/> objects and the window.
	/// </summary>
	public class DockPanel : System.Windows.Forms.Panel
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construction and dispose
		/// <summary>
		/// Initializes a new instance of the <seealso cref="Panel"/> class.
		/// </summary>
		/// <param name="container">The host container.</param>
		public DockPanel(System.ComponentModel.IContainer container)
		{
			container.Add(this);
			InitializeComponent();

			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <seealso cref="Panel"/> class.
		/// </summary>
		public DockPanel()
		{
			InitializeComponent();
			Init();
		}

		private void Init()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DockPanel
			// 
			this.AutoScroll = true;

		}
		#endregion

		#region Variables
		private RectangleF tabRect = Rectangle.Empty;
		
		private Size minFormSize;
		private Size maxFormSize;

		private DockWindow form;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the rectangle of the tab panel.
		/// </summary>
		[Browsable(false)]
		public RectangleF TabRect
		{
			get { return tabRect; }
			set { tabRect = value; }
		}

		/// <summary>
		/// Gets or sets the host window.
		/// </summary>
		[Browsable(false)]
		public DockWindow Form
		{
			get { return form; }
			set { form = value; }
		}

		/// <summary>
		/// Gets or sets the minimum size of the form.
		/// </summary>
		[Browsable(false)]
		public Size MinFormSize
		{
			get { return minFormSize; }
			set { minFormSize = value; }
		}

		/// <summary>
		/// Gets or sets the maximum size of the form.
		/// </summary>
		[Browsable(false)]
		public Size MaxFormSize
		{
			get { return maxFormSize; }
			set { maxFormSize = value; }
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Overrides the base class function OnMouseDown.
		/// Used to set the focus to the parent control.
		/// </summary>
		/// <param name="e">A <see cref="MouseEventArgs"/> that contains the mouse event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (Parent != null)
				Parent.Focus();

			base.OnMouseDown (e);
		}
		
		/// <summary>
		/// Overrides the base class function IsInputKey.
		/// </summary>
		/// <param name="keyData">The key that is to be evaluated.</param>
		/// <returns>Always set to true.</returns>
		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}

		/// <summary>
		/// Overrides the base class function OnPaint.
		/// Fires the PostPaint event after drawing the contents.
		/// </summary>
		/// <param name="e">A <see cref="PaintEventArgs"/> that contains the paint data.</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			if (PostPaint != null)
				PostPaint(this, e);
		}
		
		/// <summary>
		/// Overrides the base class function ToString.
		/// </summary>
		/// <returns>The caption string of the attached form.</returns>
		public override string ToString()
		{
			return form.Text;
		}
		#endregion

		#region Own events
		/// <summary>
		/// Occurs after the base drawing is finished.
		/// </summary>
		public event PaintEventHandler PostPaint;

		/// <summary>
		/// Occurs when the panel gets activated.
		/// </summary>
		public event EventHandler Activated;

		/// <summary>
		/// Occurs when the panel gets deactivated.
		/// </summary>
		public event EventHandler Deactivate;

		/// <summary>
		/// Sets the focus to the panel.
		/// </summary>
		/// <param name="activate">True, if the panel is activated.</param>
		public void SetFocus(bool activate)
		{
			if (activate && (Activated != null))
				Activated(this, EventArgs.Empty);
			else if (!activate && (Deactivate != null))
				Deactivate(this, EventArgs.Empty);
		}
		#endregion

		#region XML r/w
		/// <summary>
		/// Writes the panel data to the window save list.
		/// </summary>
		/// <param name="writer">The <see cref="XmlTextWriter"/> object that writes to the target stream.</param>
		internal void WriteXml(XmlTextWriter writer)
		{
			writer.WriteStartElement("panel");
			writer.WriteAttributeString("dock", this.Dock.ToString());
			writer.WriteAttributeString("width", this.Width.ToString());
			writer.WriteAttributeString("height", this.Height.ToString());
			writer.WriteAttributeString("type", form.GetType().AssemblyQualifiedName);
			form.WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Reads the panel data from the window save list.
		/// </summary>
		/// <param name="reader">The <see cref="XmlReader"/> object that reads from the source stream.</param>
        /// <returns>The success of the operation</returns>
		internal bool ReadXml(XmlReader reader)
		{
			try
			{
				string s;
                int w = 100, h = 100;

                // read out dock settings
				switch (reader.GetAttribute("dock"))
				{
					case "Fill":
						this.Dock = DockStyle.Fill;
						break;
					case "Top":
						this.Dock = DockStyle.Top;
						break;
					case "Bottom":
						this.Dock = DockStyle.Bottom;
						break;
					case "Left":
						this.Dock = DockStyle.Left;
						break;
					case "Right":
						this.Dock = DockStyle.Right;
						break;
					default:
						return false;
				}

                // read width and height
				s = reader.GetAttribute("width");
				if (s != null)
					w = int.Parse(s);

				s = reader.GetAttribute("height");
				if (s != null)
					h = int.Parse(s);

                // read the class type
				s = reader.GetAttribute("type");
				if (s == null)
                    return false;
				
				Type type = Type.GetType(s, true);

				if (type == null)
					return false;

                // fire FormLoad event - the application may take control of the window creation / show operation
                FormLoadEventArgs e = new FormLoadEventArgs(type);
                DockManager.InvokeFormLoad(this, e);
                DockWindow wnd = e.Form;

                if (e.Cancel)
                    return false;

                // no window specified by the application - create an own one using the given type
                if (wnd == null)
                {
                    ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);

                    if (info == null)
                        return false;

                    wnd = info.Invoke(new object[0]) as DockWindow;
                }

                // important! -> copy dockpanel size first
                this.Size = wnd.ControlContainer.Size;

                // copy the container content to the (new) window's controls
                int cNum = wnd.ControlContainer.Controls.Count;
                if (cNum > 0)
                {
                    Control[] cArray = new Control[cNum];
                    wnd.ControlContainer.Controls.CopyTo(cArray, 0);
                    this.Controls.AddRange(cArray);
                }

                // now it's safe to change the panel size to the stored values as all anchored controls will obey this
                this.Size = new Size(w, h);

                // create links to the container and the form
                wnd.ControlContainer = this;
				this.form = wnd;

                // show the window
                wnd.ShowFormAtOnLoad = false;
                wnd.Show();
                wnd.ShowFormAtOnLoad = true;

                // release panel from any DockContainer
                if (wnd.HostContainer != null)
                    wnd.Release();

                // additional reading of window settings
                wnd.ReadXml(reader);
			}
			catch (Exception ex)
			{
				Console.WriteLine("DockPanel.ReadXml: " + ex.Message);
			}

            return true;
		}
		#endregion

		/// <summary>
		/// Selects this panel as the active panel in the <see cref="DockContainer"/> parent.
		/// </summary>
		public void SelectTab()
		{
			if (this.Parent is DockContainer)
			{
				(this.Parent as DockContainer).SelectTab(this);
				(this.Parent as DockContainer).Select();
			}
		}
	}
}
