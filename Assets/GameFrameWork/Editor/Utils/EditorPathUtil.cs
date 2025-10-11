using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class EditorPathUtil
    {
        public const string applicationDataPath = "Assets/";
        public static string applicationDataFullPath = Application.dataPath + "/";

        public static string streamingAssetsPath = applicationDataPath + "StreamingAssets/";
        public static string streamingAssetsFullPath = applicationDataFullPath + "StreamingAssets/";

        public const string editorUIRootPath = applicationDataPath + "GameFrameWork/UI/UIRoot.prefab";
        public const string editorUIRootScenePath = applicationDataPath + "GameFrameWork/UI/UIRootScene.prefab";

        public const string editorConfigPath = applicationDataPath +  "Editor/Config/";
        public static string editorConfigFullPath = applicationDataFullPath + "Editor/Config/";

        public const string editorResourcesPath = applicationDataPath + "Resources/";
        public static string editorResourcesFullPath = applicationDataFullPath + "Resources/";

        public const string editorScriptPath = applicationDataPath + "Scripts/";
        public static string editorScriptFullPath = applicationDataFullPath + "Scripts/";

        public const string behaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string behaviourTreeWindowDataExtend = ".json";
        public const string behaviourTreeWindowDataFullPath =  editorConfigPath + behaviourTreeWindowDataName + behaviourTreeWindowDataExtend;

        public const string assetBundleWindowDataName = "AssetBundleWindowData";
        public const string assetBundleWindowDataExtend = ".asset";
        public const string assetBundleWindowDataPath = editorConfigPath + assetBundleWindowDataName + assetBundleWindowDataExtend;
        public static string assetBundleWindowDataFullPath = applicationDataFullPath + editorConfigPath + assetBundleWindowDataName + assetBundleWindowDataExtend;

        public const string gameFrameWorkConfigWindowDataName = "GameFrameWorkConfigWindowData";
        public const string gameFrameWorkConfigWindowDataExtend = ".asset";
        public static string gameFrameWorkConfigWindowDataPath = editorConfigPath + gameFrameWorkConfigWindowDataName + gameFrameWorkConfigWindowDataExtend;
        public static string gameFrameWorkConfigWindowDataFullPath = editorConfigFullPath + gameFrameWorkConfigWindowDataName + gameFrameWorkConfigWindowDataExtend;
        
        public const string entryScriptName = "GameEntry";
        public const string entryScriptExtend = ".cs";
        public static string entryScriptFullPath = editorScriptFullPath + entryScriptName + entryScriptExtend;

        public const string defaultUIPath = "UI";
        public const string defaultUIScriptsPath = "Scripts/UI";
        public const string defaultConfigDataPath = "ConfigData";

        public const string uiPrefabsPath = "Prefabs";
        public const string uiAtlasPath = "Atlas";
        public const string uiScenesPath = "Scenes";
        public const string uiSpritesPath = "Sprites";

        public const string assetMapFileDefaultName = "AssetMap";
        public const string assetMapFileDefaultExt = ".txt";

        public const string versionFileDefaultName = "Version";
        public const string versionFileDefaultExt = ".txt";

        public static string GetUIPrefabPath()
        {
            string uiPath = EditorMgr.GetGameFrameWorkConfig().uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return PathUtil.FormatPath(uiPath, uiPrefabsPath);
        }

        public static string GetUIAtlasPath()
        {
            string uiPath = EditorMgr.GetGameFrameWorkConfig().uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return PathUtil.FormatPath(uiPath, uiAtlasPath);
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