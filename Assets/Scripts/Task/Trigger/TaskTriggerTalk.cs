using GameFrameWork.Event;
using GameFrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TaskTriggerTalk : BaseTaskTrigger
{
    // Start is called before the first frame update
    public TaskTriggerTalk(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        base.Enter();
        UIMgr.instance.Open<TalkPanel>(m_TaskData.TalkID);
        PlayerMgr.instance.player.currCtrl.Move(Vector2.zero);
        EventMgr.instance.Subscribe(EventDefine.TalkEndEvent, OnTalkEnd);
    }

    private void OnTalkEnd(object sender, GameEventArgs e)
    {
        Complete();
    }

    public override void Trigger()
    {
        base.Trigger();
    }

    public override void Complete()
    {
        base.Complete();
        EventMgr.instance.UnSubscribe(EventDefine.TalkEndEvent, OnTalkEnd);
    }
}
