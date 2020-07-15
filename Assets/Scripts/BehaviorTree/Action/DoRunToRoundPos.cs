using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.BehaviourTree;
using FrameWork.Camera;

public class DoRunToRoundPos : Action
{
    public DoRunToRoundPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        Vector2 center = PlayerMgr.Ins.Player.Pos;
        float randomY = StageMgr.Ins.GetRandomY(center.x);
        Vector2 current = m_Owner.Owner.Pos;
        float dir = center.x > m_Owner.Owner.Pos.x ? 1 : -1;
        float radius = Random.Range(0.2f, Vector2.Distance(current, center));
        Vector2 to = center + Vector2.right * radius * dir;
        //float angle = Vector2.Angle(current - center, to - center);
        //float lerpX = radius * Mathf.Cos(angle / 2);
        //float lerpY = radius * Mathf.Sin(angle / 2);
        // m_RoundPos[0] = new Vector2(center.x + lerpX, center.y + lerpY);
        m_RoundPos[0].x = center.x;
        m_RoundPos[0].y = randomY;
        m_RoundPos[1] = to;
        m_CurrIndex = 0;
        m_IsArravied = false;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            if (m_CurrIndex >= m_RoundPos.Length - 1)
            {
                m_Owner.Move(Vector2.zero);
                m_Owner.OppositePlayer();
                m_Owner.IsRoundPos = false;          
                return BehaviorTreeState.Success;
            }

            m_Owner.OppositePlayer();
            m_CurrIndex++;
        }

        Vector2 enemyPos = m_Owner.Owner.Pos;
        m_IsArravied = Vector2.Distance(enemyPos, m_RoundPos[m_CurrIndex]) < 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RoundPos[m_CurrIndex] - enemyPos).normalized);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.IsRoundPos = false;
    }

    private int m_CurrIndex = 0;
    private Vector2[] m_RoundPos = new Vector2[2] { Vector2.zero, Vector2.zero };
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}