using System;
using UnityEngine;

namespace GameFrameWork.Utils
{
    public class PathUtil
    {
        public static string appDataPath = Application.dataPath;
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string persistentDataPath = Application.persistentDataPath;

        public const string gameFrameWorkConfigDataName = "GameFrameWorkConfig.asset";
        public const string behaviourTreeDataPath = "BehaviourTreeData";
        public const string maniFestName = "StreamingAssets";

        public static string runTimeAssetsPath
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                return persistentDataPath;
#else
                return streamingAssetsPath; 
#endif
            }
        }

        public static string FormatPath(string arg1)
        {
            return FormatPath(arg1, null, null, null, null, null, null);
        }

        public static string FormatPath(string arg1, string arg2)
        {
            return FormatPath(arg1, arg2, null, null, null, null, null);
        }

        public static string FormatPath(string arg1, string arg2, string arg3)
        {
            return FormatPath(arg1, arg2, arg3, null, null, null, null);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4)
        {
            return FormatPath(arg1, arg2, arg3, arg4, null, null, null);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, null, null);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            StringUtil.ClearArgs();
            StringUtil.AddArg(arg1);
            StringUtil.AddArg(arg2);
            StringUtil.AddArg(arg3);
            StringUtil.AddArg(arg4);
            StringUtil.AddArg(arg5);
            StringUtil.AddArg(arg6);
            StringUtil.AddArg(arg7);
            return StringUtil.Append(true);
        }

        public static string FormatPath(params string[] args)
        {
            StringUtil.ClearArgs();

            foreach (string arg in args)
            {
                StringUtil.AddArg(arg);
            }

            return StringUtil.Append(true);
        }

        public static string GetUIPrefabsPath()
        {
            return GameFrameWorkEntry.config.uiPrefabsPath;
        }

        public static string GetUISpritesPath()
        {
            return GameFrameWorkEntry.config.uiSpritesPath;
        }

        public static string GetAssetPath(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "Assets")
            {
                return "Assets/";
            }

            int assetIndex = path.IndexOf("Assets/", StringComparison.Ordinal);
            return assetIndex < 0 ? FormatPath("Assets", path) : FormatPath(path[assetIndex..]);
        }

        public static string GetAssetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || assetPath == "Assets")
            {
                return appDataPath;
            }

            int assetIndex = assetPath.IndexOf("Assets/", StringComparison.Ordinal);

            if (assetIndex > -1)
            {
                assetPath = assetPath[(assetIndex + 7)..];
            }

            return FormatPath(appDataPath, assetPath);
        }
    }
}