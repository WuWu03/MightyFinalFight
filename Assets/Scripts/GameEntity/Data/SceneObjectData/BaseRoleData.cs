using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoleData : BaseSceneObjectData
{
   public float AttackSpeed { get; set; }
    public float AttackValue { get; set; }
    public float Defense { get; set; }
    public Vector2 JumpForce { get; set; }
    public float MoveSpeed { get; set; }
}
