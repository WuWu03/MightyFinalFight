using UnityEngine;

namespace GameFrameWork.Utils
{
    public class PathUtil
    {
        public static string appDataPath = Application.dataPath;
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string persistentDataPath = Application.persistentDataPath;

        public const string configDataPath = "ConfigData";
        public const string uiPrefabPath = "Prefabs";
        public const string uiAtlasPath = "Atlas";

        public const string gameFrameWorkConfigDataName = "GameFrameWorkConfig.asset";
        public const string behaviourTreeConfigDataName = "BehaviourTreeConfigData.json";
        public const string assetBundleVersionName = "Version.txt";
        public const string assetMapName = "AssetMap.txt";
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

        public static string FormatPath(string arg1)
        {
            return StringUtil.Format(true, arg1);
        }

        public static string FormatPath(string arg1, string arg2)
        {
            return StringUtil.Format(true, arg1, arg2);
        }

        public static string FormatPath(string arg1, string arg2, string arg3)
        {
            return StringUtil.Format(true, arg1, arg2, arg3);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4)
        {
            return StringUtil.Format(true, arg1, arg2, arg3, arg4);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return StringUtil.Format(true, arg1, arg2, arg3, arg4, arg5);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return StringUtil.Format(true, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            return StringUtil.Format(true, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static string FormatPath(params string[] args)
        {
            return StringUtil.Format(true, args);
        }

        public static string GetUIPrefabPath()
        {
            string uiPath = GameFrameWorkEntry.config.uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return FormatPath(uiPath, uiPrefabPath);
        }

        public static string GetUIAtlasPath()
        {
            string uiPath = GameFrameWorkEntry.config.uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return FormatPath(uiPath, uiAtlasPath);
        }

        public static string GetAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "Assets/";
            }

            int assetIndex = path.IndexOf("Assets");

            if (assetIndex < 0)
            {
                return FormatPath("Assets", path);
            }

            return path.Substring(assetIndex);
        }

        public static string GetAssetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return appDataPath;
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