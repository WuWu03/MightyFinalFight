using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsInSpecialState : PreCondition
{
    public PreIsInSpecialState(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        if(m_Owner.Owner.IsAutoMove)
        {
            return true;
        }

        if (m_Owner.Owner.IsBeCatch || m_Owner.Owner.IsAnyState(typeof(RoleHurt), typeof(RoleDead), typeof(RoleSwoon), typeof(RoleSkill), typeof(RoleAwaken)))
        {
            m_Timer = Time.time;
            return true;
        }

        if (m_Timer > 0 && Time.time - m_Timer < 0.5f)
        {
            return true;
        }
        else
        {
            m_Timer = -1;
        }

        return false;
    }

    private float m_Timer = -1f;
    private new BaseRoleCtrl m_Owner = null;
}
