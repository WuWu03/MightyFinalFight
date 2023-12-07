using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsInSpecialState : PreCondition
{
    public PreIsInSpecialState(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_Owner = base.m_Owner as BaseRoleCtrl;

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);

            if (m.Success)
            {
                m_ResumeTime = float.Parse(m.Groups[2].Value);
            }
        }
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_Owner.owner.isAutoMove)
        {
            m_Timer = -1f;
            return true;
        }

        if (m_Timer > 0 && Time.time - m_Timer < m_ResumeTime)
        {
            return true;
        }
        else
        {
            m_Timer = -1f;
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
    private float m_ResumeTime = 1f;
    private Regex m_Regex = new Regex(@"(ResumeTime:)([0-9]+\.?[0-9]*)");
}
