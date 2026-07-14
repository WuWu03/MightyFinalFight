using WuWuFramework.Serialize;
using System;
using UnityEngine;

[Serializable]
public class SkillEditorConfig : BaseScriptableObject<SkillEditorConfigData>
{

}

[Serializable]
public class SkillEditorConfigData : BaseScriptableConfigData
{
    [Serializable]
    public enum SkillSelectorType
    {
        None,
        NearHitSelector,
        BulletSelector,
    }

    [Serializable]
    public enum SkillDeployerType
    {
        None,
        NormalAttack,
        JumpAttack,
        SkillAttack,
    }

    [Serializable]
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

    [Serializable]
    public enum SkillConditionType
    {
        None,
        Ground,//站立在陆地上
        DropGround,//刚刚落到地上
        Float,//浮空
        Catch,//抓人
        GroundNotCatch,//着陆且没有抓人
        HpMoreThan,//hp大于
        HpLessThan,//hp小于
        TimeElapsed,//从释放技能开始时间流逝x秒
    }

    [Serializable]
    public class SkillKey
    {
        public ComboKey[] keys;
        public bool addTrigger;
    }

    [Serializable]
    public class SkillEvent
    {
        public SkillEventType skillEventType;

        [Serializable]//动画事件
        public class AnimEventInfo
        {
            public string animName;
            public float animSpeed;
            public float animPlayTimes;
        }

        public AnimEventInfo animEventInfo;

        [Serializable]//声音事件
        public class AudioEventInfo
        {
            public string audioClipName;
            public float audioPlaySpeed;
            public bool audioPlayLoop;
            public float audioPlayVolume;
        }

        public AudioEventInfo audioEventInfo;

        [Serializable]//位移事件
        public class TweenInfo
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
            public TweenInfo positionTweenInfo = new();
            public bool isRotationAnim;//动画补间
            public TweenInfo rotationTweenInfo = new();
            public DG.Tweening.RotateMode rotateMode;
            public bool isScaleAnim;//动画补间
            public TweenInfo scaleTweenInfo = new();
        }

        private TransformEventInfo m_TargetTransformInfo;
        public TransformEventInfo targetTransformEventInfo
        {
            get
            {
                return m_TargetTransformInfo;
            }
            set
            {
                Debug.Log(value);
                m_TargetTransformInfo = value;
            }
        }
        public TransformEventInfo selfTransformEventInfo;

        [Serializable] //受力参数
        public class ForceInfo
        {
            public Vector2 force;
            public Vector2 velocity;
            public float drag;
            public float gravity;
            public float distanceLimit;
        }
        
        [Serializable]//物理事件
        public class PhysicsEventInfo
        {
            public ForceInfo groundForceInfo = new();
            public ForceInfo floatForceInfo = new();
            public ForceInfo lieGroundForceInfo = new();
        }

        public PhysicsEventInfo targetPhysicsEventInfo;
        public PhysicsEventInfo selfPhysicsEventInfo;

        
        [Serializable]//子弹事件
        public class BulletEventInfo
        {
            public string bulletName;
            public string assetPath;
            public string bulletClass;//子弹脚本
            public string normalAnim;
            public string hitAnim;
            public float normalAnimSpeed;
            public float hitAnimSpeed;
            public float hitRange;
            public int bulletCount = -1;//子弹数量，-1表示跟随脚本数量
            public Vector2 pos;//初始相对位置
            public Vector2 velocity;//物理运动初始速度
            public float drag;//摩擦力
            public float moveSpeed;//线性移动速度
            public bool isPhysicsMove;//是否是物理运动
        }

        public BulletEventInfo bulletEventInfo;//子弹事件

        [Serializable]//伤害事件
        public class HurtEventInfo
        {
            public bool isSwoon;//是否击昏
            public bool isOnGroundHurt;//是否落地才触发伤害
            public bool isOnGroundEffect;//落地才触发效果
            public bool canBeDefense;//能否被防御
            public bool hitFinish;//击中任意敌人就结束事件
        }

        [SerializeField] public HurtEventInfo hurtEventInfo;
        
        [Serializable]//特效事件
        public class EffectEventInfo
        {
            public string assetPath;//资源路径
            public Vector2 pos;//初始相对位置
            public Vector3 rotation;//角度
            public Vector3 scale;//缩放
        }

        public EffectEventInfo effectEventInfo;
        
        [Serializable]//延时事件
        public class WaitEventInfo
        {
            public SkillConditionType conditionType;
            public float timeElapsed;
        }
        
        public bool continuous;//持续检测事件触发
        public int nextSkill;//技能结束后连接下一个技能
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
        public SkillConditionType conditionType;
        public bool isRevert;
        public int hpLimit;
        public float timeElapsed;
    }

    public string skillName; 
    public int skillFrameCount;
    public SkillDeployerType deployerType;//释放器类型
    public SerializableDictionary<int, SkillSelector> dicSkillSelectors;
    public SerializableDictionary<int, SerializableList<SkillEvent>> dicSkillEvents;
    public SkillKey skillKey;
    public SkillPrevCondition[] skillPrevConditions;//释放技能的前置条件
}