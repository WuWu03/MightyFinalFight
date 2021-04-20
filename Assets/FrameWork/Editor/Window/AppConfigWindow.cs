using UnityEditor;


namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(AppConfig))]
    public class AppConfigWindow : UnityEditor.Editor
    {
        private AppConfig appConfig;

        void OnEnable()
        {
            appConfig = (target as AppConfig);
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty checkVersion = serializedObject.FindProperty("CheckVersion");
            SerializedProperty loadAB = serializedObject.FindProperty("LoadAB");
            SerializedProperty openLog = serializedObject.FindProperty("OpenLog");
            SerializedProperty useLua = serializedObject.FindProperty("UseLua");
            SerializedProperty loadLuaAB = serializedObject.FindProperty("LoadLuaAB");
            SerializedProperty luaByteMode = serializedObject.FindProperty("LuaByteMode");
            SerializedProperty luaDirectory = serializedObject.FindProperty("LuaDirectory");

            EditorGUILayout.PropertyField(checkVersion);
            EditorGUILayout.PropertyField(loadAB);
            EditorGUILayout.PropertyField(openLog);
            EditorGUILayout.PropertyField(useLua);

            if (appConfig.UseLua)
            {
                EditorGUILayout.PropertyField(loadLuaAB);
                EditorGUILayout.PropertyField(luaByteMode);
                EditorGUILayout.PropertyField(luaDirectory);
            }

            if (checkVersion.boolValue != appConfig.CheckVersion
             || loadAB.boolValue != appConfig.LoadAB
             || openLog.boolValue != appConfig.OpenLog
             || useLua.boolValue != appConfig.UseLua
             || loadLuaAB.boolValue != appConfig.LoadLuaAB
             || luaByteMode.boolValue != appConfig.LuaByteMode
             || luaDirectory.stringValue != appConfig.LuaDirectory)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }

            appConfig.CheckVersion = checkVersion.boolValue;
            appConfig.LoadAB = loadAB.boolValue;
            appConfig.OpenLog = openLog.boolValue;
            appConfig.UseLua = useLua.boolValue;

            if (!appConfig.AssetsExtendName.Contains("."))
            {
                appConfig.AssetsExtendName = "." + appConfig.AssetsExtendName;
            }

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