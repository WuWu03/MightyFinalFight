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

    }

    public override void Update()
    {
        base.Update();
        Vector2 pos = PlayerMgr.Ins.Player.Pos;
        if (Mathf.Abs(m_TaskData.PosCondition.Pos.x / 100 - pos.x) <= 0.05f)
            m_XArrived = true;
        if (Mathf.Abs(m_TaskData.PosCondition.Pos.y / 100 - pos.y) <= 0.05f) m_YArrived = true;
    }

    public override bool CheckCondition()
    {
        if (m_TaskData.PosCondition.PosType == TaskConfigData.PosType.X) return m_XArrived;
        if (m_TaskData.PosCondition.PosType == TaskConfigData.PosType.Y) return m_YArrived;
        return m_XArrived && m_YArrived;
    }

    private bool m_XArrived = false;
    private bool m_YArrived = false;
}
