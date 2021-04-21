using GameFrameWork.BehaviourTree;
using GameFrameWork.Editor;
using UnityEditor;


[CustomEditor(typeof(BehaviourTreeConfig), true)]
public class BehaviourTreeConfigEditor : ConfigDataEditor<BehaviourTreeConfig, BehaviourTreeData> { }

[CustomEditor(typeof(EnemyConfig), true)]
public class EnemyConfigEditor : ConfigDataEditor<EnemyConfig, EnemyData> { }

[CustomEditor(typeof(HeroConfig), true)]
public class HeroConfigEditor : ConfigDataEditor<HeroConfig, HeroData>{}

[CustomEditor(typeof(SceneItemConfig), true)]
public class SceneItemConfigEditor : ConfigDataEditor<SceneItemConfig, SceneItemData> { }

[CustomEditor(typeof(SkillConfig), true)]
public class SkillConfigEditor : ConfigDataEditor<SkillConfig, SkillData> { }