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
        public float AttackSpeed;
        public int AttackValue;
        public int DefenseValue;
        [Range(0, 100)] public int CriticalValue;
        public float MoveSpeed;
        public Vector2 JumpForce;
        public int Health;
        public float HPBarWidth;
        public int EXP;
    }

    public LevelInfo[] Levels;
}
