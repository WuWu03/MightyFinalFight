using GameFrameWork.BehaviourTree;
using GameFrameWork.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyConfig), true)]
public class EnemyConfigEditor : ConfigDataEditor<EnemyConfig, EnemyConfigData> { }

[CustomEditor(typeof(HeroConfig), true)]
public class HeroConfigEditor : ConfigDataEditor<HeroConfig, HeroConfigData>{}

[CustomEditor(typeof(SceneItemConfig), true)]
public class SceneItemConfigEditor : ConfigDataEditor<SceneItemConfig, SceneItemConfigData> { }

[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillConfigData> { }

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