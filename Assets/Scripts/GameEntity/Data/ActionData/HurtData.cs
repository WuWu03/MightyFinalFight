using UnityEngine;
using GameFrameWork;
using System;

public class HurtData : BaseEventArgs
{
    public Vector2 AttackForce { get; set; }
    public Vector2 AttackerPos { get; set; }
    public int AttackValue { get; set; }
    public int AttackerId { get; set; }
    public int SkillExp { get; set; }
    public bool IsSwoon { get; set; }//是否击飞
    public bool IsGroundHurt { get; set; }//是否落地触发
    public bool IsCritical { get; set; }
    public float AttackerDir { get; set; }
    public string HurtSound { get; set; }
    public string HurtAnim { get; set; }
    public bool CanBeDefense { get; set; }
    public bool IsBoss { get; set; }

    public static HurtData Create()
    {
        return ReferencePool.Acquire<HurtData>();
    }

    public override void Clear()
    {
        AttackForce = Vector2.zero;
        AttackerPos = Vector2.zero;
        AttackValue = 0;
        AttackerId = 0;
        SkillExp = 0;
        IsSwoon = false;
        IsGroundHurt = false;
        IsCritical = false;
        CanBeDefense = false;
        AttackerDir = 0;
        HurtSound = string.Empty;
        HurtAnim = string.Empty;
        IsBoss = false;
    }

    public override GameFrameWorkEventArgs Clone()
    {
        return Activator.CreateInstance<HurtData>();
    }
}