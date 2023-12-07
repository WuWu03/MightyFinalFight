using GameFrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsAnimComplete : PreCondition
{
    public PreIsAnimComplete(string name, string args, object owner,int priority) : base(name, args, owner, priority)
    {
        m_PreOwner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_PreOwner.owner.IsPlayComplete();
    }

    private BaseRoleCtrl m_PreOwner = null;
}

