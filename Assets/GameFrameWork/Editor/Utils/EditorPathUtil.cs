using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrameWork.Editor
{
    public class EditorPathUtil
    {
        public static string appDataPath = Application.dataPath + "/";

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

        public static string luaPath = appDataPath + "Lua";
    }
}