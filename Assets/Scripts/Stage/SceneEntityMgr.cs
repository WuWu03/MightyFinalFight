using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private List<BaseSceneObject> m_SceneBuildings;
    private List<BaseEnemy> m_Enemies;
    private List<BaseSceneItem> m_SceneItems;
    private List<Barrel> m_Barrels;
    private List<int> m_DeadEnemies;
    private List<int> m_BreakBarrels;
    
    protected override void OnAwake()
    {
        m_DeadEnemies = new();
        m_BreakBarrels = new();
        m_Enemies = new();
        m_SceneBuildings = new();
        m_SceneItems = new();
        m_Barrels = new();
    }
    
    protected override void OnShutDown()
    {
        base.OnShutDown();
        ReleaseAll();
    }

    protected override void OnDestory()
    {
        base.OnDestory();
        m_SceneBuildings = null;
        m_Enemies = null;
        m_SceneItems = null;
        m_Barrels = null;
        m_DeadEnemies = null;
        m_BreakBarrels = null;
    }

    public void CreateBarrels()
    {
        for (int i = 0; i < 5; i++)
        {
            Barrel sceneItem = GameEntry.entityMgr.GetEntity<Barrel>("Barrel");
            BarrelData barrelData = BarrelData.Create();
            EntityAttribute barrelAttribute = EntityAttribute.Create();
            barrelData.entityId = 1;
            barrelData.value = 0;
            barrelData.canDrop = false;
            barrelData.dir = 1;
            barrelData.groundY = 0;
            barrelData.isFloat = false;
            barrelData.moveSpeed = 0f;
            barrelData.itemId = 1001 + i;
            barrelAttribute.health = 1;
            barrelAttribute.maxHealth = 1;
            sceneItem.SetAttribute(barrelAttribute);
            sceneItem.SetData(barrelData);
            sceneItem.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, "SceneBuilding/Barrel.prefab"));
            sceneItem.SetObjectType(ObjectType.BreakItem);
            sceneItem.SetMapPos(new Vector2Int(-400 + i * 50, -66));
            sceneItem.SetLayer(LayerName.Unit);
            m_Barrels.Add(sceneItem);
        }
    }

    public BaseEnemy CreateEnemy(int sourceId, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos, bool startBehaviourTree = true)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(ConfigDataSheet.roleConfigDatas.GetConfigDataById(sourceId), entityId, hp, attack, defense, hpBarWidth, pos);
        Log.LogInfo("创建 Enemy , sourceId : " + sourceId + " , entityId : " + entityId);

        if (enemy is null)
        {
            Log.LogError("创建 Enemy 失败 , sourceId : " + sourceId + " , entityId : " + entityId);
            return null;
        }   

        enemy.onReleaseEvent += OnEnemyRelease;
        m_Enemies.Add(enemy);
        return enemy;
    }

    public BaseSceneObject CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemConfigData sceneItemConfigData = ConfigDataSheet.sceneItemConfigDatas.GetConfigDataById(id);
        BaseSceneItem sceneItem = SceneEntityFactory.CreateSceneItem(sceneItemConfigData, pos);

        if (sceneItem is null)
        {
            Log.LogError("创建 SceneItem 失败 sceneItemId : " + id);
            return null;
        }

        m_SceneItems.Add(sceneItem);
        return sceneItem;
    }

    public void CreateSceneBuildings(StageConfigData stageConfigData)
    {
        foreach (var sceneBuilding in stageConfigData.SceneBuildings)
        {
            BaseSceneObject baseSceneObject = SceneEntityFactory.CreateSceneBuilding(sceneBuilding);

            if (baseSceneObject is null)
            {
                Log.LogError("创建 SceneItem 失败 , stageId : " + stageConfigData.id + " , buildingId : " + sceneBuilding.Id);
                continue;
            }

            m_SceneBuildings.Add(baseSceneObject);
        }
    }

    public Barrel CreateBarrel(int entityId, float dir, int groundY, int itemId, bool isFloat, float moveSpeed, Vector2Int pos)
    {
        Barrel barrel = SceneEntityFactory.CreateBarrel(entityId, dir, groundY, itemId, isFloat, moveSpeed, pos);

        if (barrel is null)
        {
            Log.LogError("创建 Barrel 失败 , entityId : " + entityId);
            return null;
        }

        barrel.onReleaseEvent += OnBarrelRelease;
        m_Barrels.Add(barrel);
        return barrel;
    }

    public List<BaseEnemy> GetEnemies()
    {
        return m_Enemies;
    }

    public BaseEnemy GetEnemyById(int entityId)
    {
        return m_Enemies.Find(x => x.entityId == entityId);
    }

    public List<BaseSceneItem> GetSceneItems()
    {
        return m_SceneItems;
    }

    public List<BaseSceneObject> GetSceneBuildings()
    {
        return m_SceneBuildings;
    }

    public List<Barrel> GetBarrels()
    {
        return m_Barrels;
    }

    public BaseSceneObject GetSceneBuildingByName(string name)
    {
        foreach (var sceneBuilding in m_SceneBuildings)
        {
            if (sceneBuilding.name == name)
            {
                return sceneBuilding;
            }
        }

        return null;
    }

    public void ReleaseEnemies()
    {
        m_DeadEnemies.Clear();

        foreach (var enemy in m_Enemies)
        {
            enemy.onReleaseEvent -= OnEnemyRelease;
            enemy.Release();
        }

        m_Enemies.Clear();
    }

    public void ReleaseSceneItem(BaseSceneItem item)
    {
        if (item is null)
        {
            return;
        }

        m_SceneItems.Remove(item);
    }

    public void ReleaseSceneItems()
    {
        for (int i = m_SceneItems.Count - 1; i > -1; i--)
        {
            if (m_SceneItems[i] is null)
            {
                m_SceneItems.RemoveAt(i);
                continue;
            }

            if (!m_SceneItems[i].canReleaseInSceneChange)
            {
                continue;
            }

            m_SceneItems[i].Release();
        }
    }

    public void ReleaseSceneBuildings()
    {
        foreach (var sceneBuilding in m_SceneBuildings)
        {
            sceneBuilding.Release();
        }

        m_SceneBuildings.Clear();
    }

    public void ReleaseBarrels()
    {
        m_Barrels.Clear();

        foreach (var barrel in m_Barrels)
        {
            barrel.onReleaseEvent -= OnBarrelRelease;
            barrel.Release();
        }

        m_Barrels.Clear();
    }

    public void ReleaseAll()
    {
        ReleaseEnemies();
        ReleaseSceneItems();
        ReleaseSceneBuildings();
        ReleaseBarrels();
    }

    public bool IsEnemyDead(int entityId)
    {
        foreach (var enemy in m_DeadEnemies)
        {
            if (enemy == entityId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsBarrelBreak(int entityId)
    {
        foreach (var breakBarrel in m_BreakBarrels)
        {
            if (breakBarrel == entityId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAllEnemyDead()
    {
        return m_Enemies.Count <= 0;
    }

    public bool IsAllBarrelsBreak()
    {
        return m_Barrels.Count <= 0;
    }

    public int GetDeadEnemyCount()
    {
        return m_DeadEnemies.Count;
    }

    public int GetBreakBarrelsCount()
    {
        return m_BreakBarrels.Count;
    }

    private void OnEnemyRelease(int entityId)
    {
        m_DeadEnemies.Add(entityId);

        for (int i = m_Enemies.Count - 1; i >= 0; i--)
        {
            if (m_Enemies[i].entityId == entityId)
            {
                m_Enemies[i].onReleaseEvent -= OnEnemyRelease;
                m_Enemies.RemoveAt(i);
                break;
            }
        }
    }

    private void OnBarrelRelease(int entityId)
    {
        m_BreakBarrels.Add(entityId);

        for (int i = m_Barrels.Count - 1; i >= 0; i--)
        {
            if (m_Barrels[i].entityId == entityId)
            {
                m_Barrels[i].onReleaseEvent -= OnEnemyRelease;
                m_Barrels.RemoveAt(i);
                break;
            }
        }
    }
}