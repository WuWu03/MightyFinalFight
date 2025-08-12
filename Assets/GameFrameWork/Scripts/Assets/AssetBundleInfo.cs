using UnityEngine;

namespace GameFrameWork.Assets
{
    public class AssetBundleInfo : BaseEventArgs
    {
        public AssetBundle assetBundle { get; set; }
        public int referencedCount { get; set; }

        public static AssetBundleInfo Create(AssetBundle assetBundle)
        {
            AssetBundleInfo assetBundleInfo = ReferencePool.Acquire<AssetBundleInfo>();
            assetBundleInfo.assetBundle = assetBundle;
            assetBundleInfo.referencedCount = 0;
            return assetBundleInfo;
        }

        public override void Clear()
        {
            base.Clear();
            assetBundle = null;
            referencedCount = 0;
        }
    }
}