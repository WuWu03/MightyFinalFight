using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsAttack : PreCondition
{
    public PreIsAttack(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.GetBehaviourState(BehaviourType.Attack);
    }

    private new BaseEnemyCtrl m_Owner = null;
}
