using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerCanBeHit : PreCondition
{
    public PreIsPlayerCanBeHit(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.instance.player.canBeHit || PlayerMgr.instance.player.isRebirthState;
    }
}