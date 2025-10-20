using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoRunToPlayer : Action
{
    public DoRunToPlayer(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    public override bool CanExcute()
    {
        return m_State == BehaviourTreeState.Running;
    }

    protected override void OnEnter()
    {
        base.OnEnter();
        m_State = BehaviourTreeState.Running;
        m_RunTimer = -1f;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if(m_RunTimer > 0 && Time.time - m_RunTimer > 2f)
        {
            m_State = BehaviourTreeState.Failure;
            return;
        }

        Vector2 targetPos = PlayerMgr.instance.player.pos;
        float distance = PlayerMgr.instance.player.bound.width / 2 + m_Owner.bound.width / 2 - 0.05f;

        targetPos.x += distance * (targetPos.x - m_Owner.pos.x > 0 ? -1f : 1f);
        bool isArravied = Vector2.Distance(targetPos, m_Owner.pos) <= 0.01f;// Mathf.Abs(m_TargetPos.x - m_Owner.owner.pos.x) <= 0.03f && Mathf.Abs(m_TargetPos.y - m_Owner.owner.pos.y) <= 0.03f;

        if (isArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_Owner.Move((targetPos - m_Owner.pos).normalized, false);
            m_Owner.OppositePlayer();
        }

        m_RunTimer = Time.time;
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
        m_RunTimer = -1f;
    }

    private float m_RunTimer = -1f;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
    private BaseEnemy m_Owner = null;
}
