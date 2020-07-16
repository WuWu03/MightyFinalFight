using FrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToPlayer : Action
{
    public DoRunToPlayer(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }
    
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        Vector2 enemyPos = m_Owner.Owner.Pos;
        playerPos = playerPos + Vector2.right * 0.2f * (playerPos.x - enemyPos.x > 0 ? -1f : 1f);

        m_IsArravied = Mathf.Abs(playerPos.x - enemyPos.x) <= 0.05f && Mathf.Abs(playerPos.y - enemyPos.y) <= 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((playerPos - enemyPos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
    }

    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}
