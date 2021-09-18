using GameFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerAutoMoveToPos : BaseTaskTrigger
{
    public TaskTriggerAutoMoveToPos(TaskConfigData data) : base(data) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        float x = (float)m_TaskData.Position.Pos.x / 100f;
        float y = (float)m_TaskData.Position.Pos.y / 100f;
        PlayerMgr.Ins.Player.AutoMoveToPos(new Vector2(x, y));
    }

    public override void Trigger()
    {
        base.Trigger();

        if (!PlayerMgr.Ins.Player.IsAutoMove)
        {
            Complete();
        }
    }
}