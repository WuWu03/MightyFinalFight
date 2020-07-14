using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsFoundPlayer : PreCondition
{
    public PreIsFoundPlayer(string name, string args, object owner) : base(name, args, owner) { }

    protected override bool OnCheckPreCondition()
    {
        return true;
    }

    protected override void OnEnter()
    {
        m_Owner = base.m_Owner as BaseEnemy;
    }

    private new BaseEnemy m_Owner = null;
}
