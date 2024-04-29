using GameFrameWork.Serialize;
using System;
using System.Security.Policy;
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
        TargetTransformEvent,
        SelfTransformEvent,
        TargetPhysicsEvent,
        SelfPhysicsEvent,
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
    public class SkillEvent
    {
        public SkillEventType skillEventType;

        //动画事件
        public string animName;
        public float animSpeed;
        public float animPlayTimes;

        //声音事件
        public string audioClipName;
        public float audioPlaySpeed;
        public bool audioPlayLoop;
        public float audioPlayVolume;

        [Serializable]//位移事件
        public class AnimInfo
        {
            public float duration;
            public float delay;
            public DG.Tweening.Ease ease;
        }

        [Serializable]//位移事件
        public class TransformEventInfo
        {
            public Vector2 position;//目标位置变化 
            public Vector3 rotation;//目标旋转变化
            public Vector3 scale;//目标缩放变化
            public bool isPositionBasedOnSelf;//位置变化是否在自身基础上变化
            public bool isRotationBasedOnSelf;//旋转变化是否在自身基础上变化
            public bool isPositionAnim;//动画补间
            public AnimInfo positionAnimInfo = new AnimInfo();
            public bool isRotationAnim;//动画补间
            public AnimInfo rotationAnimInfo = new AnimInfo();
            public DG.Tweening.RotateMode rotateMode;
            public bool isScaleAnim;//动画补间
            public AnimInfo scaleAnimInfo = new AnimInfo();
        }

        public TransformEventInfo targetTransformEventInfo = null;
        public TransformEventInfo selfTransformEventInfo = null;

        [Serializable]//
        public class PhysicsEventInfo
        {
            public Vector2 force;
            public Vector2 velocity;
            public float drag;
            public float gravity;
            public float distanceLimit;
        }

        public PhysicsEventInfo targetPhysicsEventInfo = null;
        public PhysicsEventInfo selfPhysicsEventInfo = null;

        ////子弹事件
        //[Serializable]
        //public class Bullet
        //{
        //    public string bulletName;
        //    public string assetName;
        //    public string normalAnim;
        //    public string hitAnim;
        //    public float normalAnimSpeed;
        //    public float hitAnimSpeed;
        //    public Vector2 dir;
        //    public Vector2 pos;
        //    public Vector2 velocity;
        //    public float hitRange;
        //    public float drag;
        //    public bool isPenatrate;//是否穿透
        //}

        //public Bullet[] bullets;//发射子弹

        ////伤害事件
        //public bool isSmoon;//是否击昏
        //public bool isShakeCamera;//击中敌人是否震屏
        //public bool isOnGroundHurt;//是否落地才触发伤害
        //public bool isOnGroundEffect;//落地才触发效果
        //public bool canBeDefense;//能否被防御
        //public bool hitFinish;//攻击到任何敌人就结束技能

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