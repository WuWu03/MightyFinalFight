using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

public class TaskConfig : BaseScriptableObject<TaskData>
{

}

[Serializable]
public class TaskData : BaseConfigData
{
    [Serializable]
    public enum ConditionType
    {
        None,
        MoveToPos,//到达指定位置
        KillEnemy,//杀死目标
        WaitBarrels,//等待桶
    }

    [Serializable]
    public enum EffectType
    {
        None,
        Enemy,//产生敌人
        Barrels,//产生桶
        Talk,//对话
        Finger,//出现手指
    }

    [Serializable]
    public enum PosType
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
        public Vector2Int Pos;
    }

    [Serializable]
    public class TaskPositon
    {
        public PosType PosType;
        public Vector2Int Pos;
    }

    public ConditionType TriggerCondition;
    public TaskPositon PosCondition;
    public int[] KillIDs;
    public int BarrelsCount;
    public bool KillAll;
    public bool BarrelsAll;

    public EffectType TriggerEffect;
    public InsTarget[] Targets;
    public bool TriggerStopCamera;
    public bool ExitStartCamera;
    public int TalkID;
    public int PrevID;
    public int NextID;
}
