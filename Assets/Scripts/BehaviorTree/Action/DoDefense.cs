using GameFrameWork.BehaviourTree;

public class DoDefense : Action
{
    public DoDefense(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_Owner = base.m_Owner as BaseEnemy;
    }

    protected override void OnEnter()
    {

    }

    public override BehaviourTreeState Excute()
    {
        m_Owner.OnDefenseMsg(PlayerMgr.instance.player.dir);
        m_Owner.OppositePlayer();
        return BehaviourTreeState.Success;
    }

    protected new BaseEnemy m_Owner = null;
}
