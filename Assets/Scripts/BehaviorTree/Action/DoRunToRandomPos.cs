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
        Rect visionRect = CameraMgr.Ins.GetVision();
        m_RandomPos = StageMgr.Ins.GetRandomPos2(new Vector2(PlayerMgr.Ins.Player.Pos.x, PlayerMgr.Ins.Player.Bound.yMin));
        m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + m_Owner.Owner.Bound.width, visionRect.xMax - m_Owner.Owner.Bound.width);
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

        float x = m_Owner.Owner.Pos.x;
        float y = m_Owner.Owner.Bound.yMin;
        m_IsArravied = Mathf.Abs(m_RandomPos.x - x) <= 0.05f &&
                       Mathf.Abs(m_RandomPos.y - y) <= 0.05f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RandomPos - (Vector2.right * x + Vector2.up * y)).normalized, false);
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