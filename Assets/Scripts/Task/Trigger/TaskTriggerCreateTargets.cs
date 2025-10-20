using UnityEngine;

public class TaskTriggerCreateTargets : BaseTaskTrigger
{
    public TaskTriggerCreateTargets(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        foreach (var target in taskConfigData.Targets)
        {
            int entityId = target.EntityID;       
            Vector2Int pos = target.Pos;

            if (target.IsBarrel)
            {
                float dir = target.Dir;
                int groundY = target.GroundY;
                int itemId = target.ItemId;
                bool isFloat = target.IsFloat;
                float moveSpeed = target.MoveSpeed;
                SceneEntityMgr.instance.CreateBarrel(entityId, dir, groundY, itemId, isFloat, moveSpeed, pos);
            }
            else
            {
                int sourceId = target.SourceID;
                int hp = target.Hp;
                int attack = target.AttackValue;
                int defense = target.DefenseValue;
                int hpBarWidth = target.HpBarWidth;
                SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
            }
        }

        Complete();
    }
}