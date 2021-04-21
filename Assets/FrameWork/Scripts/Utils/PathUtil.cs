using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public class PathUtil
    {
        public static string StreamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string PersistentDataPath = Application.persistentDataPath + "/";
        public static string AppDataPath = Application.dataPath + "/";
        public static string AssetsDirectory = "Assets/StreamingAssets/";
        public const string AssetBundleConfigPath = "Assets/FrameWork/Editor/AssetBundleConfig/";
        public static string AssetBundleConfigFullPath = AppDataPath + AssetBundleConfigPath.Substring(6);
        public const string AssetBundleDataName = "AssetBundleData";
        public const string AssetBundleDataExtend = ".asset";
        public const string AssetBundleDataPath = AssetBundleConfigPath + AssetBundleDataName + AssetBundleDataExtend;
        public static string AssetBundleDataFullPath = AppDataPath + AssetBundleConfigPath.Substring(6) + AssetBundleDataName + AssetBundleDataExtend;

        public static string GetAssetFullDir()
        {
            return AppDataPath + AssetsDirectory.Substring(AssetsDirectory.IndexOf("Assets/") + "Assets/".Length);
        }

        public static string GetLuaTempDir()
        {
            return AppDataPath + "LuaTemp/";
        }
    }
}
