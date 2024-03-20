using GameFrameWork;
using System;
using UnityEngine;

public class AttackStateData : BaseEventArgs
{
    public int skillID { get; set; }
    public string animName { get; set; }
    public int animTime { get; set; }
    public float animSpeed { get; set; }
    public float dir { get; set; }
    public bool canChangeDir { get; set; }

    public static AttackStateData Create()
    {
        return ReferencePool.Acquire<AttackStateData>();
    }

    public override void Clear()
    {
        skillID = 0;
        animName = string.Empty;
        animTime = 0;
        animSpeed = 0;
        dir = 0;
        canChangeDir = false;
    }

    public override GameFrameWorkEventArgs Clone()
    {
        return Activator.CreateInstance<AttackStateData>();
    }
}