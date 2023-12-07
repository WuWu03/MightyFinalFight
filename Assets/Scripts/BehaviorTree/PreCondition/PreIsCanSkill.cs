using GameFrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsCanSkill : PreCondition
{
    public PreIsCanSkill(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_PreOwner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_PreOwner.owner.canSkill || m_PreOwner.owner.IsCurrState<RoleSkill>();
    }

    private BaseRoleCtrl m_PreOwner = null;
}
