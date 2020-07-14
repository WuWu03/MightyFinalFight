using FrameWork.BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    public bool IsRandomPos
    {
        get
        {
            return m_IsRandomPos;
        }
    }

    public bool IsRoundPos
    {
        get
        {
            return m_IsRoundPos;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this, StaticConfig.BehaviourTreeConfig);
        m_BehaviourTreeMgr.Init(1001);
        m_BehaviourTreeMgr.Start();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_BehaviourTreeMgr.Update(Time.deltaTime);
        RandomPos();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
    }

    public virtual void SetCanRandomPos(bool value)
    {
        m_CanRandomPos = value;
    }

    private void RandomPos()
    {
        if (!m_CanRandomPos)
        {
            m_RandomTimer = 0f;
            m_IsRandomPos = false;
            m_IsRoundPos = false;
            return;
        }

        if (m_RandomTimer == 0 || Time.time - m_RandomTimer >= 3.0f)
        {
            m_RandomTimer = Time.time;
            m_IsRandomPos = Random.Range(1, 101) <= 50;
            m_IsRoundPos = !m_IsRandomPos && Random.Range(1, 101) <= 50;
        }
    }

    private float m_RandomTimer = 0;
    private bool m_CanRandomPos = true;
    private bool m_IsRoundPos = false;
    private bool m_IsRandomPos = false;
    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
