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
        base.Enter();
        PlayerMgr.Ins.CanContrl = false;
        m_XArrived = false;
        m_YArrived = false;
    }

    public override void Update()
    {
        base.Update();
        Vector2 pos = PlayerMgr.Ins.Player.Pos;

        if (!m_YArrived)
        {
            float yOffest = (float)m_TaskData.Position.Pos.y / 100 - pos.y;
            m_YArrived = Mathf.Abs(yOffest) <= 0.05f;
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.up * yOffest).normalized;
            PlayerMgr.Ins.Player.OnMoveMsg(data);
            ReferencePool.Release(data);
            return;
        }

        if (!m_XArrived)
        {
            float xOffest = (float)m_TaskData.Position.Pos.x / 100 - pos.x;
            m_XArrived = Mathf.Abs(xOffest) <= 0.05f;
            MoveData data = MoveData.Create();
            data.Dir = (Vector2.right * xOffest).normalized;
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
