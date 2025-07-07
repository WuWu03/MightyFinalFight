using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class EditorPathUtil
    {
        public static string appDataPath = Application.dataPath + "/";
        public static string appDataPathWithoutAsset = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
        public const string editorUIRootPath = "Assets/GameFrameWork/UI/UIRoot.prefab";

        public const string editorConfigPath = "Assets/GameFrameWork/Editor/Config/";
        public static string editorConfigFullPath = appDataPath + "GameFrameWork/Editor/Config/";

        public const string editorResourcesPath = "Assets/Resources/";
        public static string editorResourcesFullPath = appDataPath + "Resources/";

        public const string editorScriptPath = "Assets/Scripts/";
        public static string editorScriptFullPath = appDataPath + "Scripts/";

        public const string behaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string behaviourTreeWindowDataExtend = ".json";
        public const string behaviourTreeWindowDataFullPath =  editorConfigPath + behaviourTreeWindowDataName + behaviourTreeWindowDataExtend;

        public const string assetBundleWindowDataName = "AssetBundleWindowData";
        public const string assetBundleWindowDataExtend = ".asset";
        public const string assetBundleWindowDataPath = editorConfigPath + assetBundleWindowDataName + assetBundleWindowDataExtend;
        public static string assetBundleWindowDataFullPath = appDataPath + editorConfigPath + assetBundleWindowDataName + assetBundleWindowDataExtend;

        public const string gameFrameWorkConfigWindowDataName = "GameFrameWorkConfigWindowData";
        public const string gameFrameWorkConfigWindowDataExtend = ".asset";
        public static string gameFrameWorkConfigWindowDataPath = editorConfigPath + gameFrameWorkConfigWindowDataName + gameFrameWorkConfigWindowDataExtend;
        public static string gameFrameWorkConfigWindowDataFullPath = editorConfigFullPath + gameFrameWorkConfigWindowDataName + gameFrameWorkConfigWindowDataExtend;

        public const string entryScriptName = "GameEntry";
        public const string entryScriptExtend = ".cs";
        public static string entryScriptFullPath = editorScriptFullPath + entryScriptName + entryScriptExtend;

        public const string uiScenesPath = "Scenes";
    }
}