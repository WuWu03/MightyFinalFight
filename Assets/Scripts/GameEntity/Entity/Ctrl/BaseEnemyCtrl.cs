using GameFrameWork.BehaviourTree;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    protected override void OnInit()
    {
        base.OnInit();
    }

    public override void SetData(BaseRoleSkillData data)
    {
        BaseEnemySkillData baseEnemySkillData = data as BaseEnemySkillData;
        m_BehaviourID = baseEnemySkillData.behaviourTreeIds[0];
        BehaviourTreeMgr.instance.AddBehaviourTree(this, m_BehaviourID);
        base.SetData(data);
    }

    protected override void OnStart()
    {
        base.OnStart();
        BehaviourTreeMgr.instance.StartTree(this, m_BehaviourID);
    }

    protected override void OnStop()
    {
        base.OnStop();
        BehaviourTreeMgr.instance.StopTree(this, m_BehaviourID);
    }

    protected override void OnRelease()
    {
        BehaviourTreeMgr.instance.RemoveBehaviourTree(this, m_BehaviourID);
        base.OnRelease();
    }

    public void OppositePlayer()
    {
        m_Owner.SetDir(PlayerMgr.instance.player.pos.x - m_Owner.pos.x > 0 ? 1f : -1f);
    }

    public void Resume()
    {
        BehaviourTreeMgr.instance.ResumeTree(this, m_BehaviourID);
        (m_Owner as BaseEnemy).Resume();
    }

    public void Pause()
    {
        BehaviourTreeMgr.instance.PauseTree(this, m_BehaviourID);
        (m_Owner as BaseEnemy).Pause();
    }

    private int m_BehaviourID = 0;
}
