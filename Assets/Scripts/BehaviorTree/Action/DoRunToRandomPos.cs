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
        float size = m_Owner.owner.GetCurrTriggerSize().x / 2;
        Rect visionRect = CameraMgr.instance.GetVision();
        m_RandomPos = StageMgr.instance.GetRandomPos();
        m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + size, visionRect.xMax - size);
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero, false);
            m_Owner.OppositePlayer();
            return BehaviorTreeState.Success;
        }

        m_IsArravied = Mathf.Abs(m_RandomPos.x - m_Owner.owner.pos.x) <= 0.03f &&
                       Mathf.Abs(m_RandomPos.y - m_Owner.owner.pos.y) <= 0.03f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RandomPos - m_Owner.owner.pos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
    }

    private Vector2 m_RandomPos = Vector2.zero;
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}