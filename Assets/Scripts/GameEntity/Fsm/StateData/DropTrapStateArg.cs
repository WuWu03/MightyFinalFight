using WuWuFramework;
using WuWuFramework.Fsm;
using UnityEngine;

public class DropTrapStateArg : FsmStateArg
{
    public int attackValue { get; set; }
    public Vector2 rebirthPos { get; set; }

    public static DropTrapStateArg Create()
    {
        return ReferencePool.Acquire<DropTrapStateArg>();
    }

    public override void Clear()
    {
        rebirthPos = Vector2.zero;
        attackValue = 0;
    }
}
