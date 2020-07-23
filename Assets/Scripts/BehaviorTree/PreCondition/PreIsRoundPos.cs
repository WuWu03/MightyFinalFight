using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsRoundPos : PreCondition
{
    public PreIsRoundPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.GetBehaviourState(BehaviourType.RoundPos);
    }

    private new BaseEnemyCtrl m_Owner = null;
}

