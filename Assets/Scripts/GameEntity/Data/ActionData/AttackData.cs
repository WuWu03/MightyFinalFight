using GameFrameWork;
using System;
using UnityEngine;

public class AttackData : BaseEventArgs
{
    public int SkillID { get; set; }
    public string AnimName { get; set; }
    public int AnimTime { get; set; }
    public float AnimSpeed { get; set; }
    public float Dir { get; set; }
    public bool CanChangeDir { get; set; }
    public Vector2 AddSelfForce { get; set; }

    public static AttackData Create()
    {
        return ReferencePool.Acquire<AttackData>();
    }

    public override void Clear()
    {
        SkillID = 0;
        AnimName = string.Empty;
        AnimTime = 0;
        AnimSpeed = 0;
        Dir = 0;
        CanChangeDir = false;
        AddSelfForce = Vector2.zero;
    }

    public override GameFrameWorkEventArgs Clone()
    {
        return Activator.CreateInstance<AttackData>();
    }
}