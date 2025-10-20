using System;
using System.Collections.Generic;

namespace GameFrameWork.Pool
{
    public class ArrayPool<T> : Singleton<ArrayPool<T>>
    {
        private readonly Dictionary<int, Queue<T[]>> m_Pools;
        public ArrayPool() 
        {
            m_Pools = new();
        }

        public T[] Get(int length)
        {
            if (!m_Pools.TryGetValue(length, out Queue<T[]> pool))
            {
                pool = new Queue<T[]>();
                m_Pools.Add(length, pool);
            }

            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }

            return new T[length];
        }

        public void Put(T[] array)
        {
            if(array == null)
            {
                throw new Exception("数组为空，无法进行回收");
            }

            if (!m_Pools.TryGetValue(array.Length, out Queue<T[]> pool))
            {
                pool = new Queue<T[]>();
                m_Pools.Add(array.Length, pool);
            }

            pool.Enqueue(array);
        }
        
        protected override void OnDispose()
        {
            m_Pools.Clear();
        }
    }
}