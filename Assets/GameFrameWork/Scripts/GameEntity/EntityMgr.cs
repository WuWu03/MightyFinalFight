using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.GameEntity
{
    public class EntityMgr : BaseMgr<EntityMgr>
    {
        public int AcquireCount
        {
            get
            {
                return m_AcquireCount;
            }
        }


        public int CreateCount
        {
            get
            {
                return m_CreateCount;
            }
        }

        public int ReleaseCount
        {
            get
            {
                return m_ReleaseCount;
            }
        }

        public int DestroyCount
        {
            get
            {
                return m_DestroyCount;
            }
        }

        public int UsingEntityCount
        {
            get
            {
                return m_DicUsingEntity.Count;
            }
        }

        public int UnUsedEntityCount
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
            m_DicUnUsedEntity = new Dictionary<Type, Queue<BaseEntity>>();
        }

        public T GetEntity<T>(string name = null,Transform parent = null) where T : BaseEntity
        {
            T entity = null;
            Type key = typeof(T);
            Queue<BaseEntity> unUsedQueue = GetUnUsedQueue(key);

            if (unUsedQueue.Count > 0)
            {
                entity = unUsedQueue.Dequeue() as T;
            }

            if(entity == null)
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

            GetUnUsedQueue(key).Enqueue(entity);
            GetUsingList(key).Remove(entity);
            m_ReleaseCount++;
        }

        public void DestroyEntities(BaseEntity[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                PutEntity(entities[i]);
            }
        }

        public void DestroyEntity(BaseEntity entity)
        {
            entity.Release();
            GetUsingList(entity.GetType()).Remove(entity);
            GameObject.DestroyImmediate(entity.gameObject);
            m_DestroyCount++;
        }

        public T[] FindEntities<T>(string name = null) where T : BaseEntity
        {
            List<T> entityList = new List<T>();
            List<BaseEntity> usingList = GetUsingList(typeof(T));

            for (int i = 0; i < usingList.Count; i++)
            {
                if (string.IsNullOrEmpty(name) || usingList[i].name.Equals(name))
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
                if (string.IsNullOrEmpty(name) || usingList[i].name.Equals(name))
                {
                    return usingList[i] as T;
                }
            }

            return null;
        }
        
        public bool HasEntity<T>(string name = null)  where T:BaseEntity
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

        private Queue<BaseEntity> GetUnUsedQueue(Type type)
        {
            if (!m_DicUnUsedEntity.TryGetValue(type, out Queue<BaseEntity> unUsedQueue))
            {
                unUsedQueue = new Queue<BaseEntity>();
                m_DicUnUsedEntity.Add(type, unUsedQueue);
            }

            return unUsedQueue;
        }

        protected override void OnShutDown()
        {
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
        private Dictionary<Type,List<BaseEntity>> m_DicUsingEntity = null;
        private Dictionary<Type,Queue<BaseEntity>> m_DicUnUsedEntity = null;
    }
}