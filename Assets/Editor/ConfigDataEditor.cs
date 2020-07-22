using FrameWork;
using FrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ConfigDataEditor<T,P> : Editor where T:BaseScriptableObject<P> where P:BaseConfigData
{
    T Config;

    private void OnEnable()
    {
        Config = (target as T);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("排序"))
        {
            Array.Sort(Config.Datas);
        }
    }
}

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