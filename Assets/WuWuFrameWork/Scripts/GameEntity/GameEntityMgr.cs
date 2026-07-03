using System;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Pool;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.GameEntity
{
    public class GameEntityMgr : WuWuFrameworkModule, IGameEntityMgr
    {
        private readonly List<BaseEntity> m_UsingEntities;
        private readonly List<BaseEntity> m_TempEntities;
        private readonly Dictionary<Type, List<BaseEntity>> m_DicUnUsedEntity;
        private Transform m_PoolRoot;
        private IGameObjectPoolMgr m_GameObjectPoolMgr;
        private int m_AcquireCount;
        private int m_CreateCount;
        private int m_ReleaseCount;
        private int m_DestroyCount;

        public GameEntityMgr()
        {
            m_UsingEntities = new List<BaseEntity>();
            m_TempEntities = new List<BaseEntity>();
            m_DicUnUsedEntity = new Dictionary<Type, List<BaseEntity>>();
        }

        public int acquireCount
        {
            get
            {
                return m_AcquireCount;
            }
        }


        public int createCount
        {
            get
            {
                return m_CreateCount;
            }
        }

        public int releaseCount
        {
            get
            {
                return m_ReleaseCount;
            }
        }

        public int destroyCount
        {
            get
            {
                return m_DestroyCount;
            }
        }

        public int usingEntityCount
        {
            get
            {
                return m_UsingEntities.Count;
            }
        }

        public int unUsedEntityCount
        {
            get
            {
                return m_DicUnUsedEntity.Count;
            }
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {

        }

        public override void Shutdown()
        {
            DestroyAll();
            m_UsingEntities.Clear();
            m_DicUnUsedEntity.Clear();
            m_AcquireCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_DestroyCount = 0;
        }

        public void SetGameObjectPoolMgr(IGameObjectPoolMgr gameObjectPoolMgr)
        {
            m_GameObjectPoolMgr = gameObjectPoolMgr;
            m_PoolRoot = new GameObject("EntityPool").transform;
            m_PoolRoot.SetParent(WuWuFrameworkEntry.gameEntryObj.transform, false);
            m_PoolRoot.localPosition = new Vector3(9999, 9999, 9999);
        }

        public T GetEntity<T>(string entityName = null, Transform parent = null) where T : BaseEntity, new()
        {
            T entity = null;
            Type type = typeof(T);
            List<BaseEntity> unUsedList = GetUnUsedList(type);

            if (unUsedList.Count > 0)
            {
                lock (unUsedList)
                {
                    entity = unUsedList[0] as T;
                    unUsedList.RemoveAt(0);
                }
            }

            if (entity is null)
            {
                entity = new GameObject().GetOrAddComponent<T>();
                UnityObject.DontDestroyOnLoad(entity);
                m_CreateCount++;
            }

            m_AcquireCount++;
            entity.Init(m_AcquireCount, entityName, this, m_GameObjectPoolMgr);
            entity.SetParent(parent);
            entity.gameObject.SetActiveSelf(true);
            m_UsingEntities.Add(entity);
            return entity;
        }

        public void PutEntities(BaseEntity[] entities)
        {
            foreach (var entity in entities)
            {
                PutEntity(entity);
            }
        }

        public void PutEntity(BaseEntity entity)
        {
            entity.gameObject.SetActiveSelf(false);
            entity.SetParent(m_PoolRoot);
            Type key = entity.GetType();
            GetUnUsedList(key).Add(entity);
            m_UsingEntities.Remove(entity);
            m_ReleaseCount++;
        }

        public void DestroyEntities(BaseEntity[] entities)
        {
            foreach (var entity in entities)
            {
                DestroyEntity(entity);
            }
        }

        public void DestroyEntity(BaseEntity entity)
        {
            entity.Release();
            Type key = entity.GetType();

            if (m_DicUnUsedEntity.TryGetValue(key, out List<BaseEntity> list))
            {
                list.Remove(entity);

                if (list.Count < 1)
                {
                    m_DicUnUsedEntity.Remove(key);
                }
            }

            m_UsingEntities.Remove(entity);
            UnityObject.Destroy(entity.gameObject);
            m_DestroyCount++;
        }

        public void DestroyAllUnUsedEntities()
        {
            foreach (KeyValuePair<Type, List<BaseEntity>> kvp in m_DicUnUsedEntity)
            {
                foreach (var entity in kvp.Value)
                {
                    UnityObject.Destroy(entity.gameObject);
                    m_ReleaseCount++;
                    m_DestroyCount++;
                }
            }

            m_DicUnUsedEntity.Clear();
        }

        public void DestroyAll()
        {
            m_TempEntities.AddRange(m_UsingEntities);
            foreach (var entity in m_TempEntities)
            {
                entity.Release();
            }

            m_TempEntities.Clear();
            m_UsingEntities.Clear();
            DestroyAllUnUsedEntities();
            m_AcquireCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_DestroyCount = 0;
        }

        public List<T> FindEntities<T>(string entityName) where T : BaseEntity
        {
            List<T> entities = new();
            FindEntities(entityName, entities);
            return entities;
        }

        public void FindEntities<T>(string entityName, List<T> entityList) where T : BaseEntity
        {
            if (entityList == null)
            {
                throw new WuWuFrameworkException("实体列表为空");
            }

            entityList.Clear();

            foreach (var entity in m_UsingEntities)
            {
                if (entity is T baseEntity && baseEntity.name.Equals(entityName))
                {
                    entityList.Add(baseEntity);
                }
            }
        }

        public T FindEntity<T>(string entityName) where T : BaseEntity
        {
            if (string.IsNullOrEmpty(entityName))
            {
                return null;
            }

            foreach (var entity in m_UsingEntities)
            {
                if (entity is T baseEntity && baseEntity.name.Equals(entityName))
                {
                    return baseEntity;
                }
            }

            return null;
        }

        public bool HasEntity<T>(string entityName) where T : BaseEntity
        {
            return FindEntity<T>(entityName) is not null;
        }

        private List<BaseEntity> GetUnUsedList(Type type)
        {
            if (!m_DicUnUsedEntity.TryGetValue(type, out List<BaseEntity> unUsedList))
            {
                unUsedList = new List<BaseEntity>();
                m_DicUnUsedEntity.Add(type, unUsedList);
            }

            return unUsedList;
        }
    }
}