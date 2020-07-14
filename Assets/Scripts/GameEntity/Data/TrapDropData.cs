using FrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTragData : BaseEventArgs
{
    public Vector2 InitPos;
    public int AttackValue;//伤害值

    public override void Clear()
    {
        InitPos = Vector2.zero;
        AttackValue = 0;
    }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<MoveData>();
    }
}