using GameFrameWork.BehaviourTree;
using GameFrameWork.Input;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this, StaticConfig.BehaviourTreeConfig);
    }

    public override void SetData(BaseRoleSkillData data)
    {
        base.SetData(data);
        BaseEnemySkillData baseEnemySkillInfo = data as BaseEnemySkillData;
        m_BehaviourTreeMgr.Init(baseEnemySkillInfo.BehaviourTreesID);
        m_BehaviourTreeMgr.Start();
    }
 
    protected override void OnUpdate()
    {
        m_BehaviourTreeMgr.Update(Time.deltaTime);
        base.OnUpdate();
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
        return false;
    }

    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
