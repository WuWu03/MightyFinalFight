using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToPlayer : Action
{
    public DoRunToPlayer(string name, string args, object owner) : base(name, args, owner) 
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
       
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        m_TargetPos = PlayerMgr.Ins.Player.Pos;
        float distance = PlayerMgr.Ins.Player.GetCurrTriggerSize().x / 2 + m_Owner.Owner.GetCurrTriggerSize().x / 2 - 0.05f;
        m_TargetPos.x += distance * (m_TargetPos.x - m_Owner.Owner.Pos.x > 0 ? -1f : 1f);
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }
    
        m_IsArravied = Mathf.Abs(m_TargetPos.x - m_Owner.Owner.Pos.x) <= 0.03f && Mathf.Abs(m_TargetPos.y - m_Owner.Owner.Pos.y) <= 0.03f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_TargetPos - m_Owner.Owner.Pos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
    }

    private Vector2 m_TargetPos = Vector2.zero;
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}
