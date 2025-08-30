using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
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
    public class AssetBundleData : BaseConfigData
    {
        public enum BundleBuildType
        {
            Mulity,//包体下每个资源单独打ab
            Single,//包体下所有资源打成一个ab
            MulitySingle,//按文件夹打包
        }

        public BundleBuildType bundleBuildType;
        public string bundleName;
        public string bundleExtend;
        public string pattern;
        public string assetPath;
        public List<string> assetPaths;

        public override int CompareTo(object obj)
        {
            AssetBundleData data = obj as AssetBundleData;
            return string.Compare(this.bundleName, data.bundleName);
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