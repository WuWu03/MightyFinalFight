using GameFrameWork.BehaviourTree;
using GameFrameWork.Input;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    public BehaviourTreeMgr behaviourTreeMgr
    {
        get
        {
            return m_BehaviourTreeMgr;
        }
    }

    public void OppositePlayer()
    {
        m_Owner.SetDir(PlayerMgr.instance.player.pos.x - m_Owner.pos.x > 0 ? 1f : -1f);
    }

    public void Resume()
    {
        m_BehaviourTreeMgr.ResumeAll();
        (m_Owner as BaseEnemy).Resume();
    }

    public void Pause()
    {
        m_BehaviourTreeMgr.PauseAll();
        (m_Owner as BaseEnemy).Pause();
    }

    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this);
    }

    public override void SetData(BaseRoleSkillData data)
    {
        BaseEnemySkillData baseEnemySkillInfo = data as BaseEnemySkillData;
        m_BehaviourTreeMgr.InitTree(baseEnemySkillInfo.behaviourTreeIds);
        base.SetData(data);
    }

    protected override void OnStart()
    {
        base.OnStart();
        m_BehaviourTreeMgr.Start();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_BehaviourTreeMgr.Update(Time.deltaTime);
    }

    protected override void OnRelease()
    {
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
        base.OnRelease();
    }



    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
