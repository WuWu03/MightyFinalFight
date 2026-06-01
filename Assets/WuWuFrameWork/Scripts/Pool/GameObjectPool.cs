using WuWuFramework.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace WuWuFramework.Pool
{
    public class GameObjectPool
    {
        private readonly Queue<PoolObjectInfo> m_QueuePool;
        private GameObject m_Prefab;
        private Transform m_Root;
        private int m_UsingCount;
        private string m_Tag;
        private bool m_IsFromAsset;
        private IResourcePoolMgr m_ResourcePoolMgr;
        
        public GameObjectPool(IResourcePoolMgr resourcePoolMgr,string tag, Transform poolRoot, GameObject prefab, bool isFromAsset)
        {
            GameObject root = new(tag);
            root.transform.SetParent(poolRoot, false);
            root.SetActiveSelf(false);
            m_Root = root.transform;
            m_Prefab = prefab;
            m_Tag = tag;
            m_IsFromAsset = isFromAsset;
            m_QueuePool = new Queue<PoolObjectInfo>();
            m_ResourcePoolMgr = resourcePoolMgr;
        }
        
        public int count
        {
            get
            {
                return m_QueuePool.Count;
            }
        }

        public int usingCount
        {
            get
            {
                return m_UsingCount;
            }
        }

        public bool isFromAsset
        {
            get
            {
                return m_IsFromAsset;
            }
        }
        
        public GameObject Get(bool isActive = true)
        {
            GameObject go = null;

            if (m_QueuePool.Count > 0)
            {
                lock (m_QueuePool)
                {
                    PoolObjectInfo info = m_QueuePool.Dequeue();
                    go = info.poolObject as GameObject;
                    info.Release();
                }
            }

            if (go == null)
            {
                go = Object.Instantiate(m_Prefab, null, false);
            }

            if (m_IsFromAsset)
            {
                GameObjectUnLoader resourceUnLoader = go.GetOrAddComponent<GameObjectUnLoader>();
                resourceUnLoader.ResetAssetInfo();
                resourceUnLoader.gameObjectPath = m_Tag;
            }

            m_UsingCount++;
            go.SetActiveSelf(isActive);
            return go;
        }

        public void Cache()
        {
            if (m_Prefab == null)
            {
                return;
            }

            GameObject go = GameObject.Instantiate(m_Prefab, m_Root, false);
            go.SetActiveSelf(false);
            m_QueuePool.Enqueue(PoolObjectInfo.Create(go, -1, false, string.Empty));
        }

        public void Put(GameObject go, bool isReleaseImmdiately)
        {
            if (go != null)
            {
                go.SetActiveSelf(false);
                go.transform.SetParent(m_Root, false);
                go.transform.localPosition = Vector3.zero;
                m_UsingCount--;
                m_QueuePool.Enqueue(PoolObjectInfo.Create(go, Time.time, isReleaseImmdiately, string.Empty));

                if (isReleaseImmdiately)
                {
                    CheckRelease();
                }
            }
        }

        public void CheckRelease()
        {
            int poolCount = m_QueuePool.Count;

            while (poolCount > 0)
            {
                poolCount--;

                PoolObjectInfo info = m_QueuePool.Dequeue();

                if (info.isReleaseImmediate || (info.releaseTime > 0 && Time.time - info.releaseTime >= ConstField.CollectTime))
                {
                    DestroyPoolObject(info);
                    info.Release();
                }
                else
                {
                    m_QueuePool.Enqueue(info);
                }
            }
        }

        public void Clear()
        {
            while (m_QueuePool.Count > 0)
            {
                PoolObjectInfo info = m_QueuePool.Dequeue();

                if (info != null)
                {
                    DestroyPoolObject(info);
                    info.Release();
                }
            }

            if (m_IsFromAsset)
            {
                m_ResourcePoolMgr.Put(m_Tag, m_Prefab);
            }

            Object.DestroyImmediate(m_Root.gameObject);
            m_QueuePool.Clear();
            m_Prefab = null;
            m_Root = null;
            m_Tag = string.Empty;
            m_IsFromAsset = false;
        }

        private void DestroyPoolObject(PoolObjectInfo info)
        {
            Object.DestroyImmediate(info.poolObject);
        }
    }
}