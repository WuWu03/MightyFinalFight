using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork;
using System;

public class LevelConfig : BaseScriptableObject<LevelData>
{

}

[Serializable]
public class LevelData : BaseConfigData
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
