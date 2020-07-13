using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoleSkillData : BaseEventArgs
{
    public int[] AttackIDs;
    public int[] JumpAttackIDs;
    public int[] Skills;//技能序列
    public float[] AttackWait;//连击时间
    public float AttackNextTime;
}
