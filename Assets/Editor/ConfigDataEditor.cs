using WuWuFramework.Editor;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TaskConfig), true)]
public class TaskConfigEditor : ConfigDataEditor<TaskConfig, TaskConfigData> { }


[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillConfigData> 
{
    public override void OnInspectorGUI()
    {
        //GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;
    }
}

[CustomEditor(typeof(StageConfig),true)]
public class StageConfigEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;
    }
}

[CustomEditor(typeof(MapEditorConfig), true)]
public class MapEditorConfigEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;
    }
}