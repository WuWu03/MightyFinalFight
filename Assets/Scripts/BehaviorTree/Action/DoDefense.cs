using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoDefense : Action
{
    public DoDefense(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {

    }

    public override BehaviourTreeState Excute()
    {
        m_Owner.owner.OnDefenseMsg(PlayerMgr.instance.player.dir);
        m_Owner.OppositePlayer();
        return BehaviourTreeState.Success;
    }

    protected new BaseEnemyCtrl m_Owner = null;
}
