using FrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTragData : BaseEventArgs
{
    public Vector2 InitPos;
    public float AttackValue;//伤害值
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<MoveData>();
    }
}