using GameFrameWork.BehaviourTree;

public class PreIsPlayerSpecialState : PreCondition
{
    public PreIsPlayerSpecialState(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {

    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.instance.player.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken), typeof(RoleDead), typeof(HeroRebirth));
    }
}