using UnityEngine;
using FrameWork;
using System;

public class HurtData : BaseEventArgs
{
    public Vector2 AttackForce { get; set; }
    public float AttackValue { get; set; }
    public int AttackerID { get; set; }
    public bool IsSwoon { get; set; }//是否击飞
    public float AttackerDir { get; set; }
    public string HurtSound { get; set; }

    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<HurtData>();
    }
}