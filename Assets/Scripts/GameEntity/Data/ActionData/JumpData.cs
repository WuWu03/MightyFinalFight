using GameFrameWork;
using System;
using UnityEngine;

public class JumpData : BaseEventArgs
{
    public bool CanChangeDir { get; set; }
    public Vector2 Dir { get; set; }
    public bool IsCatch { get; set; }

    public static JumpData Create()
    {
        return ReferencePool.Acquire<JumpData>();
    }

    public override void Clear()
    {
        Dir = Vector2.zero;
        CanChangeDir = false;
        IsCatch = false;
    }
}