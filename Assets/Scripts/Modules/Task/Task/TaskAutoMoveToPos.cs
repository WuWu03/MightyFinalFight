using GameFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAutoMoveToPos : BaseTask
{
    public TaskAutoMoveToPos(TaskConfigData data) : base(data) 
    {
        m_XArrived = false;
        m_YArrived = false;
    }
  
    public override void Enter()
    {
        PlayerMgr.Ins.CanContrl = false;
        m_XArrived = false;
        m_YArrived = false;
    }

    public override void Update()
    {
        base.Update();
        Vector2 pos = PlayerMgr.Ins.Player.Pos;

        m_YArrived = Mathf.Abs(m_TaskData.Position.Pos.y / 100 - pos.y) > 0.05f;

        if (!m_YArrived)
        {
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.up * (m_TaskData.Position.Pos.y / 100 - pos.y)).normalized;
            PlayerMgr.Ins.Player.OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }

        m_XArrived = Mathf.Abs(m_TaskData.Position.Pos.x / 100 - pos.x) <= 0.05f;

        if (!m_XArrived)
        {
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.right * (m_TaskData.Position.Pos.x / 100 - pos.x)).normalized;
            PlayerMgr.Ins.Player.OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }
    }

    public override bool CheckCondition()
    {
        PlayerMgr.Ins.CanContrl = m_XArrived && m_YArrived;
        return m_XArrived && m_YArrived;
    }

    private bool m_XArrived = false;
    private bool m_YArrived = false;
}
