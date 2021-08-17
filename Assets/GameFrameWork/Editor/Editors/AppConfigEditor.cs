using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(AppConfig))]
    public class AppConfigEditor : UnityEditor.Editor
    {
        private AppConfig appConfig;

        void OnEnable()
        {
            appConfig = (target as AppConfig);
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty checkVersion = serializedObject.FindProperty("CheckVersion");
            SerializedProperty openUpdate = serializedObject.FindProperty("OpenUpdate");
            SerializedProperty loadAB = serializedObject.FindProperty("LoadAB");
            SerializedProperty openLog = serializedObject.FindProperty("OpenLog");
            SerializedProperty useLua = serializedObject.FindProperty("UseLua");
            SerializedProperty loadLuaAB = serializedObject.FindProperty("LoadLuaAB");
            SerializedProperty luaByteMode = serializedObject.FindProperty("LuaByteMode");
            SerializedProperty luaDirectory = serializedObject.FindProperty("LuaDirectory");
            SerializedProperty logColor = serializedObject.FindProperty("LogColor");

            EditorGUILayout.PropertyField(checkVersion);
            EditorGUILayout.PropertyField(openUpdate);
            EditorGUILayout.PropertyField(loadAB);
            EditorGUILayout.PropertyField(openLog);
            EditorGUILayout.PropertyField(useLua);
            EditorGUILayout.PropertyField(logColor);

#if UNITY_IOS
            SerializedProperty iOSBuildPath = serializedObject.FindProperty("iOSBuildPath");
            EditorGUILayout.PropertyField(iOSBuildPath);
#endif

#if UNITY_STANDALONE_WIN
            SerializedProperty pcBuildPath = serializedObject.FindProperty("PCBuildPath");
            EditorGUILayout.PropertyField(pcBuildPath);
#endif

#if UNITY_ANDROID
            SerializedProperty androidBuildPath = serializedObject.FindProperty("AndroidBuildPath");
            EditorGUILayout.PropertyField(androidBuildPath);
#endif

            if (appConfig.UseLua)
            {
                EditorGUILayout.PropertyField(loadLuaAB);
                EditorGUILayout.PropertyField(luaByteMode);
                EditorGUILayout.PropertyField(luaDirectory);
            }

            if (checkVersion.boolValue != appConfig.CheckVersion
             || openUpdate.boolValue != appConfig.OpenUpdate
             || loadAB.boolValue != appConfig.LoadAB
             || openLog.boolValue != appConfig.OpenLog
             || useLua.boolValue != appConfig.UseLua
             || loadLuaAB.boolValue != appConfig.LoadLuaAB
             || luaByteMode.boolValue != appConfig.LuaByteMode
             || luaDirectory.stringValue != appConfig.LuaDirectory
             || logColor.colorValue != appConfig.LogColor)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }

            appConfig.CheckVersion = checkVersion.boolValue;
            appConfig.LoadAB = loadAB.boolValue;
            appConfig.OpenUpdate = openUpdate.boolValue;
            appConfig.OpenLog = openLog.boolValue;
            appConfig.UseLua = useLua.boolValue;
            appConfig.LogColor = logColor.colorValue;

#if UNITY_IOS
            if(appConfig.iOSBuildPath != iOSBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
            appConfig.iOSBuildPath = iOSBuildPath.stringValue;
#endif

#if UNITY_STANDALONE_WIN
            if (appConfig.PCBuildPath != pcBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
            appConfig.PCBuildPath = pcBuildPath.stringValue;
#endif

#if UNITY_ANDROID
            if (appConfig.AndroidBuildPath != androidBuildPath.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }
            appConfig.AndroidBuildPath = androidBuildPath.stringValue;
#endif

            if (appConfig.UseLua)
            {
                appConfig.LoadLuaAB = loadLuaAB.boolValue;
                appConfig.LuaByteMode = luaByteMode.boolValue;
                appConfig.LuaDirectory = luaDirectory.stringValue;
            }
            else
            {
                appConfig.LoadLuaAB = false;
                appConfig.LuaByteMode = false;
                appConfig.LuaDirectory = "Assets/Scripts/Lua";
            }
        }
    }
}