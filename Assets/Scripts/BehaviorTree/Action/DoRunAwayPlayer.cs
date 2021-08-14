using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunAwayPlayer : Action
{
    public DoRunAwayPlayer(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    public override BehaviorTreeState Excute()
    {
        if (PlayerMgr.Ins.Player.Dir != m_Owner.Owner.Dir)
        {
            m_Owner.Move((PlayerMgr.Ins.Player.MoveDir).normalized, false);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Running;
        }

        return BehaviorTreeState.Success;
    }

    public override void Reset()
    {
        base.Reset();
    }

    private new BaseEnemyCtrl m_Owner = null;
}
