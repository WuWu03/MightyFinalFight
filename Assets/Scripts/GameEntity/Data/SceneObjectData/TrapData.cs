using GameFrameWork;
using System;
using UnityEngine;

public class TrapData : SceneItemData
{
    public Vector2 triggerOffest { get; set; }
    public Vector2 triggerSize { get; set; }

    public override void Clear()
    {
        triggerOffest = Vector2.zero;
        triggerSize = Vector2.zero;
    }
}