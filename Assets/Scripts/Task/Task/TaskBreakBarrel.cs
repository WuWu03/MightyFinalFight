public class TaskBreakBarrel : BaseTask
{
    public TaskBreakBarrel(TaskConfigData data) : base(data) { }
    public override bool CheckCondition()
    {
        if (mTaskData.KillAllBarrels)
        {
            return SceneEntityMgr.instance.IsAllBarrelsBreak();
        }

        if (mTaskData.KillEnemyIDs.Length < 1)
        {
            return true;
        }

        for (int i = 0; i < mTaskData.KillEnemyIDs.Length; i++)
        {
            if (!SceneEntityMgr.instance.IsBarrelBreak(mTaskData.KillEnemyIDs[i]))
            {
                return false;
            }
        }

        return true;
    }
}
