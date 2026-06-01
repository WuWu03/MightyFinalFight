using WuWuFramework;
using WuWuFramework.Fsm;
using UnityEngine;

public class MoveStateArg : FsmStateArg
{
    public Vector2 dir { get; set; }
    public bool canChangeDir { get; set; }
    public bool isCatch { get; set; }
    public static MoveStateArg Create()
    {
        return ReferencePool.Acquire<MoveStateArg>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
    }
}