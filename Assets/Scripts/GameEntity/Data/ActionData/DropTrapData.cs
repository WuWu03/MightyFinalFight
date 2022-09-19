using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTrapData : BaseEventArgs
{
    public int attackValue { get; set; }
    public Vector2 rebirthPos { get; set; }

    public override void Clear()
    {
        rebirthPos = Vector2.zero;
        attackValue = 0;
    }
}
