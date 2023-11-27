using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Debug;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private void Awake()
    {
        m_DeadEnemies = new List<int>();
        m_Enemies = new List<BaseEnemy>();
        m_SceneBuildings = new List<BaseSceneObject>();
        m_SceneItems = new List<BaseSceneItem>();
    }

    public BaseEnemy CreateEnemy(int sourceId, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos, bool startBehaviourTree = true)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(ConfigDataHelper.roleConfigDatas.GetConfigDataById(sourceId), entityId, hp, attack, defense, hpBarWidth, pos);

        if(enemy == null)
        {
            GameFrameworkLog.DebugError("create enemy failed sourceId:" + sourceId + ",entityId:" + entityId);
            return null;
        }

        enemy.onReleaseEvent += OnEnemyRelease;
        m_Enemies.Add(enemy);
        return enemy;
    }

    public BaseSceneItem CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemConfigData sceneItemConfigData = ConfigDataHelper.sceneItemConfigDatas.GetConfigDataById(id);
        BaseSceneItem sceneItem = SceneEntityFactory.CreateSceneItem(sceneItemConfigData, pos);

        if (sceneItem == null)
        {
            GameFrameworkLog.DebugError("create sceneItem failed id:" + id);
            return null;
        }
        sceneItem.onReleaseEvent += OnSceneItemRelease;
        m_SceneItems.Add(sceneItem);
        return sceneItem;
    }

    public void CreateSceneBuildings(StageConfigData data)
    {
        for (int i = 0; i < data.SceneBuildings.Length; i++)
        {
            BaseSceneObject sceneBuilding = SceneEntityFactory.CreateSceneBuilding(data.SceneBuildings[i]);

            if(sceneBuilding == null)
            {
                GameFrameworkLog.DebugError("create sceneItem stageId:" + data.Id + ",buildId:" + data.SceneBuildings[i].Id);
                continue;
            }

            sceneBuilding.onReleaseEvent += OnSceneBuildingsRelease;
            m_SceneBuildings.Add(sceneBuilding);
        }
    }


    public void CreateBarrels()
    {
        for (int i = 0; i < 5; i++)
        {
            BaseSceneItem sceneItem = EntityMgr.instance.GetEntity<Barrel>("Barrel");
            BarrelData barrelData = ReferencePool.Acquire<BarrelData>();
            EntityAttribute barrelAttribute = ReferencePool.Acquire<EntityAttribute>();

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
            sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, "Item/Barrel"));
            sceneItem.SetObjectType(ObjectType.BreakItem);
            sceneItem.SetMapPos(new Vector2Int(-400 + i * 50, -66));
            sceneItem.onReleaseEvent += OnSceneItemRelease;
            m_SceneItems.Add(sceneItem);
        }
    }

    public List<BaseEnemy> GetEnemies()
    {
        return m_Enemies;
    }

    public List<BaseSceneItem> GetSceneItems()
    {
        return m_SceneItems;
    }

    public List<BaseSceneObject> GetSceneBuildings()
    {
        return m_SceneBuildings;
    }

    public void ReleaseAll()
    {
        ReleaseEnemies();
        ReleaseSceneItems();
        ReleaseSceneBuildings();
    }

    public void ReleaseEnemies()
    {
        m_DeadEnemies.Clear();

        for (int i = 0; i < m_Enemies.Count; i++)
        {
            m_Enemies[i].onReleaseEvent -= OnEnemyRelease;
            m_Enemies[i].Release();
        }

        m_Enemies.Clear();
    }

    public void ReleaseSceneItems()
    {
        for (int i = 0; i < m_SceneItems.Count; i++)
        {
            m_SceneItems[i].Release();
        }

        m_SceneItems.Clear();
    }

    public void ReleaseSceneBuildings()
    {
        for (int i = 0; i < m_SceneBuildings.Count; i++)
        {
            m_SceneBuildings[i].Release();
        }

        m_SceneBuildings.Clear();
    }


    public bool IsEnemyDead(int id)
    {
        for (int i = 0; i < m_DeadEnemies.Count; i++)
        {
            if (m_DeadEnemies[i] == id)
                return true;
        }

        return false;
    }

    public bool IsAllEnemyDead()
    {
        return m_Enemies.Count <= 0;
    }

    public BaseSceneObject GetSceneBuildingByName(string name)
    {
        for (int i = 0; i < m_SceneBuildings.Count; i++)
        {
            if (m_SceneBuildings[i].entityName == name)
            {
                return m_SceneBuildings[i];
            }
        }

        return null;
    }

    private void OnEnemyRelease(int id)
    {
        m_DeadEnemies.Add(id);

        for (int i = m_Enemies.Count - 1; i >= 0; i--)
        {
            if (m_Enemies[i].entityId == id)
            {
                m_Enemies[i].onReleaseEvent -= OnEnemyRelease;
                m_Enemies.RemoveAt(i);
                break;
            }
        }
    }

    private void OnSceneItemRelease(int id)
    {
        for (int i = m_SceneItems.Count - 1; i >= 0; i--)
        {
            if (m_SceneItems[i].entityId == id)
            {
                m_SceneItems[i].onReleaseEvent -= OnSceneItemRelease;
                m_SceneItems.RemoveAt(i);
                break;
            }
        }
    }

    private void OnSceneBuildingsRelease(int id)
    {
        for (int i = m_SceneBuildings.Count - 1; i >= 0; i--)
        {
            if (m_SceneBuildings[i].entityId == id)
            {
                m_SceneBuildings[i].onReleaseEvent -= OnSceneBuildingsRelease;
                m_SceneBuildings.RemoveAt(i);
                break;
            }
        }
    }

    private List<BaseSceneObject> m_SceneBuildings = null;
    private List<BaseEnemy> m_Enemies;
    private List<BaseSceneItem> m_SceneItems = null;
    private List<int> m_DeadEnemies;
}
