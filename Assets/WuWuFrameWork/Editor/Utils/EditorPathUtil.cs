using WuWuFramework.Utils;
using UnityEngine;

namespace WuWuFramework.Editor
{
    public class EditorPathUtil
    {
        public const string ApplicationDataPath = "Assets/";
        public static readonly string ApplicationDataFullPath = Application.dataPath + "/";

        public static readonly string StreamingAssetsPath = ApplicationDataPath + "StreamingAssets/";
        public static readonly string StreamingAssetsFullPath = ApplicationDataFullPath + "StreamingAssets/";

        public const string EditorUIRootPath = ApplicationDataPath + "WuWuFramework/UI/UIRoot.prefab";
        public const string EditorUIRootScenePath = ApplicationDataPath + "WuWuFramework/UI/UIRootScene.prefab";

        public const string EditorConfigPath = ApplicationDataPath + "Editor/Config/";
        public static readonly string EditorConfigFullPath = ApplicationDataFullPath + "Editor/Config/";

        public const string EditorResourcesPath = ApplicationDataPath + "Resources/";
        public static readonly string EditorResourcesFullPath = ApplicationDataFullPath + "Resources/";

        public const string EditorScriptPath = ApplicationDataPath + "Scripts/";
        public static readonly string EditorScriptFullPath = ApplicationDataFullPath + "Scripts/";

        public const string BehaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string BehaviourTreeWindowDataExtend = ".json";
        public static readonly string BehaviourTreeWindowDataFullPath = EditorConfigFullPath + BehaviourTreeWindowDataName + BehaviourTreeWindowDataExtend;

        public const string AssetBundleWindowDataName = "AssetBundleWindowData";
        public const string AssetBundleWindowDataExtend = ".asset";
        public const string AssetBundleWindowDataPath = EditorConfigPath + AssetBundleWindowDataName + AssetBundleWindowDataExtend;
        public static readonly string AssetBundleWindowDataFullPath = EditorConfigFullPath + AssetBundleWindowDataName + AssetBundleWindowDataExtend;

        public const string WuWuFrameWorkConfigWindowDataName = "WuWuFrameWorkConfigWindowData";
        public const string WuWuFrameWorkConfigWindowDataExtend = ".asset";
        public static readonly string WuWuFrameWorkConfigWindowDataPath = EditorConfigPath + WuWuFrameWorkConfigWindowDataName + WuWuFrameWorkConfigWindowDataExtend;
        public static readonly string WuWuFrameWorkConfigWindowDataFullPath = EditorConfigFullPath + WuWuFrameWorkConfigWindowDataName + WuWuFrameWorkConfigWindowDataExtend;

        public const string InputConfigDataName = "InputConfigData";
        public const string InputConfigDataExtend = ".inputactions";
        public static readonly string InputConfigDataPath = ApplicationDataPath + "WuWuFramework/Editor/Input/" + InputConfigDataName + InputConfigDataExtend;
        public static readonly string InputConfigDataFullPath = ApplicationDataFullPath + "WuWuFramework/Editor/Input/" + InputConfigDataName + InputConfigDataExtend;

        public static readonly string WuWuFrameworkUIScriptsFullPath = ApplicationDataFullPath + "WuWuFramework/Scripts/UI/";

        public const string EntryScriptName = "GameEntry";
        public const string EntryScriptExtend = ".cs";
        public static readonly string EntryScriptFullPath = EditorScriptFullPath + EntryScriptName + EntryScriptExtend;

        public const string DefaultAssetsPath = "ArtResources";
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
            string uiPath = EditorMgr.GetWuWuFrameworkConfig().uiPath;

            if (string.IsNullOrEmpty(uiPath))
            {
                return string.Empty;
            }

            return PathUtil.FormatPath(uiPath, UIPrefabsPath);
        }

        public static string GetUIAtlasPath()
        {
            string uiPath = EditorMgr.GetWuWuFrameworkConfig().uiPath;

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