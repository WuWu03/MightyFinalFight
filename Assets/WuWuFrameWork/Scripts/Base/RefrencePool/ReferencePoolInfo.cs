using System;
using System.Runtime.InteropServices;

namespace WuWuFramework
{
    /// <summary>
    /// 引用池信息
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct ReferencePoolInfo
    {
        /// <summary>
        /// 引用类型
        /// </summary>
        private readonly Type m_ReferenceType;
        /// <summary>
        /// 引用计数
        /// </summary>
        private readonly int m_UsingReferenceCount;
        /// <summary>
        /// 申请引用计数
        /// </summary>
        private readonly int m_AcquireReferenceCount;
        /// <summary>
        /// 添加引用计数
        /// </summary>
        private readonly int m_AddReferenceCount;
        /// <summary>
        /// 释放引用计数
        /// </summary>
        private readonly int m_ReleaseReferenceCount;
        /// <summary>
        /// 移除引用计数
        /// </summary>
        private readonly int m_RemoveReferenceCount;

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

        public int releaseReferenceCount
        {
            get
            {
                return m_ReleaseReferenceCount;
            }
        }

        public int removeReferenceCount
        {
            get
            {
                return m_RemoveReferenceCount;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="referenceType"></param>
        /// <param name="usingCount"></param>
        /// <param name="acquireCount"></param>
        /// <param name="addCount"></param>
        /// <param name="removeCount"></param>
        /// <param name="releaseCount"></param>
        public ReferencePoolInfo(Type referenceType,int usingCount,int acquireCount,int addCount,int removeCount,int releaseCount)
        {
            m_ReferenceType = referenceType;
            m_UsingReferenceCount = usingCount;
            m_AcquireReferenceCount = acquireCount;
            m_AddReferenceCount = addCount;
            m_RemoveReferenceCount = removeCount;
            m_ReleaseReferenceCount = releaseCount;
        }
    }
}
