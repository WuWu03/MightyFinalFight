using FrameWork.BehaviourTree;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    public bool IsRandomPos
    {
        get
        {
            RandomBehaviour();
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
            RandomBehaviour();
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
            RandomBehaviour();
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

    public void OppositePlayer()
    {
        m_Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - m_Owner.Pos.x > 0 ? 1 : -1);
    }
    private void RandomBehaviour()
    {
        if (m_IsIdle || m_IsRandomPos || m_IsRoundPos)
        {
            return;
        }

        m_IsRandomPos = Random.Range(1, 1001) <= 10;
        m_IsRoundPos = Random.Range(1, 1001) <= 5;
        m_IsIdle = Random.Range(1, 1001) <= 2;

        if (m_IsRandomPos)
        {
            m_IsRoundPos = false;
            m_IsIdle = false;
        }

        if (m_IsRoundPos)
        {
            m_IsRandomPos = false;
            m_IsIdle = false;
        }

        if (m_IsIdle)
        {
            m_IsRandomPos = false;
            m_IsRoundPos = false;
        }
    }

    private bool m_IsIdle = false;
    private bool m_IsRoundPos = false;
    private bool m_IsRandomPos = false;
    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
