using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttribute : IReference
{
    /// <summary>
    /// 当前血量
    /// </summary>
    public int Health { get; set; }
    /// <summary>
    /// 血量上限
    /// </summary>
    public int MaxHealth { get; set; }
    /// <summary>
    /// 攻击速度
    /// </summary>
    public float AttackSpeed { get; set; }
    /// <summary>
    /// 攻击
    /// </summary>
    public int AttackValue { get; set; }
    /// <summary>
    /// 防御
    /// </summary>
    public int DefenseValue { get; set; }
    /// <summary>
    /// 暴击
    /// </summary>
    public int CriticalValue { get; set; }
    /// <summary>
    /// 移动速度
    /// </summary>
    public float MoveSpeed { get; set; }
    /// <summary>
    /// 跳跃
    /// </summary>
    public Vector2 JumpForce { get; set; }



    public void AddHealth(int value)
    {
        Health += value;
    }

    public void SubHealth(int value)
    {
        Health = Mathf.Max(Health - value, 0);
    }

    public void ResetHealth()
    {
        Health = MaxHealth;
    }

    public void AddMaxHealth(int value)
    {
        MaxHealth += value;
    }

    public void SubMaxHealth(int value)
    {
        MaxHealth = Mathf.Max(MaxHealth - value, 0);
    }


    public bool IsFullHealth()
    {
        return Health >= MaxHealth;
    }

    public bool IsDie()
    {
        return Health <= 0;
    }

    public void Clear()
    {
        Health = 0;
        MaxHealth = 0;
        AttackSpeed = 0;
        AttackValue = 0;
        DefenseValue = 0;
        CriticalValue = 0;
        MoveSpeed = 0;
        JumpForce = Vector2.zero;
    }
}
