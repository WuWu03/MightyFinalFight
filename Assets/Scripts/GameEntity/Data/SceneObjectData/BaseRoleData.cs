using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoleData : BaseSceneObjectData
{
    public float AttackSpeed { get; set; }
    public int AttackValue { get; set; }
    public int DefenseValue { get; set; }
    public int CriticalValue { get; set; }
    public float MoveSpeed { get; set; }
    public Vector2 JumpForce { get; set; }
    public bool CatchControl { get; set; }

    public override void Clear()
    {
        base.Clear();
        AttackSpeed = 0;
        AttackValue = 0;
        DefenseValue = 0;
        MoveSpeed = 0;
        JumpForce = Vector2.zero;
        CatchControl = false;
    }
}
