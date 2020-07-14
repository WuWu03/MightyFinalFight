using UnityEngine;
using FrameWork;
using System;

public class HurtData : BaseEventArgs
{
    public Vector2 AttackForce { get; set; }
    public int AttackValue { get; set; }
    public int AttackerID { get; set; }
    public bool IsSwoon { get; set; }//是否击飞
    public float AttackerDir { get; set; }
    public string HurtSound { get; set; }
    public string HurtAnim { get; set; }

    public override void Clear()
    {
        AttackForce = Vector2.zero;
        AttackValue = 0;
        AttackerID = 0;
        IsSwoon = false;
        AttackerDir = 0;
        HurtSound = string.Empty;
        HurtAnim = string.Empty;
    }

    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<HurtData>();
    }
}