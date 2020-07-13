using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    public override void Init(BaseRoleSkillData data)
    {
        base.Init(data);
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this, StaticConfig.BehaviourTreeConfig);
        m_BehaviourTreeMgr.Init(1001);
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        m_BehaviourTreeMgr.Start();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_BehaviourTreeMgr.Update(Time.deltaTime);
    }

    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
