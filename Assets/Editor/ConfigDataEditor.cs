using GameFrameWork.BehaviourTree;
using GameFrameWork.Editor;
using UnityEditor;


[CustomEditor(typeof(EnemyConfig), true)]
public class EnemyConfigEditor : ConfigDataEditor<EnemyConfig, EnemyData> { }

[CustomEditor(typeof(HeroConfig), true)]
public class HeroConfigEditor : ConfigDataEditor<HeroConfig, HeroData>{}

[CustomEditor(typeof(SceneItemConfig), true)]
public class SceneItemConfigEditor : ConfigDataEditor<SceneItemConfig, SceneItemData> { }

[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillData> { }