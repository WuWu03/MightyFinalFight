using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class TaskTriggerCreateTargets : BaseTaskTrigger
{
    public TaskTriggerCreateTargets(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        for (int i = 0; i < m_TaskData.Targets.Length; i++)
        {       
            int entityId = m_TaskData.Targets[i].EntityID;       
            Vector2Int pos = m_TaskData.Targets[i].Pos;

            if (m_TaskData.Targets[i].IsBarrel)
            {
                float dir = m_TaskData.Targets[i].Dir;
                int groundY = m_TaskData.Targets[i].GroundY;
                int itemId = m_TaskData.Targets[i].ItemId;
                bool isFloat = m_TaskData.Targets[i].IsFloat;
                float moveSpeed = m_TaskData.Targets[i].MoveSpeed;
                SceneEntityMgr.instance.CreateBarrel(entityId, dir, groundY, itemId, isFloat, moveSpeed, pos);
            }
            else
            {
                int sourceId = m_TaskData.Targets[i].SourceID;

                if (sourceId == 2004)
                {
                    int hp = 5000;// m_TaskData.Targets[i].Hp;
                    int attack = 1;//m_TaskData.Targets[i].AttackValue;
                    int defense = m_TaskData.Targets[i].DefenseValue;
                    int hpBarWidth = m_TaskData.Targets[i].HpBarWidth;
                    SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
                    break;
                }
            }
        }

        Complete();
    }
}