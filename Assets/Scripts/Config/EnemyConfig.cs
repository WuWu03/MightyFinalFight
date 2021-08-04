using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;

public class EnemyConfig : BaseScriptableObject<EnemyConfigData>
{
}

[Serializable]
public class EnemyConfigData : BaseConfigData
{
    public string Name;
    public string AssetName;
    public float AttackSpeed;
    public float MoveSpeed;
    public Vector2 JumpForce;
    public int[] AttackIDs;
    public int[] Skills;//技能序列
    public float[] AttackWait;//连击时间
    public float[] AttackNextTime;
    public int[] BehaviourRate;//0.Idle 1.RandomPos 2.RoundPos 3.Attack 4.Skill 
    public int[] BehaviourTreeIDs;//行为树id
    public string[] HurtEnemy;//受击动画
}