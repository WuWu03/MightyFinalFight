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
        m_IsRandomPos = Random.Range(1, 101) <= 50;
        return m_Owner.IsRandomPos;
    }

    protected override void OnEnter()
    {

    }

    private static bool m_IsRandomPos = false;
    private new BaseEnemyCtrl m_Owner = null;
}
