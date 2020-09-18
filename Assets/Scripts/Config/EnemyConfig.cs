using UnityEngine;
using System;
using FrameWork;

public class EnemyConfig : BaseScriptableObject<EnemyData>
{
}

[Serializable]
public class EnemyData : BaseConfigData
{
    public string Name;
    public string AssetName;
    public float AttackSpeed;
    public float MoveSpeed;
    public Vector2 JumpForce;
    public int[] AttackIDs;
    public int[] Skills;//技能序列
    public float[] AttackWait;//连击时间
    public float AttackNextTime;
    public int[] BehaviourRate;
    public int[] BehaviourTreeIDs;//行为树id
    public string[] HurtEnemy;//受击动画
}