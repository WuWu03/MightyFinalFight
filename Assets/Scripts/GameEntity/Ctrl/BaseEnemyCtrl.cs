using FrameWork.BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this, StaticConfig.BehaviourTreeConfig);
        m_BehaviourTreeMgr.Init(1001);
        m_BehaviourTreeMgr.Start();
        m_IsAIStart = true;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_BehaviourTreeMgr.Update(Time.deltaTime);
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
    }

    private bool m_IsAIStart = false;
    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
