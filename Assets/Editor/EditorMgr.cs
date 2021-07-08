using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class EditorMgr : MonoBehaviour
{
    [MenuItem("Tools/CharacterTriggerEditor")]
    public static void OpenCharacterTriggerEditor()
    {
        EditorWindow.GetWindow<CharacterTriggerEditor>(false, "CharacterTriggerEditor", false).Show();
    }

    [MenuItem("Assets/Config/CreateSkillData")]
    public static void CreateSkillData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<SkillConfig, SkillConfigData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<StageConfig, StageConfigData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateHeroData")]
    public static void CreatePlayerData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<HeroConfig, HeroConfigData>("HeroData", ".asset");
    }

    [MenuItem("Assets/Config/CreateEnemyData")]
    public static void CreateEnemyData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<EnemyConfig, EnemyConfigData>("EnemyData", ".asset");
    }

    [MenuItem("Assets/Config/CreateSceneItemData")]
    public static void CreateSceneItemData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<SceneItemConfig, SceneItemConfigData>("SceneItemData", ".asset");
    }

    [MenuItem("Assets/Config/CreateTaskData")]
    public static void CreateTaskData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<TaskConfig, TaskConfigData>("TaskData", ".asset");
    }

    [MenuItem("Assets/Config/CreateLevelData")]
    public static void CreateLevelData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<LevelConfig, LevelConfigData>("LevelData", ".asset");
    }
}