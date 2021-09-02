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
        sceneItem.SetLayer(LayerName.Unit);
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
        enemyData.JumpForce = enemyConfigData.JumpForce;
        enemyData.AttackSpeed = enemyConfigData.AttackSpeed;
        enemyData.AttackValue = attack;
        enemyData.DefenseValue = defense;
        enemyData.MoveSpeed = enemyConfigData.MoveSpeed;
        enemyData.HurtAnim = enemyConfigData.HurtAnim;
        enemyData.IsBoss = enemyConfigData.IsBoss;

        enemySkillData.AttackIds = enemyConfigData.AttackIDs;
        enemySkillData.SkillIds = enemyConfigData.Skills;
        enemySkillData.AttackWait = enemyConfigData.AttackWait;
        enemySkillData.JumpAttackIds = enemyConfigData.JumpAttackIDs;
        enemySkillData.AttackNextTime = enemyConfigData.AttackNextTime;
        enemySkillData.BehaviourTreeIds = enemyConfigData.BehaviourTreeIds;

        enemy.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, enemyConfigData.AssetName));
        enemy.SetData(enemyData);
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
            trapData.TriggerSize = sceneObjData.TriggerSize;
            trapData.TriggerOffest = sceneObjData.TriggerOffest;

            Trap trap = EntityMgr.Ins.GetEntity<Trap>(sceneObjData.Name);
            trap.SetData(trapData);
            trap.SetMapPos(sceneObjData.Pos);
            trap.SetLayer(LayerName.Map);
            return trap;
        }
        else if(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Building ||
                sceneObjData.SceneObjType == StageConfigData.SceneObjType.Unit)
        {
            SceneBuilding sceneBuilding = EntityMgr.Ins.GetEntity<SceneBuilding>(sceneObjData.Name);
            sceneBuilding.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, sceneObjData.AssetName));
            sceneBuilding.SetMapPos(sceneObjData.Pos);
            sceneBuilding.SetLayer(sceneObjData.SceneObjType == StageConfigData.SceneObjType.Unit ? LayerName.Unit : LayerName.Map);
            return sceneBuilding;
        }

        return null;
    }

}
