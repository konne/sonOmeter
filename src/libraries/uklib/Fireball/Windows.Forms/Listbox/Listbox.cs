//Copyright (C) 2004/2005 Sebastian Faltoni
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  US

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UKLib.Fireball.Windows.Forms.Listbox
{
	public class ListBox : ScrollableControl
	{
		ListBoxControl m_ListBoxControl = null;
		Brush m_BackgroundBrush = null;

		public ListBox()
		{
			m_ListBoxControl = new ListBoxControl(this);
			this.Controls.Add(m_ListBoxControl);
			this.AutoScroll = true;
			this.AutoScrollMargin = new Size(10, 10);

			m_ListBoxControl.Size = new Size(100, 100);
		}

		public Brush BackgroundBrush
		{
			get
			{
				return m_BackgroundBrush;
			}
			set
			{
				m_BackgroundBrush = value;
			}
		}

		public ListBoxItemCollection Items
		{
			get
			{
				return m_ListBoxControl.Items;
			}
		}
	}

	internal class ListBoxControl : Control
	{
		ListBox m_Parent = null;
		ListBoxItemCollection m_Items = null;


		public ListBoxControl(ListBox parent)
		{
			this.SetStyle(ControlStyles.UserPaint
				| ControlStyles.ResizeRedraw
				| ControlStyles.AllPaintingInWmPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.SupportsTransparentBackColor
				, true
				);

			m_Parent = parent;

			m_Items = new ListBoxItemCollection();
			m_Items.ListBoxItemAdded += new ListBoxItemAddedHandler(m_Items_ListBoxItemAdded);
		}

		void m_Items_ListBoxItemAdded(ListBoxItemCollection sender, ListBoxItem item)
		{
			ResizeControl();
			this.Invalidate();
		}

		public ListBoxItemCollection Items
		{
			get
			{
				return m_Items;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (m_Parent.BackgroundBrush == null)
				e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
			else
			{
				Rectangle rectClient = this.ClientRectangle;
				rectClient.Width--;
				rectClient.Height--;
				e.Graphics.FillRectangle(m_Parent.BackgroundBrush, rectClient);
			}

			for (int i = 0; i < m_Items.Count;i++ )
			{
				ListBoxItem current = m_Items[i];

				current.Draw(e.Graphics);
			}
		}

		void ResizeControl()
		{
			for (int i = 0; i < m_Items.Count; i++)
			{
				ListBoxItem current = m_Items[i];

				Rectangle bounds = Rectangle.Empty;
				if (i == 0)
				{
					bounds = new Rectangle(0, 0, this.ClientSize.Width - 1, current.ItemHeight);
				}
				else
				{
					ListBoxItem previous = m_Items[i - 1];

					bounds = new Rectangle(0, previous.Bounds.Top + previous.Bounds.Height,
						100 - 1, current.ItemHeight);
				}

				current.SetBounds(bounds);
			}
			if (m_Items.Count == 0) return;

			ListBoxItem last = m_Items[m_Items.Count-1];

			this.Height = last.Bounds.Top + last.Bounds.Height;
			this.Width = last.Bounds.Width;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			ResizeControl();
		}
	}
}
