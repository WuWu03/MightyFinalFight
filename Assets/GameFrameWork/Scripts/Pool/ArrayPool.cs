using System.Collections.Generic;

namespace GameFrameWork.Pool
{
    public class ArrayPool<T> : Singleton<ArrayPool<T>> where T : struct
    {
        public ArrayPool() 
        {
            m_ArrayPools = new();
        }

        public T[] Get(int length)
        {
            if (!m_ArrayPools.TryGetValue(length, out Queue<T[]> pool))
            {
                pool = new Queue<T[]>();
                m_ArrayPools.Add(length, pool);
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
                Log.LogError("数组为空，无法进行回收");
                return;
            }

            if (!m_ArrayPools.TryGetValue(array.Length, out Queue<T[]> pool))
            {
                pool = new Queue<T[]>();
                m_ArrayPools.Add(array.Length, pool);
            }

            pool.Enqueue(array);
        }

        public void Release()
        {
            m_ArrayPools.Clear();
        }

        protected override void OnDispose()
        {
            Release();
            m_ArrayPools = null;
        }

        private Dictionary<int, Queue<T[]>> m_ArrayPools = null;
    }
}