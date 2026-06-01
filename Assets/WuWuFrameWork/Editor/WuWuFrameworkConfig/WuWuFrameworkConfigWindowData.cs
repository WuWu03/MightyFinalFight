using WuWuFramework.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace WuWuFramework.Editor
{
    public class WuWuFrameworkConfigWindowData : ScriptableObject
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
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
                SaveWuWuFrameworkConfig(this);
            }
        }

        [SerializeField] public string buildPath = string.Empty;
        [SerializeField] public string uiScenesPath = string.Empty;
        [SerializeField] public string uiAtlasPath = string.Empty;
        [SerializeField] public string uiScriptsPath = string.Empty;
        [SerializeField] public string entryScene = string.Empty;
        [SerializeField] public string languageKeyFilePath = string.Empty;

        private void SaveWuWuFrameworkConfig(WuWuFrameworkConfigWindowData windowData)
        {
            string configPath = PathUtil.FormatPath(EditorPathUtil.EditorResourcesPath, PathUtil.WuWuFrameworkConfigDataName);
            string configFullPath = PathUtil.FormatPath(EditorPathUtil.EditorResourcesFullPath, PathUtil.WuWuFrameworkConfigDataName);

            Utils.FileUtil.VerifyDirectory(EditorPathUtil.EditorResourcesFullPath);

            WuWuFrameworkConfig config = null;

            if (!File.Exists(configFullPath))
            {
                config = ScriptableObject.CreateInstance<WuWuFrameworkConfig>();
                AssetDatabase.CreateAsset(config, configPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                config = AssetDatabase.LoadAssetAtPath<WuWuFrameworkConfig>(configPath);
            }

            config.isCheckVersion = windowData.isCheckVersion;
            config.isLoadFromAssetBundle = windowData.isLoadFromAssetBundle;
            config.isOpenLog = windowData.isOpenLog;
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
        [SerializeField] private string m_UIPath = string.Empty;
        [SerializeField] private string m_UIPrefabsPath = string.Empty;
        [SerializeField] private string m_UISpritesPath = string.Empty;
        [SerializeField] private string m_ConfigDataPath = string.Empty;
        [SerializeField] private string m_VersionFileName = string.Empty;
        [SerializeField] private string m_AssetMapFileName = string.Empty;
        [SerializeField] private Color m_LogColor = Color.white;
    }
}