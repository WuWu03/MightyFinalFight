using FrameWork;
using System;
using UnityEngine;

public class TaskConfig : BaseScriptableObject<TaskData>
{

}

[Serializable]
public class TaskData : BaseConfigData
{
    [Serializable]
    public enum ConditionType
    {
        MoveToPos,//到达指定位置
        KillEnemy,//杀死目标
        WaitBarrels,//等待桶
    }

    [Serializable]
    public enum EffectType
    {
        InsEnemy,//产生敌人
        InsBarrels,//产生桶
        Talk,//对话
        Finger,//出现手指
    }

    [Serializable]
    public class InsTarget
    {
        public int ID;
        public Vector2Int Pos;
    }

    public ConditionType TriggerCondition;
    public Vector2 Pos;
    public int[] KillIDs;
    public int BarrelsCount;

    public EffectType TriggerEffect;
    public InsTarget[] Targets;
    public int TalkID;
    public int PrevID;
    public int NextID;
}
