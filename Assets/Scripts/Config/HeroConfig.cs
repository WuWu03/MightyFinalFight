using UnityEngine;
using System;
using GameFrameWork.Serialize;

public class HeroConfig : BaseScriptableObject<HeroData>
{
}

[Serializable]
public class HeroData : BaseConfigData
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
    public float AttackNextTime;
}