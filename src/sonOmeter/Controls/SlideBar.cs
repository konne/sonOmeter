using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for SlideBar.
	/// </summary>
	public class SlideBar : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SlideBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SlideBar
			// 
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Name = "SlideBar";
			this.Size = new System.Drawing.Size(184, 16);

		}
		#endregion

		#region Types
		private enum BarStates { Normal, Hover, Drag, SizeStart, SizeEnd };
		public enum SlideAnchors { Width, End };
		#endregion

		#region Variables
		private double pitch = 1.0;
		private double regionWidth = 1.0;
		private double sectionStart = 0.25;
		private double sectionWidth = 0.5;
		private double sectionWidthMin = 0.01;
		private double sectionEnd = 0.75;
		private double ratio = 1.0;
		private double oldWidth = 0;

		private Color hoverColor = Color.DarkGray;
		private Color dragColor = Color.DimGray;

		private bool vertical = false;
		private bool keepRatio = false;

		private BorderStyle borderStyle = BorderStyle.None;
		private BarStates barState = BarStates.Normal;
		private Point last = new Point(0, 0);
		private SlideAnchors slideAnchor = SlideAnchors.Width;

		private int borderX = 1;
		private int borderY = 1;

		private int widthOffset = 0;
		#endregion

		#region Properties
		[Description("The pitch for mouse interaction."), Category("Behavior")]
		public double Pitch
		{
			get { return pitch; }
			set
			{
				if (value < 0.0)
					value = 0.0;
				if (value > RegionWidth)
					value = RegionWidth;
				pitch = value;
				if (sectionWidthMin < pitch)
					sectionWidthMin = pitch;
			}
		}

		[Description("Fixes either end or width when changing the section start."), Category("Behavior")]
		public SlideAnchors SlideAnchor
		{
			get { return slideAnchor; }
			set { slideAnchor = value; }
		}

		[Description("The distance of the slider from the edge."), Category("Appearance")]
		public int BorderX
		{
			get { return borderX; }
			set { borderX = value; Invalidate(); }
		}

		[Description("The distance of the slider from the edge."), Category("Appearance")]
		public int BorderY
		{
			get { return borderY; }
			set { borderY = value; Invalidate(); }
		}

		[Description("The size of the whole region."), Category("Data")]
		public double RegionWidth
		{
			get { return regionWidth; }
			set
			{
				if (value <= 0.0)
					return;
				regionWidth = value;
				if (sectionWidth > regionWidth)
					SectionWidth = regionWidth;

				UpdateRatio();
				Invalidate();
			}
		}

		[Description("The start of the pan section."), Category("Data")]
		public double SectionStart
		{
			get { return sectionStart; }
			set
			{
				if (value < 0.0)
					value = 0.0;
				if (slideAnchor == SlideAnchors.End)
				{
					sectionWidth = sectionEnd - value;
					if (sectionWidth < sectionWidthMin)
					{
						sectionWidth = sectionWidthMin;
						value = sectionEnd - sectionWidth;
					}
				}
				else
				{
					sectionEnd = value + sectionWidth;
					if (sectionEnd > regionWidth)
					{
						if (keepRatio)
						{
							if (sectionStart < value)
								value = sectionStart;
							sectionEnd = value + sectionWidth;
						}
						else
						{
							sectionEnd = regionWidth;
							value = sectionEnd - sectionWidth;
						}
					}
				}
				sectionStart = value;
				UpdateRatio();
				Invalidate();
			}
		}

		[Description("The width of the pan section."), Category("Data")]
		public double SectionWidth
		{
			get { return sectionWidth; }
			set
			{
				if (value < sectionWidthMin)
					value = sectionWidthMin;

				if (value > regionWidth)
					value = regionWidth;

				if (slideAnchor == SlideAnchors.End)
				{
					sectionStart = sectionEnd - value;
					if (sectionStart < 0)
					{
						sectionStart = 0;
						value = sectionEnd - sectionStart;
					}
				}
				else
				{
					sectionEnd = sectionStart + value;
					if (sectionEnd > regionWidth)
					{
						sectionEnd = regionWidth;
						sectionStart = sectionEnd - value;
					}
				}

				sectionWidth = value;
				UpdateRatio();
				Invalidate();
			}
		}

		[Description("The end of the pan section."), Category("Data")]
		public double SectionEnd
		{
			get { return sectionEnd; }
			set
			{
				if (value > regionWidth)
					value = regionWidth;
				sectionWidth = value - sectionStart;
				if (sectionWidth < sectionWidthMin)
				{
					sectionWidth = sectionWidthMin;
					value = sectionStart + sectionWidth;
				}
				sectionEnd = value;
				UpdateRatio();
				Invalidate();
			}
		}

		[Description("The minimum width of the pan section."), Category("Behavior")]
		public double SectionWidthMin
		{
			get { return sectionWidthMin; }
			set
			{
				if (value <= 0.0)
					return;
				sectionWidthMin = value;
				if (sectionWidth < sectionWidthMin)
				{
					sectionWidth = sectionWidthMin;
					sectionEnd = sectionStart + sectionWidth;
				}
				UpdateRatio();
				Invalidate();
			}
		}

		[Description("Toggles vertical pan."), Category("Appearance")]
		public bool Vertical
		{
			get { return vertical; }
			set { vertical = value; Invalidate(); }
		}

		[Description("If true, the section borders change when resizing."), Category("Behavior")]
		public bool KeepRatio
		{
			get { return keepRatio; }
			set { keepRatio = value; }
		}

		[Description("Color of slider when mouse hovers over it."), Category("Appearance")]
		public Color HoverColor
		{
			get { return hoverColor; }
			set { hoverColor = value; Invalidate(); }
		}

		[Description("Color of slider when mouse drags it."), Category("Appearance")]
		public Color DragColor
		{
			get { return dragColor; }
			set { dragColor = value; Invalidate(); }
		}

		[Description("The style of the surrounding border."), Category("Appearance")]
		public new BorderStyle BorderStyle
		{
			get { return borderStyle; }
			set { borderStyle = value; Invalidate(); }
		}

		public double Ratio
		{
			get { return ratio; }
		}

		public int WidthOffset
		{
			get { return widthOffset; }
			set { widthOffset = value; }
		}
		#endregion

		#region Events
		public event ZoomEventHandler OnZoom;

		public void InvokeZoom(ZoomEventArgs.ZoomEventTypes type)
		{
			double val = 0.0;

			switch (type)
			{
				case ZoomEventArgs.ZoomEventTypes.Start:
					val = sectionStart;
					break;
				case ZoomEventArgs.ZoomEventTypes.End:
					val = sectionEnd;
					break;
				case ZoomEventArgs.ZoomEventTypes.Position:
					val = sectionStart;
					break;
				default:
					return;
			}

			ZoomEventArgs e = new ZoomEventArgs(type, val);

			if (OnZoom != null)
				OnZoom(this, e);
		}
		#endregion

		#region Coordinate transformations
		public PointF PointToRegion(int x, int y)
		{
			return new PointF((float)((x - borderX) * regionWidth / (this.Width - 2 * borderX)),
							  (float)((y - borderY) * regionWidth / (this.Height - 2 * borderY)));
		}

		public SizeF SizeToRegion(int x, int y)
		{
			return new SizeF((float)(x * regionWidth / (this.Width - 2 * borderX)),
							 (float)(y * regionWidth / (this.Height - 2 * borderY)));
		}

		public Point RegionToPoint(double x, double y)
		{
			return new Point((int)((this.Width - 2 * borderX) / regionWidth * x) + borderX,
							 (int)((this.Height - 2 * borderY) / regionWidth * y) + borderY);
		}

		public Size RegionToSize(double x, double y)
		{
			return new Size((int)((this.Width - 2 * borderX) / regionWidth * x),
							(int)((this.Height - 2 * borderY) / regionWidth * y));
		}

		public Rectangle SlideRect()
		{
			if (vertical)
				return new Rectangle(RegionToPoint(0, sectionStart),
									 RegionToSize(regionWidth, sectionWidth));
			else
				return new Rectangle(RegionToPoint(sectionStart, 0),
									 RegionToSize(sectionWidth, regionWidth));
		}
		#endregion

		#region Paint events
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics graphics = e.Graphics;
			SolidBrush backBrush = new SolidBrush(BackColor);
			SolidBrush foreBrush = null;

			switch (barState)
			{
				default:
				case BarStates.Normal:
					foreBrush = new SolidBrush(ForeColor);
					break;
				case BarStates.Hover:
					foreBrush = new SolidBrush(HoverColor);
					break;
				case BarStates.Drag:
					foreBrush = new SolidBrush(DragColor);
					break;
			}

			// draw remainder
			Region region = new Region(new Rectangle(0, 0, Width, Height));
			region.Xor(SlideRect());
			graphics.FillRegion(backBrush, region);

			// draw slider rectangle
			graphics.FillRectangle(foreBrush, SlideRect());

			// draw border
			switch (borderStyle)
			{
				case BorderStyle.FixedSingle:
					graphics.DrawRectangle(SystemPens.WindowFrame, 0, 0, Width - 1, Height - 1);
					break;
				case BorderStyle.Fixed3D:
					graphics.DrawLine(SystemPens.ControlDark, 0, 0, Width - 1, 0);
					graphics.DrawLine(SystemPens.ControlDark, 0, 0, 0, Height - 1);
					graphics.DrawLine(SystemPens.ControlLightLight, Width - 1, 0, Width - 1, Height - 1);
					graphics.DrawLine(SystemPens.ControlLightLight, 0, Height - 1, Width - 1, Height - 1);
					break;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (keepRatio)
			{
				sectionWidth = (this.Width + widthOffset) / ratio;
				sectionEnd = sectionStart + sectionWidth;
			}

			Invalidate();
		}
		#endregion

		#region Keyboard events
		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}
		#endregion

		#region Mouse events
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (!Enabled)
				return;

			Rectangle rcSizeStart = SlideRect();
			Rectangle rcSizeEnd = SlideRect();
			Rectangle rcHover = SlideRect();

			if (vertical)
			{
				rcSizeStart.Offset(0, -1);
				rcSizeEnd.Offset(0, 1);
				rcHover.Inflate(0, -1);
			}
			else
			{
				rcSizeStart.Offset(-1, 0);
				rcSizeEnd.Offset(1, 0);
				rcHover.Inflate(-1, 0);
			}

			// set state
			if (rcHover.Contains(e.X, e.Y))
				barState = BarStates.Drag;
			else if (rcSizeStart.Contains(e.X, e.Y))
				barState = BarStates.SizeStart;
			else if (rcSizeEnd.Contains(e.X, e.Y))
				barState = BarStates.SizeEnd;
			else
				barState = BarStates.Normal;

			last.X = e.X;
			last.Y = e.Y;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// recover normal conditions
			barState = BarStates.Normal;
			Cursor = Cursors.Arrow;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (!Enabled)
				return;

			Rectangle rcSize = SlideRect();
			Rectangle rcHover = SlideRect();

			if (vertical)
			{
				rcSize.Inflate(0, 1);
				rcHover.Inflate(0, -1);
			}
			else
			{
				rcSize.Inflate(1, 0);
				rcHover.Inflate(-1, 0);
			}

			// switch cursor
			if ((barState == BarStates.Hover) || (barState == BarStates.Normal))
			{
				if (rcHover.Contains(e.X, e.Y))
				{
					Cursor = Cursors.Hand;
					if (barState != BarStates.Hover)
					{
						barState = BarStates.Hover;
						Invalidate();
					}
				}
				else if (rcSize.Contains(e.X, e.Y))
					if (vertical)
						Cursor = Cursors.SizeNS;
					else
						Cursor = Cursors.SizeWE;
				else
				{
					Cursor = Cursors.Arrow;
					if (barState != BarStates.Normal)
					{
						barState = BarStates.Normal;
						Invalidate();
					}
				}
			}

			// perform action
			SizeF sizeRel = SizeToRegion(e.X - last.X, e.Y - last.Y);
			SizeF sizeAbs = SizeToRegion(e.X, e.Y);

			if ((vertical && (Math.Abs(sizeRel.Height) < pitch)) ||
				(!vertical && (Math.Abs(sizeRel.Width) < pitch)))
				return;

			SlideAnchors anchor = slideAnchor;

			sizeRel.Width = (float)pitch * (int)(sizeRel.Width / pitch);
			sizeRel.Height = (float)pitch * (int)(sizeRel.Height / pitch);
			sizeAbs.Width = (float)pitch * (int)(sizeAbs.Width / pitch);
			sizeAbs.Height = (float)pitch * (int)(sizeAbs.Height / pitch);

			switch (barState)
			{
				case BarStates.Drag:
					slideAnchor = SlideAnchors.Width;
					if (vertical)
						SectionStart += sizeRel.Height;
					else
						SectionStart += sizeRel.Width;
					Invalidate();
					InvokeZoom(ZoomEventArgs.ZoomEventTypes.Position);
					break;
				case BarStates.SizeStart:
					slideAnchor = SlideAnchors.End;
					if (vertical)
						SectionStart = sizeAbs.Height;
					else
						SectionStart = sizeAbs.Width;
					Invalidate();
					InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
					break;
				case BarStates.SizeEnd:
					if (vertical)
						SectionEnd = sizeAbs.Height;
					else
						SectionEnd = sizeAbs.Width;
					Invalidate();
					InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
					break;
			}

			slideAnchor = anchor;

			last.X = e.X;
			last.Y = e.Y;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			barState = BarStates.Normal;
			Invalidate();
		}
		#endregion

		#region 42
		public void ZoomMax()
		{
			sectionStart = 0;
			SectionWidth = regionWidth;

			InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
			Invalidate();
		}

		public void SetWidth(double w)
		{
			if (w == 0)
				return;

			sectionEnd = sectionStart + w;
			sectionWidth = w;
			UpdateRatio();
		}

		public void UpdateRatio()
		{
			if (sectionWidth == 0)
				return;            

			if (this.Width > 0)
				oldWidth = this.Width;

			ratio = (oldWidth + widthOffset) / sectionWidth;		
		}
		#endregion
	}
}
