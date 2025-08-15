using GameFrameWork.BehaviourTree;

public class PreIsPlayerCanBeHit : PreCondition
{
    public PreIsPlayerCanBeHit(string name, int id, object owner, int priority, bool isAndCondiont, string args) : base(name, id, owner, priority, isAndCondiont, args)
    {
    }

    protected override bool OnCheckPreCondition()
    {
        return PlayerMgr.instance.player.canBeHit || PlayerMgr.instance.player.isRebirthState;
    }
}