using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

public class LevelConfig : BaseScriptableObject<LevelConfigData>
{

}

[Serializable]
public class LevelConfigData : BaseConfigData
{
    [Serializable]
    public class LevelInfo
    {
        public int Health;
        public float HPBarWidth;
        public int EXP;
    }

    public LevelInfo[] Levels;
}
