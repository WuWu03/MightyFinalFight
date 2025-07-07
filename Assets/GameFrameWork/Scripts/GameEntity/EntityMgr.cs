using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public class EntityMgr : BaseMgr<EntityMgr>
    {
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
                return m_ListUsingEntities.Count;
            }
        }

        public int unUsedEntityCount
        {
            get
            {
                return m_DicUnUsedEntity.Count;
            }
        }

        protected override void OnAwake()
        {
            m_PoolRoot = new GameObject("EntityMgr").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999, 9999, 9999);
            m_ListUsingEntities = new List<BaseEntity>();
            m_DicUnUsedEntity = new Dictionary<Type, List<BaseEntity>>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = m_ListUsingEntities.Count - 1; i > - 1 ; i--)
            {
                m_ListUsingEntities[i].Update(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();

            for (int i = m_ListUsingEntities.Count - 1; i > -1; i--)
            {
                m_ListUsingEntities[i].LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            for (int i = m_ListUsingEntities.Count - 1; i > -1; i--)
            {
                m_ListUsingEntities[i].FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
            }
        }

        public T GetEntity<T>(string name = null, Transform parent = null) where T : BaseEntity, new()
        {
            T entity = null;
            GameObject entityGameObject = null;
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

            if (entity == null)
            {
                entityGameObject = new GameObject();
                entity = Activator.CreateInstance<T>();
                entity.SetGameObject(entityGameObject);
                DontDestroyOnLoad(entityGameObject);
                m_CreateCount++;
            }

            m_AcquireCount++;

            entity.Init(m_AcquireCount, name);
            entity.SetParent(parent, false);
            entity.SetActive(true);
            m_ListUsingEntities.Add(entity);

            return entity;
        }

        public void PutEntities(BaseEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                PutEntity(entities[i]);
            }
        }

        public void PutEntity(BaseEntity entity)
        {
            entity.SetActive(false);
            entity.SetParent(m_PoolRoot, false);

            Type key = entity.GetType();
            GetUnUsedList(key).Add(entity);
            m_ListUsingEntities.Remove(entity);
            m_ReleaseCount++;
        }

        public void DestroyEntities(BaseEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                DestroyEntity(entities[i]);
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

            entity.BeforeDestroy();
            GameObject.Destroy(entity.gameObject);
            m_DestroyCount++;
        }

        public void DestoryAllUnUsedEntities()
        {
            foreach (KeyValuePair<Type, List<BaseEntity>> kvp in m_DicUnUsedEntity)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i].BeforeDestroy();
                    GameObject.Destroy(kvp.Value[i].gameObject);
                    m_ReleaseCount++;
                    m_DestroyCount++;
                }
            }

            m_DicUnUsedEntity.Clear();
        }

        public void DestroyAll()
        {
            for (int i = 0; i < m_ListUsingEntities.Count; i++)
            {
                m_ListUsingEntities[i].Release();
            }

            m_ListUsingEntities.Clear();
            DestoryAllUnUsedEntities();

            m_ListUsingEntities.Clear();
            m_AcquireCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_DestroyCount = 0;
        }

        public List<T> FindEntities<T>(string name = null) where T : BaseEntity
        {
            List<T> entityList = new List<T>();
            Type type = typeof(T);

            for (int i = 0; i < m_ListUsingEntities.Count; i++)
            {
                if (m_ListUsingEntities[i].GetType() == type)
                {
                    if (string.IsNullOrEmpty(name) || m_ListUsingEntities[i].entityName.Equals(name))
                    {
                        entityList.Add(m_ListUsingEntities[i] as T);
                    }
                }
            }

            return entityList;
        }

        public T FindEntity<T>(string name = null) where T : BaseEntity
        {
            Type type = typeof(T);

            for (int i = 0; i < m_ListUsingEntities.Count; i++)
            {
                if (m_ListUsingEntities[i].GetType() == type)
                {
                    if (string.IsNullOrEmpty(name) || m_ListUsingEntities[i].entityName.Equals(name))
                    {
                        return m_ListUsingEntities[i] as T;
                    }
                }
            }

            return null;
        }

        public bool HasEntity<T>(string name = null) where T : BaseEntity
        {
            return FindEntity<T>(name) != null;
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

        protected override void OnShutDown()
        {
            DestroyAll();
            m_ListUsingEntities.Clear();
            m_DicUnUsedEntity.Clear();
            m_AcquireCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_DestroyCount = 0;
        }

        private int m_AcquireCount = 0;
        private int m_CreateCount = 0;
        private int m_ReleaseCount = 0;
        private int m_DestroyCount = 0;
        private Transform m_PoolRoot = null;
        private List<BaseEntity> m_ListUsingEntities = null;
        private Dictionary<Type, List<BaseEntity>> m_DicUnUsedEntity = null;
    }
}