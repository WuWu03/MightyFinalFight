using WuWuFramework.Serialize;
using System;
using System.Collections.Generic;

namespace WuWuFramework.Editor
{
    public class AssetBundleConfig : BaseScriptableObject<AssetBundleData>
    {
        public bool lockConfig = false;
        public int platFormIndex = 0;
        public string assetCopyDir = string.Empty;
        public bool isCopyAsset = false;
        public List<string> listExtendName = null;
        public List<string> listPattern = null;
    }

    [Serializable]
    public class AssetBundleData : BaseScriptableConfigData
    {
        public enum BundleBuildType : byte
        {
            Multi,//包体下每个资源单独打ab
            Single,//包体下所有资源打成一个ab
            MultiSingle,//按文件夹打包
        }

        public BundleBuildType bundleBuildType;
        public string bundleName;
        public string bundleExtend;
        public string pattern;
        public string assetPath;
        public List<string> assetPaths;

        public override int CompareTo(object obj)
        {
            if (obj is AssetBundleData data)
            {
                return String.CompareOrdinal(this.bundleName, data.bundleName);
            }
            
            return base.CompareTo(obj);
        }

        public AssetBundleData Clone()
        {
            AssetBundleData data = new()
            {
                bundleBuildType = bundleBuildType,
                bundleName = bundleName,
                bundleExtend = bundleExtend,
                pattern = pattern,
                assetPath = assetPath,
                assetPaths = assetPaths != null ? new(assetPaths) : null,
            };
            return data;
        }
    }
}