using FrameWork.BehaviourTree;
using UnityEngine;

public class DoAttack : Action
{
    public DoAttack(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseRoleCtrl;
    }

    protected override void OnEnter()
    {

    }

    protected override void OnUpdate(float deltaTime)
    {

    }

    public override BehaviorTreeState Excute()
    {
        m_Owner.Attack(Vector2.zero);
        if (m_Owner.Owner.IsPlayComplete())
        {
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
    }

    private new BaseRoleCtrl m_Owner = null;
}
