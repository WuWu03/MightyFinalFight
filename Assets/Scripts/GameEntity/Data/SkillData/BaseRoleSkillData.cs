using GameFrameWork;

public class BaseRoleSkillData : BaseEventArgs
{
    public int[] AttackIds;
    public int[] JumpAttackIds;
    public int[] SkillIds;
    public float[] AttackWait;
    public float AttackNextTime;

    public override void Clear()
    {
        AttackIds = null;
        JumpAttackIds = null;
        SkillIds = null;
        AttackWait = null;
        AttackNextTime = 0;
    }
}
