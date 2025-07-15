using GameFrameWork.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GameFrameWork.Editor
{
    public class GameFrameWorkConfigWindowData : ScriptableObject
    {
        public bool isCheckVersion
        {
            get
            {
                return m_IsCheckVersion;
            }
            set
            {
                m_IsCheckVersion = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public bool isLoadFromAssetBundle
        {
            get
            {
                return m_IsLoadFromAssetBundle;
            }
            set
            {
                m_IsLoadFromAssetBundle = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public bool isOpenLog
        {
            get
            {
                return m_IsOpenLog;
            }
            set
            {
                m_IsOpenLog = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public bool isUseLua
        {
            get
            {
                return m_IsUseLua;
            }
            set
            {
                m_IsUseLua = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public bool isLoadLuaFromAssetBundle
        {
            get
            {
                return m_IsLoadLuaFromAssetBundle;
            }
            set
            {
                m_IsLoadLuaFromAssetBundle = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public bool isLuaByteMode
        {
            get
            {
                return m_IsLuaByteMode;
            }
            set
            {
                m_IsLuaByteMode = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string luaPath
        {
            get
            {
                return m_LuaPath;
            }
            set
            {
                m_LuaPath = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string uiPath
        {
            get
            {
                return m_UIPath;
            }
            set
            {
                m_UIPath = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string uiPrefabsPath
        {
            get
            {
                return m_UIPrefabsPath;
            }
            set
            {
                m_UIPrefabsPath = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string uiSpritesPath
        {
            get
            {
                return m_UISpritesPath;
            }
            set
            {
                m_UISpritesPath = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string configDataPath
        {
            get
            {
                return m_ConfigDataPath;
            }
            set
            {
                m_ConfigDataPath = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string versionFileName
        {
            get
            {
                return m_VersionFileName;
            }
            set
            {
                m_VersionFileName = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public string assetMapFileName
        {
            get
            {
                return m_AssetMapFileName;
            }
            set
            {
                m_AssetMapFileName = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        public Color logColor
        {
            get
            {
                return m_LogColor;
            }
            set
            {
                m_LogColor = value;
                SaveGameFrameWorkConfig(this);
            }
        }

        [SerializeField] public string buildPath = string.Empty;
        [SerializeField] public string uiScenesPath = string.Empty;
        [SerializeField] public string uiAtlasPath = string.Empty;
        [SerializeField] public string entryScene = string.Empty;
        [SerializeField] public string languageKeyFilePath = string.Empty;

        private void SaveGameFrameWorkConfig(GameFrameWorkConfigWindowData windowData)
        {
            string configPath = PathUtil.FormatPath(EditorPathUtil.editorResourcesPath, PathUtil.gameFrameWorkConfigDataName);
            string configFullPath = PathUtil.FormatPath(EditorPathUtil.editorResourcesFullPath, PathUtil.gameFrameWorkConfigDataName);

            Utils.FileUtil.VerifyDirectory(EditorPathUtil.editorResourcesFullPath);

            GameFrameWorkConfig config = null;

            if (!File.Exists(configFullPath))
            {
                config = ScriptableObject.CreateInstance<GameFrameWorkConfig>();
                AssetDatabase.CreateAsset(config, configPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                config = AssetDatabase.LoadAssetAtPath<GameFrameWorkConfig>(configPath);
            }

            config.isCheckVersion = windowData.isCheckVersion;
            config.isLoadFromAssetBundle = windowData.isLoadFromAssetBundle;
            config.isOpenLog = windowData.isOpenLog;
            config.isUseLua = windowData.isUseLua;
            config.isLoadLuaFromAssetBundle = windowData.isLoadLuaFromAssetBundle;
            config.isLuaByteMode = windowData.isLuaByteMode;
            config.luaPath = EditorPathUtil.GetPathWithoutAssets(windowData.luaPath);
            config.uiPrefabsPath = EditorPathUtil.GetPathWithoutAssets(windowData.uiPrefabsPath);
            config.uiSpritesPath = EditorPathUtil.GetPathWithoutAssets(windowData.m_UISpritesPath);
            config.configDataPath = EditorPathUtil.GetPathWithoutAssets(windowData.configDataPath);
            config.versionFileName = windowData.versionFileName;
            config.assetMapFileName = windowData.assetMapFileName;

            EditorUtility.SetDirty(config);
        }

        [SerializeField] private bool m_IsCheckVersion = false;
        [SerializeField] private bool m_IsLoadFromAssetBundle = false;
        [SerializeField] private bool m_IsOpenLog = false;
        [SerializeField] private bool m_IsUseLua = false;
        [SerializeField] private bool m_IsLoadLuaFromAssetBundle = false;
        [SerializeField] private bool m_IsLuaByteMode = false;
        [SerializeField] private string m_LuaPath = string.Empty;
        [SerializeField] private string m_UIPath = string.Empty;
        [SerializeField] private string m_UIPrefabsPath = string.Empty;
        [SerializeField] private string m_UIAtlasPath = string.Empty;
        [SerializeField] private string m_UISpritesPath = string.Empty;
        [SerializeField] private string m_ConfigDataPath = string.Empty;
        [SerializeField] private string m_VersionFileName = string.Empty;
        [SerializeField] private string m_AssetMapFileName = string.Empty;
        [SerializeField] private Color m_LogColor = Color.white;
    }
}