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

    [MenuItem("Tools/MapEditor")]
    public static void OpenMapEditorWindow()
    {
        MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();
        window.Show();
    }

    [MenuItem("Tools/SkillEditor")]
    public static void OpenSkillEditorWindow()
    {
        SkillEditorWindow window = EditorWindow.GetWindow<SkillEditorWindow>();
        window.Show();
    }

    [MenuItem("Tools/Build/Build Game")]
    public static void BuildGame()
    {
        if (UnityEditor.EditorUtility.DisplayDialog("提示", "点击确认开始打包", "确认", "取消"))
        {
            BuildGame(false);
        }
    }

    [MenuItem("Tools/Build/Build Game Log")]
    public static void BuildGameLog()
    {
        if (UnityEditor.EditorUtility.DisplayDialog("提示", "点击确认开始打包", "确认", "取消"))
        {
            BuildGame(true);
        }
    }

    private static void BuildGame(bool openLog)
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
        AppConfig appConfig = GameObject.FindObjectOfType<AppConfig>();
        appConfig.loadAB = true;
        appConfig.openLog = openLog;

        using (AssetBundleBuilder builder = new AssetBundleBuilder())
        {
            builder.Build(BuildTarget.StandaloneWindows, false);
        }

        AssetDatabase.Refresh();

        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = appConfig.pcBuildPath;
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary buildSummary = buildReport.summary;

        if (buildSummary.result == BuildResult.Succeeded)
        {
            appConfig.loadAB = false;
            appConfig.openLog = true;
            Debug.Log("Build success");
        }
        else if (buildSummary.result == BuildResult.Failed)
        {
            Debug.LogError("Build windows error : [" + buildSummary.ToString() + "]");
        }
    }

    [MenuItem("Assets/Config/CreateSkillData")]
    public static void CreateSkillData()
    {
        GameFrameWork.Editor.EditorUtil.CreateConfigData<SkillConfig, SkillConfigData>("SkillData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        GameFrameWork.Editor.EditorUtil.CreateConfigData<StageConfig, StageConfigData>("StageData", ".asset");
    }


    [MenuItem("Assets/Config/CreateTaskData")]
    public static void CreateTaskData()
    {
        GameFrameWork.Editor.EditorUtil.CreateConfigData<TaskConfig, TaskConfigData>("TaskData", ".asset");
    }

}