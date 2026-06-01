using WuWuFramework.BehaviourTree;

public class PreIsCanSkill : PreCondition
{
    public PreIsCanSkill(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Owner = owner as BaseRole;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.canSkill || m_Owner.IsCurrState<RoleSkill>();
    }

    private BaseRole m_Owner = null;
}
