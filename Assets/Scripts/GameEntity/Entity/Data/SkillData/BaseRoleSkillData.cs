using GameFrameWork;

public class BaseRoleSkillData : BaseEventArgs
{
    public int[] attackIds { get; set; }
    public int[] jumpAttackIds { get; set; }
    public int[] skillIds { get; set; }
    public float[] attackWait { get; set; }

    public override void Clear()
    {
        attackIds = null;
        jumpAttackIds = null;
        skillIds = null;
        attackWait = null;
    }
}
