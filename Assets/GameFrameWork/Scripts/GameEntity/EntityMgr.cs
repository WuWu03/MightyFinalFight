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
                return m_ListUsingEntity.Count;
            }
        }

        public int UnUsedEntityCount
        {
            get
            {
                return m_QueueUnUsedEntity.Count;
            }
        }

        private void Awake()
        {
            m_PoolRoot = new GameObject("EntityMgr").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999, 9999, 9999);
            m_ListUsingEntity = new List<BaseEntity>();
            m_QueueUnUsedEntity = new Queue<BaseEntity>();
        }

        public T GetEntity<T>(string name = null,Transform parent = null) where T : BaseEntity
        {
            T obj = null;

            if (m_QueueUnUsedEntity.Count > 0)
            {
                obj = m_QueueUnUsedEntity.Dequeue() as T;
            }

            if(obj == null)
            {
                obj = new GameObject().GetOrAddComponent<T>();
                DontDestroyOnLoad(obj);
                m_CreateCount++;
            }

            m_AcquireCount++;
            obj.Init(m_ListUsingEntity.Count, name);
            obj.SetParent(parent, false);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
            m_ListUsingEntity.Add(obj);
            return obj;
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
            m_QueueUnUsedEntity.Enqueue(entity);
            m_ListUsingEntity.Remove(entity);
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
            m_ListUsingEntity.Remove(entity);
            GameObject.DestroyImmediate(entity);
            m_DestroyCount++;
        }

        public T[] FindEntities<T>(string name = null) where T : BaseEntity
        {
            List<T> ret = new List<T>();
            Type entityType = typeof(T);

            for (int i = 0; i < m_ListUsingEntity.Count; i++)
            {
                if(m_ListUsingEntity[i].GetType().Equals(entityType))
                {
                    if(string.IsNullOrEmpty(name)|| m_ListUsingEntity[i].name.Equals(name))
                    {
                        ret.Add(m_ListUsingEntity[i] as T);
                    }
                }
            }

            return ret.ToArray();
        }

        public T FindEntity<T>(string name = null) where T : BaseEntity
        {
            Type entityType = typeof(T);

            for (int i = 0; i < m_ListUsingEntity.Count; i++)
            {
                if (m_ListUsingEntity[i].GetType().Equals(entityType))
                {
                    if (string.IsNullOrEmpty(name) || m_ListUsingEntity[i].name.Equals(name))
                    {
                        return m_ListUsingEntity[i] as T;
                    }
                }
            }

            return null;
        }

        protected override void OnShutDown()
        {
            m_ListUsingEntity.Clear();
            m_QueueUnUsedEntity.Clear();
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
        private List<BaseEntity> m_ListUsingEntity = null;
        private Queue<BaseEntity> m_QueueUnUsedEntity = null;
    }
}