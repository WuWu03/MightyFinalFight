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
       
    }

    public override BehaviorTreeState Excute()
    {
        Vector2 enemyPos = m_Owner.Owner.Pos;

        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero);
            m_Owner.Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - enemyPos.x > 0 ? 1 : -1);
            return BehaviorTreeState.Success;
        }
    
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        playerPos = playerPos + Vector2.right * 0.2f * (playerPos.x - enemyPos.x > 0 ? -1f : 1f);

        m_IsArravied = Mathf.Abs(playerPos.x - enemyPos.x) <= 0.05f && Mathf.Abs(playerPos.y - enemyPos.y) <= 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((playerPos - enemyPos).normalized);
            m_Owner.Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - enemyPos.x > 0 ? 1 : -1);
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
    }

    protected override void OnUpdate(float deltaTime)
    {
        
    }

    private bool m_IsArravied = false;
    private new BaseRoleCtrl m_Owner = null;
}
