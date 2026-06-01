using WuWuFramework.BehaviourTree;
using UnityEngine;

public class PreIsDefense : PreCondition
{
    public PreIsDefense(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Owner = owner as BaseEnemy;
        m_Owner.onHurtEvent += OnHurtEvent;
    }

    private void OnHurtEvent(HurtStateArg arg)
    {
        if (m_Owner.IsAnyState(typeof(RoleSwoon), typeof(RoleHurt), typeof(RoleSkill)) || arg.isSwoon)
        {
            m_IsDefense = false;
            m_HurtTimer = Time.time;
            return;
        }

        if (m_HurtTimer > 0 && Time.time - m_HurtTimer < 0.5f)
        {
            m_IsDefense = false;
            return;
        }

        m_HurtTimer = -1;
        m_IsDefense = !m_Owner.IsAnyState(typeof(RoleSkill)) && !m_Owner.isBeCatch && arg.canBeDefense;
        arg.isDefense = m_IsDefense;

        if (m_IsDefense)
        {
            arg.hurtSound = SoundName.Eat;
            arg.attackValue = 0;
        }

        return;
    }

    protected override bool OnCheckPreCondition()
    {
        if (m_IsDefense)
        {
            m_IsDefense = false;
            return true;
        }

        return false;
    }

    protected override void OnDestroy()
    {
        m_Owner.onHurtEvent -= OnHurtEvent;
    }

    private float m_HurtTimer = -1;
    private bool m_IsDefense = false;
    private BaseEnemy m_Owner = null;
}
