using GameFrameWork.Utilities;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(AppConfig))]
    public class AppConfigEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            m_AppConfig = (target as AppConfig);
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty checkVersion = serializedObject.FindProperty("checkVersion");
            SerializedProperty openUpdate = serializedObject.FindProperty("openUpdate");
            SerializedProperty loadAB = serializedObject.FindProperty("loadAB");
            SerializedProperty openLog = serializedObject.FindProperty("openLog");
            SerializedProperty useLua = serializedObject.FindProperty("useLua");
            SerializedProperty loadLuaAB = serializedObject.FindProperty("loadLuaAB");
            SerializedProperty luaByteMode = serializedObject.FindProperty("luaByteMode");
            SerializedProperty luaDirectory = serializedObject.FindProperty("luaDirectory");
            SerializedProperty uiDirectory = serializedObject.FindProperty("uiDirectory");
            SerializedProperty logColor = serializedObject.FindProperty("logColor");
            SerializedProperty versionFileName = serializedObject.FindProperty("versionFileName");

            EditorGUILayout.PropertyField(checkVersion);
            EditorGUILayout.PropertyField(openUpdate);
            EditorGUILayout.PropertyField(loadAB);
            EditorGUILayout.PropertyField(openLog);
            EditorGUILayout.PropertyField(useLua);


            if (m_AppConfig.useLua)
            {
                EditorGUILayout.PropertyField(loadLuaAB);
                EditorGUILayout.PropertyField(luaByteMode);
                EditorGUILayout.PropertyField(luaDirectory);
            }

            EditorGUILayout.PropertyField(uiDirectory);
            EditorGUILayout.PropertyField(logColor);


#if UNITY_IOS
            SerializedProperty iOSBuildPath = serializedObject.FindProperty("iosBuildPath");
            EditorGUILayout.PropertyField(iOSBuildPath);
#endif

#if UNITY_STANDALONE_WIN
            SerializedProperty pcBuildPath = serializedObject.FindProperty("pcBuildPath");
            EditorGUILayout.PropertyField(pcBuildPath);
#endif

#if UNITY_ANDROID
            SerializedProperty androidBuildPath = serializedObject.FindProperty("androidBuildPath");
            EditorGUILayout.PropertyField(androidBuildPath);
#endif

            if (!string.IsNullOrEmpty(versionFileName.stringValue) && !versionFileName.stringValue.Contains(".txt"))
            {
                versionFileName.stringValue += ".txt";
            }

            EditorGUILayout.PropertyField(versionFileName);


            if (checkVersion.boolValue != m_AppConfig.checkVersion
             || openUpdate.boolValue != m_AppConfig.openUpdate
             || loadAB.boolValue != m_AppConfig.loadAB
             || openLog.boolValue != m_AppConfig.openLog
             || useLua.boolValue != m_AppConfig.useLua
             || loadLuaAB.boolValue != m_AppConfig.loadLuaAB
             || luaByteMode.boolValue != m_AppConfig.luaByteMode
             || luaDirectory.stringValue != m_AppConfig.luaDirectory
             || logColor.colorValue != m_AppConfig.logColor
             || versionFileName.stringValue != m_AppConfig.versionFileName)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }

            m_AppConfig.checkVersion = checkVersion.boolValue;
            m_AppConfig.loadAB = loadAB.boolValue;
            m_AppConfig.openUpdate = openUpdate.boolValue;
            m_AppConfig.openLog = openLog.boolValue;
            m_AppConfig.useLua = useLua.boolValue;
            m_AppConfig.logColor = logColor.colorValue;
            m_AppConfig.versionFileName = versionFileName.stringValue;

#if UNITY_IOS
            if(appConfig.iosBuildPath != iosBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
            appConfig.iosBuildPath = iOSBuildPath.stringValue;
#endif

#if UNITY_STANDALONE_WIN
            if (m_AppConfig.pcBuildPath != pcBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);

                m_AppConfig.pcBuildPath = pcBuildPath.stringValue;

                if (!m_AppConfig.pcBuildPath.EndsWith(".exe"))
                {
                    m_AppConfig.pcBuildPath += ".exe";
                } 
            }
#endif

#if UNITY_ANDROID
            if (appConfig.androidBuildPath != androidBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
            appConfig.androidBuildPath = androidBuildPath.stringValue;
#endif

            if (m_AppConfig.useLua)
            {
                m_AppConfig.loadLuaAB = loadLuaAB.boolValue;
                m_AppConfig.luaByteMode = luaByteMode.boolValue;
                m_AppConfig.luaDirectory = luaDirectory.stringValue;
            }
            else
            {
                m_AppConfig.loadLuaAB = false;
                m_AppConfig.luaByteMode = false;
                m_AppConfig.luaDirectory = "Assets/Scripts/Lua";
            }

            if (m_AppConfig.uiDirectory != uiDirectory.stringValue)
            {
                m_AppConfig.uiDirectory = uiDirectory.stringValue;

                if (m_AppConfig.uiDirectory.EndsWith("/"))
                {
                    m_AppConfig.uiDirectory = m_AppConfig.uiDirectory.Substring(0, m_AppConfig.uiDirectory.Length - 1);
                }

                string uiPath = PathUtil.GetAssetFullPath(m_AppConfig.uiDirectory);
                string uiPrefabPath = PathUtil.FormatPath(uiPath, PathUtil.uiPrefabPath);
                string uiAtlasPath = PathUtil.FormatPath(uiPath, PathUtil.uiAtlasPath);
                GameFrameWork.Utilities.FileUtil.VerifyDirectory(uiPath);
                GameFrameWork.Utilities.FileUtil.VerifyDirectory(uiPrefabPath);
                GameFrameWork.Utilities.FileUtil.VerifyDirectory(uiAtlasPath);
            }
        }

        private AppConfig m_AppConfig;
    }
}