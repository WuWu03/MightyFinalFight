using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Utilities
{
    public class PathUtil
    {
        public static string streamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string persistentDataPath = Application.persistentDataPath + "/";
        public static string luaPath = "Lua";
        public static string configDataPath = "ConfigData/";
        public static string behaviourTreeConfigDataPath = configDataPath + "BehaviourTreeData.json";
        public const string assetBundleVersionName = "Version.txt";
        public const string maniFestName = "StreamingAssets";

        public static string runTimeAssetPath
        {
            get
            {

#if UNITY_EDITOR
                string resPath = streamingAssetsPath;
#else
                string resPath = streamingAssetsPath;
#endif
                return resPath;
            }
        }

        public static string FormatPath(string path, string name)
        {
            m_StringBuilder.Clear();
            m_StringBuilder.AppendFormat("{0}/{1}", path, name);
            string str = m_StringBuilder.ToString();
            m_StringBuilder.Clear();
            return str;
        }

        private static StringBuilder m_StringBuilder = new StringBuilder();
    }
}
