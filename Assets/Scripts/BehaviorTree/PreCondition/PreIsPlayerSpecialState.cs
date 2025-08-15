using GameFrameWork.BehaviourTree;

public class PreIsPlayerSpecialState : PreCondition
{
    public PreIsPlayerSpecialState(string name, int id, object owner, int priority, bool isAndCondiont, string args) : base(name, id, owner, priority, isAndCondiont, args)
    {

    }

    protected override bool OnCheckPreCondition()
    {
        BaseHero baseHero = PlayerMgr.instance.player;
        return baseHero.IsAnyState(typeof(RoleSwoon), typeof(RoleAwaken), typeof(RoleDead), typeof(HeroRebirth)) || baseHero.isRebirthState;
    }
}