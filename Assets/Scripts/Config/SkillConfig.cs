using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillConfig : BaseScriptableObject<SkillData>
{

}

[Serializable]
public class SkillData : BaseConfigData
{

    [Serializable]
    public class Bullet
    {
        public string Name;
        public Vector2 Dir;
        public Vector2 Pos;
        public Vector2 Velocity;
        public float HitRange;
        public float Drag;
        public Vector2 TriggerOffest;
        public Vector2 TriggerSize;
    }

    public enum SkillSelectorType
    {
        NearHitSelector = 1,
        BulletSelector = 2,
    }

    public enum SkillEffectorType
    {
        NearHitEffect = 1,
        BulletHitEffect = 2,
        MoveHitEffect = 3,
    }

    public enum SkillType
    {
        NormalAttack,
        JumpAttack,
        SkillAttack,
    }

    public enum SkillStatus
    {
        None,
        Ground,//着陆
        Float,//浮空
    }

    public enum SkillDeployeType
    {
        Just,//直接释放
        Animtion,//动画触发
    }

    public int Level;
    public string Name;
    public string AnimationName;
    public SkillType Type;
    public SkillSelectorType SelectorType;
    public SkillStatus Status;//释放技能需要处于什么状态
    public SkillDeployeType DeployeType;//技能释放方式
    public SkillEffectorType[] EffectorTypes;
    public FrameWork.Input.KeyType[] SkillKeys;
    public Vector2 SelectorOffest;//选择器偏移
    public float SelectorAngle;//选择器角度
    public Vector2 AddTargetForce;//对目标施加力
    public Vector2 AddSelfForce;//对自身施加力
    public float AnimSpeed = 0.4f;//动画速度
    public int AnimTime = 1;//动画播放次数
    public float AddSelfDrag;//自身的空气阻力
    public float MoveDistance;//施加力后的移动距离
    public float Gravity;//自身重力大小
    public Bullet[] Bullets;//发射子弹
    public bool IsSmoon;//是否击昏
    public bool IsShakeCamera;//击中敌人是否震屏
}