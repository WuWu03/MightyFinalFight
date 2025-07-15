using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class AssetBundleConfig : BaseScriptableObject<AssetBundleData>
    {
        [SerializeField]
        public bool lockConfig = false;
        [SerializeField]
        public int platFormIndex = 0;
        [SerializeField]
        public string assetCopyDir = string.Empty;
        [SerializeField]
        public bool isCopyAsset = false;
        [SerializeField]
        public List<string> listExtendName = null;
        [SerializeField]
        public List<string> listPattern = null;
    }

    [Serializable]
    public class AssetBundleData : BaseConfigData
    {
        public enum BundleBuildType
        {
            Mulity,//路径下每个资源单独打ab
            Single,//路径下所有资源打成一个ab
        }

        public BundleBuildType bundleBuildType;
        public string bundleName;
        public string bundleExtend;
        public string pattern;
        public string assetPath;

        public override int CompareTo(object obj)
        {
            AssetBundleData data = obj as AssetBundleData;
            return string.Compare(this.assetPath, data.assetPath);
        }

        public AssetBundleData Clone()
        {
            AssetBundleData data = new AssetBundleData();
            data.bundleBuildType = this.bundleBuildType;
            data.bundleName = this.bundleName;
            data.bundleExtend = this.bundleExtend;
            data.pattern = this.pattern;
            data.assetPath = this.assetPath;

            return data;
        }
    }
}