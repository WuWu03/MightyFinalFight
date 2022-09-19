using GameFrameWork;
using System;
using UnityEngine;

public class MoveData : BaseEventArgs
{
    public Vector2 dir { get; set; }
    public bool canChangeDir { get; set; }
    public bool isCatch { get; set; }
    public static MoveData Create()
    {
        return ReferencePool.Acquire<MoveData>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
    }
}