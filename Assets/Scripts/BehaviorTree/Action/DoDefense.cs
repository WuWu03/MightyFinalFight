using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoDefense : Action
{
    public DoDefense(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
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
