using GameFrameWork;
using GameFrameWork.Event;
using UnityEngine;

public class MoveStateData : GameFrameWorkEventArg
{
    public Vector2 dir { get; set; }
    public bool canChangeDir { get; set; }
    public bool isCatch { get; set; }
    public static MoveStateData Create()
    {
        return ReferencePool.Acquire<MoveStateData>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
    }
}