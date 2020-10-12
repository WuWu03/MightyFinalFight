using FrameWork.BehaviourTree;
using FrameWork.Input;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this, StaticConfig.BehaviourTreeConfig);
    }

    public override void InitData(BaseRoleSkillInfo data)
    {
        base.InitData(data);
        BaseEnemySkillInfo baseEnemySkillInfo = data as BaseEnemySkillInfo;
        m_BehaviourRate = baseEnemySkillInfo.BehaviourRate;
        m_BehaviourState = new bool[m_BehaviourRate.Length];  
        m_BehaviourTreeMgr.Init(baseEnemySkillInfo.BehaviourTreesID);
        m_BehaviourTreeMgr.Start();
    }
 
    protected override void OnUpdate()
    {
        m_BehaviourTreeMgr.Update(Time.deltaTime);
        base.OnUpdate();
        //Test();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
    }

    public void OppositePlayer()
    {
        m_Owner.SetDir(PlayerMgr.Ins.Player.Pos.x - m_Owner.Pos.x > 0 ? 1f : -1f);
    }

    public bool HasBehaviour()
    {
        RandomBehaviour();
        for (int i = 0; i < m_BehaviourState.Length; i++)
        {
            if (m_BehaviourState[i])
                return true;
        }

        return false;
    }

    public bool GetBehaviourState(int index)
    {
        RandomBehaviour();
        return m_BehaviourState[index];
    }

    public void SetBehaviourState(int index)
    {
        m_BehaviourState[index] = false;   
    }

    private void RandomBehaviour()
    {
        for (int i = 0; i < m_BehaviourState.Length; i++)
        {
            if (m_BehaviourState[i]) return;
        }

        for (int i = 0; i < m_BehaviourState.Length; i++)
        {
            m_BehaviourState[i] = Random.Range(1, 1001) <= m_BehaviourRate[i];
            if (m_BehaviourState[i]) break;
        }
    }

    private bool[] m_BehaviourState = null;
    private int[] m_BehaviourRate = null;
    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
