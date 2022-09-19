using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMoveToPos : BaseTask
{
    public TaskMoveToPos(TaskConfigData data) : base(data) 
    {
        m_XArrived = false;
        m_YArrived = false;
    }
  
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        Vector2 pos = PlayerMgr.instance.player.pos;

        if (!m_XArrived)
        {
            float xOffest = (float)m_TaskData.Position.Pos.x / 100 - pos.x;
            m_XArrived = Mathf.Abs(xOffest) <= 0.05f;
        }

        if (!m_YArrived)
        {
            float yOffest = (float)m_TaskData.Position.Pos.y / 100 - pos.y;
            m_YArrived = Mathf.Abs(yOffest) <= 0.05f;
        }
    }

    public override bool CheckCondition()
    {
        if (m_TaskData.Position.PosType == TaskConfigData.TaskPosType.X) return m_XArrived;
        if (m_TaskData.Position.PosType == TaskConfigData.TaskPosType.Y) return m_YArrived;
        return m_XArrived && m_YArrived;
    }

    private bool m_XArrived = false;
    private bool m_YArrived = false;
}
