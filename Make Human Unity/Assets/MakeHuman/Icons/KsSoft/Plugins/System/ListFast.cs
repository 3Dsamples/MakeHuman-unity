//==============================================================================================
/*!高速なリスト.
	@file  ListFast	
*/
//==============================================================================================
using UnityEngine;
using System;
using System.Collections.Generic;
namespace KS {
    public class ListFast<T> {
        List<T> m_item;
        public ListFast(int maxsize) {
            m_item = new List<T>();
            m_item.Capacity = maxsize;
        }
        public T this[int index] {
            get {
                return m_item[index];
            }
            set {
                m_item[index] = value;
            }
        }
        public void clear() {
            m_item.Clear();
        }
        public void push_back(T item) {
            if (m_item.Count >= m_item.Capacity) {
                Debug.LogError("can,t add!!:" + item);
                return;
            }
            m_item.Add(item);
        }
        public int size {
            get {
                return m_item.Count;
            }
        }
        public int maxsize {
            get {
                return m_item.Capacity;
            }
        }
        public void sort(Comparison<T> comparer) {
            m_item.Sort(comparer);
        }

    }
}
