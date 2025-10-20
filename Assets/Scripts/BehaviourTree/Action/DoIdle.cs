using GameFrameWork.BehaviourTree;
using UnityEngine;

public class DoIdle : Action
{
    public DoIdle(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
    }

    public override bool CanExecute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        float idleTime = 1 / m_Owner.entityAttribute.moveSpeed;
        m_IdleTime = Random.Range(Mathf.Max(idleTime - 0.5f, 0.1f), Mathf.Max(idleTime, 0.2f));
        m_IdleTimer = Time.time;
        m_State = BehaviourTreeState.Running;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Time.time - m_IdleTimer >= m_IdleTime)
        {
            m_State = BehaviourTreeState.Success;
            return;
        }

        m_Owner.Move(Vector2.zero);
        m_Owner.OppositePlayer();
    }

    private float m_IdleTime = 0f;
    private float m_IdleTimer = 0f;
    private BaseEnemy m_Owner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}