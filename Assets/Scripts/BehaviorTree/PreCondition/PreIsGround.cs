using GameFrameWork.BehaviourTree;

public class PreIsGround : PreCondition
{
    public PreIsGround(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_PreOwner = base.m_Owner as BaseRoleCtrl;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_PreOwner.owner.isInGround;
    }

    private BaseRoleCtrl m_PreOwner = null;
}