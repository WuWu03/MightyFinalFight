using System;
using System.Collections.Generic;

namespace WuWuFramework
{
    public class ReferenceCollection
    {
        private readonly Queue<IReference> m_ReleasedReferences;
        private readonly Type m_ReferenceType;
        private int m_UsingReferenceCount;
        private int m_AcquireReferenceCount;
        private int m_AddReferenceCount;
        private int m_ReleaseReferenceCount;
        private int m_RemoveReferenceCount;
        
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
