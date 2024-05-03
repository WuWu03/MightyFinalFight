using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;

    public static void InitConfig()
    {
        SkillConfig = ResourcesMgr.instance.LoadAssetSync<SkillConfig>("ConfigData/SkillData.asset");
        StageConfig = ResourcesMgr.instance.LoadAssetSync<StageConfig>("ConfigData/StageConfigData.asset");
        TaskConfig = ResourcesMgr.instance.LoadAssetSync<TaskConfig>("ConfigData/TaskData.asset");
    }

    public static void Clear()
    {

    }
}
