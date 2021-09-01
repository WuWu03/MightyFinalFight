using GameFrameWork.Serialize;
using System;
using UnityEngine;

public class CharacterConfig : BaseScriptableObject<CharacterConfigData>
{
}

[Serializable]
public class CharacterConfigData : BaseConfigData
{
    public string Name;
    public string AssetName;
    public string HitEffect;
    [TextArea()] public string Desc;
    public string HeadIcon;
    public float AttackSpeed;
    public float MoveSpeed;
    public Vector2 JumpForce;
    public int[] AttackIDs;
    public int[] JumpAttackIDs;
    public int CatchAttackID;
    public int ThrowAttackID;
    public int WeaponAttackID;
    public int ThrowWeaponID;
    public int[] Skills;//技能序列
    public float[] AttackWait;//连击时间
    public float[] AttackNextTime;
    public int WeaponId;//武器id
    public bool CatchControl;//抓取的时候是否可以控制敌人进行移动
    public int[] BehaviourTreeIds;
    public string[] HurtAnim;
    public bool IsBoss;//是否是boss
}