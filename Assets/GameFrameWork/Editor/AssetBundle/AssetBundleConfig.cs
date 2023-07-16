using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class AssetBundleConfig : BaseScriptableObject<AssetBundleData>
    {
        [SerializeField]
        public bool LockConfig = false;
        [SerializeField]
        public int PlatFormIndex = 0;
        [SerializeField]
        public string AssetBuildDir = string.Empty;
        [SerializeField]
        public string AssetCopyDir = string.Empty;
        [SerializeField]
        public bool IsCopyAsset = false;
        [SerializeField]
        public List<string> ListExtendName = null;
        [SerializeField]
        public List<string> ListPattern = null;

        public string AssetBuildFullDir => Application.dataPath + AssetBuildDir.Substring(6);
    }

    [Serializable]
    public class AssetBundleData : BaseConfigData
    {
        public enum AssetType
        {
            MapSingle,//路径下每个资源单独打ab
            Map,//路径下所有资源打成一个ab
        }

        public AssetType BundleType;
        public string BundleName;
        public string BundleExtend;
        public string Pattern;
        public string AssetPath;
        public string AssetBundlePath;

        public override int CompareTo(object obj)
        {
            AssetBundleData data = obj as AssetBundleData;
            return string.Compare(this.AssetPath, data.AssetPath);
        }

        public AssetBundleData Clone()
        {
            AssetBundleData data = new AssetBundleData();
            data.BundleType = this.BundleType;
            data.BundleName = this.BundleName;
            data.BundleExtend = this.BundleExtend;
            data.Pattern = this.Pattern;
            data.AssetPath = this.AssetPath;
            data.AssetBundlePath = this.AssetBundlePath;

            return data;
        }
    }
}