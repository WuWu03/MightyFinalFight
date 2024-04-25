using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillEditorConfig : BaseScriptableObject<SkillEditorConfigData>
{

}

[Serializable]
public class SkillEditorConfigData : BaseConfigData
{
    public enum SkillSelectorType
    {
        None,
        NearHitSelector,
        BulletSelector,
    }

    public enum SkillDeployerType
    {
        None,
        NormalAttack,
        JumpAttack,
        SkillAttack,
    }

    public enum SkillEventType
    {
        None,
        AnimEvent,
        AudioEvent,
        TransformEvent,
        PhysicsEvent,
        BulletEvent,
        HitEvent,
        EffectEvent,
        BuffEvent,
    }

    public enum SkillPrevConditionType
    {
        None,
        Ground,//站立在陆地上
        DropGround,//刚刚落到地上
        Float,//浮空
        Catch,//抓人
        GroundNotCatch,//着陆且没有抓人
        HPMoreThan,//hp大于
        HPLessThan,//hp小于
    }

    [Serializable]
    public class SkillKey
    {
        public GameFrameWork.Input.KeyType[] keys;
        public bool addTrigger;
    }

    [Serializable]
    public class Bullet
    {
        public string bulletName;
        public string assetName;
        public string normalAnim;
        public string hitAnim;
        public float normalAnimSpeed;
        public float hitAnimSpeed;
        public Vector2 dir;
        public Vector2 pos;
        public Vector2 velocity;
        public float hitRange;
        public float drag;
        public bool isPenatrate;//是否穿透
    }

    [Serializable]
    public class SkillEvent
    {
        public SkillEventType eventType;

        //动画事件
        public string animName;
        public float animSpeed;
        public float animPlayTimes;

        //声音事件
        public string audioClipName;
        public float audioPlaySpeed;
        public bool audioPlayLoop;
        public float audioPlayVolume;

        //物理事件
        public Vector2 addTargetForce;//对目标施加力
        public Vector2 addSelfForce;//对自身施加力
        public Vector2 addTargetVelocity;//目标速度
        public Vector2 addSelfVelocity;//自身速度
        public float addTargetDrag;//目标空气阻力
        public float addSelfDrag;//自身空气阻力
        public float targetGravity;//目标重力大小
        public float selfGravity;//自身重力大小
        public float moveDistance;//施加力后的移动距离

        //位移事件
        public Vector2 targetPosition;//目标位置
        public Vector2 selfPosition;//自身位置
        public Vector2 targetScale;//目标缩放
        public Vector2 selfScale;//自身缩放


        //子弹事件
        public Bullet[] bullets;//发射子弹

        //伤害事件
        public bool isSmoon;//是否击昏
        public bool isShakeCamera;//击中敌人是否震屏
        public bool isOnGroundHurt;//是否落地才触发伤害
        public bool isOnGroundEffect;//落地才触发效果
        public bool canBeDefense;//能否被防御
        public bool hitFinish;//攻击到任何敌人就结束技能

        //特效事件
        //Buff事件

        public string args;//各种数值效果的参数 每种类型效果自行解析
    }

    [Serializable]
    public class SkillSelector
    {
        public SkillSelectorType selectorType;//选择器类型
        public float selectorAngle;//选择器角度
        public float selectorRadius;//选择器半径
        public float selectorWidth;//选择器宽
        public float selectorHeight;//选择器高
        public Vector2 selectorOffest;//选择器偏移

    }

    [Serializable]
    public class SkillPrevCondition
    {
        public SkillPrevConditionType prevConditionType;
        public bool isRevert;
        public string args;
    }

    public string skillName; 
    public int skillFrameCount;
    public SkillDeployerType deployerType;//释放器类型
    public SerializableDictionary<int, SkillSelector> dicSkillSelectors = null;
    public SerializableDictionary<int, SerializableList<SkillEvent>> dicSkillEvents = null;
    public SkillKey skillKey;
    public SkillPrevCondition[] skillPrevConditions;//释放技能的前置条件
}