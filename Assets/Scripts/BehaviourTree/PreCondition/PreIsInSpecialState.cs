using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsInSpecialState : PreCondition
{
    private readonly BaseRole m_Owner;
    private readonly float m_ResumeTime = 0.5f;
    private float m_Timer = -1f;
    private bool m_IsInState;
    
    public PreIsInSpecialState(int id, object owner, int priority, bool isAndCondition, string args) : base(id, owner, priority, isAndCondition, args)
    {
        m_Owner = owner as BaseRole;
        Regex mRegex = new(@"(ResumeTime:)([0-9]+\.?[0-9]*)");

        if (!string.IsNullOrEmpty(args))
        {
            Match m = mRegex.Match(args);

            if (m.Success)
            {
                m_ResumeTime = float.Parse(m.Groups[2].Value);
            }
        }
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_Owner.isAutoMove)
        {
            m_Timer = -1f;
            return true;
        }

        if (m_Timer > 0 && Time.time - m_Timer < m_ResumeTime)
        {
            return true;
        }
        
        m_Timer = -1f;
        
        if (m_Owner.isBeCatch || m_Owner.IsAnyState(typeof(RoleHurt), typeof(RoleDead), typeof(RoleSwoon), typeof(RoleAwaken)))
        {
            m_Timer = Time.time;
            return true;
        }

        return false;
    }
}
