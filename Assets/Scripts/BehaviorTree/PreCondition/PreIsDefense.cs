using GameFrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsDefense : PreCondition
{
    public PreIsDefense(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
        m_Owner.Owner.OnHurtEvent += OnHurtEvent;
    }

    private bool OnHurtEvent(HurtData data)
    {
        if (m_Owner.Owner.IsAnyState(typeof(RoleHurt)))
        {
            m_HurtTimer = Time.time;
            return false;
        }

        if (m_HurtTimer > 0 && Time.time - m_HurtTimer < 0.3f)
        {
            return false;
        }

        m_HurtTimer = -1;
        m_IsDefense = !m_Owner.Owner.IsAnyState(typeof(RoleAttack)) && !m_Owner.Owner.IsBeCatch && data.CanBeDefense;

        return !m_IsDefense;
    }

    protected override bool OnCheckPreCondition()
    {
        if(m_IsDefense)
        {
            m_IsDefense = false;
            return true;
        }

        return false;
    }

    private float m_HurtTimer = -1;
    private bool m_IsDefense = false;
    private new BaseEnemyCtrl m_Owner = null;
}
