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
                return m_DicUsingEntity.Count;
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
            m_DicUsingEntity = new Dictionary<Type, List<BaseEntity>>();
            m_DicUnUsedEntity = new Dictionary<Type, List<BaseEntity>>();
        }

        public T GetEntity<T>(string name = null, Transform parent = null) where T : BaseEntity
        {
            T entity = null;
            Type key = typeof(T);
            List<BaseEntity> unUsedList = GetUnUsedList(key);

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
                entity = new GameObject().GetOrAddComponent<T>();
                DontDestroyOnLoad(entity);
                m_CreateCount++;
            }

            m_AcquireCount++;

            entity.Init(m_DicUsingEntity.Count, name);
            entity.SetParent(parent, false);
            entity.transform.localPosition = Vector3.zero;
            entity.SetActive(true);
            GetUsingList(key).Add(entity);

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
            entity.transform.localPosition = Vector3.zero;
            entity.SetParent(m_PoolRoot, false);

            Type key = entity.GetType();

            GetUnUsedList(key).Add(entity);

            if (m_DicUsingEntity.TryGetValue(key, out List<BaseEntity> list))
            {
                list.Remove(entity);

                if (list.Count < 1)
                {
                    m_DicUsingEntity.Remove(key);
                }
            }

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
            List<BaseEntity> releaseList = new List<BaseEntity>();

            foreach (KeyValuePair<Type, List<BaseEntity>> kvp in m_DicUsingEntity)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    releaseList.Add(kvp.Value[i]);
                }
            }

            for (int i = 0; i < releaseList.Count; i++)
            {
                releaseList[i].Release();
            }

            releaseList.Clear();
            DestoryAllUnUsedEntities();

            m_DicUsingEntity.Clear();
            m_AcquireCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_DestroyCount = 0;
        }

        public T[] FindEntities<T>(string name = null) where T : BaseEntity
        {
            List<T> entityList = new List<T>();
            List<BaseEntity> usingList = GetUsingList(typeof(T));

            for (int i = 0; i < usingList.Count; i++)
            {
                if (string.IsNullOrEmpty(name) || usingList[i].entityName.Equals(name))
                {
                    entityList.Add(usingList[i] as T);
                }
            }

            return entityList.ToArray();
        }

        public T FindEntity<T>(string name = null) where T : BaseEntity
        {
            List<BaseEntity> usingList = GetUsingList(typeof(T));

            for (int i = 0; i < usingList.Count; i++)
            {
                if (string.IsNullOrEmpty(name) || usingList[i].entityName.Equals(name))
                {
                    return usingList[i] as T;
                }
            }

            return null;
        }

        public bool HasEntity<T>(string name = null) where T : BaseEntity
        {
            return FindEntity<T>(name) != null;
        }

        private List<BaseEntity> GetUsingList(Type type)
        {
            if (!m_DicUsingEntity.TryGetValue(type, out List<BaseEntity> usingList))
            {
                usingList = new List<BaseEntity>();
                m_DicUsingEntity.Add(type, usingList);
            }

            return usingList;
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
            m_DicUsingEntity.Clear();
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
        private Dictionary<Type, List<BaseEntity>> m_DicUsingEntity = null;
        private Dictionary<Type, List<BaseEntity>> m_DicUnUsedEntity = null;
    }
}