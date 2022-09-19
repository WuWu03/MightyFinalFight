using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    public static class ReferencePool
    {
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

            lock(m_DicReferenceCollection)
            {
                foreach (KeyValuePair<Type,ReferenceCollection> kvp in m_DicReferenceCollection)
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

        public static void ClearAll()
        {
            lock(m_DicReferenceCollection)
            {
                foreach (KeyValuePair<Type,ReferenceCollection> kvp in m_DicReferenceCollection)
                {
                    kvp.Value.RemoveAll();
                }

                m_DicReferenceCollection.Clear();
            }
        }

        public static T Acquire<T>(params object[] args) where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>(args);
        }

        public static IReference Acquire(Type referenceType, params object[] args)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire(args);
        }

        public static void Release(IReference reference)
        {
            if(reference == null)
            {
                throw new Exception("Reference is invalid");
            }

            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference, m_EnableStrickCheck);
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

        public static void Remove(Type referenceType,int count)
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

        private static void InternalCheckReferenceType(Type referenceType)
        {
            if(!m_EnableStrickCheck)
            {
                return;
            }

            if(referenceType == null)
            {
                throw new Exception("Reference type is invalid.");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new Exception("Reference type is not a non-abstract class type.");
            }

            if(!referenceType.IsAssignableFrom(typeof(IReference)))
            {
                throw new Exception("Reference type is invalid.");
            }
        }

        private static ReferenceCollection GetReferenceCollection(Type type)
        {
            if(type == null)
            {
                throw new Exception("Reference type is invalid.");
            }

            ReferenceCollection referenceCollection = null;
            lock(m_DicReferenceCollection)
            {
                if(!m_DicReferenceCollection.TryGetValue(type,out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(type);
                    m_DicReferenceCollection.Add(type, referenceCollection);
                }
            }

            return referenceCollection;
        }

        private static bool m_EnableStrickCheck = false;
        private static Dictionary<Type, ReferenceCollection> m_DicReferenceCollection = new Dictionary<Type, ReferenceCollection>();
    }
}
