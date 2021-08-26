using GameFrameWork;
using System;
using UnityEngine;

public class TrapData : SceneItemData
{
    public Vector2 TriggerOffest { get; set; }
    public Vector2 TriggerSize { get; set; }

    public override void Clear()
    {
        TriggerOffest = Vector2.zero;
        TriggerSize = Vector2.zero;
    }
}