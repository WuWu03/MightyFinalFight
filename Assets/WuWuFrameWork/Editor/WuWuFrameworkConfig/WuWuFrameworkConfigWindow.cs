using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WuWuFramework.Utils;

namespace WuWuFramework.Editor
{
    public class WuWuFrameworkConfigWindow : EditorWindow
    {
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
                () => { return m_EditorConfig.uiPath != m_UIPath; },
                () => { m_UIPath = EditorGUILayout.TextField("UI资源目录", m_UIPath); },
                () =>
                {
                    m_UIPath = PathUtil.GetAssetPath(m_UIPath);
                    string uiPath = PathUtil.GetAssetPath(m_UIPath);
                    string uiPrefabsPath = PathUtil.FormatPath(uiPath, EditorPathUtil.UIPrefabsPath);
                    string uiAtlasPath = PathUtil.FormatPath(uiPath, EditorPathUtil.UIAtlasPath);
                    string uiScenesPath = PathUtil.FormatPath(uiPath, EditorPathUtil.UIScenesPath);
                    string uiSpritesPath = PathUtil.FormatPath(uiPath, EditorPathUtil.UISpritesPath);

                    string uiFullPath = PathUtil.GetAssetFullPath(m_UIPath);
                    string uiPrefabsFullPath = PathUtil.FormatPath(uiFullPath, EditorPathUtil.UIPrefabsPath);
                    string uiAtlasFullPath = PathUtil.FormatPath(uiFullPath, EditorPathUtil.UIAtlasPath);
                    string uiScenesFullPath = PathUtil.FormatPath(uiFullPath, EditorPathUtil.UIScenesPath);
                    string uiSpritesFullPath = PathUtil.FormatPath(uiFullPath, EditorPathUtil.UISpritesPath);

                    Utils.FileUtil.VerifyDirectory(uiFullPath);
                    Utils.FileUtil.VerifyDirectory(uiPrefabsFullPath);
                    Utils.FileUtil.VerifyDirectory(uiAtlasFullPath);
                    Utils.FileUtil.VerifyDirectory(uiScenesFullPath);
                    Utils.FileUtil.VerifyDirectory(uiSpritesFullPath);

                    AssetDatabase.Refresh();

                    m_UIPath = uiPath;
                    m_EditorConfig.uiPath = uiPath;
                    m_EditorConfig.uiPrefabsPath = uiPrefabsPath;
                    m_EditorConfig.uiAtlasPath = uiAtlasPath;
                    m_EditorConfig.uiScenesPath = uiScenesPath;
                    m_EditorConfig.uiSpritesPath = uiSpritesPath;
                }, 20);

            DrawField(
                () => { return m_EditorConfig.configDataPath != m_ConfigDataPath; },
                () => { m_ConfigDataPath = EditorGUILayout.TextField("配置文件目录", m_ConfigDataPath); },
                () =>
                {
                    m_ConfigDataPath =  PathUtil.GetAssetPath(m_ConfigDataPath);
                    Utils.FileUtil.VerifyDirectory(PathUtil.GetAssetFullPath(m_ConfigDataPath));
                    AssetDatabase.Refresh();
                    m_EditorConfig.configDataPath = m_ConfigDataPath;
                }, 20);
            DrawField(
                () => { return m_EditorConfig.uiScriptsPath != m_UIScriptsPath; },
                () => { m_UIScriptsPath = EditorGUILayout.TextField("UI脚本目录", m_UIScriptsPath); },
                () =>
                {
                    m_UIScriptsPath =  PathUtil.GetAssetFullPath(m_ConfigDataPath);
                    Utils.FileUtil.VerifyDirectory(PathUtil.GetAssetFullPath(m_UIScriptsPath));
                    AssetDatabase.Refresh();
                    m_EditorConfig.uiScriptsPath = m_UIScriptsPath;
                }, 20);

            DrawField(
                () => { return m_EditorConfig.versionFileName != m_VersionFileName; },
                () => { m_VersionFileName = EditorGUILayout.TextField("资源版本文件名称", m_VersionFileName); },
                () =>
                {
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
                   () =>
                   {
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
                () => { return m_EditorConfig.buildPath != m_BuildPath; },
                () => { m_BuildPath = EditorGUILayout.TextField("打包绝对路径", m_BuildPath); },
                () => { m_EditorConfig.buildPath = m_BuildPath; }, 20);

            DrawField(
                () => { return m_EditorConfig.logColor != m_LogColor; },
                () => { m_LogColor = EditorGUILayout.ColorField("日志文本颜色", m_LogColor); },
                () => { m_EditorConfig.logColor = m_LogColor; }, 20);

            m_LanguageKeyFile = EditorGUILayout.ObjectField("多语言矫正文件", m_LanguageKeyFile, typeof(TextAsset),false);
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
                if(m_EditorConfig.entryScene != EditorSceneManager.GetActiveScene().path)
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
            m_UIPath = m_EditorConfig.uiPath;
            m_ConfigDataPath = m_EditorConfig.configDataPath;
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

            if(string.IsNullOrEmpty(Path.GetExtension(m_AssetMapFileName)))
            {
                m_AssetMapFileName += EditorPathUtil.AssetMapFileDefaultExt;
                m_EditorConfig.assetMapFileName = m_AssetMapFileName;
            }

            if (string.IsNullOrEmpty(m_UIPath))
            {
                m_UIPath = PathUtil.GetAssetPath(EditorPathUtil.DefaultUIPath);
                m_EditorConfig.uiPath = m_UIPath;
            }

            if (string.IsNullOrEmpty(m_UIScriptsPath))
            {
                m_UIScriptsPath = PathUtil.GetAssetPath(EditorPathUtil.DefaultUIScriptsPath);
                m_EditorConfig.uiScriptsPath = m_UIScriptsPath;
            }
            
            if (string.IsNullOrEmpty(m_ConfigDataPath))
            {
                m_ConfigDataPath = PathUtil.GetAssetPath(EditorPathUtil.DefaultConfigDataPath);
                m_EditorConfig.configDataPath = m_ConfigDataPath;
            }

            if (!string.IsNullOrEmpty(m_EditorConfig.languageKeyFilePath))
            {
                m_LanguageKeyFile = AssetDatabase.LoadAssetAtPath<TextAsset>(m_EditorConfig.languageKeyFilePath);

                if(m_LanguageKeyFile == null)
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

            Utils.FileUtil.VerifyDirectory(EditorPathUtil.EditorConfigFullPath);

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

        private void DrawField(Func<bool> modify, Action draw, Action change, int changeBtnHeight, bool showMsg = true)
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
                if(isModify)
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

        private bool m_IsCheckVersion = false;
        private bool m_IsLoadFromAssetBundle = false;
        private bool m_IsOpenLog = false;
        private string m_UIPath = string.Empty;
        private string m_UIScriptsPath = string.Empty;
        private string m_ConfigDataPath = string.Empty;
        private string m_VersionFileName = string.Empty;
        private string m_AssetMapFileName = string.Empty;
        private Color m_LogColor = Color.white;

        private string m_BuildPath = string.Empty;
        private UnityEngine.Object m_LanguageKeyFile = null;
        private WuWuFrameworkConfigWindowData m_EditorConfig = null;
    }
}