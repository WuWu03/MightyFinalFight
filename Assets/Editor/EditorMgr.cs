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