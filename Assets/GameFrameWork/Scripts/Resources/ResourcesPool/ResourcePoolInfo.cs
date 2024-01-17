using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Resources
{
    public class ResourcePoolInfo : IReference
    {
        public UnityEngine.Object poolObject;
        public float releaseTime;
        public bool isReleaseImmediate;
        public string assetPath;

        public static ResourcePoolInfo Create(UnityEngine.Object poolObject, float releaseTime, bool isReleaseImmediate, string assetPath)
        {
            ResourcePoolInfo resourcePoolInfo = ReferencePool.Acquire<ResourcePoolInfo>();
            resourcePoolInfo.poolObject = poolObject;
            resourcePoolInfo.releaseTime = releaseTime;
            resourcePoolInfo.isReleaseImmediate = isReleaseImmediate;
            resourcePoolInfo.assetPath = assetPath;
            return resourcePoolInfo;
        }

        public ResourcePoolInfo()
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