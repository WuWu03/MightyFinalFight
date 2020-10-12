using FrameWork.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseEnemy : BaseEnemy
{
    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<RoleDefense>();
    }

    public override void OnHurtMsg(HurtData data)
    {
        bool isDefense = !IsAnyState(typeof(RoleAttack)) && !m_IsBeCatch && data.CanBeDefense && !m_IsNotDefense;

        if (isDefense)
        {
            data.IsSwoon = false;
            data.AttackValue = 0;
        }

        base.OnHurtMsg(data);

        if (isDefense)
        {
            SetDir(-data.AttackerDir);
            SetPos2(m_Pos.x + data.AttackerDir * 0.07f, m_Pos.y);
            ChangeState<RoleDefense>(true);
        }
        else
        {
            m_NotDefenseTimer = Time.time;
            m_IsNotDefense = true;   
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (m_IsNotDefense && Time.time - m_NotDefenseTimer > 0.5f)
        {
            m_IsNotDefense = false;
            m_NotDefenseTimer = 0;
        }
    }

    private float m_NotDefenseTimer = 0;
    private bool m_IsNotDefense = false;//破防
}
