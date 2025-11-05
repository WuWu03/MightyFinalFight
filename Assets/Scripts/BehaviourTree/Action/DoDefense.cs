using GameFrameWork.BehaviourTree;

public class DoDefense : Action
{
    public DoDefense(int id, object owner, int priority, string args) : base(id, owner, priority, args)
    {
        m_Owner = owner as BaseEnemy;
    }

    protected override void OnEnter()
    {

    }

    public override BehaviourTreeState Excute()
    {
        m_Owner.DefenseState(PlayerMgr.instance.player.dir);
        m_Owner.OppositePlayer();
        return BehaviourTreeState.Success;
    }

    protected BaseEnemy m_Owner = null;
}
