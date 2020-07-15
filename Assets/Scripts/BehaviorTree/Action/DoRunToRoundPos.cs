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
        m_RoundPos[0] = new Vector2(center.x, randomY);
        m_RoundPos[1] = to;
        m_CurrIndex = 0;
        m_IsArravied = false;
    }

    public override BehaviorTreeState Excute()
    {
        Vector2 enemyPos = m_Owner.Owner.Pos;
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;

        if (m_IsArravied)
        {
            if (m_CurrIndex >= m_RoundPos.Length - 1)
            {
                m_Owner.Move(Vector2.zero);
                m_Owner.IsRoundPos = false;
                m_Owner.Owner.SetDir(playerPos.x - enemyPos.x > 0 ? 1 : -1);
                return BehaviorTreeState.Success;
            }

            m_CurrIndex++;
        }

        m_IsArravied = Vector2.Distance(enemyPos, m_RoundPos[m_CurrIndex]) < 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RoundPos[m_CurrIndex] - enemyPos).normalized);
            m_Owner.Owner.SetDir(playerPos.x - enemyPos.x > 0 ? 1 : -1);
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


    private int m_CurrIndex = 0;
    private Vector2[] m_RoundPos = new Vector2[2];
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}