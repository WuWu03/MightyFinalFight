using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

public class TaskConfig : BaseScriptableObject<TaskConfigData>
{

}

[Serializable]
public class TaskConfigData : BaseConfigData
{
    [Serializable]
    public enum TaskConditionType
    {
        None,
        MoveToPos,//到达指定位置
        KillEnemy,//杀死目标
        WaitBarrels,//等待桶
        PrevTask,//完成某任务
    }

    [Serializable]
    public enum TaskTriggerType
    {
        None,
        Enemy,//产生敌人
        Barrels,//产生桶
        Story,//剧情
        Finger,//出现手指
        ChangeScene,//切换场景
        AutoMoveToPos,//自动移动
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
        [Min(1)] public int Hp;
        [Min(1)] public int AttackValue;
        [Min(1)] public int DefenseValue;
        public int HpBarWidth;
        public Vector2Int Pos;
    }

    [Serializable]
    public class TaskPositon
    {
        public TaskPosType PosType;
        public Vector2Int Pos;
    }

    public TaskConditionType ConditionType;
    public TaskPositon Position;
    public int[] KillIDs;
    public int StoryId;
    public int BarrelsCount;
    public bool KillAll;
    public bool BarrelsAll;

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
