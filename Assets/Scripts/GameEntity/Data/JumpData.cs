using GameFrameWork;
using System;
using UnityEngine;

public class JumpData : BaseEventArgs
{
    public bool CanChangeDir;
    public Vector2 Dir
    {
        get;
        set;
    }

    public override void Clear()
    {
        Dir = Vector2.zero;
        CanChangeDir = false;
    }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<AttackData>();
    }
}