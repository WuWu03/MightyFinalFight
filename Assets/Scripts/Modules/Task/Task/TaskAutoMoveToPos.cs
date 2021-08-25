using GameFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskAutoMoveToPos : BaseTask
{
    public TaskAutoMoveToPos(TaskConfigData data) : base(data) 
    {

    }
  
    public override void Enter()
    {
        base.Enter();
        float x = (float)m_TaskData.Position.Pos.x / 100f;
        float y = (float)m_TaskData.Position.Pos.y / 100f;

        PlayerMgr.Ins.CanContrl = false;
        PlayerMgr.Ins.Player.AutoMoveToPos(new Vector2(x, y));
    }

    public override void Update()
    {
        base.Update();
    }

    public override bool CheckCondition()
    {
        if(!PlayerMgr.Ins.Player.IsAutoMove)
        {
            PlayerMgr.Ins.CanContrl = true;
            return true;
        }

        return false;
    }
}
