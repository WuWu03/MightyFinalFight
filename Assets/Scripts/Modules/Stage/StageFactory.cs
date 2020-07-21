using FrameWork.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StageFactory
{
    public static void CreateSceneItem(SceneItemData data, Vector2Int pos)
    {
        if (data == null) return;

        BaseSceneItem sceneItem = null;
        SceneItemInfo.ItemType type = SceneItemInfo.ItemType.None;

        if (data.Type == SceneItemData.ItemType.Weapon)
        {
            type = SceneItemInfo.ItemType.Weapon;
            sceneItem = SceneObjectPool.Ins.Get<Weapon>(data.Name);
        }
        else if(data.Type == SceneItemData.ItemType.Trap)
        {
            type = SceneItemInfo.ItemType.Trap;
            sceneItem = SceneObjectPool.Ins.Get<Trap>(data.Name);
        }
        else
        {
            if (data.Type == SceneItemData.ItemType.EXP) type = SceneItemInfo.ItemType.EXP;
            else if(data.Type == SceneItemData.ItemType.HP) type = SceneItemInfo.ItemType.HP;
            else if(data.Type == SceneItemData.ItemType.Life) type = SceneItemInfo.ItemType.Life;
            else if(data.Type == SceneItemData.ItemType.Money) type = SceneItemInfo.ItemType.Money;
            sceneItem = SceneObjectPool.Ins.Get<Consume>(data.Name);
        }

        if (sceneItem == null) return;

        sceneItem.InitInfo(new SceneItemInfo()
        {
            ID = data.ID,
            Type = type,
            Health = data.Value,
            MaxHealth = data.Value,
            TriggerOffest = data.TriggerOffest,
            TriggerSize = data.TriggerSize,
            Value = data.Value,
        });
        sceneItem.SetRes(string.Format("{0}/{1}", ResDefine.PREFAB_PATH, data.AssetName));
        sceneItem.SetObjectType(ObjectType.Weapon);
        sceneItem.SetMapPos(pos);
    }

    public static BaseEnemy CreateEnemy(EnemyData enemyData,Vector2Int pos)
    {
        BaseEnemy enemy = SceneObjectPool.Ins.Get<BaseEnemy>(enemyData.Name);
        enemy.SetRes(string.Format("{0}/{1}", ResDefine.PREFAB_PATH, enemyData.AssetName));
        enemy.InitInfo(new BaseRoleInfo()
        {
            ID = enemyData.ID,
            Health = 5,
            MaxHealth = 5,
            AttackSpeed = enemyData.AttackSpeed,
            AttackValue = 1,
            Defense = 1,
            MoveSpeed = enemyData.MoveSpeed,
        });

        enemy.AddCtrl<BaseEnemyCtrl>().InitData(new BaseRoleSkillInfo()
        {
            AttackIDs = enemyData.AttackIDs,
            Skills = enemyData.Skills,
            AttackWait = enemyData.AttackWait,
            AttackNextTime = enemyData.AttackNextTime,
        });

        enemy.SetObjectType(ObjectType.Monster);
        enemy.SetMapPos(pos);

        return enemy;
    }
}
