using System;
using System.Runtime.InteropServices;

namespace GameFrameWork
{
    [StructLayout(LayoutKind.Auto)]
    public struct ReferencePoolInfo
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

        public ReferencePoolInfo(Type referenceType,int usingCount,int acquireCount,int addCount,int removeCount,int releaseCount)
        {
            m_ReferenceType = referenceType;
            m_UsingReferenceCount = usingCount;
            m_AcquireReferenceCount = acquireCount;
            m_AddReferenceCount = addCount;
            m_RemoveReferenceCount = removeCount;
            m_ReleaseReferenceCount = releaseCount;
        }

        private readonly Type m_ReferenceType;
        private readonly int m_UsingReferenceCount;
        private readonly int m_AcquireReferenceCount;
        private readonly int m_AddReferenceCount;
        private readonly int m_ReleaseReferenceCount;
        private readonly int m_RemoveReferenceCount;
    }
}
