using FrameWork.BehaviourTree;
using UnityEngine;

public class DoAttack : Action
{
    public DoAttack(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    public override BehaviorTreeState Excute()
    {
        m_Owner.Attack(Vector2.zero);
        m_Owner.OppositePlayer();

        if (m_Owner.Owner.IsPlayComplete())
        {
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }

    private new BaseEnemyCtrl m_Owner = null;
}
