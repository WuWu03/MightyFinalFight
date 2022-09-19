using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTriggerEnemy : BaseTaskTrigger
{
    public TaskTriggerEnemy(TaskConfigData data) : base(data) { }

    public override void Trigger()
    {
        base.Trigger();
        for (int i = 0; i < m_TaskData.Targets.Length; i++)
        {
            int sourceId = m_TaskData.Targets[i].SourceID;
            int entityId = m_TaskData.Targets[i].EntityID;
            int hp = m_TaskData.Targets[i].Hp;
            int attack = m_TaskData.Targets[i].AttackValue;
            int defense = m_TaskData.Targets[i].DefenseValue;
            int hpBarWidth = m_TaskData.Targets[i].HpBarWidth;
            Vector2Int pos = m_TaskData.Targets[i].Pos;
            SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
        }

        //SceneEntityMgr.Ins.CreateBarrels();
        Complete();
    }
}