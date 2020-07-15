using FrameWork.BehaviourTree;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    public bool IsRandomPos
    {
        get
        {
            if (!m_IsRandomPos)
            {
                m_IsRoundPos = Random.Range(1, 101) <= 20;
            }
            return m_IsRandomPos;
        }
        set
        {
            m_IsRandomPos = value;
        }
    }

    public bool IsRoundPos
    {
        get
        {
            if (!m_IsRoundPos)
            {
                m_IsRoundPos = Random.Range(1, 101) <= 10;
            }
            return m_IsRoundPos;
        }
        set
        {
            m_IsRoundPos = value;
        }
    }


    public bool IsIdle
    {
        get
        {
            if (!m_IsIdle)
            {
                m_IsIdle = Random.Range(1, 101) <= 5;
            }
            return m_IsIdle;
        }
        set
        {
            m_IsIdle = value;
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
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
    }


    private bool m_IsIdle = false;
    private bool m_IsRoundPos = false;
    private bool m_IsRandomPos = false;
    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
