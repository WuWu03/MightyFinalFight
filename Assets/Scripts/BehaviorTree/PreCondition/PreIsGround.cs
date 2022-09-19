using GameFrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsGround : PreCondition
{
    public PreIsGround(string name, string args, object owner) : base(name, args, owner)
    {
        m_PreOwner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_PreOwner.owner.isInGround;
    }

    private BaseRoleCtrl m_PreOwner = null;
}
