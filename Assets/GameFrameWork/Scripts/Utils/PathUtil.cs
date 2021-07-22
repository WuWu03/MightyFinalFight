using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameFrameWork.Utility
{
    public class PathUtil
    {
        public static string StreamingAssetsPath = Application.streamingAssetsPath + "/";
        public static string PersistentDataPath = Application.persistentDataPath + "/";
        public static string AppDataPath = Application.dataPath + "/";
        public const string AssetBundleVersion = "Version.txt";
        public static string LuaTempDir = AppDataPath + "LuaTemp";
        public const string ManiFest = "StreamingAssets";

        public const string EditorUIRootPath = "Assets/GameFrameWork/UI/UIRoot.prefab";
        public const string EdiorConfiglPath = "Assets/GameFrameWork/Editor/Config/";
        public const string ConfigDataDefaultPath = "Assets/ConfigData/";
        public static string ConfigDataDefaultFullPath = AppDataPath + ConfigDataDefaultPath.Substring(7);

        public static string BehaviourTreeWindowConfigFullPath = AppDataPath + EdiorConfiglPath.Substring(7);
        public const string BehaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string BehaviourTreeWindowDataExtend = ".asset";
        public const string BehaviourTreeWindowDataPath = EdiorConfiglPath + BehaviourTreeWindowDataName + BehaviourTreeWindowDataExtend;

        public static string AssetBundleConfigFullPath = AppDataPath + EdiorConfiglPath.Substring(7);
        public const string AssetBundleDataName = "AssetBundleData";
        public const string AssetBundleDataExtend = ".asset";
        public const string AssetBundleDataPath = EdiorConfiglPath + AssetBundleDataName + AssetBundleDataExtend;
        public static string AssetBundleDataFullPath = AppDataPath + EdiorConfiglPath.Substring(7) + AssetBundleDataName + AssetBundleDataExtend;

       

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
