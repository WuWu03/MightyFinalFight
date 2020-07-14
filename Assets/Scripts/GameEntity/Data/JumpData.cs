using FrameWork;
using System;
using UnityEngine;

public class JumpData : BaseEventArgs
{
    public Vector2 Dir
    {
        get;
        set;
    }

    public override void Clear()
    {
        Dir = Vector2.zero;
    }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<AttackData>();
    }
}