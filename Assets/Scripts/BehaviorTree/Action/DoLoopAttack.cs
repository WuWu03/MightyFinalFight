using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoLoopAttack : DoAttack
{
    public DoLoopAttack(string name, string args, object owner) : base(name, args, owner) { }

    protected override void OnEnter()
    {
        if (m_IsRandomAttckCount && m_AttackCount == 0)
        {
            m_AttackCount = Random.Range(1, 9);
        }
    }

    public override BehaviorTreeState Excute()
    {
        m_Owner.Attack(Vector2.zero);
        m_Owner.OppositePlayer();

        if (m_Owner.Owner.IsPlayComplete())
        {
            m_CurrAttackCount++;
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        if (m_CurrAttackCount >= m_AttackCount)
        {
            m_CurrAttackCount = 0;
            m_AttackCount = 0;
        }
    }
}
