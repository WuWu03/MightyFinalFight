using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    //public static BehaviourTreeConfig BehaviourTreeConfig = null;
    public static SceneItemConfig SceneItemConfig = null;
    public static TaskConfig TaskConfig = null;
    public static LevelConfig LevelConfig = null;

    public static void InitConfig()
    {
        SkillConfig = ResMgr.instance.LoadAsset<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResMgr.instance.LoadAsset<StageConfig>("ConfigData/StageData");
        //BehaviourTreeConfig = ResMgr.instance.LoadAsset<BehaviourTreeConfig>("ConfigData/BehaviourTreeData");
        SceneItemConfig = ResMgr.instance.LoadAsset<SceneItemConfig>("ConfigData/SceneItemData");
        TaskConfig = ResMgr.instance.LoadAsset<TaskConfig>("ConfigData/TaskData");
        LevelConfig = ResMgr.instance.LoadAsset<LevelConfig>("ConfigData/LevelData");
    }

    public static void Clear()
    {
    }
}
