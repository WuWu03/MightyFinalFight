using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTrapData : BaseEventArgs
{
    public int AttackValue { get; set; }
    public Vector2 RebirthPos { get; set; }

    public override void Clear()
    {
        RebirthPos = Vector2.zero;
        AttackValue = 0;
    }
}
