using GameFrameWork;
using System;
using UnityEngine;

public class TrapData : SceneItemData
{
    public Vector2 Pos;
    public Vector2 TriggerOffest;
    public Vector2 TriggerSize;
    public int AttackValue;//伤害值

    public override void Clear()
    {
        Pos = Vector2.zero;
        AttackValue = 0;
    }

    public override GameFrameWorkEventArgs Clone()
    {
        return Activator.CreateInstance<TrapData>();
    }
}