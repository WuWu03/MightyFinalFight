using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FrameWork;

public class StageConfig : BaseScriptableObject<StageData>
{
}

[Serializable]
public class StageData : BaseConfigData
{
    [Serializable]
    public class EventArea : Area
    {
        public int[] EnemyIDs;
        public int[] EventIDs;
    }

    public string Name;
    public string AssetName;
    public string AudioName;
    public int Width;
    public int Height;
    public int[] SceneObjIDs;//场景出现的物体（陷阱，障碍物等）
    public Vector2Int InitPos;
    public Area[] MoveArea;//可行走区域
    public EventArea[] Areas;//每个关卡的区域
}

[Serializable]
public class Area
{
    public Vector2Int Pos;
    public int Width;
    public int Height;
}