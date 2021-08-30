using GameFrameWork.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterConfig), true)]
public class HeroConfigEditor : ConfigDataEditor<CharacterConfig, CharacterConfigData>{}

[CustomEditor(typeof(SceneItemConfig), true)]
public class SceneItemConfigEditor : ConfigDataEditor<SceneItemConfig, SceneItemConfigData> { }

[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillConfigData> 
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
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