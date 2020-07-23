using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoIdle : Action
{
    public DoIdle(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        m_IdleTime = Random.Range(0.5f, 2f);
        m_IdleTimer = Time.time;
    }

    public override BehaviorTreeState Excute()
    {
        if(Time.time - m_IdleTimer >= m_IdleTime)
        {
            m_Owner.SetBehaviourState(BehaviourType.Idle);
            return BehaviorTreeState.Success;
        }

        m_Owner.Move(Vector2.zero);
        m_Owner.OppositePlayer();

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_Owner.SetBehaviourState(BehaviourType.Idle);
    }

    private float m_IdleTime = 0f;
    private float m_IdleTimer = 0f;
    private new BaseEnemyCtrl m_Owner = null;
}
