using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace UKLib.Controls
{
	public partial class RelScrollBar : Panel
	{
		#region Constructor
		public RelScrollBar()
		{
			this.Width = 20;
			this.Height = 100;

			InitializeComponent();

			this.DoubleBuffered = true;
			timer.Interval = 50;
			timer.Enabled = false;
			timer.Tick += new EventHandler(timer_Tick);
		}
		#endregion

		#region Variables
		private Color baseColor = Color.FromKnownColor(KnownColor.MenuBar);
		private Color arrowColor = Color.FromKnownColor(KnownColor.ControlText);

		private int sliderPos = 0;
		private int edgeHandleSize = 30;
		private int sliderSize = 80;

		private float range = 100F;
		private float value = 0F;
		private float speed = 0.05F;

		private bool drawEdgeHandles = true;
		private bool roundSlider = true;
		private bool draw3D = true;

		private Point lastPos;

		private RectangleF rcSlider = new RectangleF();
		private Rectangle rcEdgeHandleA = new Rectangle();
		private Rectangle rcEdgeHandleB = new Rectangle();

		private Timer timer = new Timer();
		#endregion

		#region Properties
		/// <summary>
		/// The base color.
		/// Further color shades will be derived from this one.
		/// </summary>
		[Description("The base color."), Category("Relative Scroll Bar"), DefaultValue(KnownColor.MenuBar)]
		public Color BaseColor
		{
			get { return baseColor; }
			set { baseColor = value; Invalidate(); }
		}

		/// The color of the arrows and thin borders.
		/// </summary>
		[Description("The color of the arrows and thin borders."), Category("Relative Scroll Bar"), DefaultValue(KnownColor.ControlText)]
		public Color ArrowColor
		{
			get { return arrowColor; }
			set { arrowColor = value; Invalidate(); }
		}

		/// <summary>
		/// The size of the handles on the control's edges.
		/// </summary>
		[Description("The Size of the handles on the control's edges."), Category("Relative Scroll Bar"), DefaultValue(30)]
		public int EdgeHandleSize
		{
			get { return edgeHandleSize; }
			set { edgeHandleSize = value; Invalidate(); }
		}

		/// <summary>
		/// The size of the central slider.
		/// </summary>
		[Description("The size of the central slider."), Category("Relative Scroll Bar"), DefaultValue(80)]
		public int SliderSize
		{
			get { return sliderSize; }
			set { sliderSize = value; Invalidate(); }
		}

		/// <summary>
		/// Toggle the edge handles.
		/// </summary>
		[Description("Toggle the edge handles."), Category("Relative Scroll Bar"), DefaultValue(true)]
		public bool DrawEdgeHandles
		{
			get { return drawEdgeHandles; }
			set { drawEdgeHandles = value; Invalidate(); }
		}

		/// <summary>
		/// Toggle the 3D mode.
		/// </summary>
		[Description("Toggle the 3D mode."), Category("Relative Scroll Bar"), DefaultValue(true)]
		public bool Draw3D
		{
			get { return draw3D; }
			set { draw3D = value; Invalidate(); }
		}

		/// <summary>
		/// Toggle the drawing mode of the central slider.
		/// </summary>
		[Description("Toggle the drawing mode of the central slider."), Category("Relative Scroll Bar"), DefaultValue(true)]
		public bool RoundSlider
		{
			get { return roundSlider; }
			set { roundSlider = value; Invalidate(); }
		}

		/// <summary>
		/// The value range covered by the scroll bar.
		/// </summary>
		[Description("The value range covered by the scroll bar."), Category("Relative Scroll Bar"), DefaultValue(100F)]
		public float Range
		{
			get { return range; }
			set { range = value; Invalidate(); }
		}

		/// <summary>
		/// The value of the scroll bar.
		/// </summary>
		[Description("The value of the scroll bar."), Category("Relative Scroll Bar"), DefaultValue(0F)]
		public float Value
		{
			get { return value; }
			set
			{
				float old = this.value;
				this.value = value;

				ScrollOrientation orientation;

				if (this.Width > this.Height)
					orientation = ScrollOrientation.HorizontalScroll;
				else
					orientation = ScrollOrientation.VerticalScroll;

				if (ScrollEx != null)
					ScrollEx(this, new ScrollEventArgsEx(old, value, orientation));

				Invalidate();
			}
		}

		/// <summary>
		/// The scroll speed of the slider.
		/// </summary>
		[Description("The scroll speed of the slider."), Category("Relative Scroll Bar"), DefaultValue(0.05F)]
		public float Speed
		{
			get { return speed; }
			set { speed = value; Invalidate(); }
		}
		#endregion

		#region ScrollEx event
		public event ScrollEventHandlerEx ScrollEx;
		#endregion

		#region Mouse interaction
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			// Store mouse position.
			lastPos = e.Location;

			if (rcSlider.Contains(lastPos))
			{
				timer.Enabled = true;
			}

			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.Button == MouseButtons.Left)
			{
				// Get delta.
				Rectangle rc;
				int w = this.Width;
				int h = this.Height;

				int border = 0;
				int delta = 0;

				if (w > h)
				{
					// horizontal
					border = ((w - sliderSize) >> 1) - 3;					
					delta = e.X - lastPos.X;
					rc = new Rectangle(border, 0, sliderSize, h);
				}
				else
				{
					// vertical
					border = ((h - sliderSize) >> 1) - 3;
					delta = e.Y - lastPos.Y;
					rc = new Rectangle(0, border, w, sliderSize);
				}

				if (drawEdgeHandles)
					border -= edgeHandleSize;

				if (rc.Contains(lastPos))
				{
					//if (!timer.Enabled)
					//	timer.Enabled = true;

                    sliderPos = System.Math.Min(System.Math.Max(delta, -border), border);
					Invalidate();
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			sliderPos = 0;
			timer.Enabled = false;

			Invalidate();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (drawEdgeHandles && (e.Button == MouseButtons.Left))
			{
				if (rcEdgeHandleA.Contains(e.Location))
				{
					// Handle A (up / left)
                    this.Value = System.Math.Max(value - 1, 0);
				}
				else if (rcEdgeHandleB.Contains(e.Location))
				{
					// Handle B (down / right)
                    this.Value = System.Math.Min(value + 1, range);
				}
			}
		}

		void timer_Tick(object sender, EventArgs e)
		{
			float length = (this.Width > this.Height) ? this.Width : this.Height;

            this.Value = System.Math.Max(System.Math.Min(value + (float)sliderPos / length * range * speed, range), 0);
		} 
		#endregion

		#region Resizing
		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
		}
		#endregion

		#region Paint
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Overridden...
			// base.OnPaintBackground(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			#region Variables
			Graphics g = e.Graphics;

			float w = this.Width;
			float mw = w / 2.0F;
			float h = this.Height;
			float mh = h / 2.0F;
			float x1 = 0;
			float x2 = 0;
			float y1 = 0;
			float y2 = 0;

			Color llBaseColor = Color.FromArgb(System.Math.Min(baseColor.R + 40, 255), System.Math.Min(baseColor.G + 40, 255), System.Math.Min(baseColor.B + 40, 255));
			Color lBaseColor = Color.FromArgb(System.Math.Min(baseColor.R + 20, 255), System.Math.Min(baseColor.G + 20, 255), System.Math.Min(baseColor.B + 20, 255));
			Color dBaseColor = Color.FromArgb(System.Math.Max(baseColor.R - 20, 0), System.Math.Max(baseColor.G - 20, 0), System.Math.Max(baseColor.B - 20, 0));
			Color ddBaseColor = Color.FromArgb(System.Math.Max(baseColor.R - 40, 0), System.Math.Max(baseColor.G - 40, 0), System.Math.Max(baseColor.B - 40, 0));
			Color interpolateColor = Color.FromArgb((arrowColor.R + dBaseColor.R) >> 1, (arrowColor.G + dBaseColor.G) >> 1, (arrowColor.B + dBaseColor.B) >> 1);
				
			Brush backBrush = new SolidBrush(baseColor);
			Brush ctrlBrush = new SolidBrush(dBaseColor);
			Brush ctrlBrushDim = new SolidBrush(ddBaseColor);
			Brush dimBrush = new SolidBrush(interpolateColor);

			Pen forePen = new Pen(arrowColor, 1);
			Pen thickPen = new Pen(arrowColor, 3);
			Pen thickDimPen = new Pen(interpolateColor, 3);

			Rectangle rcCircle;

			GraphicsPath sliderPath = new GraphicsPath();
			GraphicsPath arrowPathSA = new GraphicsPath(); // Slider arrow (l / t)
			GraphicsPath arrowPathSB = new GraphicsPath(); // Slider arrow (r / b)
			GraphicsPath arrowPathMA = new GraphicsPath(); // Slider movement arrow (l / t)
			GraphicsPath arrowPathMB = new GraphicsPath(); // Slider movement arrow (r / b)
			GraphicsPath arrowPathEA = new GraphicsPath(); // Edge handle arrow (l / t)
			GraphicsPath arrowPathEB = new GraphicsPath(); // Edge handle arrow (r / b)
			#endregion

			#region Create the drawing regions
			if (w > h)
			{
				// horizonzal
				rcEdgeHandleA = new Rectangle(0, 0, edgeHandleSize, (int)h - 1);
				rcEdgeHandleB = new Rectangle((int)w - edgeHandleSize - 1, 0, edgeHandleSize, (int)h - 1);

				rcCircle = new Rectangle((int)mw + sliderPos - 5, (int)mh - 5, 10, 10);

				x1 = mw + sliderPos - sliderSize / 2.0F;
				x2 = mw + sliderPos + sliderSize / 2.0F - 1;

				if (roundSlider)
				{
					sliderPath.AddArc(x1, 0, h, h - 1, 90, 180); 
					sliderPath.AddLine(x1 + mh, 0, x2 - mh, 0);
					sliderPath.AddArc(x2 - h, 0, h, h - 1, 270, 180);
					sliderPath.CloseFigure();
				}
				else
				{
					sliderPath.AddRectangle(new RectangleF(x1, 0, sliderSize, h));
				}

				x1 += 25;
				x2 -= 25;

				CreateArrow(mh, x1, x2, arrowPathSA, arrowPathSB, ScrollOrientation.HorizontalScroll);

				x1 -= 7.5F;
				x2 += 7.5F;

				CreateArrow(mh, x1, x2, arrowPathMA, arrowPathMB, ScrollOrientation.HorizontalScroll);

				x1 = 18;
				x2 = w - 18 - 1;

				CreateArrow(mh, x1, x2, arrowPathEA, arrowPathEB, ScrollOrientation.HorizontalScroll);

				if (draw3D)
				{
					backBrush = new LinearGradientBrush(this.ClientRectangle, baseColor, lBaseColor, LinearGradientMode.Vertical);
					ctrlBrush = new LinearGradientBrush(this.ClientRectangle, dBaseColor, ddBaseColor, LinearGradientMode.Vertical);
					ctrlBrushDim = new LinearGradientBrush(this.ClientRectangle, ddBaseColor, baseColor, LinearGradientMode.Vertical);
					dimBrush = new LinearGradientBrush(rcCircle, ddBaseColor, baseColor, LinearGradientMode.Vertical);
				}
			}
			else
			{
				// vertical
				rcEdgeHandleA = new Rectangle(0, 0, (int)w - 1, edgeHandleSize);
				rcEdgeHandleB = new Rectangle(0, (int)h - edgeHandleSize - 1, (int)w - 1, edgeHandleSize);

				rcCircle = new Rectangle((int)mw - 5, (int)mh + sliderPos - 5, 10, 10);

				y1 = mh + sliderPos - sliderSize / 2.0F;
				y2 = mh + sliderPos + sliderSize / 2.0F - 1;

				if (roundSlider)
				{
					sliderPath.AddArc(0, y2 - w, w - 1, w, 0, 180);
					sliderPath.AddLine(0, y2 - mw, 0, y1 + mw);
					sliderPath.AddArc(0, y1, w - 1, w, 180, 180);
					sliderPath.CloseFigure();
				}
				else
				{
					sliderPath.AddRectangle(new RectangleF(0, y1, w, sliderSize));
				}

				y1 += 25;
				y2 -= 25;

				CreateArrow(mw, y1, y2, arrowPathSA, arrowPathSB, ScrollOrientation.VerticalScroll);

				y1 -= 7.5F;
				y2 += 7.5F;

				CreateArrow(mw, y1, y2, arrowPathMA, arrowPathMB, ScrollOrientation.VerticalScroll);

				y1 = 18;
				y2 = h - 18 - 1;

				CreateArrow(mw, y1, y2, arrowPathEA, arrowPathEB, ScrollOrientation.VerticalScroll);

				if (draw3D)
				{
					backBrush = new LinearGradientBrush(this.ClientRectangle, baseColor, lBaseColor, LinearGradientMode.Horizontal);
					ctrlBrush = new LinearGradientBrush(this.ClientRectangle, dBaseColor, ddBaseColor, LinearGradientMode.Horizontal);
					ctrlBrushDim = new LinearGradientBrush(this.ClientRectangle, ddBaseColor, baseColor, LinearGradientMode.Horizontal);
					dimBrush = new LinearGradientBrush(rcCircle, ddBaseColor, baseColor, LinearGradientMode.Horizontal);
				}
			}

			rcSlider = sliderPath.GetBounds();
			#endregion

			#region Background
			g.FillRectangle(backBrush, this.ClientRectangle);
			#endregion

			#region Draw
            g.SmoothingMode = SmoothingMode.AntiAlias;

			if (drawEdgeHandles)
			{
				if ((MouseButtons == MouseButtons.Left) & (rcEdgeHandleA.Contains(lastPos)))
					g.FillRectangle(ctrlBrushDim, rcEdgeHandleA);
				else
					g.FillRectangle(ctrlBrush, rcEdgeHandleA);

				if ((MouseButtons == MouseButtons.Left) & (rcEdgeHandleB.Contains(lastPos)))
					g.FillRectangle(ctrlBrushDim, rcEdgeHandleB);
				else
					g.FillRectangle(ctrlBrush, rcEdgeHandleB);

				g.DrawRectangle(forePen, rcEdgeHandleA);
				g.DrawRectangle(forePen, rcEdgeHandleB);

				if (value == 0)
					g.DrawPath(thickDimPen, arrowPathEA);
				else
					g.DrawPath(thickPen, arrowPathEA);

				if (value == range)
					g.DrawPath(thickDimPen, arrowPathEB);
				else
					g.DrawPath(thickPen, arrowPathEB);
			}

			g.FillPath(ctrlBrush, sliderPath);
			g.DrawPath(forePen, sliderPath);
			g.DrawRectangle(forePen, 0, 0, w - 1, h - 1);

			if (value == 0)
				g.DrawPath(thickDimPen, arrowPathSA);
			else
				g.DrawPath(thickPen, arrowPathSA);
			
			if (value == range)
				g.DrawPath(thickDimPen, arrowPathSB);
			else
				g.DrawPath(thickPen, arrowPathSB);

			if (timer.Enabled)
			{
				g.FillEllipse(dimBrush, rcCircle);

				if ((value > 0) & (sliderPos < 0))
					g.DrawPath(thickPen, arrowPathMA);
				if ((value < range) & (sliderPos > 0))
					g.DrawPath(thickPen, arrowPathMB);
			}

			//g.DrawString(value.ToString("F"), this.Font, forePen.Brush, sliderPath.GetBounds());
			#endregion
		}

		private static void CreateArrow(float half, float a, float b, GraphicsPath arrowPathA, GraphicsPath arrowPathB, ScrollOrientation orientation)
		{
			if (orientation == ScrollOrientation.HorizontalScroll)
			{
				arrowPathA.AddLine(a, half - 5, a - 5, half);
				arrowPathA.AddLine(a - 5, half, a, half + 5);

				arrowPathB.AddLine(b, half - 5, b + 5, half);
				arrowPathB.AddLine(b + 5, half, b, half + 5);
			}
			else
			{
				arrowPathA.AddLine(half - 5, a, half, a - 5);
				arrowPathA.AddLine(half, a - 5, half + 5, a);

				arrowPathB.AddLine(half - 5, b, half, b + 5);
				arrowPathB.AddLine(half, b + 5, half + 5, b);
			}
		}
		#endregion
	}

	public class ScrollEventArgsEx : EventArgs
	{
		#region Variables
		private float oldValue;
		private float newValue;
		private ScrollOrientation orientation; 
		#endregion

		#region Properties
		public float OldValue
		{
			get { return oldValue; }
			set { oldValue = value; }
		}

		public float NewValue
		{
			get { return newValue; }
			set { newValue = value; }
		}

		public ScrollOrientation Orientation
		{
			get { return orientation; }
			set { orientation = value; }
		} 
		#endregion

		#region Constructor
		public ScrollEventArgsEx(float oldValue, float newValue, ScrollOrientation orientation)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.orientation = orientation;
		} 
		#endregion
	}

	public delegate void ScrollEventHandlerEx(object sender, ScrollEventArgsEx e);
}
