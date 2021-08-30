using UnityEngine;
using UnityEditor;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Resources;

public static class StaticConfig
{
    public static CharacterConfig CharacterConfig = null;
    public static SkillConfig SkillConfig = null;
    public static StageConfig StageConfig = null;
    public static BehaviourTreeConfig BehaviourTreeConfig = null;
    public static SceneItemConfig SceneItemConfig = null;
    public static TaskConfig TaskConfig = null;
    public static LevelConfig LevelConfig = null;
    public static RoleSelectConfig RoleSelectConfig = null;

    public static void InitConfig()
    {
        CharacterConfig = ResMgr.Ins.LoadAsset<CharacterConfig>("ConfigData/CharacterData");
        SkillConfig = ResMgr.Ins.LoadAsset<SkillConfig>("ConfigData/SkillData");
        StageConfig = ResMgr.Ins.LoadAsset<StageConfig>("ConfigData/StageData");
        BehaviourTreeConfig = ResMgr.Ins.LoadAsset<BehaviourTreeConfig>("ConfigData/BehaviourTreeData");
        SceneItemConfig = ResMgr.Ins.LoadAsset<SceneItemConfig>("ConfigData/SceneItemData");
        TaskConfig = ResMgr.Ins.LoadAsset<TaskConfig>("ConfigData/TaskData");
        LevelConfig = ResMgr.Ins.LoadAsset<LevelConfig>("ConfigData/LevelData");
        RoleSelectConfig = ResMgr.Ins.LoadAsset<RoleSelectConfig>("ConfigData/RoleSelectData");
    }

    public static void Clear()
    {
    }
}
