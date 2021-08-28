using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utility;
using UnityEngine;

public static class SceneEntityFactory
{
    public static void CreateSceneItem(SceneItemConfigData data, Vector2Int pos)
    {
        if (data == null)
        {
            return;
        }

        BaseSceneItem sceneItem = null;
        SceneItemData.ItemType type = SceneItemData.ItemType.None;
        ObjectType objectType = ObjectType.NONE;

        if (data.Type == SceneItemConfigData.ItemType.Weapon)
        {
            data = StaticConfig.SceneItemConfig.GetData(PlayerMgr.Ins.CharacterData.WeaponId);
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

        if (sceneItem == null)
        {
            return;
        }

        SceneItemData sceneItemData = ReferencePool.Acquire<SceneItemData>();
        sceneItemData.Id = data.Id;
        sceneItemData.Type = type;
        sceneItemData.Health = data.Value;
        sceneItemData.MaxHealth = data.Value;
        sceneItemData.Value = data.Value;
        sceneItemData.CanDrop = data.CanDrop;

        sceneItem.SetData(sceneItemData);
        sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, data.AssetName));
        sceneItem.SetObjectType(objectType);
        sceneItem.SetMapPos(pos);
    }

    public static BaseEnemy CreateEnemy(CharacterConfigData enemyConfigData, int engityID, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos)
    {
        BaseEnemy enemy = EntityMgr.Ins.GetEntity<BaseEnemy>(enemyConfigData.Name);
        BaseEnemyData enemyData = ReferencePool.Acquire<BaseEnemyData>();
        BaseEnemySkillData enemySkillData = ReferencePool.Acquire<BaseEnemySkillData>();

        enemyData.Id = engityID;
        enemyData.Health = hp;
        enemyData.MaxHealth = hp;
        enemyData.HpBarWdith = hpBarWidth;
        enemyData.AttackSpeed = enemyConfigData.AttackSpeed;
        enemyData.AttackValue = attack;
        enemyData.DefenseValue = defense;
        enemyData.MoveSpeed = enemyConfigData.MoveSpeed;
        enemyData.HurtAnim = enemyConfigData.HurtAnim;
  
        enemySkillData.AttackIds = enemyConfigData.AttackIDs;
        enemySkillData.SkillIds = enemyConfigData.Skills;
        enemySkillData.AttackWait = enemyConfigData.AttackWait;
        enemySkillData.AttackNextTime = enemyConfigData.AttackNextTime;
        enemySkillData.BehaviourTreeIds = enemyConfigData.BehaviourTreeIds;

        enemy.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, enemyConfigData.AssetName));
        enemy.SetData(enemyData);
        enemy.AddCtrl<BaseEnemyCtrl>().SetData(enemySkillData);
        enemy.SetObjectType(ObjectType.Monster);
        enemy.SetMapPos(pos);

        return enemy;
    }

    public static BaseSceneObject CreateSceneBuilding(StageConfigData.SceneBuilding sceneObjData)
    {
        BaseSceneObject sceneObject = null;
        if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Trap)
        {
            sceneObject = EntityMgr.Ins.GetEntity<Trap>(sceneObjData.Name);
            TrapData trapData = ReferencePool.Acquire<TrapData>();
            trapData.TriggerSize = sceneObjData.TriggerSize;
            trapData.TriggerOffest = sceneObjData.TriggerOffest;
            sceneObject.SetData(trapData);
        }
        else if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Normal)
        {
            sceneObject = EntityMgr.Ins.GetEntity<BaseSceneObject>(sceneObjData.Name);
            sceneObject.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, sceneObjData.AssetName));
        }

        sceneObject.SetMapPos(sceneObjData.Pos);
        return sceneObject;
    }

}
