using GameFrameWork.Utils;
using SkillNew;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class GameFrameWorkConfigWindow : EditorWindow
    {
        public GameFrameWorkConfigWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
        }

        private void OnEnable()
        {
            InitConfig();
            Debug.Log("GameFrameWorkConfigWindow OnEnable");
        }

        private void OnDisable()
        {
            Debug.Log("GameFrameWorkConfigWindow OnDisable");
            EditorUtility.SetDirty(m_EditorConfig);
            m_EditorConfig = null;
        }

        private void OnGUI()
        {
            InitConfig();

            DrawField(
                () => m_EditorConfig.isCheckVersion != m_IsCheckVersion, 
                () => m_IsCheckVersion = EditorGUILayout.Toggle("是否进行版本检查", m_IsCheckVersion), 
                () => m_EditorConfig.isCheckVersion = m_IsCheckVersion, 20);

            DrawField(
                () => m_EditorConfig.isLoadFromAssetBundle != m_IsLoadFromAssetBundle, 
                () => m_IsLoadFromAssetBundle = EditorGUILayout.Toggle("是否从AssetBundle加载资源", m_IsLoadFromAssetBundle), 
                () => m_EditorConfig.isLoadFromAssetBundle = m_IsLoadFromAssetBundle, 20);

            DrawField(
                () => m_EditorConfig.isOpenLog != m_IsOpenLog,
                () => m_IsOpenLog = EditorGUILayout.Toggle("是否开启日志", m_IsOpenLog),
                () => m_EditorConfig.isOpenLog = m_IsOpenLog, 20);

            DrawField(
                () => m_EditorConfig.isUseLua != m_IsUseLua,
                () => m_IsUseLua = EditorGUILayout.Toggle("是否使用Lua", m_IsUseLua),
                () => m_EditorConfig.isUseLua = m_IsUseLua, 20);

            if (m_IsUseLua)
            {
                DrawField(
                    () => m_EditorConfig.isLoadLuaFromAssetBundle != m_IsLoadLuaFromAssetBundle,
                    () => m_IsLoadLuaFromAssetBundle = EditorGUILayout.Toggle("Lua脚本是否从AssetBundle加载", m_IsLoadLuaFromAssetBundle),
                    () => m_EditorConfig.isLoadLuaFromAssetBundle = m_IsLoadLuaFromAssetBundle, 20);

                DrawField(
                    () => m_EditorConfig.isLuaByteMode != m_IsLuaByteMode,
                    () => m_IsLuaByteMode = EditorGUILayout.Toggle("Lua脚本是否使用字节模式", m_IsLuaByteMode),
                    () => m_EditorConfig.isLuaByteMode = m_IsLuaByteMode, 20);
                DrawField(
                    () => m_EditorConfig.luaPath != m_LuaPath,
                    () => m_LuaPath = EditorGUILayout.TextField("Lua脚本目录", m_LuaPath),
                    () => m_EditorConfig.luaPath = m_LuaPath, 20);
            }
            else
            {
                m_IsLoadLuaFromAssetBundle = false;
                m_IsLuaByteMode = false;
                m_LuaPath = string.Empty;

                m_EditorConfig.isLoadLuaFromAssetBundle = false;
                m_EditorConfig.isLuaByteMode = false;
                m_EditorConfig.luaPath = string.Empty;
            }

            DrawField(
                () => m_EditorConfig.uiPath != m_UIPath,
                () => m_UIPath = EditorGUILayout.TextField("UI目录", m_UIPath),
                () =>
                {
                    string uiPath = PathUtil.GetAssetPath(m_UIPath);
                    string uiPrefabPath = PathUtil.FormatPath(uiPath, "Prefabs");
                    string uiAtlasPath = PathUtil.FormatPath(uiPath, "Atlas");
                    string uiScenesPath = PathUtil.FormatPath(uiPath, "Scenes");

                    string uiFullPath = PathUtil.GetAssetFullPath(m_UIPath);
                    string uiPrefabsFullPath = PathUtil.FormatPath(uiFullPath, "Prefabs");
                    string uiAtlasFullPath = PathUtil.FormatPath(uiFullPath, "Atlas");
                    string uiScenesFullPath = PathUtil.FormatPath(uiFullPath, "Scenes");

                    Utils.FileUtil.VerifyDirectory(uiFullPath);
                    Utils.FileUtil.VerifyDirectory(uiPrefabsFullPath);
                    Utils.FileUtil.VerifyDirectory(uiAtlasFullPath);
                    Utils.FileUtil.VerifyDirectory(uiScenesFullPath);

                    AssetDatabase.Refresh();

                    m_UIPath = uiPath;
                    m_EditorConfig.uiPath = uiPath;
                    m_EditorConfig.uiPrefabsPath = uiPrefabPath;
                    m_EditorConfig.uiAtlasPath = uiAtlasPath;
                    PlayerPrefs.SetString(EditorPathUtil.uiScenesPath, uiScenesPath);
                }, 20);

            DrawField(
                () => m_EditorConfig.configDataPath != m_ConfigDataPath,
                () => m_ConfigDataPath = EditorGUILayout.TextField("配置文件目录", m_ConfigDataPath),
                () =>
                {
                    string configDataPath = PathUtil.GetAssetPath(m_ConfigDataPath);
                    string configDataFullPath = PathUtil.GetAssetFullPath(m_ConfigDataPath);

                    Utils.FileUtil.VerifyDirectory(configDataFullPath);

                    AssetDatabase.Refresh();
                    m_EditorConfig.configDataPath = configDataPath;
                }, 20);

            DrawField(
                () => m_EditorConfig.pcBuildPath != m_PCBuildPath,
                () => m_PCBuildPath = EditorGUILayout.TextField("PC平台打包路径", m_PCBuildPath),
                () => m_EditorConfig.pcBuildPath = m_PCBuildPath, 20);

            DrawField(
               () => m_EditorConfig.androidBuildPath != m_AndroidBuildPath,
               () => m_AndroidBuildPath = EditorGUILayout.TextField("Android平台打包路径", m_AndroidBuildPath),
               () => m_EditorConfig.androidBuildPath = m_AndroidBuildPath, 20);

            DrawField(
               () => m_EditorConfig.iosBuildPath != m_IOSBuildPath,
               () => m_IOSBuildPath = EditorGUILayout.TextField("iOS平台打包路径", m_IOSBuildPath),
               () => m_EditorConfig.iosBuildPath = m_IOSBuildPath, 20);

            DrawField(
                () => m_EditorConfig.versionFileName != m_VersionFileName,
                () => m_VersionFileName = EditorGUILayout.TextField("版本文件名", m_VersionFileName),
                () =>
                {
                    string ext = Path.GetExtension(m_VersionFileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        m_VersionFileName += ".txt";
                    }
                    else if (!ext.Equals(".txt"))
                    {
                        m_VersionFileName = Path.GetFileNameWithoutExtension(m_VersionFileName) + ".txt";
                    }

                    m_EditorConfig.versionFileName = m_VersionFileName;
                }, 20);

            DrawField(
                () => m_EditorConfig.logColor != m_LogColor,
                () => m_LogColor = EditorGUILayout.ColorField("日志文本颜色", m_LogColor),
                () => m_EditorConfig.logColor = m_LogColor, 20);

            GUILayout.FlexibleSpace();

            Color oriColor = GUI.color;
            if (string.IsNullOrEmpty(m_EditorConfig.entryScene))
            {
                GUI.color = Color.red;
            }

            if (GUILayout.Button("设置当前场景为框架启动场景"))
            {
                CheckEntry();
            }

            GUI.color = oriColor;
        }

        private void CheckEntry()
        {
            if (string.IsNullOrEmpty(m_EditorConfig.entryScene))
            {
                m_EditorConfig.entryScene = EditorSceneManager.GetActiveScene().path;
                EditorMgr.GoToGameFrameWorkEntryScene();
                UnityEditor.EditorUtility.DisplayDialog("提示", "已设置当前场景为框架启动场景，请继续进框架行相关配置", "确认");
                return;
            }

            bool result = UnityEditor.EditorUtility.DisplayDialog("提示", "已创建过启动场景，是否跳转？", "确认", "取消");

            if (result)
            {
                EditorMgr.GoToGameFrameWorkEntryScene();
            }
        }

        private void InitConfig()
        {
            if (m_EditorConfig != null)
            {
                return;
            }

            m_EditorConfig = GetGameFrameWorkConfig();

            m_IsCheckVersion = m_EditorConfig.isCheckVersion;
            m_IsLoadFromAssetBundle = m_EditorConfig.isLoadFromAssetBundle;
            m_IsOpenLog = m_EditorConfig.isOpenLog;
            m_IsUseLua = m_EditorConfig.isUseLua;
            m_IsLoadLuaFromAssetBundle = m_EditorConfig.isLoadLuaFromAssetBundle;
            m_IsLuaByteMode = m_EditorConfig.isLuaByteMode;
            m_LuaPath = m_EditorConfig.luaPath;
            m_UIPath = m_EditorConfig.uiPath;
            m_ConfigDataPath = m_EditorConfig.configDataPath;
            m_VersionFileName = m_EditorConfig.versionFileName;
            m_PCBuildPath = m_EditorConfig.pcBuildPath;
            m_AndroidBuildPath = m_EditorConfig.androidBuildPath;
            m_IOSBuildPath = m_EditorConfig.iosBuildPath;
            m_LogColor = m_EditorConfig.logColor;
        }

        private GameFrameWorkConfigWindowData GetGameFrameWorkConfig()
        {
            GameFrameWorkConfigWindowData config = null;

            Utils.FileUtil.VerifyDirectory(EditorPathUtil.editorConfigFullPath);

            if (!File.Exists(EditorPathUtil.gameFrameWorkConfigWindowDataFullPath))
            {
                config = ScriptableObject.CreateInstance<GameFrameWorkConfigWindowData>();
                AssetDatabase.CreateAsset(config, EditorPathUtil.gameFrameWorkConfigWindowDataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                config = AssetDatabase.LoadAssetAtPath<GameFrameWorkConfigWindowData>(EditorPathUtil.gameFrameWorkConfigWindowDataPath);
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
                        this.ShowNotification("更改成功");
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool m_IsCheckVersion = false;
        private bool m_IsLoadFromAssetBundle = false;
        private bool m_IsOpenLog = false;
        private bool m_IsUseLua = false;
        private bool m_IsLoadLuaFromAssetBundle = false;
        private bool m_IsLuaByteMode = false;
        private string m_LuaPath = string.Empty;
        private string m_UIPath = string.Empty;
        private string m_ConfigDataPath = string.Empty;
        private string m_VersionFileName = string.Empty;
        private Color m_LogColor = Color.white;

        private string m_PCBuildPath = string.Empty;
        private string m_AndroidBuildPath = string.Empty;
        private string m_IOSBuildPath = string.Empty;

        private GameFrameWorkConfigWindowData m_EditorConfig = null;
    }
}