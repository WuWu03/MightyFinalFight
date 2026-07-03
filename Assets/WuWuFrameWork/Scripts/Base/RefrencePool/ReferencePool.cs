using System;
using System.Collections.Generic;

namespace WuWuFramework
{
    /// <summary>
    /// 引用池，用于管理引用类型对象的申请和释放，避免频繁的内存分配和垃圾回收，提高性能。
    /// </summary>
    public static class ReferencePool
    {
        /// <summary>
        /// 释放的引用类型集合（用于在 ReleaseAll 时移除空的引用类型集合）
        /// </summary>
        private static readonly List<Type> m_ReleasedCollection = new();
        /// <summary>
        /// 引用类型集合（用于存储不同类型的引用对象池）
        /// </summary>
        private static readonly Dictionary<Type, ReferenceCollection> m_ReferenceCollection = new();
        /// <summary>
        /// 是否启用严格检查（用于在 Release 时检查引用类型是否正确）
        /// </summary>
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
                return m_ReferenceCollection.Count;
            }
        }

        /// <summary>
        /// 获取所有引用类型信息
        /// </summary>
        /// <returns></returns>
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] referencePoolInfos = new ReferencePoolInfo[m_ReferenceCollection.Count];

            lock (m_ReferenceCollection)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_ReferenceCollection)
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

        /// <summary>
        /// 框架关闭时调用，清理所有引用类型集合
        /// </summary>
        public static void Shutdown()
        {
            lock (m_ReferenceCollection)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_ReferenceCollection)
                {
                    kvp.Value.RemoveAll();
                }

                m_ReferenceCollection.Clear();
            }
        }

        /// <summary>
        /// 申请一个引用类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 申请一个引用类型对象
        /// </summary>
        /// <param name="referenceType"></param>
        /// <returns></returns>
        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        /// <summary>
        /// 添加指定数量的引用类型对象到对象池中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        /// <summary>
        /// 添加指定数量的引用类型对象到对象池中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        /// <summary>
        /// 从对象池中移除指定数量的引用类型对象（彻底移除，不再使用）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public static void Remove<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        /// <summary>
        /// 从对象池中移除指定数量的引用类型对象（彻底移除，不再使用）
        /// </summary>
        /// <param name="referenceType"></param>
        /// <param name="count"></param>
        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        /// <summary>
        /// 移除所有引用类型对象（彻底移除，不再使用）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RemoveAll<T>() where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        /// <summary>
        /// 移除所有引用类型对象（彻底移除，不再使用）
        /// </summary>
        /// <param name="referenceType"></param>
        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        /// <summary>
        /// 释放一个引用类型对象，将其归还到对象池中
        /// </summary>
        /// <param name="reference"></param>
        /// <exception cref="WuWuFrameworkException"></exception>
        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw new WuWuFrameworkException("对象为空，无法回收");
            }

            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference, m_EnableStrickCheck);
        }

        /// <summary>
        /// 释放所有引用类型对象，将其归还到对象池中，并移除空的引用类型集合
        /// </summary>
        public static void ReleaseAll()
        {
            lock (m_ReferenceCollection)
            {
                m_ReleasedCollection.Clear();

                foreach (KeyValuePair<Type, ReferenceCollection> kvp in m_ReferenceCollection)
                {
                    if (kvp.Value.usingReferenceCount < 1)
                    {
                        kvp.Value.RemoveAll();
                        m_ReleasedCollection.Add(kvp.Key);
                    }
                }

                foreach (var releaseCollection in m_ReleasedCollection)
                {
                    m_ReferenceCollection.Remove(releaseCollection);
                }
            }
        }

        /// <summary>
        /// 检查引用类型是否正确（是否为类类型、是否实现了 IReference 接口）
        /// </summary>
        /// <param name="referenceType"></param>
        /// <exception cref="WuWuFrameworkException"></exception>
        private static void InternalCheckReferenceType(Type referenceType)
        {
            if (!m_EnableStrickCheck)
            {
                return;
            }

            if (referenceType == null)
            {
                throw new WuWuFrameworkException("引用类型为空");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new WuWuFrameworkException("引用类型错误");
            }

            if (!referenceType.IsAssignableFrom(typeof(IReference)))
            {
                throw new WuWuFrameworkException("未实现 [IRefenece] 接口");
            }
        }

        /// <summary>
        /// 获取指定类型的引用类型集合，如果不存在则创建一个新的引用类型集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="WuWuFrameworkException"></exception>
        private static ReferenceCollection GetReferenceCollection(Type type)
        {
            if (type == null)
            {
                throw new WuWuFrameworkException("引用类型为空.");
            }

            ReferenceCollection referenceCollection;
            
            lock (m_ReferenceCollection)
            {
                if (!m_ReferenceCollection.TryGetValue(type, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(type);
                    m_ReferenceCollection.Add(type, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }
}