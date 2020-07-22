using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreIsArrived : PreCondition
{
    public PreIsArrived(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        Vector2 enemyPos = m_Owner.Owner.Pos;
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        playerPos = playerPos + Vector2.right * 0.2f * (playerPos.x - enemyPos.x > 0 ? -1f : 1f);

        return Mathf.Abs(playerPos.x - enemyPos.x) <= 0.05f && Mathf.Abs(playerPos.y - enemyPos.y) <= 0.01f;
    }

    private new BaseRoleCtrl m_Owner = null;
}
