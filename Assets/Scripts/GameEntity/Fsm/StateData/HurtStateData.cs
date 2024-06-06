using GameFrameWork;
using UnityEngine;

public class HurtStateData : BaseEventArgs
{
    public Vector2 attackForce { get; set; }
    public Vector2 attackerPos { get; set; }
    public int attackValue { get; set; }
    public int attackerId { get; set; }
    public int skillExp { get; set; }
    public bool isSwoon { get; set; }//是否击飞
    public bool isGroundHurt { get; set; }//是否落地触发
    public bool isCritical { get; set; }
    public float attackerDir { get; set; }
    public string hurtSound { get; set; }
    public string hurtAnim { get; set; }
    public bool canBeDefense { get; set; }
    public bool isBoss { get; set; }
    public bool isDefense { get; set; }
    public bool isPause { get; set; }
    public bool isNotPlayHurtSound { get; set; }

    public static HurtStateData Create()
    {
        return ReferencePool.Acquire<HurtStateData>();
    }

    public override void Clear()
    {
        attackForce = Vector2.zero;
        attackerPos = Vector2.zero;
        attackValue = 0;
        attackerId = 0;
        skillExp = 0;
        isSwoon = false;
        isGroundHurt = false;
        isCritical = false;
        canBeDefense = false;
        attackerDir = 0;
        hurtSound = string.Empty;
        hurtAnim = string.Empty;
        isBoss = false;
        isDefense = false;
        isNotPlayHurtSound = false;
    }
}