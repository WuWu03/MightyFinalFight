using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;
using UnityEngine;

public static class SceneEntityFactory
{
    public static BaseRole CreateRole(string name, string asset, float moveSpeed, Vector2 pos)
    {
        BaseRole role = EntityMgr.instance.GetEntity<BaseRole>(name);
        EntityAttribute attribute = ReferencePool.Acquire<EntityAttribute>();

        attribute.moveSpeed = moveSpeed;

        role.SetAttribute(attribute);
        role.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, asset));
        role.SetLayer(LayerName.Unit);
        role.SetPos2(pos);

        return role;
    }

    public static BaseSceneItem CreateSceneItem(SceneItemConfigData sceneItemConfigData, Vector2Int pos)
    {
        if (sceneItemConfigData == null)
        {
            return null;
        }

        BaseSceneItem sceneItem = null;
        ObjectType objectType = ObjectType.NONE;

        if (sceneItemConfigData.type == 1)
        {
            sceneItemConfigData = ConfigDataHelper.sceneItemConfigDatas.GetConfigDataById(PlayerMgr.instance.roleConfigData.weaponId);
            objectType = ObjectType.Weapon;
            sceneItem = EntityMgr.instance.GetEntity<Weapon>(sceneItemConfigData.name);
        }
        else if(sceneItemConfigData.type == 6)
        {
            objectType = ObjectType.CantBreakItem;
            sceneItem = EntityMgr.instance.GetEntity<Trap>(sceneItemConfigData.name);
        }
        else
        {
            objectType = ObjectType.Consume;
            sceneItem = EntityMgr.instance.GetEntity<Consume>(sceneItemConfigData.name);
        }

        if (sceneItem == null)
        {
            return null;
        }

        SceneItemData sceneItemData = ReferencePool.Acquire<SceneItemData>();

        sceneItemData.id = sceneItemConfigData.id;
        sceneItemData.itemType = sceneItemConfigData.type;
        sceneItemData.value = sceneItemConfigData.value;
        sceneItemData.canDrop = sceneItemConfigData.canDrop;

        sceneItem.SetData(sceneItemData);
        sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, sceneItemConfigData.assetName));
        sceneItem.SetObjectType(objectType);
        sceneItem.SetMapPos(pos);
        sceneItem.SetLayer(LayerName.Unit);

        return sceneItem;
    }

    public static BaseEnemy CreateEnemy(RoleConfigData enemyConfigData, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos)
    {
        BaseEnemy enemy = EntityMgr.instance.GetEntity<BaseEnemy>(enemyConfigData.name);
        BaseEnemyData enemyData = ReferencePool.Acquire<BaseEnemyData>();
        BaseEnemySkillData enemySkillData = ReferencePool.Acquire<BaseEnemySkillData>();
        EntityAttribute enemyAttribute = ReferencePool.Acquire<EntityAttribute>();

        enemyData.entityId = entityId;
        enemyData.hurtAnims = enemyConfigData.hurtAnims;
        enemyData.isBoss = enemyConfigData.isBoss;
        enemyData.hpBarWdith = hpBarWidth;

        enemySkillData.attackIds = enemyConfigData.attactIds;
        enemySkillData.skillIds = enemyConfigData.skillIds;
        enemySkillData.attackWait = enemyConfigData.attackWait;
        enemySkillData.jumpAttackIds = enemyConfigData.jumpAttackIds;
        enemySkillData.attackNextTime = enemyConfigData.attackNextTime;
        enemySkillData.behaviourTreeIds = enemyConfigData.behaviourTreeIds;

        enemyAttribute.health = hp;
        enemyAttribute.maxHealth = hp;
        enemyAttribute.jumpForce = enemyConfigData.jumpForce;
        enemyAttribute.attackSpeed = enemyConfigData.attackSpeed;
        enemyAttribute.attackValue = attack;
        enemyAttribute.defenseValue = defense;
        enemyAttribute.moveSpeed = enemyConfigData.moveSpeed;

        enemy.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, enemyConfigData.assetName));
        enemy.SetData(enemyData);
        enemy.SetAttribute(enemyAttribute);
        enemy.AddCtrl<BaseEnemyCtrl>().SetData(enemySkillData);
        enemy.SetObjectType(ObjectType.Monster);
        enemy.SetMapPos(pos);
        enemy.SetLayer(LayerName.Unit);

        return enemy;
    }

    public static BaseSceneObject CreateSceneBuilding(StageConfigData.SceneBuilding sceneObjData)
    {
        if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Trap)
        {
            TrapData trapData = ReferencePool.Acquire<TrapData>();
            trapData.triggerSize = sceneObjData.TriggerSize;
            trapData.triggerOffest = sceneObjData.TriggerOffest;

            Trap trap = EntityMgr.instance.GetEntity<Trap>(sceneObjData.Name);
            trap.SetData(trapData);
            trap.SetMapPos(sceneObjData.Pos);
            trap.SetLayer(LayerName.Map);
            return trap;
        }
        else if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Building ||
                sceneObjData.SceneObjType == StageConfigData.SceneObjType.Unit)
        {
            SceneBuilding sceneBuilding = EntityMgr.instance.GetEntity<SceneBuilding>(sceneObjData.Name);
            sceneBuilding.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, sceneObjData.AssetName));
            sceneBuilding.SetMapPos(sceneObjData.Pos);
            sceneBuilding.SetLayer(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Unit ? LayerName.Unit : LayerName.Map);
            return sceneBuilding;
        }

        return null;
    }

}
