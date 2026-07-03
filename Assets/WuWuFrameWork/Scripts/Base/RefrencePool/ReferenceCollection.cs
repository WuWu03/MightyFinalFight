using System;
using System.Collections.Generic;

namespace WuWuFramework
{
    /// <summary>
    /// 引用集合类，用于管理同一类型的引用对象
    /// </summary>
    public class ReferenceCollection
    {
        /// <summary>
        /// 闲置对象队列
        /// </summary>
        private readonly Queue<IReference> m_ReleasedReferences;
        /// <summary>
        /// 对象类型
        /// </summary>
        private readonly Type m_ReferenceType;
        /// <summary>
        /// 引用计数
        /// </summary>
        private int m_UsingReferenceCount;
        /// <summary>
        /// 已经申请的对象数量
        /// </summary>
        private int m_AcquireReferenceCount;
        /// <summary>
        /// 已经添加的闲置对象数量
        /// </summary>
        private int m_AddReferenceCount;
        /// <summary>
        /// 已经释放的对象数量
        /// </summary>
        private int m_ReleaseReferenceCount;
        /// <summary>
        /// 已经移除的闲置对象数量
        /// </summary>
        private int m_RemoveReferenceCount;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        public ReferenceCollection(Type type)
        {
            m_ReferenceType = type;
            m_ReleasedReferences = new Queue<IReference>();
            m_UsingReferenceCount = 0;
            m_AcquireReferenceCount = 0;
            m_AddReferenceCount = 0;
            m_ReleaseReferenceCount = 0;
            m_RemoveReferenceCount = 0;
        }
        
        public Type referenceType
        {
            get
            {
                return m_ReferenceType;
            }
        }

        public int usingReferenceCount
        {
            get
            {
                return m_UsingReferenceCount;
            }
        }
        
        public int acquireReferenceCount
        {
            get
            {
                return m_AcquireReferenceCount;
            }
        }

        public int addReferenceCount
        {
            get
            {
                return m_AddReferenceCount;
            }
        }

        public int removeReferenceCount
        {
            get
            {
                return m_RemoveReferenceCount;
            }
        }

        public int releaseReferenceCount
        {
            get
            {
                return m_ReleaseReferenceCount;
            }
        }

        /// <summary>
        /// 申请对象，如果有闲置对象则返回闲置对象，否则创建新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        public T Acquire<T>() where T : class, IReference, new()
        {
            if (typeof(T) != m_ReferenceType)
            {
                throw new WuWuFrameworkException("创建对象的类型错误，请检查");
            }

            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;

            lock(m_ReleasedReferences)
            {
                if(m_ReleasedReferences.Count>0)
                {
                    return m_ReleasedReferences.Dequeue() as T;
                }
            }

            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// 申请对象，如果有闲置对象则返回闲置对象，否则创建新的对象
        /// </summary>
        /// <returns></returns>
        public IReference Acquire()
        {
            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;

            lock (m_ReleasedReferences)
            {
                if (m_ReleasedReferences.Count > 0)
                {
                    return m_ReleasedReferences.Dequeue();
                }
            }

            return Activator.CreateInstance(m_ReferenceType) as IReference;
        }

        /// <summary>
        /// 释放对象，释放后对象会被放入闲置队列中
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="strictCheck"></param>
        /// <exception cref="WuWuFrameworkException"></exception>
        public void Release(IReference reference, bool strictCheck)
        {
            reference.Clear();

            lock (m_ReleasedReferences)
            {
                if (strictCheck && m_ReleasedReferences.Contains(reference))
                {
                    throw new WuWuFrameworkException("实例已经被释放");
                }
                
                m_ReleasedReferences.Enqueue(reference);
            }

            m_ReleaseReferenceCount++;
            m_UsingReferenceCount--;
        }

        /// <summary>
        /// 创建指定数量的对象，并放入闲置队列中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <exception cref="WuWuFrameworkException"></exception>
        public void Add<T>(int count) where T : class, IReference, new()
        {
            if (typeof(T) != m_ReferenceType)
            {
                throw new WuWuFrameworkException("创建对象失败，类型错误");
            }

            m_AddReferenceCount += count;

            lock (m_ReleasedReferences)
            {
                for (int i = 0; i < count; i++)
                {
                    m_ReleasedReferences.Enqueue(new T());
                }
            }
        }

        /// <summary>
        /// 创建指定数量的对象，并放入闲置队列中
        /// </summary>
        /// <param name="count"></param>
        public void Add(int count)
        {
            m_AddReferenceCount += count;

            lock (m_ReleasedReferences)
            {
                for (int i = 0; i < count; i++)
                {
                    m_ReleasedReferences.Enqueue(Activator.CreateInstance(m_ReferenceType) as IReference);
                }
            }
        }

        /// <summary>
        /// 移除指定数量的闲置对象（彻底移除，不会再被使用）
        /// </summary>
        /// <param name="count"></param>
        public void Remove(int count)
        {
            lock (m_ReleasedReferences)
            {
                if(count > m_ReleasedReferences.Count)
                {
                    count = m_ReleasedReferences.Count;
                }
                
                m_RemoveReferenceCount += count;
                
                for (int i = 0; i < count; i++)
                {
                    m_ReleasedReferences.Dequeue();
                }
            }
        }

        /// <summary>
        /// 移除所有的闲置对象（彻底移除，不会再被使用）
        /// </summary>
        public void RemoveAll()
        {
            lock (m_ReleasedReferences)
            {
                m_RemoveReferenceCount += m_ReleasedReferences.Count;
                m_ReleasedReferences.Clear();
            }
        }
    }
}
