using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScriptableObject<T>:ScriptableObject where T:BaseConfigData
{
    public T[] Datas = null;
    public T GetData(int id)
    {
        if (Datas == null) return null;
        for (int i = 0; i < Datas.Length; i++)
        {
            if (Datas[i].ID.Equals(id))
            {
                return Datas[i];
            }
        }

        return null;
    }
}

[Serializable]
public abstract class BaseConfigData
{
    public int ID;
}
public class SkillConfig : BaseScriptableObject<SkillData>
{

}

[Serializable]
public class SkillData: BaseConfigData
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

    public int Level;
    public string Name;
    public string AnimationName;
    public SkillType Type;
    public SkillSelectorType SelectorType;
    public SkillEffectorType[] EffectorTypes;
    public Vector2 SelectorOffest;//选择器偏移
    public float SelectorAngle;//选择器角度
    public Vector2 AddTargetForce;//对目标施加力
    public Vector2 AddSelfForce;//对自身施加力
    public float AddSelfDrag;//自身的空气阻力
    public float MoveDistance;//施加力后的移动距离
    public float Gravity;//自身重力大小
    public Bullet[] Bullets;//发射子弹
    public bool IsSmoon;//是否击昏
    public bool IsFloat;//是否浮空
}
