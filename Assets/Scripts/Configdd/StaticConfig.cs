using GameFrameWork.Resources;

public static class StaticConfig
{
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static TaskConfig TaskConfig = null;


    public static void InitConfig()
    {
        SkillConfig = ResourcesMgr.instance.LoadAsset<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResourcesMgr.instance.LoadAsset<StageConfig>("ConfigData/StageData");
        TaskConfig = ResourcesMgr.instance.LoadAsset<TaskConfig>("ConfigData/TaskData");
    }

    public static void Clear()
    {
    }
}
