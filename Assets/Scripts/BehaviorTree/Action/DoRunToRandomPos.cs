using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;

public class DoRunToRandomPos : Action
{
    public DoRunToRandomPos(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_ActionOwner = base.m_Owner as BaseEnemyCtrl;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }
    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;

        Vector2 ownerSize = m_ActionOwner.owner.GetCurrTriggerSize();
        Vector2 playerSize = PlayerMgr.instance.player.GetCurrTriggerSize();
        Rect visionRect = CameraMgr.instance.GetVision();
        Vector2 playerPos = PlayerMgr.instance.player.pos;

        visionRect.xMin = Mathf.Max(visionRect.xMin, playerPos.x - visionRect.width / 3);
        visionRect.xMax = Mathf.Min(visionRect.xMax, playerPos.x + visionRect.width / 3);

        m_RandomPos = StageMgr.instance.GetRandomPos(visionRect);
       // m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + size, visionRect.xMax - size);

        if(m_RandomPos.x <= playerPos.x)
        {
            m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, visionRect.xMin + ownerSize.x / 2, playerPos.x - (playerSize.x + ownerSize.x) / 2);
        }
        else
        {
            m_RandomPos.x = Mathf.Clamp(m_RandomPos.x, playerPos.x + (playerSize.x + ownerSize.x) / 2, visionRect.xMax - ownerSize.x / 2);
        }
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        bool isArrive = Mathf.Abs(m_RandomPos.x - m_ActionOwner.owner.pos.x) <= 0.03f && Mathf.Abs(m_RandomPos.y - m_ActionOwner.owner.pos.y) <= 0.03f;

        if (isArrive)
        {
            m_ActionOwner.Move(Vector2.zero, false);
            m_ActionOwner.OppositePlayer();
            m_State = BehaviourTreeState.Success;
        }
        else
        {
            m_ActionOwner.Move((m_RandomPos - m_ActionOwner.owner.pos).normalized, false);
            m_ActionOwner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
    }

    private Vector2 m_RandomPos = Vector2.zero;
    private BaseEnemyCtrl m_ActionOwner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}