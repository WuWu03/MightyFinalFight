using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using UnityEngine;

public static class SceneEntityFactory
{
    public static BaseRole CreateRole(string name, string asset, float moveSpeed, Vector2 pos)
    {
        BaseRole role = EntityMgr.instance.GetEntity<BaseRole>(name);
        EntityAttribute attribute = ReferencePool.Acquire<EntityAttribute>();

        attribute.moveSpeed = moveSpeed;

        role.SetAttribute(attribute);
        role.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, asset));
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

        ObjectType objectType;
        BaseSceneItem sceneItem;

        if (sceneItemConfigData.type == 1)
        {
            EntityAttribute weaponAttribute = ReferencePool.Acquire<EntityAttribute>();
            weaponAttribute.health = sceneItemConfigData.value;
            weaponAttribute.maxHealth = sceneItemConfigData.value;

            sceneItemConfigData = ConfigDataSheet.sceneItemConfigDatas.GetConfigDataById(PlayerMgr.instance.roleConfigData.weaponId);
            objectType = ObjectType.Weapon;
            sceneItem = EntityMgr.instance.GetEntity<Weapon>(sceneItemConfigData.name);

            sceneItem.SetAttribute(weaponAttribute);
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

        SceneItemData sceneItemData = SceneItemData.Create();
        sceneItemData.id = sceneItemConfigData.id;
        sceneItemData.itemType = sceneItemConfigData.type;
        sceneItemData.value = sceneItemConfigData.value;
        sceneItemData.canDrop = sceneItemConfigData.canDrop;

        sceneItem.SetData(sceneItemData);
        sceneItem.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, sceneItemConfigData.assetName));
        sceneItem.SetObjectType(objectType);
        sceneItem.SetMapPos(pos);
        sceneItem.SetLayer(LayerName.Unit);

        return sceneItem;
    }

    public static BaseEnemy CreateEnemy(RoleConfigData enemyConfigData, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos)
    {
        BaseEnemy enemy = EntityMgr.instance.GetEntity<BaseEnemy>(enemyConfigData.name);
        enemy.SetObjectType(ObjectType.Enemy);
        enemy.SetMapPos(pos);
        enemy.SetLayer(LayerName.Unit);
        enemy.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, enemyConfigData.assetName));

        BaseEnemyData enemyData = BaseEnemyData.Create();
        enemyData.entityId = entityId;
        enemyData.hurtAnims = enemyConfigData.hurtAnims;
        enemyData.isBoss = enemyConfigData.isBoss;
        enemyData.hpBarWdith = hpBarWidth;
        enemy.SetData(enemyData);

        EntityAttribute enemyAttribute = ReferencePool.Acquire<EntityAttribute>();
        enemyAttribute.health = hp;
        enemyAttribute.maxHealth = hp;
        enemyAttribute.jumpForce = enemyConfigData.jumpForce;
        enemyAttribute.attackSpeed = enemyConfigData.attackSpeed;
        enemyAttribute.attackValue = attack;
        enemyAttribute.defenseValue = defense;
        enemyAttribute.moveSpeed = enemyConfigData.moveSpeed;
        enemy.SetAttribute(enemyAttribute);

        BaseEnemySkillData enemySkillData = ReferencePool.Acquire<BaseEnemySkillData>();
        enemySkillData.attackIds = enemyConfigData.attactIds;
        enemySkillData.skillIds = enemyConfigData.skillIds;
        enemySkillData.attackWait = new float[1] { -1f };
        enemySkillData.jumpAttackIds = enemyConfigData.jumpAttackIds;
        enemySkillData.behaviourTreeIds = enemyConfigData.behaviourTreeIds;
        enemy.SetSkillData(enemySkillData);

        return enemy;
    }

    public static BaseSceneObject CreateSceneBuilding(StageConfigData.SceneBuilding sceneObjData)
    {
        if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Trap)
        {
            TrapData trapData = TrapData.Create();
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
            sceneBuilding.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, sceneObjData.AssetName));
            sceneBuilding.SetMapPos(sceneObjData.Pos);
            sceneBuilding.SetLayer(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Unit ? LayerName.Unit : LayerName.Map);
            return sceneBuilding;
        }

        return null;
    }



    public static Barrel CreateBarrel(int entityId, float dir, int groundY, int itemId, bool isFloat, float moveSpeed, Vector2Int pos)
    {
        Barrel barrel = EntityMgr.instance.GetEntity<Barrel>("Barrel");
        BarrelData barrelData = BarrelData.Create();
        EntityAttribute barrelAttribute = EntityAttribute.Create();

        barrelData.entityId = entityId;
        barrelData.value = 0;
        barrelData.canDrop = false;
        barrelData.dir = dir;
        barrelData.groundY = groundY;
        barrelData.isFloat = isFloat;
        barrelData.moveSpeed = moveSpeed;
        barrelData.itemId = itemId == -1 ? ConfigDataSheet.sceneItemConfigDatas[Random.Range(0, ConfigDataSheet.sceneItemConfigDatas.Length)].id : itemId;
        barrelAttribute.health = 1;
        barrelAttribute.maxHealth = 1;

        barrel.SetData(barrelData);
        barrel.SetMapPos(pos);
        barrel.SetAttribute(barrelAttribute);
        barrel.SetObjectType(ObjectType.Barrel);
        barrel.SetLayer(LayerName.Unit);
        barrel.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, "SceneBuilding/Barrel.prefab"));

        return barrel;
    }
}
