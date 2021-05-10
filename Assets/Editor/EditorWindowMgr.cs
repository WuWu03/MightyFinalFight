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
        GameFrameWork.Editor.EditorUtility.CreateConfigData<SkillConfig, SkillData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<StageConfig, StageData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateHeroData")]
    public static void CreatePlayerData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<HeroConfig, HeroData>("HeroData", ".asset");
    }

    [MenuItem("Assets/Config/CreateEnemyData")]
    public static void CreateEnemyData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<EnemyConfig, EnemyData>("EnemyData", ".asset");
    }

    [MenuItem("Assets/Config/CreateSceneItemData")]
    public static void CreateSceneItemData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<SceneItemConfig, SceneItemData>("SceneItemData", ".asset");
    }

    [MenuItem("Assets/Config/CreateTaskData")]
    public static void CreateTaskData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<TaskConfig, TaskData>("TaskData", ".asset");
    }

    [MenuItem("Assets/Config/CreateLevelData")]
    public static void CreateLevelData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<LevelConfig, LevelData>("LevelData", ".asset");
    }
}