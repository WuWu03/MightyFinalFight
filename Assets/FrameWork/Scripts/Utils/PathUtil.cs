using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public class PathUtil
    {
        public static string StreamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string PersistentDataPath = Application.persistentDataPath + "/";
        public static string DataPath = Application.dataPath + "/";

        public const string AssetBundleDataPath = "Assets/FrameWork/Editor/AssetBundleConfig/";
        public const string AssetBundleDataName = "AssetBundleData";
        public const string AssetBundleDataExtend = ".asset";
        public const string AssetBundleConfig = AssetBundleDataPath + AssetBundleDataName + AssetBundleDataExtend;

        public static string GetAssetFullDir()
        {
            return DataPath + AppConfig.Ins.AssetsDirectory.Substring(AppConfig.Ins.AssetsDirectory.IndexOf("Assets/") + "Assets/".Length);
        }

        public static string GetLuaTempDir()
        {
            return DataPath + "LuaTemp/";
        }
    }
}
