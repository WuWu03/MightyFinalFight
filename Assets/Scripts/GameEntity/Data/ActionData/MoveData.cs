using GameFrameWork;
using System;
using UnityEngine;

public class MoveData : BaseEventArgs
{
    public Vector2 Dir { get; set; }
    public bool CanChangeDir { get; set; }
    public bool IsCatch { get; set; }
    public static MoveData Create()
    {
        return ReferencePool.Acquire<MoveData>();
    }

    public override void Clear()
    {
        Dir = Vector2.zero;
    }
    public override GameFrameWorkEventArgs Clone()
    {
        return Activator.CreateInstance<MoveData>();
    }
}