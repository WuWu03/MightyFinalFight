using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Utilities
{
    public class PathUtil
    {
        public static string appDataPath = Application.dataPath + "/";
        public static string streamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string persistentDataPath = Application.persistentDataPath + "/";
        public static string luaPath = "Lua";
        public static string configDataPath = "ConfigData/";
        public static string behaviourTreeConfigDataPath = configDataPath + "BehaviourTreeConfigData";
        public const string assetBundleVersionName = "Version.txt";
        public const string maniFestName = "StreamingAssets";

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
    }
}
