using System;
using UnityEngine;

namespace WuWuFramework.Utils
{
    public class PathUtil
    {
        /// <summary>
        /// 应用程序数据路径
        /// </summary>
        public static string AppDataPath = Application.dataPath;
        /// <summary>
        /// 保留路径，用于存放只读数据
        /// </summary>
        public static string StreamingAssetsPath = Application.streamingAssetsPath;
        /// <summary>
        /// 持久化数据路径，用于存放可读写数据
        /// </summary>
        public static string PersistentDataPath = Application.persistentDataPath;
        /// <summary>
        /// 框架配置数据名称
        /// </summary>
        public const string WuWuFrameworkConfigDataName = "WuWuFrameworkConfig.asset";
        /// <summary>
        /// 行为树数据路径
        /// </summary>
        public const string BehaviourTreeDataPath = "BehaviourTreeData";
        /// <summary>
        /// AB包依赖清单文件名称
        /// </summary>
        public const string ManiFestName = "StreamingAssets";

        /// <summary>
        /// 实时资源路径，Android和iOS平台使用持久化数据路径，其他平台使用保留路径
        /// </summary>
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

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1)
        {
            return FormatPath(arg1, null, null, null, null, null, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1, string arg2)
        {
            return FormatPath(arg1, arg2, null, null, null, null, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1, string arg2, string arg3)
        {
            return FormatPath(arg1, arg2, arg3, null, null, null, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1, string arg2, string arg3, string arg4)
        {
            return FormatPath(arg1, arg2, arg3, arg4, null, null, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, null, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static string FormatPath(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return FormatPath(arg1, arg2, arg3, arg4, arg5, arg6, null);
        }

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 格式化路径
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatPath(params string[] args)
        {
            StringUtil.ClearArgs();

            foreach (string arg in args)
            {
                StringUtil.AddArg(arg);
            }

            return StringUtil.Append(true);
        }

        /// <summary>
        /// 获取UI预制体路径
        /// </summary>
        /// <returns></returns>
        public static string GetUIPrefabsPath()
        {
            return WuWuFrameworkEntry.config.uiPrefabsPath;
        }

        /// <summary>
        /// 获取UI图集路径
        /// </summary>
        /// <returns></returns>
        public static string GetUIAtlasPath()
        {
            return WuWuFrameworkEntry.config.uiAtlasPath;
        }

        /// <summary>
        /// 获取资源相对路径，以Assets开头
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || assetPath == "Assets")
            {
                return "Assets";
            }

            int assetIndex = assetPath.IndexOf("Assets/", StringComparison.Ordinal);
            return assetIndex < 0 ? FormatPath("Assets", assetPath) : assetPath[assetIndex..];
        }

        /// <summary>
        /// 获取资源完整路径，以应用程序数据路径开头
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || assetPath == "Assets")
            {
                return AppDataPath;
            }

            int assetIndex = assetPath.IndexOf("Assets/", StringComparison.Ordinal);
            return assetIndex < 0 ? FormatPath(AppDataPath, assetPath) : FormatPath(AppDataPath, assetPath[(assetIndex + 7)..]);
        }
    }
}