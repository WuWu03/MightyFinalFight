using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public class PathUtil
    {
        public const string EdiorConfiglPath = "Assets/FrameWork/Editor/Config/";
        public static string ConfigDataDefaultPath = Application.dataPath + "/ConfigData/";

        public static string BehaviourTreeWindowConfigFullPath = AppDataPath + EdiorConfiglPath.Substring(6);
        public const string BehaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string BehaviourTreeWindowDataExtend = ".asset";
        public const string BehaviourTreeWindowDataPath = EdiorConfiglPath + BehaviourTreeWindowDataName + BehaviourTreeWindowDataExtend;

        public static string AssetBundleConfigFullPath = AppDataPath + EdiorConfiglPath.Substring(6);
        public const string AssetBundleDataName = "AssetBundleData";
        public const string AssetBundleDataExtend = ".asset";
        public const string AssetBundleDataPath = EdiorConfiglPath + AssetBundleDataName + AssetBundleDataExtend;
        public static string AssetBundleDataFullPath = AppDataPath + EdiorConfiglPath.Substring(6) + AssetBundleDataName + AssetBundleDataExtend;

        public static string StreamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string PersistentDataPath = Application.persistentDataPath + "/";
        public static string AppDataPath = Application.dataPath + "/";
        public const string AssetBundleVersion = "Version.txt";
        public static string LuaTempDir = AppDataPath + "LuaTemp";
        public const string ManiFest = "StreamingAssets";

        public static string RunTimeAssetPath
        {
            get
            {

#if UNITY_EDITOR
                string resPath = StreamingAssetsPath;
#else
                string resPath = StreamingAssetsPath;
#endif
                return resPath;
            }
        }
    }
}
