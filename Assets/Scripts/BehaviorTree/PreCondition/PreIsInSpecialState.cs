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
        if (m_Timer > 0 && Time.time - m_Timer < 1f)
        {
            return true;
        }
        else
        {
            m_Timer = -1;
        }

        if (m_Owner.owner.isAutoMove)
        {
            return true;
        }

        if (m_Owner.owner.isBeCatch || m_Owner.owner.IsAnyState(typeof(RoleHurt), typeof(RoleDead), typeof(RoleSwoon), typeof(RoleSkill), typeof(RoleAwaken)))
        {
            m_Timer = Time.time;
            return true;
        }

        return false;
    }

    private float m_Timer = -1f;
    private new BaseRoleCtrl m_Owner = null;
}
