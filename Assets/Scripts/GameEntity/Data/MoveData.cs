using FrameWork;
using System;
using UnityEngine;

public class MoveData : BaseEventArgs
{
    public Vector2 Dir { get; set; }

    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<MoveData>();
    }
}