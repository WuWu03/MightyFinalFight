using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;

    public static void InitConfig()
    {
        SkillConfig = GameEntry.resourceMgr.Load<SkillConfig>("ConfigData/SkillConfig.asset");
        StageConfig = GameEntry.resourceMgr.Load<StageConfig>("ConfigData/StageConfig.asset");
        TaskConfig = GameEntry.resourceMgr.Load<TaskConfig>("ConfigData/TaskConfig.asset");
    }

    public static void ShutDown()
    {
        SkillConfig = null;
        StageConfig = null;
        TaskConfig = null;
    }
}
