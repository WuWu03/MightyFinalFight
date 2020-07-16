using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.BehaviourTree;
using FrameWork.Camera;

public class DoRunToRandomPos : Action
{
    public DoRunToRandomPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        float x = Random.Range(PlayerMgr.Ins.Player.Pos.x - 1f, PlayerMgr.Ins.Player.Pos.x + 1f);
        float y = StageMgr.Ins.GetRandomY(x);
        m_RandomPos.x = x;
        m_RandomPos.y = y;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.IsRandomPos = false;
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }

        Vector2 enemyPos = m_Owner.Owner.Pos;
        m_IsArravied = Mathf.Abs(m_RandomPos.x - enemyPos.x) <= 0.05f && Mathf.Abs(m_RandomPos.y - enemyPos.y) <= 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RandomPos - enemyPos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.IsRandomPos = false;
    }

    private Vector2 m_RandomPos = Vector2.zero;
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}