using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private void Awake()
    {
        m_ListDeadEnemies = new List<int>();
        m_ListBreakBarrels = new List<int>();
        m_ListEnemies = new List<BaseEnemy>();
        m_ListSceneBuildings = new List<BaseSceneObject>();
        m_ListSceneItems = new List<BaseSceneItem>();
        m_ListBarrels = new List<Barrel>();
    }

    public void CreateBarrels()
    {
        for (int i = 0; i < 5; i++)
        {
            Barrel sceneItem = EntityMgr.instance.GetEntity<Barrel>("Barrel");
            BarrelData barrelData = BarrelData.Create();
            EntityAttribute barrelAttribute = EntityAttribute.Create();

            barrelData.id = 1;
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
            sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, "SceneBuilding/Barrel"));
            sceneItem.SetObjectType(ObjectType.BreakItem);
            sceneItem.SetMapPos(new Vector2Int(-400 + i * 50, -66));
            sceneItem.SetLayer(LayerName.Unit);
            m_ListBarrels.Add(sceneItem);
        }
    }


    public BaseEnemy CreateEnemy(int sourceId, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos, bool startBehaviourTree = true)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(ConfigDataHelper.roleConfigDatas.GetConfigDataById(sourceId), entityId, hp, attack, defense, hpBarWidth, pos);

        if (enemy == null)
        {
            Log.LogError("创建 Enemy 失败 , sourceId : " + sourceId + " , entityId : " + entityId);
            return null;
        }

        enemy.onReleaseEvent += OnEnemyRelease;
        m_ListEnemies.Add(enemy);
        return enemy;
    }

    public BaseSceneObject CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemConfigData sceneItemConfigData = ConfigDataHelper.sceneItemConfigDatas.GetConfigDataById(id);
        BaseSceneItem sceneItem = SceneEntityFactory.CreateSceneItem(sceneItemConfigData, pos);

        if (sceneItem == null)
        {
            Log.LogError("创建 SceneItem 失败 sceneItemId : " + id);
            return null;
        }

        m_ListSceneItems.Add(sceneItem);
        return sceneItem;
    }

    public void CreateSceneBuildings(StageConfigData data)
    {
        for (int i = 0; i < data.SceneBuildings.Length; i++)
        {
            BaseSceneObject sceneBuilding = SceneEntityFactory.CreateSceneBuilding(data.SceneBuildings[i]);

            if (sceneBuilding == null)
            {
                Log.LogError("创建 SceneItem 失败 , stageId : " + data.Id + " , buildingId : " + data.SceneBuildings[i].Id);
                continue;
            }

            m_ListSceneBuildings.Add(sceneBuilding);
        }
    }

    public Barrel CreateBarrel(int entityId, float dir, int groundY, int itemId, bool isFloat, float moveSpeed, Vector2Int pos)
    {
        Barrel barrel = SceneEntityFactory.CreateBarrel(entityId, dir, groundY, itemId, isFloat, moveSpeed, pos);

        if (barrel == null)
        {
            Log.LogError("创建 Barrel 失败 , entityId : " + entityId);
            return null;
        }

        barrel.onReleaseEvent += OnBarrelRelease;
        m_ListBarrels.Add(barrel);
        return barrel;
    }

    public List<BaseEnemy> GetEnemies()
    {
        return m_ListEnemies;
    }

    public List<BaseSceneItem> GetSceneItems()
    {
        return m_ListSceneItems;
    }

    public List<BaseSceneObject> GetSceneBuildings()
    {
        return m_ListSceneBuildings;
    }

    public List<Barrel> GetBarrels()
    {
        return m_ListBarrels;
    }

    public BaseSceneObject GetSceneBuildingByName(string name)
    {
        for (int i = 0; i < m_ListSceneBuildings.Count; i++)
        {
            if (m_ListSceneBuildings[i].entityName == name)
            {
                return m_ListSceneBuildings[i];
            }
        }

        return null;
    }

    public void ReleaseEnemies()
    {
        m_ListDeadEnemies.Clear();

        for (int i = 0; i < m_ListEnemies.Count; i++)
        {
            m_ListEnemies[i].onReleaseEvent -= OnEnemyRelease;
            m_ListEnemies[i].Release();
        }

        m_ListEnemies.Clear();
    }

    public void ReleaseSceneItem(BaseSceneItem item)
    {
        m_ListSceneItems.Remove(item);
    }

    public void ReleaseSceneItems()
    {
        for (int i = 0; i < m_ListSceneItems.Count; i++)
        {
            m_ListSceneItems[i].Release();
        }

        m_ListSceneItems.Clear();
    }

    public void ReleaseSceneBuildings()
    {
        for (int i = 0; i < m_ListSceneBuildings.Count; i++)
        {
            m_ListSceneBuildings[i].Release();
        }

        m_ListSceneBuildings.Clear();
    }

    public void RleaseBarrels()
    {
        m_ListBarrels.Clear();

        for (int i = 0; i < m_ListBarrels.Count; i++)
        {
            m_ListBarrels[i].onReleaseEvent -= OnBarrelRelease;
            m_ListBarrels[i].Release();
        }

        m_ListBarrels.Clear();
    }

    public void ReleaseAll()
    {
        ReleaseEnemies();
        ReleaseSceneItems();
        ReleaseSceneBuildings();
        RleaseBarrels();
    }

    public bool IsEnemyDead(int entityId)
    {
        for (int i = 0; i < m_ListDeadEnemies.Count; i++)
        {
            if (m_ListDeadEnemies[i] == entityId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsBarrelBreak(int entityId)
    {
        for (int i = 0; i < m_ListBarrels.Count; i++)
        {
            if (m_ListBreakBarrels[i] == entityId)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAllEnemyDead()
    {
        return m_ListEnemies.Count <= 0;
    }

    public bool IsAllBarrelsBreak()
    {
        return m_ListBarrels.Count <= 0;
    }

    public int GetDeadEnemyCount()
    {
        return m_ListDeadEnemies.Count;
    }

    public int GetBreakBarrelsCount()
    {
        return m_ListBreakBarrels.Count;
    }

    private void OnEnemyRelease(int entityId)
    {
        m_ListDeadEnemies.Add(entityId);

        for (int i = m_ListEnemies.Count - 1; i >= 0; i--)
        {
            if (m_ListEnemies[i].entityId == entityId)
            {
                m_ListEnemies[i].onReleaseEvent -= OnEnemyRelease;
                m_ListEnemies.RemoveAt(i);
                break;
            }
        }
    }

    private void OnBarrelRelease(int entityId)
    {
        m_ListBreakBarrels.Add(entityId);

        for (int i = m_ListBarrels.Count - 1; i >= 0; i--)
        {
            if (m_ListBarrels[i].entityId == entityId)
            {
                m_ListBarrels[i].onReleaseEvent -= OnEnemyRelease;
                m_ListBarrels.RemoveAt(i);
                break;
            }
        }
    }

    protected override void OnShutDown()
    {
        ReleaseAll();
        base.OnShutDown();
    }

    private List<BaseSceneObject> m_ListSceneBuildings = null;
    private List<BaseEnemy> m_ListEnemies;
    private List<BaseSceneItem> m_ListSceneItems = null;
    private List<Barrel> m_ListBarrels = null;
    private List<int> m_ListDeadEnemies = null;
    private List<int> m_ListBreakBarrels = null;
}