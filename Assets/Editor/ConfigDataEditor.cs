using GameFrameWork.BehaviourTree;
using GameFrameWork.Editor;
using UnityEditor;


[CustomEditor(typeof(EnemyConfig), true)]
public class EnemyConfigEditor : ConfigDataEditor<EnemyConfig, EnemyConfigData> { }

[CustomEditor(typeof(HeroConfig), true)]
public class HeroConfigEditor : ConfigDataEditor<HeroConfig, HeroConfigData>{}

[CustomEditor(typeof(SceneItemConfig), true)]
public class SceneItemConfigEditor : ConfigDataEditor<SceneItemConfig, SceneItemConfigData> { }

[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillConfigData> { }