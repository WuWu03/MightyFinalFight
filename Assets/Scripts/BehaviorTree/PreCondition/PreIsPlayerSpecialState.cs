using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsPlayerSpecialState : PreCondition
{
    public PreIsPlayerSpecialState(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {

    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.instance.player.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken), typeof(RoleDead), typeof(HeroRebirth));
    }
}