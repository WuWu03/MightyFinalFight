using System;
using System.Collections.Generic;

namespace GameFrameWork
{
    public static class ReferencePool
    {
        private static readonly List<Type> m_ReleasedCollection = new();
        private static readonly Dictionary<Type, ReferenceCollection> m_DicReferenceCollection = new();
        private static bool m_EnableStrickCheck;
        
        public static bool enableStrickCheck
        {
            get
            {
                return m_EnableStrickCheck;
            }
            set
            {
                m_EnableStrickCheck = value;
            }
        }

        public static int count
        {
            get
            {
                return m_DicReferenceCollection.Count;
            }
        }

        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] referencePoolInfos = new ReferencePoolInfo[m_DicReferenceCollection.Count];

            lock (m_DicReferenceCollection)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_DicReferenceCollection)
                {
                    Type type = kvp.Value.referenceType;
                    int usingCount = kvp.Value.usingReferenceCount;
                    int acquireCount = kvp.Value.acquireReferenceCount;
                    int addCount = kvp.Value.addReferenceCount;
                    int releaseCount = kvp.Value.releaseReferenceCount;
                    int removeCount = kvp.Value.removeReferenceCount;

                    referencePoolInfos[index] = new ReferencePoolInfo(type, usingCount, acquireCount, addCount, releaseCount, removeCount);
                    index++;
                }
            }

            return referencePoolInfos;
        }


        public static void Shutdown()
        {
            lock (m_DicReferenceCollection)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_DicReferenceCollection)
                {
                    kvp.Value.RemoveAll();
                }

                m_DicReferenceCollection.Clear();
            }
        }

        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        public static void Remove<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        public static void RemoveAll<T>() where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw new GameFrameWorkException("对象为空，无法回收");
            }

            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference, m_EnableStrickCheck);
        }

        public static void ReleaseAll()
        {
            lock (m_DicReferenceCollection)
            {
                m_ReleasedCollection.Clear();

                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_DicReferenceCollection)
                {
                    if (kvp.Value.usingReferenceCount < 1)
                    {
                        kvp.Value.RemoveAll();
                        m_ReleasedCollection.Add(kvp.Key);
                    }
                }

                foreach (var releaseCollection in m_ReleasedCollection)
                {
                    m_DicReferenceCollection.Remove(releaseCollection);
                }
            }
        }

        private static void InternalCheckReferenceType(Type referenceType)
        {
            if (!m_EnableStrickCheck)
            {
                return;
            }

            if (referenceType == null)
            {
                throw new GameFrameWorkException("引用类型为空");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new GameFrameWorkException("引用类型错误");
            }

            if (!referenceType.IsAssignableFrom(typeof(IReference)))
            {
                throw new GameFrameWorkException("未实现 [IRefenece] 接口");
            }
        }

        private static ReferenceCollection GetReferenceCollection(Type type)
        {
            if (type == null)
            {
                throw new GameFrameWorkException("引用类型为空.");
            }

            ReferenceCollection referenceCollection;
            
            lock (m_DicReferenceCollection)
            {
                if (!m_DicReferenceCollection.TryGetValue(type, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(type);
                    m_DicReferenceCollection.Add(type, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }
}