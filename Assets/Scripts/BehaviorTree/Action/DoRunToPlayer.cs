using FrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToPlayer : Action
{
    public DoRunToPlayer(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseRoleCtrl;
    }

    protected override void OnEnter()
    {
        m_IsArravied = false;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero);
            return BehaviorTreeState.Success;
        }

        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        Vector2 enemyPos = m_Owner.Owner.Pos;

        m_IsArravied = Vector2.Distance(playerPos, enemyPos) <= 0.2f;
        if (!m_IsArravied)
        {
            m_Owner.Move((playerPos - enemyPos).normalized);
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.Move(Vector2.zero);
    }

    protected override void OnUpdate(float deltaTime)
    {
        
    }

    private bool m_IsArravied = false;
    private new BaseRoleCtrl m_Owner = null;
}
