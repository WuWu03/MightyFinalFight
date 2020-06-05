using FrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectConfig : BaseScriptableObject<SceneObjectData>
{
}

[Serializable]
public class SceneObjectData : BaseConfigData
{
    public enum SceneObjectType
    {
        Trag,//陷阱
        Drop,//下落物体
        Obstacle,//障碍
    }

    public string AssetName;
    public Area Area;//区域
    public SceneObjectType Type;//类型
}