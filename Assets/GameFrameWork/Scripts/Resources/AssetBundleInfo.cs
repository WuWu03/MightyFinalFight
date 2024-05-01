using UnityEngine;

namespace GameFrameWork.Resources
{
    public class AssetBundleInfo :IReference
    {
        public AssetBundle assetBundle;
        public int referencedCount;

        public static AssetBundleInfo Create(AssetBundle assetBundle)
        {
            AssetBundleInfo assetBundleInfo = ReferencePool.Acquire<AssetBundleInfo>();
            assetBundleInfo.assetBundle = assetBundle;
            assetBundleInfo.referencedCount = 0;
            return assetBundleInfo;
        }

        public void Clear()
        {
            assetBundle = null;
            referencedCount = 0;
        }
    }

    public class AssetBundleVersion
    {
        public string filePath;
        public string extendName;
        public string md5Value;

        public AssetBundleVersion(string filePath, string extendName, string md5Value)
        {
            this.filePath = filePath;
            this.extendName = extendName;
            this.md5Value = md5Value;
        }
    }
}