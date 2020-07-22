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
    public class BGM
    {
        public string AssetName;
        public bool IsLoop;
    }

    public string Name;
    public string AssetName;
    public int StageIndex;
    public int Width;
    public int Height;
    public Vector2Int InitPos;//主角出生地点
    public int[] SceneObjIDs;//场景出现的物体（陷阱，障碍物等） 
    public BGM[] BGMs;//场景音乐组
    public Area[] MoveArea;//可行走区域
}

[Serializable]
public class Area
{
    public Vector2Int Pos;
    public Vector2Int Size;
}