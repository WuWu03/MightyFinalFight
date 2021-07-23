using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utility;
using UnityEngine;

public static class SceneEntityFactory
{
    public static void CreateSceneItem(SceneItemConfigData data, Vector2Int pos)
    {
        if (data == null) return;

        BaseSceneItem sceneItem = null;
        SceneItemData.ItemType type = SceneItemData.ItemType.None;
        ObjectType objectType = ObjectType.NONE;

        if (data.Type == SceneItemConfigData.ItemType.Weapon)
        {
            type = SceneItemData.ItemType.Weapon;
            objectType = ObjectType.Weapon;
            sceneItem = EntityMgr.Ins.GetEntity<Weapon>(data.Name);
        }
        else if(data.Type == SceneItemConfigData.ItemType.Trap)
        {
            type = SceneItemData.ItemType.Trap;
            objectType = ObjectType.CantBreakItem;
            sceneItem = EntityMgr.Ins.GetEntity<Trap>(data.Name);
        }
        else
        {
            if (data.Type == SceneItemConfigData.ItemType.EXP) type = SceneItemData.ItemType.EXP; 
            else if(data.Type == SceneItemConfigData.ItemType.HP) type = SceneItemData.ItemType.HP;
            else if(data.Type == SceneItemConfigData.ItemType.Life) type = SceneItemData.ItemType.Life;
            else if(data.Type == SceneItemConfigData.ItemType.Money) type = SceneItemData.ItemType.Money;
            objectType = ObjectType.Consume;
            sceneItem = EntityMgr.Ins.GetEntity<Consume>(data.Name);
        }

        if (sceneItem == null) return;

        SceneItemData sceneItemData = ReferencePool.Acquire<SceneItemData>();
        sceneItemData.Id = data.ID;
        sceneItemData.Type = type;
        sceneItemData.Health = data.Value;
        sceneItemData.MaxHealth = data.Value;
        sceneItemData.TriggerOffest = data.TriggerOffest;
        sceneItemData.TriggerSize = data.TriggerSize;
        sceneItemData.Value = data.Value;
        sceneItemData.CanDrop = data.CanDrop;

        sceneItem.SetData(sceneItemData);
        sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, data.AssetName));
        sceneItem.SetObjectType(objectType);
        sceneItem.SetMapPos(pos);
    }

    public static BaseEnemy CreateEnemy(EnemyConfigData enemyConfigData, int engityID, int hp, int attack, int defense, Vector2Int pos)
    {
        BaseEnemy enemy = GetEnemyEntity(enemyConfigData);
        BaseEnemyData enemyData = ReferencePool.Acquire<BaseEnemyData>();
        BaseEnemySkillData enemySkillData = ReferencePool.Acquire<BaseEnemySkillData>();

        enemyData.Id = engityID;
        enemyData.Health = hp;
        enemyData.MaxHealth = hp;
        enemyData.AttackSpeed = enemyConfigData.AttackSpeed;
        enemyData.AttackValue = attack;
        enemyData.DefenseValue = defense;
        enemyData.MoveSpeed = enemyConfigData.MoveSpeed;
        enemyData.HurtAnim = enemyConfigData.HurtEnemy;

        enemySkillData.AttackIds = enemyConfigData.AttackIDs;
        enemySkillData.SkillIds = enemyConfigData.Skills;
        enemySkillData.AttackWait = enemyConfigData.AttackWait;
        enemySkillData.AttackNextTime = enemyConfigData.AttackNextTime;
        enemySkillData.BehaviourRate = enemyConfigData.BehaviourRate;
        enemySkillData.BehaviourTreesID = enemyConfigData.BehaviourTreeIDs;

        enemy.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, enemyConfigData.AssetName));
        enemy.SetData(enemyData);
        enemy.AddCtrl<BaseEnemyCtrl>().SetData(enemySkillData);
        enemy.SetObjectType(ObjectType.Monster);
        enemy.SetMapPos(pos);

        return enemy;
    }

    private static BaseEnemy GetEnemyEntity(EnemyConfigData enemyData)
    {
        return EntityMgr.Ins.GetEntity<BaseEnemy>(enemyData.Name);
    }
}
