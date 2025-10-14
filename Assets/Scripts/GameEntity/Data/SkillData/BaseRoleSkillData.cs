using GameFrameWork;

public class BaseRoleSkillData : GameFrameWorkEventArg
{
    public int roleId { get; set; }
    public int[] attackIds { get; set; }
    public int[] jumpAttackIds { get; set; }
    public int[] skillIds { get; set; }
    public float[] attackWait { get; set; }

    public override void Clear()
    {
        roleId = 0;
        attackIds = null;
        jumpAttackIds = null;
        skillIds = null;
        attackWait = null;
    }
}