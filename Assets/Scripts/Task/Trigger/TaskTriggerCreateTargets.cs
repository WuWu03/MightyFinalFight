using UnityEngine;

public class TaskTriggerCreateTargets : BaseTaskTrigger
{
    public TaskTriggerCreateTargets(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        for (int i = 0; i < mTaskData.Targets.Length; i++)
        {       
            int entityId = mTaskData.Targets[i].EntityID;       
            Vector2Int pos = mTaskData.Targets[i].Pos;

            if (mTaskData.Targets[i].IsBarrel)
            {
                float dir = mTaskData.Targets[i].Dir;
                int groundY = mTaskData.Targets[i].GroundY;
                int itemId = mTaskData.Targets[i].ItemId;
                bool isFloat = mTaskData.Targets[i].IsFloat;
                float moveSpeed = mTaskData.Targets[i].MoveSpeed;
                SceneEntityMgr.instance.CreateBarrel(entityId, dir, groundY, itemId, isFloat, moveSpeed, pos);
            }
            else
            {
                int sourceId = mTaskData.Targets[i].SourceID;
                int hp = mTaskData.Targets[i].Hp;
                int attack = mTaskData.Targets[i].AttackValue;
                int defense = mTaskData.Targets[i].DefenseValue;
                int hpBarWidth = mTaskData.Targets[i].HpBarWidth;
                SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
            }
        }

        Complete();
    }
}