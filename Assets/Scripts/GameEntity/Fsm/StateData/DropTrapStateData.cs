using GameFrameWork;
using GameFrameWork.Event;
using UnityEngine;

public class DropTrapStateData : GameFrameWorkEventArg
{
    public int attackValue { get; set; }
    public Vector2 rebirthPos { get; set; }

    public static DropTrapStateData Create()
    {
        return ReferencePool.Acquire<DropTrapStateData>();
    }

    public override void Clear()
    {
        rebirthPos = Vector2.zero;
        attackValue = 0;
    }
}
