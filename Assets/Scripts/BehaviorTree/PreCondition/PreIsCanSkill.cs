using GameFrameWork.BehaviourTree;

public class PreIsCanSkill : PreCondition
{
    public PreIsCanSkill(string name, int id, object owner, int priority, bool isAndCondiont, string args) : base(name, id, owner, priority, isAndCondiont, args)
    {
        m_Owner = base.m_Owner as BaseRole;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.canSkill || m_Owner.IsCurrState<RoleSkill>();
    }

    private new BaseRole m_Owner = null;
}
