using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
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
            MapSingle,
            Map,
        }

        public AssetType BundleType;
        public string BundleName;
        public string BundleExtend;
        public string Pattern;
        public string AssetPath;
        public string AssetBundlePath;
    }
}