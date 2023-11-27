using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrameWork.Editor
{
    public class EditorPathUtil
    {
        public static string appDataPath = Application.dataPath + "/";
        public static string appDataPathWithoutAsset = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
        public const string editorUIRootPath = "Assets/GameFrameWork/UI/UIRoot.prefab";
        public const string ediorConfigPath = "Assets/GameFrameWork/Editor/Config/";

        public const string configDataPath = "Assets/ConfigData/";
        public static string configDataFullPath = appDataPath + "ConfigData/";

        public static string behaviourTreeWindowConfigFullPath = appDataPath + "GameFrameWork/Editor/Config/";
        public const string behaviourTreeWindowDataName = "BehaviourTreeWindowData";
        public const string behaviourTreeWindowDataExtend = ".json";
        public const string behaviourTreeWindowDataFullPath = ediorConfigPath + behaviourTreeWindowDataName + behaviourTreeWindowDataExtend;

        public const string behaviourTreeConfigDataName = "BehaviourTreeConfigData";
        public const string behaviourTreeConfigDataExtend = ".json";
        public static string behaviourTreeConfigDataFullPath = appDataPath + "ConfigData/" + behaviourTreeConfigDataName + behaviourTreeConfigDataExtend;

        public static string assetBundleConfigFullPath = appDataPath + "GameFrameWork/Editor/Config/";
        public const string assetBundleDataName = "AssetBundleData";
        public const string assetBundleDataExtend = ".asset";
        public const string assetBundleDataPath = ediorConfigPath + assetBundleDataName + assetBundleDataExtend;
        public static string assetBundleDataFullPath = appDataPath + "GameFrameWork/Editor/Config/" + assetBundleDataName + assetBundleDataExtend;

        public static string assetBuildFilePath = appDataPath + "GameFrameWork/Editor/Config/AssetBuild";
        public const string assetBuildFileExtend = ".txt";
        public static string assetBuildFileFullPath = assetBuildFilePath + assetBuildFileExtend;

        public static string luaPath = appDataPath + "Lua";
    }
}