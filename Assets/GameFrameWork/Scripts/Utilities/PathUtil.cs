using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Utilities
{
    public class PathUtil
    {
        public static string appDataPath = Application.dataPath;
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string persistentDataPath = Application.persistentDataPath;
        public static string luaPath = "Lua";
        public static string configDataPath = "ConfigData";
        public const string behaviourTreeConfigDataPath = "ConfigData/BehaviourTreeConfigData";
        public const string uiPrefabPath = "Prefabs";
        public const string uiAtlasPath = "UIAtlas";
        public const string assetBundleVersionName = "Version.txt";
        public const string maniFestName = "StreamingAssets";
        public const string assetBundleExtension = ".assetbundle";

        public static string runTimeAssetPath
        {
            get
            {

#if UNITY_EDITOR
                return streamingAssetsPath;
#elif UNITY_STANDALONE_WIN
                return streamingAssetsPath;
#elif UNITY_ANDROID
                return persistentDataPath;
#elif UNITY_IOS
                return persistentDataPath;
#endif
            }
        }

        public static string FormatPath(params string[] args)
        {
            return StringUtil.Format(true, args);
        }

        public static string GetUIPrefabPath()
        {
            string uiDirectory = AppConfig.instance.uiDirectory;

            if (string.IsNullOrEmpty(uiDirectory))
            {
                return null;
            }

            return FormatPath(uiDirectory, uiPrefabPath);
        }

        public static string GetUIAtlasPath()
        {
            string uiDirectory = AppConfig.instance.uiDirectory;

            if (string.IsNullOrEmpty(uiDirectory))
            {
                return null;
            }

            return FormatPath(uiDirectory, uiAtlasPath);
        }

        public static string GetAssetPath(string fullPath)
        {
            int assetIndex = fullPath.IndexOf("Assets");

            if (assetIndex < 0)
            {
                return FormatPath("Assets", fullPath);
            }

            return fullPath.Substring(assetIndex);
        }

        public static string GetAssetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            int assetIndex = assetPath.IndexOf("Assets");

            if (assetIndex > -1)
            {
                assetPath = assetPath.Substring(assetIndex + 6);
            }

            return FormatPath(appDataPath, assetPath);
        }
    }
}
