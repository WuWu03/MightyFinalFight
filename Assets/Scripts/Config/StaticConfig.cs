using UnityEngine;
using UnityEditor;
using FrameWork.BehaviourTree;

public static class StaticConfig
{
    public static HeroConfig HeroConfig = null;
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static SceneObjectConfig SceneObjectConfig = null;
    public static EnemyConfig EnemyConfig = null;
    public static BehaviourTreeConfig BehaviourTreeConfig = null;
    public static void InitConfig()
    {
        HeroConfig = AssetDatabase.LoadAssetAtPath<HeroConfig>("Assets/ConfigData/HeroData.asset");
        SkillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>("Assets/ConfigData/SkillData.asset");
        StageConfig = AssetDatabase.LoadAssetAtPath<StageConfig>("Assets/ConfigData/StageData.asset");
        SceneObjectConfig = AssetDatabase.LoadAssetAtPath<SceneObjectConfig>("Assets/ConfigData/SceneObjectData.asset");
        EnemyConfig = AssetDatabase.LoadAssetAtPath<EnemyConfig>("Assets/ConfigData/EnemyData.asset");
        BehaviourTreeConfig = AssetDatabase.LoadAssetAtPath<BehaviourTreeConfig>("Assets/ConfigData/BehaviourTreeData.asset");
    }

    public static void Clear()
    {
    }
}
