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
            SerializedProperty logColor = serializedObject.FindProperty("logColor");

            EditorGUILayout.PropertyField(checkVersion);
            EditorGUILayout.PropertyField(openUpdate);
            EditorGUILayout.PropertyField(loadAB);
            EditorGUILayout.PropertyField(openLog);
            EditorGUILayout.PropertyField(useLua);
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

            if (m_AppConfig.useLua)
            {
                EditorGUILayout.PropertyField(loadLuaAB);
                EditorGUILayout.PropertyField(luaByteMode);
                EditorGUILayout.PropertyField(luaDirectory);
            }

            if (checkVersion.boolValue != m_AppConfig.checkVersion
             || openUpdate.boolValue != m_AppConfig.openUpdate
             || loadAB.boolValue != m_AppConfig.loadAB
             || openLog.boolValue != m_AppConfig.openLog
             || useLua.boolValue != m_AppConfig.useLua
             || loadLuaAB.boolValue != m_AppConfig.loadLuaAB
             || luaByteMode.boolValue != m_AppConfig.luaByteMode
             || luaDirectory.stringValue != m_AppConfig.luaDirectory
             || logColor.colorValue != m_AppConfig.logColor)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }

            m_AppConfig.checkVersion = checkVersion.boolValue;
            m_AppConfig.loadAB = loadAB.boolValue;
            m_AppConfig.openUpdate = openUpdate.boolValue;
            m_AppConfig.openLog = openLog.boolValue;
            m_AppConfig.useLua = useLua.boolValue;
            m_AppConfig.logColor = logColor.colorValue;

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
            }
            m_AppConfig.pcBuildPath = pcBuildPath.stringValue;
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
        }

        private AppConfig m_AppConfig;
    }
}