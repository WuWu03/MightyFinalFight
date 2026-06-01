using WuWuFramework;
using WuWuFramework.Fsm;
using UnityEngine;

public class JumpStateArg : FsmStateArg
{
    public bool canChangeDir { get; set; }
    public Vector2 dir { get; set; }
    public bool isCatch { get; set; }

    public static JumpStateArg Create()
    {
        return ReferencePool.Acquire<JumpStateArg>();
    }

    public override void Clear()
    {
        dir = Vector2.zero;
        canChangeDir = false;
        isCatch = false;
    }
}