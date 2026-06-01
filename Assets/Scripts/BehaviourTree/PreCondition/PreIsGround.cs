using WuWuFramework.BehaviourTree;

public class PreIsGround : PreCondition
{
    public PreIsGround(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Owner = owner as BaseRole;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.isInGround;
    }

    private BaseRole m_Owner = null;
}