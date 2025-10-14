using GameFrameWork.Serialize;
using System;
using UnityEngine;

public class StageConfig : BaseScriptableObject<StageConfigData>
{
}

[Serializable]
public class StageConfigData : BaseScriptableConfigData
{
    [Serializable]
    public class BGM
    {
        public string ClipName;
        public bool IsLoop = false;
        public float Volume = 1.0f;
        public float LerpTime = 0f;
    }

    public enum SceneObjType
    {
        Trap,//陷阱
        Unit,//单位
        Building,//建筑
    }

    [Serializable]
    public class SceneBuilding
    {
        public SceneObjType SceneObjType;
        public int Id;
        public string Name;
        public Vector2Int Pos;
        public Vector2 TriggerSize;
        public Vector2 TriggerOffest;
        public string AssetName;
    }

    public string Name;
    public string SceneName;
    public string assetPath;
    public bool showMainPanel;
    public int StageIndex;
    public int Level;
    public int Width;
    public int Height;
    public string StageColor;//关卡色调
    public int StageShowColor;//关卡面板色调
    public Vector2Int InitPos;//主角出生地点
    public SceneBuilding[] SceneBuildings;//场景出现的物体（陷阱，障碍物等）
    public int[] TaskIDs;//场景的任务
    public BGM[] BGMs;//场景音乐组
    public Vector2Int[] MovePoints;//可行走区域
}