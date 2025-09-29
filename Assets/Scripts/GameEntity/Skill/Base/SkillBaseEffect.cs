public abstract class SkillBaseEffect : SkillBase, ISkillEffect
{
    public SkillBaseEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex)
    {
        m_Deployer = deployer;
    }

    public bool isCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }

    public abstract void Effect(ISkillSelector selector);

    public void Update(ISkillSelector selector)
    {
        OnUpdate(selector);
    }
 
    public void Complete()
    {
        m_IsCompleted = true;
        OnComplete();
    }

    public void Exit()
    {
        OnExit();
    }

    public void Reset()
    {
        m_IsCompleted = false;
        OnReset();
    }

    protected virtual void OnUpdate(ISkillSelector selector) { }
    protected virtual void OnReset() { }
    protected virtual void OnComplete() { }
    protected virtual void OnExit() { }

    protected SkillBaseDeployer m_Deployer = null;
    private bool m_IsCompleted = false;
}
