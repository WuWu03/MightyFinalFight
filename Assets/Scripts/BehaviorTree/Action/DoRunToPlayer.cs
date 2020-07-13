using FrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToPlayer : Action
{
    public DoRunToPlayer(string name, string args, object owner) : base(name, args, owner) 
    {
        m_MoveData = new MoveData();
    }

    protected override void OnEnter()
    {
        m_Owner = base.m_Owner as BaseEnemy;
        m_IsArravied = false;
    }

    protected override void OnUpdate(float deltaTime)
    {
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        Vector2 enemyPos = m_Owner.Pos;

        m_IsArravied = Vector2.Distance(playerPos, enemyPos) <= 0.1f;
        if (!m_IsArravied)
        {
            m_MoveData.Dir = (playerPos - enemyPos).normalized;
            m_Owner.OnMoveMsg(m_MoveData);
        }
    }

    public override BehaviorTreeState Excute()
    {
        if(m_IsArravied)
        {
            return BehaviorTreeState.Success;
        }

        return BehaviorTreeState.Running;
    }

    private MoveData m_MoveData = null;
    private bool m_IsArravied = false;
    private new BaseEnemy m_Owner = null;
}
