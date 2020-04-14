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

    [MenuItem("Assets/Config/CreateSkillData")]
    public static void CreateSkillData()
    {
        CreateConfigData<SkillConfig, SkillData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        CreateConfigData<StageConfig, StageData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateSceneObjectData")]
    public static void CreateSceneObjectData()
    {
        CreateConfigData<SceneObjectConfig, SceneObjectData>("SceneObjectData", ".asset");
    }

    [MenuItem("Assets/Config/CreateHeroData")]
    public static void CreatePlayerData()
    {
        CreateConfigData<HeroConfig, HeroData>("HeroData", ".asset");
    }

    private static void CreateConfigData<T,P>(string name,string ext)
        where T: BaseScriptableObject<P>
        where P:BaseConfigData
    {
        string directory = Application.dataPath + "/ConfigData/";
        string fileName = directory + name + ext;
        if (File.Exists(fileName))
        {
            return;
        }

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        T data = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(data, "Assets/ConfigData/" + name + ext);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
