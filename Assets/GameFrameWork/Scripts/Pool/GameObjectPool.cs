using GameFrameWork.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class GameObjectPool
    {
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

        public GameObjectPool(string tag, Transform poolRoot, GameObject prefab, bool isFromAsset)
        {
            GameObject root = new GameObject(tag);
            root.transform.SetParent(poolRoot, false);
            root.SetActiveSelf(false);
            m_Root = root.transform;
            m_Prefab = prefab;
            m_Tag = tag;
            m_IsFromAsset = isFromAsset;
            m_QueuePool = new Queue<PoolObjectInfo>();
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
                    ReferencePool.ReleaseReference(info);
                }
            }

            if (go == null)
            {
                go = GameObject.Instantiate(m_Prefab, null, false);
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
            int count = m_QueuePool.Count;

            while (count > 0)
            {
                count--;

                PoolObjectInfo info = m_QueuePool.Dequeue();

                if (info.isReleaseImmediate || (info.releaseTime > 0 && Time.time - info.releaseTime >= ConstField.CollectTime))
                {
                    DestoryPoolObject(info);
                    ReferencePool.ReleaseReference(info);
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
                    DestoryPoolObject(info);
                    ReferencePool.ReleaseReference(info);
                }
            }

            if (m_IsFromAsset)
            {
                AssetsPool.instance.Put(m_Tag, m_Prefab);
            }

            m_QueuePool.Clear();
            m_Prefab = null;
            m_Tag = string.Empty;
            m_IsFromAsset = false;
            GameObject.Destroy(m_Root.gameObject);
        }

        private void DestoryPoolObject(PoolObjectInfo info)
        {
            GameObject.Destroy(info.poolObject);
        }

        private GameObject m_Prefab = null;
        private Transform m_Root = null;
        private Queue<PoolObjectInfo> m_QueuePool = null;
        private int m_UsingCount = 0;
        private string m_Tag = string.Empty;
        private bool m_IsFromAsset = false;
    }
}