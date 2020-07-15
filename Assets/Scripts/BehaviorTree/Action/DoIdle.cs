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

    protected override void OnUpdate(float deltaTime)
    {
        
    }

    public override BehaviorTreeState Excute()
    {
        if(Time.time - m_IdleTimer >= m_IdleTime)
        {
            m_Owner.IsIdle = false;
            return BehaviorTreeState.Success;
        }

        Vector2 enemyPos = m_Owner.Owner.Pos;
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        m_Owner.Move(Vector2.zero);
        m_Owner.Owner.SetDir(playerPos.x - enemyPos.x > 0 ? 1 : -1);

        return BehaviorTreeState.Running;
    }

    private float m_IdleTime = 0f;
    private float m_IdleTimer = 0f;
    private new BaseEnemyCtrl m_Owner = null;
}
