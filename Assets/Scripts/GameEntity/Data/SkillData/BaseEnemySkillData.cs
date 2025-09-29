public class BaseEnemySkillData : BaseRoleSkillData
{
    public int[] behaviourTreeIds { get; set; }

    public override void Clear()
    {
        base.Clear();
        behaviourTreeIds = null;
    }
}
