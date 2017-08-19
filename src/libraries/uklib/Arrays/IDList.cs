using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace UKLib.Arrays
{
    public interface IHasID
    {
        int ID { get; set; }
    }

    public class IDBList<T> : BindingList<T> where T : IHasID, new()
    {
        public IDBList()
        {            
            this.ListChanged += new ListChangedEventHandler(IDBList_ListChanged);
        }

        void IDBList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {               
                this[e.NewIndex].ID = IDList<T>.FindMaxID(this, this[e.NewIndex]);
            }
        }
    }

    public class IDList<T> : List<T> where T : IHasID
    {
        public new void Add(T item)
        {
            UpdateID(item);            
            base.Add(item);
        }
       
        public static int FindMaxID (IList<T> list, T newItem)
        {
            int count = list.Count;
            int id = newItem.ID;
            List<int> idList = new List<int>();

            for (int i = 0; i < count; i++)
            {
                if (list[i].Equals(newItem))
                    continue;

                idList.Add(list[i].ID);
            }

            while (id <= count)
            {
                if (idList.Contains(id))
                    id++;
                else
                    break;
            }

            return  id;            
        }

        public void UpdateID(T item)
        {
            item.ID = FindMaxID(this, item);
        }

        public T GetByID(int id)
        {
            int count = this.Count;
            T item = default(T);

            for (int i = 0; i < count; i++)
            {
                if (this[i].ID == id)
                {
                    item = this[i];
                    break;
                }
            }

            return item;
        }
    }
}