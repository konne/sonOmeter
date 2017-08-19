
//    Copyright (C) 2004  Riccardo Marzi
//
//    Copyright (C) 2004  Sebastian Faltoni
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;
using sys = System;

namespace UKLib.Fireball.Collections.Generic
{
	public interface IKeyedCollection<T>
	{

		int Count { get;}

		T this[int index] { get;set;}

		T this[string key]{ get;set;}
		
		string[] Keys{ get;}

		void Add(string key, T item);

		void AddRange(string[] keys, T[] items);

		void Clear();

		void Insert(int index, string key, T item);

		void RemoveAt(int index);

		void Remove(string key);

		void Remove(T item);

		bool Contains(string key);

		bool Contains(T item);

		int IndexOf(string key);

		int IndexOf(T item);

		T[] GetItems();

		T[] GetItems(int startIndex);

		T[] GetItems(int startIndex, int finalIndex);

		void Sort();

		void Reverse();

		void Reverse(int index, int length);

		string CreateFreeKey();

		void Swap(int index1, int index2);
		void Swap(T item1, T item2);

		void Move(int index, int newIndex);
		void Move(T item, int newIndex);

        bool TryGetItem(int index, out T item);
        bool TryGetItem(string key, out T item);
	}
}
