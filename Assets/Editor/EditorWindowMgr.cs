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
        GameFrameWork.Editor.Utility.CreateConfigData<SkillConfig, SkillData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<StageConfig, StageData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateHeroData")]
    public static void CreatePlayerData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<HeroConfig, HeroData>("HeroData", ".asset");
    }

    [MenuItem("Assets/Config/CreateEnemyData")]
    public static void CreateEnemyData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<EnemyConfig, EnemyData>("EnemyData", ".asset");
    }

    [MenuItem("Assets/Config/CreateSceneItemData")]
    public static void CreateSceneItemData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<SceneItemConfig, SceneItemData>("SceneItemData", ".asset");
    }

    [MenuItem("Assets/Config/CreateTaskData")]
    public static void CreateTaskData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<TaskConfig, TaskData>("TaskData", ".asset");
    }

    [MenuItem("Assets/Config/CreateLevelData")]
    public static void CreateLevelData()
    {
        GameFrameWork.Editor.Utility.CreateConfigData<LevelConfig, LevelData>("LevelData", ".asset");
    }
}