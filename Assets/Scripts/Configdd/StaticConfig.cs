using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    //public static BehaviourTreeConfig BehaviourTreeConfig = null;
    //public static SceneItemConfig SceneItemConfig = null;
    public static TaskConfig TaskConfig = null;


    public static void InitConfig()
    {
        SkillConfig = ResourcesMgr.instance.LoadAsset<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResourcesMgr.instance.LoadAsset<StageConfig>("ConfigData/StageData");
        //BehaviourTreeConfig = ResMgr.instance.LoadAsset<BehaviourTreeConfig>("ConfigData/BehaviourTreeData");
        //SceneItemConfig = ResourcesMgr.instance.LoadAsset<SceneItemConfig>("ConfigData/SceneItemData");
        TaskConfig = ResourcesMgr.instance.LoadAsset<TaskConfig>("ConfigData/TaskData");
    }

    public static void Clear()
    {
    }
}
