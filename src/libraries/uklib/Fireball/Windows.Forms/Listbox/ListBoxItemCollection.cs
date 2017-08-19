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
using System.Collections.Generic;
using System.Text;
using UKLib.Fireball.Collections.Generic;


namespace UKLib.Fireball.Windows.Forms.Listbox
{
	public delegate void ListBoxItemAddedHandler(ListBoxItemCollection sender, ListBoxItem item);
	public delegate void ListBoxItemInsertHandler(ListBoxItemCollection sender, ListBoxItem item,int index);
	public delegate void ListBoxItemRemoveHandler(ListBoxItemCollection sender, ListBoxItem item);

	public class ListBoxItemCollection:ILightCollection<ListBoxItem>
	{
		LightCollection<ListBoxItem> m_Coll = null;

		public event ListBoxItemAddedHandler ListBoxItemAdded;
		public event ListBoxItemInsertHandler ListBoxItemInsert;
		public event ListBoxItemRemoveHandler ListBoxItemRemove;

		public ListBoxItemCollection()
		{
			m_Coll = new LightCollection<ListBoxItem>();
		}

		#region ILightCollection<ListBoxItem> Members

		public int Count
		{
			get {
				return m_Coll.Count;
			}
		}

		public ListBoxItem this[int index]
		{
			get
			{
				return m_Coll[index];
			}
			set
			{
				m_Coll[index] = value;
			}
		}

		public int Add(ListBoxItem item)
		{
			int i = m_Coll.Add(item);

			if (ListBoxItemAdded != null)
				ListBoxItemAdded(this, item);

			return i;
		}

		public void AddRange(ListBoxItem[] items)
		{
			m_Coll.AddRange(items);
		}

		public void Clear()
		{
			m_Coll.Clear();
		}

		public bool Contains(ListBoxItem item)
		{
			return m_Coll.Contains(item);
		}

		public void Insert(int index, ListBoxItem item)
		{
			m_Coll.Insert(index, item);
			if (ListBoxItemInsert != null)
				ListBoxItemInsert(this, item, index);
		}

		public bool Remove(ListBoxItem item)
		{
			bool ret = m_Coll.Remove(item);

			if (ListBoxItemRemove != null)
				ListBoxItemRemove(this, item);

			return ret;
		}

		public void RemoveAt(int index)
		{
			m_Coll.RemoveAt(index);
		}

		public ListBoxItem Find(Predicate<ListBoxItem> match)
		{
			return m_Coll.Find(match);
		}

		public int IndexOf(ListBoxItem item)
		{
			return m_Coll.IndexOf(item);
		}

		public int IndexOf(ListBoxItem item, int index)
		{
			return m_Coll.IndexOf(item, index);
		}

		public int IndexOf(ListBoxItem item, int index, int count)
		{
			return m_Coll.IndexOf(item, index,count);
		}

		public ListBoxItem[] GetItems()
		{
			return m_Coll.GetItems();
		}

		public ListBoxItem[] GetItems(int startIndex)
		{
			return m_Coll.GetItems(startIndex);
		}

		public ListBoxItem[] GetItems(int startIndex, int finalIndex)
		{
			return m_Coll.GetItems(startIndex,finalIndex);
		}

		public IEnumerator<ListBoxItem> GetEnumerator()
		{
			return m_Coll.GetEnumerator();
		}

		#endregion

		#region ILightCollection<ListBoxItem> Members

		public void CopyTo(Array array, int index)
		{
			m_Coll.CopyTo(array, index);
		}

		#endregion

        #region ILightCollection<ListBoxItem> Members

        public void Move(ListBoxItem item, int newIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Move(int index, int newIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Reverse(int index, int length)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Reverse()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Swap(ListBoxItem item1, ListBoxItem item2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Swap(int index1, int index2)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ILightCollection<ListBoxItem> Members

        public bool TryGetItem(int index, out ListBoxItem item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
}
}
