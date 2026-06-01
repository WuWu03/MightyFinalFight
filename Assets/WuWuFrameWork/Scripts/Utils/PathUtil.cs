using System;
using UnityEngine;

namespace WuWuFramework.Utils
{
    public class PathUtil
    {
        public static string AppDataPath = Application.dataPath;
        public static string StreamingAssetsPath = Application.streamingAssetsPath;
        public static string PersistentDataPath = Application.persistentDataPath;

        public const string WuWuFrameworkConfigDataName = "WuWuFrameworkConfig.asset";
        public const string BehaviourTreeDataPath = "BehaviourTreeData";
        public const string ManiFestName = "StreamingAssets";

        public static string runTimeAssetsPath
        {
            get
            {
#if UNITY_ANDROID || UNITY_IOS
                return PersistentDataPath;
#else
                return StreamingAssetsPath;
#endif
            }
        }

        public static string FormatPath(string arg1, bool isLastPath = true)
        {
            return FormatPath(arg1, null, null, null, null, null, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, bool isLastPath = true)
        {
            return FormatPath(arg1, arg2, null, null, null, null, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, bool isLastPath = true)
        {
            return FormatPath(arg1, arg2, arg3, null, null, null, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, bool isLastPath = true)
        {
            return FormatPath(arg1, arg2, arg3, arg4, null, null, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, bool isLastPath = true)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, null, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, bool isLastPath = true)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, arg6, null, isLastPath);
        }

        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, bool isLastPath = true)
        {
            StringUtil.ClearArgs();
            StringUtil.AddArg(arg1);
            StringUtil.AddArg(arg2);
            StringUtil.AddArg(arg3);
            StringUtil.AddArg(arg4);
            StringUtil.AddArg(arg5);
            StringUtil.AddArg(arg6);
            StringUtil.AddArg(arg7);
            return StringUtil.Append(true, isLastPath);
        }

        public static string FormatPath(params string[] args)
        {
            StringUtil.ClearArgs();

            foreach (string arg in args)
            {
                StringUtil.AddArg(arg);
            }

            return StringUtil.Append(true, true);
        }

        public static string GetUIPrefabsPath()
        {
            return WuWuFrameworkEntry.config.uiPrefabsPath;
        }

        public static string GetUISpritesPath()
        {
            return WuWuFrameworkEntry.config.uiSpritesPath;
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
                return AppDataPath;
            }

            int assetIndex = assetPath.IndexOf("Assets/", StringComparison.Ordinal);

            if (assetIndex > -1)
            {
                assetPath = assetPath[(assetIndex + 7)..];
            }

            return FormatPath(AppDataPath, assetPath);
        }
    }
}