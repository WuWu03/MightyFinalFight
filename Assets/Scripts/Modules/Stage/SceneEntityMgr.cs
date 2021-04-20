using GameFrameWork;
using GameFrameWork.Pool;
using System.Collections.Generic;
using UnityEngine;

public class SceneEntityMgr : BaseMgr<SceneEntityMgr>
{
    private void Awake()
    {
        m_ListDeadEnemy = new List<int>();
        m_ListCurrEnemy = new List<BaseEnemy>();
    }


    public void CreateEnemy(int sourceID, int engityID, Vector2Int pos)
    {
        BaseEnemy enemy = SceneEntityFactory.CreateEnemy(StaticConfig.EnemyConfig.GetData(sourceID), engityID, pos);
        enemy.OnDead += OnEnemyDead;
        m_ListCurrEnemy.Add(enemy);
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

    public void CreateSceneItemTest()
    {
        //SceneItemData data = StaticConfig.SceneItemConfig.GetData(1002);
        //StageFactory.CreateSceneItem(data, new Vector2Int(-320, -60));

        //for (int i = 0; i < 4; i++)
        //{
        //    data = StaticConfig.SceneItemConfig.GetData(1004 + i);
        //    StageFactory.CreateSceneItem(data, new Vector2Int(-300 + i * 20, -60));
        //}
    }

    public void CreateSceneItem(int id, Vector2Int pos)
    {
        SceneItemData data = StaticConfig.SceneItemConfig.GetData(id);
        SceneEntityFactory.CreateSceneItem(data, pos);
    }

    public void CreateBarrels()
    {
        for (int i = 0; i < 5; i++)
        {
            BaseSceneItem sceneItem = SceneObjectPool.Ins.Get<Barrel>("Barrel");
            sceneItem.InitInfo(new BarrelInfo()
            {
                ID = 1,
                Health = 1,
                MaxHealth = 1,
                TriggerOffest = new Vector2(0, 0.13f),
                TriggerSize = new Vector2(0.17f, 0.25f),
                Value = 0,
                CanDrop = false,
                Dir = 1,
                GroundY = 0,
                IsFloat = false,
                MoveSpeed = 0f,
                Item = 1001 + i,
            });
            sceneItem.SetRes(string.Format("{0}/{1}", ResDefine.PREFAB_PATH, "Item/Barrel"));
            sceneItem.SetObjectType(ObjectType.Monster);
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
                m_ListCurrEnemy.RemoveAt(i);
            }
        }
    }


    private List<int> m_ListDeadEnemy;
    private List<BaseEnemy> m_ListCurrEnemy;
}
