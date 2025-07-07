using GameFrameWork.BehaviourTree;
using UnityEngine;

public class PreIsDefense : PreCondition
{
    public PreIsDefense(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
        m_Owner.owner.onHurtEvent += OnHurtEvent;
    }

    private void OnHurtEvent(HurtStateData data)
    {
        if (m_Owner.owner.IsAnyState(typeof(RoleHurt), typeof(RoleAttack)) || data.isSwoon)
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
        m_IsDefense = !m_Owner.owner.IsAnyState(typeof(RoleAttack)) && !m_Owner.owner.isBeCatch && data.canBeDefense;
        data.isDefense = m_IsDefense;

        if (m_IsDefense)
        {
            data.hurtSound = SoundName.Eat;
            data.attackValue = 0;
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
        m_Owner.owner.onHurtEvent -= OnHurtEvent;
    }

    private float m_HurtTimer = -1;
    private bool m_IsDefense = false;
    private new BaseEnemyCtrl m_Owner = null;
}
