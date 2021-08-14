using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoDefense : Action
{
    public DoDefense(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {

    }

    public override BehaviorTreeState Excute()
    {
        m_Owner.Owner.OnDefenseMsg(PlayerMgr.Ins.Player.Dir);
        m_Owner.OppositePlayer();
        return BehaviorTreeState.Success;
    }


    public override void Reset()
    {
        base.Reset();
    }

    protected new BaseEnemyCtrl m_Owner = null;
}
