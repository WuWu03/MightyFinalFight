using FrameWork;
using System;
using UnityEngine;
public class AttackData : BaseEventArgs
{
    public string AnimationName { get; set; }
    public float Dir { get; set; }
    public bool CanChangeDir { get; set; }
    public float AnimSpeed { get; set; }
    public Vector2 AddSelfForce { get; set; }
    public int AnimTime { get; set; }

    public override void Clear()
    {
        AnimationName = string.Empty;
        Dir = 0;
        CanChangeDir = false;
        AnimSpeed = 0;
        AddSelfForce = Vector2.zero;
        AnimTime = 0;
    }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<AttackData>();
    }
}