using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WuWuFramework.Event;
using UnityObject = UnityEngine.Object;
using WuWuFileUtil = WuWuFramework.Utils.FileUtil;
using WuWuPathUtil = WuWuFramework.Utils.PathUtil;

namespace WuWuFramework.Editor
{
    public class WuWuFrameworkConfigWindow : EditorWindow
    {
        private bool m_IsCheckVersion = false;
        private bool m_IsLoadFromAssetBundle = false;
        private bool m_IsOpenLog = false;
        private string m_AssetsPath = string.Empty;
        private string m_UIScriptsPath = string.Empty;
        private string m_ConfigDataPath = string.Empty;
        private string m_VersionFileName = string.Empty;
        private string m_AssetMapFileName = string.Empty;
        private UnityObject m_AssetsFolder = null;
        private UnityObject m_ConfigDataFolder = null;
        private UnityObject m_UIScriptsFolder = null;
        private UnityObject m_LanguageKeyFile = null;
        private Color m_LogColor = Color.white;
        private string m_BuildPath = string.Empty;

        private WuWuFrameworkConfigWindowData m_EditorConfig = null;

        public WuWuFrameworkConfigWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
        }

        private void OnEnable()
        {
            InitConfig();
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(m_EditorConfig);
            SerializedObject s = new(m_EditorConfig);
            s.ApplyModifiedProperties();
            m_EditorConfig = null;
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private void OnGUI()
        {
            InitConfig();
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            DrawField(() => { return m_EditorConfig.isCheckVersion != m_IsCheckVersion; },
                () => { m_IsCheckVersion = EditorGUILayout.Toggle("是否进行版本检查", m_IsCheckVersion); },
                () => { m_EditorConfig.isCheckVersion = m_IsCheckVersion; }, 20);

            DrawField(
                () => { return m_EditorConfig.isLoadFromAssetBundle != m_IsLoadFromAssetBundle; },
                () => { m_IsLoadFromAssetBundle = EditorGUILayout.Toggle("是否从AssetBundle加载资源", m_IsLoadFromAssetBundle); },
                () => { m_EditorConfig.isLoadFromAssetBundle = m_IsLoadFromAssetBundle; }, 20);

            DrawField(
                () => { return m_EditorConfig.isOpenLog != m_IsOpenLog; },
                () => { m_IsOpenLog = EditorGUILayout.Toggle("是否开启日志", m_IsOpenLog); },
                () => { m_EditorConfig.isOpenLog = m_IsOpenLog; }, 20);

            DrawField(
                () => { return m_AssetsFolder == null || m_EditorConfig.assetsPath != m_AssetsPath; },
                () => {
                    UnityObject folder = EditorGUILayout.ObjectField("资源根目录", m_AssetsFolder, typeof(DefaultAsset), false);
                    if (folder != m_AssetsFolder)
                    {
                        m_AssetsFolder = folder;
                        m_AssetsPath = WuWuPathUtil.GetAssetPath(AssetDatabase.GetAssetPath(folder));
                    }
                },
                () => {
                    string uiPath = WuWuPathUtil.GetAssetPath(WuWuPathUtil.FormatPath(m_AssetsPath, EditorPathUtil.DefaultUIPath));
                    string uiPrefabsPath = WuWuPathUtil.FormatPath(uiPath, EditorPathUtil.UIPrefabsPath);
                    string uiAtlasPath = WuWuPathUtil.FormatPath(uiPath, EditorPathUtil.UIAtlasPath);
                    string uiScenesPath = WuWuPathUtil.FormatPath(uiPath, EditorPathUtil.UIScenesPath);
                    string uiSpritesPath = WuWuPathUtil.FormatPath(uiPath, EditorPathUtil.UISpritesPath);
                    string assetsFullPath = WuWuPathUtil.GetAssetFullPath(m_AssetsPath);
                    string uiFullPath = WuWuPathUtil.GetAssetFullPath(uiPath);
                    string uiPrefabsFullPath = WuWuPathUtil.FormatPath(uiFullPath, EditorPathUtil.UIPrefabsPath);
                    string uiAtlasFullPath = WuWuPathUtil.FormatPath(uiFullPath, EditorPathUtil.UIAtlasPath);
                    string uiScenesFullPath = WuWuPathUtil.FormatPath(uiFullPath, EditorPathUtil.UIScenesPath);
                    string uiSpritesFullPath = WuWuPathUtil.FormatPath(uiFullPath, EditorPathUtil.UISpritesPath);
                    WuWuFileUtil.VerifyDirectory(assetsFullPath);
                    WuWuFileUtil.VerifyDirectory(uiFullPath);
                    WuWuFileUtil.VerifyDirectory(uiPrefabsFullPath);
                    WuWuFileUtil.VerifyDirectory(uiAtlasFullPath);
                    WuWuFileUtil.VerifyDirectory(uiScenesFullPath);
                    WuWuFileUtil.VerifyDirectory(uiSpritesFullPath);
                    AssetDatabase.Refresh();
                    m_EditorConfig.assetsPath = m_AssetsPath;
                    m_EditorConfig.uiPath = uiPath;
                    m_EditorConfig.uiPrefabsPath = uiPrefabsPath;
                    m_EditorConfig.uiAtlasPath = uiAtlasPath;
                    m_EditorConfig.uiScenesPath = uiScenesPath;
                    m_EditorConfig.uiSpritesPath = uiSpritesPath;
                }, 20, m_EditorConfig.assetsPath == m_AssetsPath);

            GUI.enabled = false;
            EditorGUILayout.TextField("UI资源目录", m_EditorConfig.uiPath);
            GUI.enabled = true;

            DrawField(
                () => { return m_ConfigDataFolder == null || m_EditorConfig.configDataPath != m_ConfigDataPath; },
                () => {
                    UnityObject folder = EditorGUILayout.ObjectField("配置文件目录", m_ConfigDataFolder, typeof(DefaultAsset), false);
                    if (folder != m_ConfigDataFolder)
                    {
                        m_ConfigDataFolder = folder;
                        m_ConfigDataPath = WuWuPathUtil.GetAssetPath(AssetDatabase.GetAssetPath(folder));
                    }
                },
                () => {
                    WuWuFileUtil.VerifyDirectory(WuWuPathUtil.GetAssetFullPath(m_ConfigDataPath));
                    AssetDatabase.Refresh();
                    m_EditorConfig.configDataPath = m_ConfigDataPath;
                }, 20);
            DrawField(
                () => { return m_UIScriptsFolder == null || m_EditorConfig.uiScriptsPath != m_UIScriptsPath; },
                () => {
                    UnityObject folder = EditorGUILayout.ObjectField("UI脚本目录", m_UIScriptsFolder, typeof(DefaultAsset), false);
                    if (folder != m_UIScriptsFolder)
                    {
                        m_UIScriptsFolder = folder;
                        m_UIScriptsPath = WuWuPathUtil.GetAssetPath(AssetDatabase.GetAssetPath(folder));
                    }
                },
                () => {
                    WuWuFileUtil.VerifyDirectory(WuWuPathUtil.GetAssetFullPath(m_UIScriptsPath));
                    AssetDatabase.Refresh();
                    m_EditorConfig.uiScriptsPath = m_UIScriptsPath;
                }, 20);

            DrawField(
                () => { return m_EditorConfig.versionFileName != m_VersionFileName; },
                () => { m_VersionFileName = EditorGUILayout.TextField("资源版本文件名称", m_VersionFileName); },
                () => {
                    if (string.IsNullOrEmpty(m_VersionFileName))
                    {
                        m_VersionFileName = EditorPathUtil.VersionFileDefaultName;
                    }

                    if (string.IsNullOrEmpty(Path.GetExtension(m_VersionFileName)))
                    {
                        m_VersionFileName += EditorPathUtil.VersionFileDefaultExt;
                    }

                    m_EditorConfig.versionFileName = m_VersionFileName;
                }, 20);

            DrawField(
                   () => { return m_EditorConfig.assetMapFileName != m_AssetMapFileName; },
                   () => { m_AssetMapFileName = EditorGUILayout.TextField("资源映射文件名称", m_AssetMapFileName); },
                   () => {
                       if (string.IsNullOrEmpty(m_AssetMapFileName))
                       {
                           m_AssetMapFileName = EditorPathUtil.AssetMapFileDefaultName;
                       }

                       if (string.IsNullOrEmpty(Path.GetExtension(m_AssetMapFileName)))
                       {
                           m_AssetMapFileName += EditorPathUtil.AssetMapFileDefaultExt;
                       }

                       m_EditorConfig.assetMapFileName = m_AssetMapFileName;
                   }, 20);

            DrawField(
                () => {
                    GUI.enabled = false;
                    EditorGUILayout.TextField("打包绝对路径", m_BuildPath);
                    GUI.enabled = true;
                },
                () =>
                {
                    string path = EditorUtility.OpenFolderPanel("路径选择", "", "");
                    m_BuildPath = path.Replace("\\", "/");
                    m_EditorConfig.buildPath = m_BuildPath;
                }, 20, "选择");

            DrawField(
                () => { return m_EditorConfig.logColor != m_LogColor; },
                () => { m_LogColor = EditorGUILayout.ColorField("日志文本颜色", m_LogColor); },
                () => { m_EditorConfig.logColor = m_LogColor; }, 20);

            m_LanguageKeyFile = EditorGUILayout.ObjectField("多语言矫正文件", m_LanguageKeyFile, typeof(TextAsset), false);
            m_EditorConfig.languageKeyFilePath = AssetDatabase.GetAssetPath(m_LanguageKeyFile);

            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            Color oriColor = GUI.color;
            CheckEntry();
            GUI.color = oriColor;
        }

        private void CheckEntry()
        {
            if (string.IsNullOrEmpty(m_EditorConfig.entryScene))
            {
                GUI.color = Color.red;

                if (GUILayout.Button("设置当前场景为框架启动场景"))
                {
                    m_EditorConfig.entryScene = EditorSceneManager.GetActiveScene().path;
                    EditorMgr.GoToWuWuFrameworkEntryScene();
                    UnityEditor.EditorUtility.DisplayDialog("提示", "已设置当前场景为框架启动场景，请继续进框架行相关配置", "确认");
                }
                return;
            }

            GUI.color = Color.green;

            if (GUILayout.Button("跳转启动场景"))
            {
                if (m_EditorConfig.entryScene != EditorSceneManager.GetActiveScene().path)
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("提示", "是否跳转？", "确认", "取消"))
                    {
                        EditorMgr.GoToWuWuFrameworkEntryScene();
                    }
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog("提示", "当前已是启动场景，无需跳转", "确认");
                }
            }
        }

        private void InitConfig()
        {
            if (m_EditorConfig != null)
            {
                return;
            }

            m_EditorConfig = GetWuWuFrameworkConfig();
            m_IsCheckVersion = m_EditorConfig.isCheckVersion;
            m_IsLoadFromAssetBundle = m_EditorConfig.isLoadFromAssetBundle;
            m_IsOpenLog = m_EditorConfig.isOpenLog;
            m_AssetsPath = m_EditorConfig.assetsPath;
            m_ConfigDataPath = m_EditorConfig.configDataPath;
            m_UIScriptsPath = m_EditorConfig.uiScriptsPath;
            m_AssetMapFileName = m_EditorConfig.assetMapFileName;
            m_VersionFileName = m_EditorConfig.versionFileName;
            m_BuildPath = m_EditorConfig.buildPath;
            m_LogColor = m_EditorConfig.logColor;

            if (string.IsNullOrEmpty(m_VersionFileName))
            {
                m_VersionFileName = EditorPathUtil.VersionFileDefaultName;
                m_EditorConfig.versionFileName = m_VersionFileName;
            }

            if (string.IsNullOrEmpty(Path.GetExtension(m_VersionFileName)))
            {
                m_VersionFileName += EditorPathUtil.VersionFileDefaultExt;
                m_EditorConfig.versionFileName = m_VersionFileName;
            }

            if (string.IsNullOrEmpty(m_AssetMapFileName))
            {
                m_AssetMapFileName = EditorPathUtil.AssetMapFileDefaultName;
                m_EditorConfig.assetMapFileName = m_AssetMapFileName;
            }

            if (string.IsNullOrEmpty(Path.GetExtension(m_AssetMapFileName)))
            {
                m_AssetMapFileName += EditorPathUtil.AssetMapFileDefaultExt;
                m_EditorConfig.assetMapFileName = m_AssetMapFileName;
            }

            if (!string.IsNullOrEmpty(m_AssetsPath))
            {
                m_AssetsFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_AssetsPath.TrimEnd('/'));
            }

            if (!string.IsNullOrEmpty(m_ConfigDataPath))
            {
                m_ConfigDataFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_ConfigDataPath.TrimEnd('/'));
            }

            if (!string.IsNullOrEmpty(m_UIScriptsPath))
            {
                m_UIScriptsFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_UIScriptsPath.TrimEnd('/'));
            }

            if (!string.IsNullOrEmpty(m_EditorConfig.languageKeyFilePath))
            {
                m_LanguageKeyFile = AssetDatabase.LoadAssetAtPath<TextAsset>(m_EditorConfig.languageKeyFilePath);

                if (m_LanguageKeyFile == null)
                {
                    Debug.LogWarning($"多语言矫正文件 {m_EditorConfig.languageKeyFilePath} 不存在，请重新设置！");
                    m_EditorConfig.languageKeyFilePath = string.Empty;
                }
            }
            else
            {
                m_LanguageKeyFile = null;
            }
        }

        private WuWuFrameworkConfigWindowData GetWuWuFrameworkConfig()
        {
            WuWuFrameworkConfigWindowData config = null;
            WuWuFileUtil.VerifyDirectory(EditorPathUtil.EditorConfigFullPath);

            if (!File.Exists(EditorPathUtil.WuWuFrameWorkConfigWindowDataFullPath))
            {
                config = ScriptableObject.CreateInstance<WuWuFrameworkConfigWindowData>();
                AssetDatabase.CreateAsset(config, EditorPathUtil.WuWuFrameWorkConfigWindowDataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                config = AssetDatabase.LoadAssetAtPath<WuWuFrameworkConfigWindowData>(EditorPathUtil.WuWuFrameWorkConfigWindowDataPath);
            }

            return config;
        }

        private void DrawField(WuWuFrameworkFunc<bool> modify, WuWuFrameworkAction draw, WuWuFrameworkAction change, int changeBtnHeight, bool showMsg = true, bool alwaysCallChange = false)
        {
            EditorGUILayout.BeginHorizontal();
            bool isModify = modify.Invoke();
            Color oriColor = GUI.color;

            if (isModify)
            {
                GUI.color = Color.red;
            }

            draw?.Invoke();
            GUI.color = oriColor;

            if (GUILayout.Button("更改", GUILayout.Width(100), GUILayout.Height(changeBtnHeight)))
            {
                if (isModify || alwaysCallChange)
                {
                    change?.Invoke();

                    if (showMsg)
                    {
                        ShowNotification(new GUIContent("更改成功"));
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawField(WuWuFrameworkAction draw, WuWuFrameworkAction change, int changeBtnHeight, string changeBtnLable)
        {
            EditorGUILayout.BeginHorizontal();

            draw?.Invoke();

            if (GUILayout.Button(changeBtnLable, GUILayout.Width(100), GUILayout.Height(changeBtnHeight)))
            {
                change?.Invoke();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}