using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

public class StageConfig : BaseScriptableObject<StageConfigData>
{
}

[Serializable]
public class StageConfigData : BaseConfigData
{
    [Serializable]
    public class BGM
    {
        public string AssetName;
        public bool IsLoop;
    }

    public string Name;
    public string SceneName;
    public int StageIndex;
    public int Width;
    public int Height;
    public Vector2Int InitPos;//主角出生地点
    public int[] SceneObjIDs;//场景出现的物体（陷阱，障碍物等）
    public int[] TaskIDs;//场景的任务
    public BGM[] BGMs;//场景音乐组
    public Area[] MoveArea;//可行走区域
}

[Serializable]
public class Area
{
    public Vector2Int Pos;
    public Vector2Int Size;
}