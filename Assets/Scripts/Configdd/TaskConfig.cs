using System;
using UnityEngine;
using GameFrameWork.Serialize;

public class TaskConfig : BaseScriptableObject<TaskConfigData>
{

}

[Serializable]
public class TaskConfigData : BaseScriptableConfigData
{
    [Serializable]
    public enum TaskConditionType
    {
        None,
        MoveToPos,//到达指定位置
        KillTarget,//杀死目标
        PrevTask,//完成某任务
    }

    [Serializable]
    public enum TaskTriggerType
    {
        None,
        CreateTargets,//产生敌人
        Story,//剧情
        Finger,//出现手指
        ChangeScene,//切换场景
        AutoMoveToPos,//自动移动
        Talk,//对话
        RoundClear,//关卡胜利
        NextStage,//下一关
    }

    [Serializable]
    public enum TaskPosType
    {
        X,
        Y,
        Both,
    }

    [Serializable]
    public class InsTarget
    {
        public int EntityID;
        public int SourceID;
        //==人属性
        [Min(1)] public int Hp;
        [Min(1)] public int AttackValue;
        [Min(1)] public int DefenseValue;

        //==油桶属性
        public int HpBarWidth;
        public float Dir;
        public int GroundY;
        public int ItemId;//-1为随机产生
        public bool IsFloat;
        public float MoveSpeed;

        public Vector2Int Pos;
        public bool IsBarrel;
    }

    [Serializable]
    public class TaskPositon
    {
        public TaskPosType PosType;
        public Vector2Int Pos;
    }

    public TaskConditionType ConditionType;
    public TaskPositon Position;
    public int[] KillEnemyIDs;
    public int[] BreakBarrelIDs;
    public int StoryId;
    public bool KillAllEnemies;
    public bool KillAllBarrels;

    public TaskTriggerType TriggerType;
    public InsTarget[] Targets;
    public bool TriggerPlayerCantCtrl;
    public bool ExitPlayerCanCtrl;
    public bool TriggerStopCamera;
    public bool ExitStartCamera;
    public float WaitTime;
    public int MapID;
    public int TalkID;
    public int PrevID;
    public int NextID;
    public int FailureID;
}
