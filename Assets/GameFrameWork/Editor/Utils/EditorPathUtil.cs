using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class EditorPathUtil
    {
        public const string ApplicationDataPath = "Assets/";
        public static readonly string ApplicationDataFullPath = Application.dataPath + "/";

        public static readonly string StreamingAssetsPath = ApplicationDataPath + "StreamingAssets/";
        public static readonly string StreamingAssetsFullPath = ApplicationDataFullPath + "StreamingAssets/";

        public const string EditorUIRootPath = ApplicationDataPath + "GameFrameWork/UI/UIRoot.prefab";
        public const string EditorUIRootScenePath = ApplicationDataPath + "GameFrameWork/UI/UIRootScene.prefab";

        public const string EditorConfigPath = ApplicationDataPath + "Editor/Config/";
        public static readonly string EditorConfigFullPath = ApplicationDataFullPath + "Editor/Config/";

        public const string EditorResourcesPath = ApplicationDataPath + "Resources/";
        public static readonly string EditorResourcesFullPath = ApplicationDataFullPath + "Resources/";

        public const string EditorScriptPath = ApplicationDataPath + "Scripts/";
        public static readonly string EditorScriptFullPath = ApplicationDataFullPath + "Scripts/";

        public const string BehaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string BehaviourTreeWindowDataExtend = ".json";
        public const string BehaviourTreeWindowDataFullPath = EditorConfigPath + BehaviourTreeWindowDataName + BehaviourTreeWindowDataExtend;

        public const string AssetBundleWindowDataName = "AssetBundleWindowData";
        public const string AssetBundleWindowDataExtend = ".asset";
        public const string AssetBundleWindowDataPath = EditorConfigPath + AssetBundleWindowDataName + AssetBundleWindowDataExtend;
        public static readonly string AassetBundleWindowDataFullPath = ApplicationDataFullPath + EditorConfigPath + AssetBundleWindowDataName + AssetBundleWindowDataExtend;

        public const string GameFrameWorkConfigWindowDataName = "GameFrameWorkConfigWindowData";
        public const string GameFrameWorkConfigWindowDataExtend = ".asset";
        public static readonly string GameFrameWorkConfigWindowDataPath = EditorConfigPath + GameFrameWorkConfigWindowDataName + GameFrameWorkConfigWindowDataExtend;
        public static readonly string GameFrameWorkConfigWindowDataFullPath = EditorConfigFullPath + GameFrameWorkConfigWindowDataName + GameFrameWorkConfigWindowDataExtend;

        public static readonly string GameFrameWorkUIScriptsFullPath = ApplicationDataFullPath + "GameFrameWork/Scripts/UI/";

        public const string EntryScriptName = "GameEntry";
        public const string EntryScriptExtend = ".cs";
        public static readonly string EntryScriptFullPath = EditorScriptFullPath + EntryScriptName + EntryScriptExtend;

        public const string DefaultUIPath = "UI";
        public const string DefaultUIScriptsPath = "Scripts/UI";
        public const string DefaultConfigDataPath = "ConfigData";

        public const string UIPrefabsPath = "Prefabs";
        public const string UIAtlasPath = "Atlas";
        public const string UIScenesPath = "Scenes";
        public const string UISpritesPath = "Sprites";

        public const string AssetMapFileDefaultName = "AssetMap";
        public const string AssetMapFileDefaultExt = ".txt";

        public const string VersionFileDefaultName = "Version";
        public const string VersionFileDefaultExt = ".txt";

        public static string GetUIPrefabPath()
        {
            string uiPath = EditorMgr.GetGameFrameWorkConfig().uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return PathUtil.FormatPath(uiPath, UIPrefabsPath);
        }

        public static string GetUIAtlasPath()
        {
            string uiPath = EditorMgr.GetGameFrameWorkConfig().uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return PathUtil.FormatPath(uiPath, UIAtlasPath);
        }

        public static string GetPathWithoutAssets(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.Contains("Assets"))
            {
                return path;
            }

            path = path.Replace("\\", "/");
            path = path.Substring(path.IndexOf("Assets") + 6);

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            return path;
        }
    }
}