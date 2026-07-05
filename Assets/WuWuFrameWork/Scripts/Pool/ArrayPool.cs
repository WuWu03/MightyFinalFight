using System;
using System.Collections.Generic;

namespace WuWuFramework.Pool
{
    /// <summary>
    /// 数组池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayPool<T> : Singleton<ArrayPool<T>>
    {
        private readonly Dictionary<int, Queue<T[]>> m_Pools;
        public ArrayPool() 
        {
            m_Pools = new();
        }

        /// <summary>
        /// 获取指定长度的数组
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 回收数组
        /// </summary>
        /// <param name="array"></param>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 框架关闭时清空数组池
        /// </summary>
        public override void Shutdown()
        {
            m_Pools.Clear();
        }
    }
}