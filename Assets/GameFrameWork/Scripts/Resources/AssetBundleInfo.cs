using UnityEngine;

namespace GameFrameWork.Resources
{
    public class AssetBundleInfo
    {
        public AssetBundle assetBundle;
        public int referencedCount;

        public AssetBundleInfo(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
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