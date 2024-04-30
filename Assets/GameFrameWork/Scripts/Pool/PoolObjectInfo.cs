using UnityEngine;

namespace GameFrameWork.Pool
{
    public class PoolObjectInfo : IReference
    {
        public UnityEngine.Object poolObject;
        public float releaseTime;
        public bool isReleaseImmediate;
        public string assetPath;
        public int referenceCount;

        public static PoolObjectInfo Create(UnityEngine.Object poolObject, float releaseTime, bool isReleaseImmediate, string assetPath)
        {
            PoolObjectInfo resourcePoolInfo = ReferencePool.Acquire<PoolObjectInfo>();
            resourcePoolInfo.poolObject = poolObject;
            resourcePoolInfo.releaseTime = releaseTime;
            resourcePoolInfo.isReleaseImmediate = isReleaseImmediate;
            resourcePoolInfo.assetPath = assetPath;
            resourcePoolInfo.referenceCount = 0;
            return resourcePoolInfo;
        }

        public PoolObjectInfo()
        {

        }

        public void Clear()
        {
            poolObject = null;
            releaseTime = 0;
            assetPath = null;
        }
    }
}