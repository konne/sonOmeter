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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;

namespace UKLib.Fireball.Windows.Forms.Listbox
{
	public class ListBoxItem
	{
		const int DEFAULT_ICON_PADDING = 2;
		const int DEFAULT_TEXT_LEFT_PADDING = 2;

		string m_Text = null;
		Icon m_Icon;
		Image m_Image;
		Rectangle m_Bounds = Rectangle.Empty;
		int m_ItemHeight = 8;
		bool m_IsSelected = false;

		public ListBoxItem()
			: this(String.Empty)
		{

		}

		public ListBoxItem(string text)
		{
			this.Text = text;
		}

		public bool IsSelected
		{
			get
			{
				return m_IsSelected;
			}
			set
			{
				m_IsSelected = value;
			}
		}

		public int ItemHeight
		{
			get
			{
				return m_ItemHeight;
			}
			set
			{
				m_ItemHeight = value;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return m_Bounds;
			}
		}

		internal void SetBounds(Rectangle bounds)
		{
			m_Bounds = bounds;
		}

		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}


		public Image Image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image = value;
			}
		}


		public Icon Icon
		{
			get
			{
				return m_Icon;
			}
			set
			{
				m_Icon = value;
			}
		}

		internal void Draw(Graphics g)
		{
			OnDraw(g);
		}

		public virtual void OnDraw(Graphics gfx)
		{
			Rectangle currentIconRect = Rectangle.Empty;

			if (CanDrawIcon)
			{
				Rectangle iconRect = new Rectangle(DEFAULT_ICON_PADDING,DEFAULT_ICON_PADDING
					,m_Icon.Width,m_Icon.Height);

				gfx.DrawIcon(m_Icon, iconRect);

				currentIconRect = iconRect;
			}
			else if (CanDrawImage)
			{
				Rectangle imgRect = new Rectangle(DEFAULT_ICON_PADDING, DEFAULT_ICON_PADDING
					, m_Image.Width, m_Image.Height);

				gfx.DrawImage(m_Image, imgRect);

				currentIconRect = imgRect;
			}
			int left = DEFAULT_TEXT_LEFT_PADDING + currentIconRect.Left
							+ currentIconRect.Width;

			Rectangle textRect = new Rectangle(left, DEFAULT_TEXT_LEFT_PADDING, m_Bounds.Width - left,
				m_Bounds.Height - currentIconRect.Height);

			StringFormat format = new StringFormat();
			format.LineAlignment = StringAlignment.Center;

			if (this.Text != null)
				gfx.DrawString(this.Text, new Font("Tahoma", 10), Brushes.Black, textRect, format);

		}

		bool CanDrawIcon
		{
			get
			{
				return (m_Icon != null);
			}
		}

		bool CanDrawImage
		{
			get
			{
				return (m_Image != null);
			}
		}
	}
}
