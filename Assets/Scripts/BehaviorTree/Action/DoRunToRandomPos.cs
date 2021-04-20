using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;

public class DoRunToRandomPos : Action
{
    public DoRunToRandomPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        m_RandomPos = StageMgr.Ins.GetRandomPos2(PlayerMgr.Ins.Player.Pos);
        m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + m_Owner.Owner.Bound.width / 2, visionRect.xMax - m_Owner.Owner.Bound.width / 2);
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.SetBehaviourState(BehaviourType.RandomPos);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }

        m_IsArravied = Mathf.Abs(m_RandomPos.x - m_Owner.Owner.Pos.x) <= 0.03f &&
                       Mathf.Abs(m_RandomPos.y - m_Owner.Owner.Pos.y) <= 0.03f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RandomPos - m_Owner.Owner.Pos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.SetBehaviourState(BehaviourType.RandomPos);
    }

    private Vector2 m_RandomPos = Vector2.zero;
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}