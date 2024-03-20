using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttribute : IReference
{
    /// <summary>
    /// 当前血量
    /// </summary>
    public int health { get; set; }
    /// <summary>
    /// 血量上限
    /// </summary>
    public int maxHealth { get; set; }
    /// <summary>
    /// 攻击速度
    /// </summary>
    public float attackSpeed { get; set; }
    /// <summary>
    /// 攻击
    /// </summary>
    public int attackValue { get; set; }
    /// <summary>
    /// 防御
    /// </summary>
    public int defenseValue { get; set; }
    /// <summary>
    /// 暴击
    /// </summary>
    public int criticalValue { get; set; }
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed { get; set; }
    /// <summary>
    /// 跳跃
    /// </summary>
    public Vector2 jumpForce { get; set; }


    public static EntityAttribute Create()
    {
        return ReferencePool.Acquire<EntityAttribute>();
    }

    public void AddHealth(int value)
    {
        health += value;
    }

    public void SubHealth(int value)
    {
        health = Mathf.Max(health - value, 0);
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }

    public void AddMaxHealth(int value)
    {
        maxHealth += value;
    }

    public void SubMaxHealth(int value)
    {
        maxHealth = Mathf.Max(maxHealth - value, 0);
    }

    public bool IsFullHealth()
    {
        return health >= maxHealth;
    }

    public bool IsDie()
    {
        return health <= 0;
    }

    public void Clear()
    {
        health = 0;
        maxHealth = 0;
        attackSpeed = 0;
        attackValue = 0;
        defenseValue = 0;
        criticalValue = 0;
        moveSpeed = 0;
        jumpForce = Vector2.zero;
    }
}
