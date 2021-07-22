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
        public string ClipName;
        public bool IsLoop = false;
        public float Volume = 1.0f;
        public float LerpTime = 0f;
    }

    public enum SceneObjType
    {
        Trap,//陷阱
        Effect,//特效
    }

    [Serializable]
    public class SceneObj
    {
        public SceneObjType SceneObjType;
        public Vector2 Pos;
        public Vector2 Size;
    }

    public string Name;
    public string SceneName;
    public int StageIndex;
    public int Width;
    public int Height;
    public Vector2Int InitPos;//主角出生地点
    public SceneObj[] SceneObjs;//场景出现的物体（陷阱，障碍物等）
    public int[] TaskIDs;//场景的任务
    public BGM[] BGMs;//场景音乐组
    public Rect[] MoveArea;//可行走区域
}