using GameFrameWork;
using System;
using UnityEngine;

public class JumpData : BaseEventArgs
{
    public bool canChangeDir { get; set; }
    public Vector2 dir { get; set; }
    public bool isCatch { get; set; }

    public static JumpData Create()
    {
        return ReferencePool.Acquire<JumpData>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
        canChangeDir = false;
        isCatch = false;
    }
}