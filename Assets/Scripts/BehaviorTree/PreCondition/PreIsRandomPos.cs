using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsRandomPos : PreCondition
{
    public PreIsRandomPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.GetBehaviourState(BehaviourType.RandomPos);
    }

    private new BaseEnemyCtrl m_Owner = null;
}
