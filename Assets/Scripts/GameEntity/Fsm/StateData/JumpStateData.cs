using GameFrameWork;
using GameFrameWork.Event;
using UnityEngine;

public class JumpStateData : GameFrameWorkEventArg
{
    public bool canChangeDir { get; set; }
    public Vector2 dir { get; set; }
    public bool isCatch { get; set; }

    public static JumpStateData Create()
    {
        return ReferencePool.Acquire<JumpStateData>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
        canChangeDir = false;
        isCatch = false;
    }
}