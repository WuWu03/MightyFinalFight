using System;
using System.Collections.Generic;

namespace GameFrameWork.Utils
{
    /// <summary>
    /// This is a super light implementation of an array that
    /// behaves like a list, automatically allocating new memory
    /// when needed, but not releasing it to garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    public class SmallList<T>
    {
        /// <summary>
        /// The number of elements in the list
        /// </summary>
        public int count
        {
            get
            {
                return m_Count;
            }
        }

        /// <summary>
        /// Indexed access to the list items
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get
            {
                if (m_Datas is null)
                {
                    return default;
                }
                else
                {
                    return m_Datas[i];
                }
            }
            set
            {
                m_Datas[i] = value;
            }
        }

        /// <summary>
        /// Resizes the array when more memory is needed.
        /// </summary>
        private void ResizeArray()
        {
            T[] newDatas;

            if (m_Datas != null)
            {
                newDatas = new T[m_Datas.Length << 1];
            }
            else
            {
                newDatas = new T[64];
            }

            List<T> newDatas2 = new List<T>();
            if (m_Datas != null && m_Count > 0)
            {
                m_Datas.CopyTo(newDatas, 0);
            }

            m_Datas = newDatas;
        }

        /// <summary>
        /// Instead of releasing the memory to garbage collection,
        /// the list size is set back to zero
        /// </summary>
        public void Clear()
        {
            Array.Clear(m_Datas, 0, m_Count);
            m_Count = 0;
        }

        /// <summary>
        /// Returns the first element of the list
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            if (m_Datas == null || m_Count == 0)
            {
                return default;
            }

            return m_Datas[0];
        }

        /// <summary>
        /// Returns the last element of the list
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            if (m_Datas == null || m_Count == 0)
            {
                return default;
            }

            return m_Datas[m_Count - 1];
        }

        /// <summary>
        /// Adds a new element to the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (m_Datas == null || m_Count == m_Datas.Length)
            {
                ResizeArray();
            }

            m_Datas[m_Count] = item;
            m_Count++;
        }

        /// <summary>
        /// Adds a new element to the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void AddRange(T[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        /// <summary>
        /// Adds a new element to the start of the array, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void AddStart(T item)
        {
            Insert(0, item);
        }

        /// <summary>
        /// Inserts a new element to the array at the index specified, creating more
        /// memory if necessary
        /// </summary>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            if (m_Datas == null || m_Count == m_Datas.Length)
            {
                ResizeArray();
            }

            for (int i = m_Count; i > index; i--)
            {
                m_Datas[i] = m_Datas[i - 1];
            }

            m_Datas[index] = item;
            m_Count++;
        }

        /// <summary>
        /// Removes an item from the start of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveStart()
        {
            return RemoveAt(0);
        }

        /// <summary>
        /// Removes an item from the index of the data
        /// </summary>
        /// <returns></returns>
        public T RemoveAt(int index)
        {
            if (m_Datas != null && m_Count != 0)
            {
                T item = m_Datas[index];
                for (int i = index; i < m_Count - 1; i++)
                {
                    m_Datas[i] = m_Datas[i + 1];
                }

                m_Datas[m_Count] = default;
                m_Count--;
                return item;
            }

            return default;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(m_Datas, item);
        }

        /// <summary>
        /// Removes an item from the data
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Remove(T item)
        {
            return RemoveAt(IndexOf(item));
        }

        public T Remove(T item, out int itemIndex)
        {
            itemIndex = IndexOf(item);
            return RemoveAt(itemIndex);
        }
        /// <summary>
        /// Removes an item from the end of the data
        /// </summary>
        /// <returns></returns>
        public bool RemoveEnd()
        {
            if (m_Datas != null && m_Count != 0)
            {
                m_Datas[m_Count] = default;
                m_Count--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the data contains the item
        /// </summary>
        /// <param name="item">The item to compare</param>
        /// <returns>True if the item exists in teh data</returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public T[] ToArray()
        {
            T[] values = new T[m_Count];
            Array.Copy(m_Datas, values, m_Count);
            return values;
        }

        /// <summary>
        /// internal storage of list data
        /// </summary>
        private T[] m_Datas = null;
        private int m_Count = 0;
    }
}