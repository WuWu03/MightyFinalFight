using GameFrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PreIsPlayerCanBeHit : PreCondition
{
    public PreIsPlayerCanBeHit(string name, string args, object owner) : base(name, args, owner)
    {
    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.Ins.Player.CanBeHit || PlayerMgr.Ins.Player.IsRebirthState;
    }
}