using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using GameFrameWork;
using GameFrameWork.Editor;
using UnityEditor.Build.Reporting;

public class EditorMgr : MonoBehaviour
{
    [MenuItem("Tools/CharacterTriggerEditor")]
    public static void OpenCharacterTriggerEditorWindow()
    {
        EditorWindow.GetWindow<CharacterTriggerEditorWindow>(false, "CharacterTriggerEditor", false).Show();
    }

    [MenuItem("Tools/Build Game")]
    public static void BuildGame()
    {
        BuildGame(false);
    }

    [MenuItem("Tools/Build Game Log")]
    public static void BuildGameLog()
    {
        BuildGame(true);
    }

    private static void BuildGame(bool openLog)
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        AppConfig appConfig = GameObject.FindObjectOfType<AppConfig>();
        appConfig.LoadAB = true;
        appConfig.OpenLog = openLog;
        AssetBundleBuilder.Build(BuildTarget.StandaloneWindows, false);
        AssetDatabase.Refresh();

        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = appConfig.PCBuildPath;
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary buildSummary = buildReport.summary;

        if(buildSummary.result == BuildResult.Succeeded)
        {
            appConfig.LoadAB = false;
            appConfig.OpenLog = true;
            Debug.Log("Build success");
        }
        else if (buildSummary.result == BuildResult.Failed)
        {
            Debug.LogError("Build windows error : [" + buildSummary.ToString() + "]");
        }
    }

    [MenuItem("Tools/MapEditor")]
    public static void OpenMapEditorWindow()
    {
        MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
        window.Show();
    }

    [MenuItem("Assets/Config/CreateSkillData")]
    public static void CreateSkillData()
    {
        GameFrameWork.Editor.EditorUtility.CreateConfigData<SkillConfig, SkillConfigData>("SkillData", ".asset");
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