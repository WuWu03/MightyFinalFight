using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Log;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private void Awake()
    {
        m_DeadEnemies = new List<int>();
        m_CurrEnemies = new List<BaseEnemy>();
        m_SceneBuildings = new List<BaseSceneObject>();
        m_SceneItems = new List<BaseSceneItem>();
    }

    public BaseEnemy CreateEnemy(int sourceId, int entityId, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos, bool startBehaviourTree = true)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(DataHelper.roleDatas.GetDataById(sourceId), entityId, hp, attack, defense, hpBarWidth, pos);

        if(enemy == null)
        {
            GameFrameworkLog.LogError("create enemy failed sourceId:" + sourceId + ",entityId:" + entityId);
            return null;
        }

        enemy.onDeadEvent += OnEnemyDead;
        m_CurrEnemies.Add(enemy);
        return enemy;
    }

    public BaseSceneItem CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemConfigData data = StaticConfig.SceneItemConfig.GetData(id);
        BaseSceneItem sceneItem = SceneEntityFactory.CreateSceneItem(data, pos);

        if (sceneItem == null)
        {
            GameFrameworkLog.LogError("create sceneItem failed id:" + id);
            return null;
        }

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
                GameFrameworkLog.LogError("create sceneItem stageId:" + data.Id + ",buildId:" + data.SceneBuildings[i].Id);
                continue;
            }

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
        }
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

        for (int i = 0; i < m_CurrEnemies.Count; i++)
        {
            m_CurrEnemies[i].onDeadEvent -= OnEnemyDead;
            m_CurrEnemies[i].Release();
        }

        m_CurrEnemies.Clear();
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
        return m_CurrEnemies.Count <= 0;
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

    private void OnEnemyDead(int id)
    {
        m_DeadEnemies.Add(id);

        for (int i = m_CurrEnemies.Count - 1; i >= 0; i--)
        {
            if (m_CurrEnemies[i].entityId == id)
            {
                m_CurrEnemies[i].onDeadEvent -= OnEnemyDead;
                m_CurrEnemies.RemoveAt(i);
            }
        }
    }

    private List<BaseSceneObject> m_SceneBuildings = null;
    private List<int> m_DeadEnemies;
    private List<BaseEnemy> m_CurrEnemies;
    private List<BaseSceneItem> m_SceneItems = null;
}
