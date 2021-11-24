using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private void Awake()
    {
        m_ListDeadEnemy = new List<int>();
        m_ListCurrEnemy = new List<BaseEnemy>();
        m_ListSceneBuilding = new List<BaseSceneObject>();
    }

    public BaseEnemy CreateEnemy(int sourceID, int engityID, int hp, int attack, int defense, int hpBarWidth, Vector2Int pos, bool startBehaviourTree = true)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(StaticConfig.CharacterConfig.GetData(sourceID), engityID, hp, attack, defense, hpBarWidth, pos);
        enemy.OnDeadEvent += OnEnemyDead;
        m_ListCurrEnemy.Add(enemy);
        return enemy;
    }

    public bool IsEnemyDead(int id)
    {
        for (int i = 0; i < m_ListDeadEnemy.Count; i++)
        {
            if (m_ListDeadEnemy[i] == id)
                return true;
        }

        return false;
    }

    public bool IsAllEnemyDead()
    {
        return m_ListCurrEnemy.Count <= 0;
    }

    public void CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemConfigData data = StaticConfig.SceneItemConfig.GetData(id);
        SceneEntityFactory.CreateSceneItem(data, pos);
    }

    public void CreateSceneBuildings(StageConfigData data)
    {
        for (int i = 0; i < data.SceneBuildings.Length; i++)
        {
            m_ListSceneBuilding.Add(SceneEntityFactory.CreateSceneBuilding(data.SceneBuildings[i]));
        }
    }

    public BaseSceneObject GetSceneBuildingByName(string name)
    {
        for (int i = 0; i < m_ListSceneBuilding.Count; i++)
        {
            if (m_ListSceneBuilding[i].Name == name)
            {
                return m_ListSceneBuilding[i];
            }
        }

        return null;
    }

    public void ReleaseSceneBuildings()
    {
        for (int i = 0; i < m_ListSceneBuilding.Count; i++)
        {
            m_ListSceneBuilding[i].Release();
        }

        m_ListSceneBuilding.Clear();
    }

    public void CreateBarrels()
    {
        for (int i = 0; i < 5; i++)
        {
            BaseSceneItem sceneItem = EntityMgr.Ins.GetEntity<Barrel>("Barrel");
            BarrelData barrelData = ReferencePool.Acquire<BarrelData>();
            barrelData.Id = 1;
            barrelData.Health = 1;
            barrelData.MaxHealth = 1;
            barrelData.Value = 0;
            barrelData.CanDrop = false;
            barrelData.Dir = 1;
            barrelData.GroundY = 0;
            barrelData.IsFloat = false;
            barrelData.MoveSpeed = 0f;
            barrelData.ItemId = 1001 + i;

            sceneItem.SetData(barrelData);
            sceneItem.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, "Item/Barrel"));
            sceneItem.SetObjectType(ObjectType.BreakItem);
            sceneItem.SetMapPos(new Vector2Int(-400 + i * 50, -66));
        }
    }

    private void OnEnemyDead(int id)
    {
        m_ListDeadEnemy.Add(id);

        for (int i = m_ListCurrEnemy.Count - 1; i >= 0; i--)
        {
            if (m_ListCurrEnemy[i].EntityID == id)
            {
                m_ListCurrEnemy[i].OnDeadEvent -= OnEnemyDead;
                m_ListCurrEnemy.RemoveAt(i);
            }
        }
    }

    private List<BaseSceneObject> m_ListSceneBuilding = null;
    private List<int> m_ListDeadEnemy;
    private List<BaseEnemy> m_ListCurrEnemy;
}
