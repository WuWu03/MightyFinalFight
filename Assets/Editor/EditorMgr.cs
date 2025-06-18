using GameFrameWork;
using GameFrameWork.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

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

    [MenuItem("Tools/NewSkillEditor")]
    public static void OpenNewSkillEditorWindow()
    {
        SkillNew.SkillEditorWindow window = EditorWindow.GetWindow<SkillNew.SkillEditorWindow>();
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
        BuildGame(false);

    }

    [MenuItem("Tools/Build/Build Game Log")]
    public static void BuildGameLog()
    {
        BuildGame(true);
    }

    private static void BuildGame(bool openLog)
    {
        if(!GameFrameWork.Editor.EditorMgr.CheckEntryScene())
        {
            UnityEditor.EditorUtility.DisplayDialog("提示", "没有创建启动场景，无法打包", "确认");
            return;
        }

        if (!UnityEditor.EditorUtility.DisplayDialog("提示", "点击确认开始打包", "确认", "取消"))
        {
            return;
        }

        AppConfig.instance.loadAB = true;
        AppConfig.instance.openLog = openLog;
        EditorUtility.SetDirty(AppConfig.instance);
        EditorSceneManager.SaveOpenScenes();

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
        buildPlayerOptions.locationPathName = AppConfig.instance.pcBuildPath;
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;
        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary buildSummary = buildReport.summary;

        if (buildSummary.result == BuildResult.Succeeded)
        {
            AppConfig.instance.loadAB = false;
            AppConfig.instance.openLog = true;
            EditorUtility.SetDirty(AppConfig.instance);
            EditorSceneManager.SaveOpenScenes();
            UnityEditor.EditorUtility.DisplayDialog("提示", "打包成功", "确认");
            System.Diagnostics.Process.Start("explorer.exe", Path.GetFullPath(AppConfig.instance.pcBuildPath) + @"\");
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