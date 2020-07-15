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
        Vector2[] vision = CameraMgr.Ins.GetVision();
        float x = Random.Range(vision[0].x, vision[1].x);
        float y = StageMgr.Ins.GetRandomY(x);
        m_RandomPos = new Vector2(x, y);
    }

    public override BehaviorTreeState Excute()
    {
        Vector2 enemyPos = m_Owner.Owner.Pos;

        if (m_IsArravied)
        {
            m_Owner.Move(Vector2.zero);
            m_Owner.IsRandomPos = false;
            m_Owner.Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - enemyPos.x > 0 ? 1 : -1);
            return BehaviorTreeState.Success;
        }

        m_IsArravied = Mathf.Abs(m_RandomPos.x - enemyPos.x) <= 0.05f && Mathf.Abs(m_RandomPos.y - enemyPos.y) <= 0.01f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RandomPos - enemyPos).normalized);
            m_Owner.Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - enemyPos.x > 0 ? 1 : -1);
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.IsRandomPos = false;
    }

    protected override void OnUpdate(float deltaTime)
    {

    }

    private Vector2 m_RandomPos = Vector2.zero;
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}