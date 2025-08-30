using GameFrameWork.BehaviourTree;

public class PreIsAnimComplete : PreCondition
{
    public PreIsAnimComplete(int id, object owner, int priority, bool isAndCondiont, string args) : base(id, owner, priority, isAndCondiont, args)
    {
        m_Owner = owner as BaseRole;
    }

    protected override bool OnCheckPreCondition()
    {
        return m_Owner.IsAllAnimationComplete();
    }

    private BaseRole m_Owner = null;
}

