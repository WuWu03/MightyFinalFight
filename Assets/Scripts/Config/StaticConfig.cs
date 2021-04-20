using UnityEngine;
using UnityEditor;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Resources;

public static class StaticConfig
{
    public static HeroConfig HeroConfig = null;
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static EnemyConfig EnemyConfig = null;
    public static BehaviourTreeConfig BehaviourTreeConfig = null;
    public static SceneItemConfig SceneItemConfig = null;
    public static TaskConfig TaskConfig = null;
    public static LevelConfig LevelConfig = null;
    public static void InitConfig()
    {
        HeroConfig = ResMgr.Ins.LoadAsset<HeroConfig>("ConfigData/HeroData");
        SkillConfig = ResMgr.Ins.LoadAsset<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResMgr.Ins.LoadAsset<StageConfig>("ConfigData/StageData");
        EnemyConfig = ResMgr.Ins.LoadAsset<EnemyConfig>("ConfigData/EnemyData");
        BehaviourTreeConfig = ResMgr.Ins.LoadAsset<BehaviourTreeConfig>("ConfigData/BehaviourTreeData");
        SceneItemConfig = ResMgr.Ins.LoadAsset<SceneItemConfig>("ConfigData/SceneItemData");
        TaskConfig = ResMgr.Ins.LoadAsset<TaskConfig>("ConfigData/TaskData");
        LevelConfig = ResMgr.Ins.LoadAsset<LevelConfig>("ConfigData/LevelData");
    }

    public static void Clear()
    {
    }
}
