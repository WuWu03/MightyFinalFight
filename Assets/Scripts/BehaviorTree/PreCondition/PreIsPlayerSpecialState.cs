using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsPlayerSpecialState : PreCondition
{
    public PreIsPlayerSpecialState(string name, string args, object owner) : base(name, args, owner)
    {

    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.Ins.Player.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken), typeof(RoleDead), typeof(HeroRebirth));
    }
}