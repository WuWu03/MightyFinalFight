using System;
using System.Collections.Generic;

namespace GameFrameWork
{
    public class ReferenceCollection
    {

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

        public ReferenceCollection(Type type)
        {
            m_ReferenceType = type;
            m_QueueReference = new Queue<IReference>();
            m_UsingReferenceCount = 0;
            m_AcquireReferenceCount = 0;
            m_AddReferenceCount = 0;
            m_ReleaseReferenceCount = 0;
            m_RemoveReferenceCount = 0;
        }

        public T Acquire<T>(object[] args) where T : class, IReference, new()
        {
            if (!typeof(T).Equals(m_ReferenceType))
            {
                throw new Exception("Reference type is invalid.");
            }

            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;

            lock(m_QueueReference)
            {
                if(m_QueueReference.Count>0)
                {
                    return m_QueueReference.Dequeue() as T;
                }
            }

            return Activator.CreateInstance(typeof(T), args) as T;
        }

        public IReference Acquire(object[] args)
        {
            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;

            lock (m_QueueReference)
            {
                if (m_QueueReference.Count > 0)
                {
                    return m_QueueReference.Dequeue();
                }
            }

            return Activator.CreateInstance(m_ReferenceType, args) as IReference;
        }

        public void Release(IReference reference, bool strictCheck)
        {
            reference.Clear();

            lock (m_QueueReference)
            {
                if (strictCheck && m_QueueReference.Contains(reference))
                {
                    throw new Exception("The reference has been released.");
                }
                m_QueueReference.Enqueue(reference);
            }

            m_ReleaseReferenceCount++;
            m_UsingReferenceCount--;
        }

        public void Add<T>(int count) where T : class, IReference, new()
        {
            if (!typeof(T).Equals(m_ReferenceType))
            {
                throw new Exception("Reference type is invalid.");
            }

            m_AddReferenceCount += count;

            lock (m_QueueReference)
            {
                for (int i = 0; i < count; i++)
                {
                    m_QueueReference.Enqueue(new T());
                }
            }
        }

        public void Add(int count)
        {
            m_AddReferenceCount += count;

            lock (m_QueueReference)
            {
                for (int i = 0; i < count; i++)
                {
                    m_QueueReference.Enqueue(Activator.CreateInstance(m_ReferenceType) as IReference);
                }
            }
        }

        public void Remove(int count)
        {
            if(count > m_QueueReference.Count)
            {
                count = m_QueueReference.Count;
            }

            m_RemoveReferenceCount += count;

            lock (m_QueueReference)
            {
                for (int i = 0; i < count; i++)
                {
                    m_QueueReference.Dequeue();
                }
            }
        }

        public void RemoveAll()
        {
            m_RemoveReferenceCount += m_QueueReference.Count;

            lock (m_QueueReference)
            {
                m_QueueReference.Clear();
            }
        }

        private int m_UsingReferenceCount = 0;
        private int m_AcquireReferenceCount = 0;
        private int m_AddReferenceCount = 0;
        private int m_ReleaseReferenceCount = 0;
        private int m_RemoveReferenceCount = 0;
        private Queue<IReference> m_QueueReference = null;
        private Type m_ReferenceType = null;
    }
}
