using GameFrameWork.BehaviourTree;
using System.Text.RegularExpressions;

public class DoSkill : Action
{
    public DoSkill(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_Owner = base.m_Owner as BaseEnemy;
        m_Regex = new(@"(SkillId:)(-?[0-9]+)");

        if (!string.IsNullOrEmpty(args))
        {
            Match m = m_Regex.Match(args);
            if (m.Success)
            {
                m_SkllId = int.Parse(m.Groups[2].Value);
            }
        }
    }

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;
        m_HasDeploy = false;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (!m_HasDeploy)
        {
            m_Owner.DeploySkill(m_SkllId);
            m_HasDeploy = true;
        }

        if (m_Owner.IsSkillComplete(m_SkllId))
        {
            m_State = BehaviourTreeState.Success;
            return;
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_State = BehaviourTreeState.None;
        m_HasDeploy = false;
    }

    private int m_SkllId = 0;
    private bool m_HasDeploy = false;
    private Regex m_Regex = null;
    private new BaseEnemy m_Owner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}