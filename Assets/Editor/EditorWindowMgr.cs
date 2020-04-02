using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
public class EditorWindowMgr : MonoBehaviour
{
    [MenuItem("Tools/CharacterTriggerEditor")]
    public static void OpenCharacterTriggerEditor()
    {
        EditorWindow.GetWindow<CharacterTriggerEditor>(false, "CharacterTriggerEditor", false).Show();
    }

    [MenuItem("Assets/CreateSkillData")]
    public static void CreateSkillData()
    {
        if(File.Exists(Application.dataPath+ "/ConfigData/SkillData/SkillData.asset"))
        {
            return;
        }

        if (!Directory.Exists(Application.dataPath + "/ConfigData/SkillData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ConfigData/SkillData");
        }

        SkillConfig data = ScriptableObject.CreateInstance<SkillConfig>();
        AssetDatabase.CreateAsset(data, "Assets/ConfigData/SkillData/SkillData.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Assets/CreateStageData")]
    public static void CreateStageData()
    {
        if (File.Exists(Application.dataPath + "/ConfigData/StageData/StageData.asset"))
        {
            return;
        }

        if(!Directory.Exists(Application.dataPath + "/ConfigData/StageData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ConfigData/StageData");
        }
        StageConfig data = ScriptableObject.CreateInstance<StageConfig>();
        AssetDatabase.CreateAsset(data, "Assets/ConfigData/StageData/StageData.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
