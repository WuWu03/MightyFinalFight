using FrameWork;
using System;
using UnityEngine;

public class MoveData : BaseEventArgs
{
    public Vector2 Dir { get; set; }
    public override void Clear()
    {
        Dir = Vector2.zero;
    }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<MoveData>();
    }
}