using GameFrameWork;
using GameFrameWork.Fsm;

public class SkillStateArg : FsmStateArg
{
    public int skillID { get; set; }
    public string animName { get; set; }
    public int animTime { get; set; }
    public float animSpeed { get; set; }
    public float dir { get; set; }
    public bool canChangeDir { get; set; }
    public bool canMove { get; set; }

    public static SkillStateArg Create()
    {
        return ReferencePool.Acquire<SkillStateArg>();
    }

    public override void Clear()
    {
        skillID = 0;
        animName = string.Empty;
        animTime = 0;
        animSpeed = 0;
        dir = 0;
        canChangeDir = false;
        canMove = false;
    }
}