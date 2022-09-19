using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class PoolMgr : BaseMgr<PoolMgr>
    {
        class Pool
        {
            public List<GameObject> poolList
            {
                get
                {
                    return m_ListPool;
                }
            }

           
            public string tag
            {
                get
                {
                    return m_Tag;
                }
            }

            public Pool(string tag, Transform parent, GameObject obj)
            {
                GameObject go = new GameObject(tag);
                go.transform.SetParent(parent, false);

                m_CachePool = go.transform;
                m_Obj = obj;
                m_Tag = tag;
                m_Parent = parent;

                m_ListPool = new List<GameObject>();
            }

            public GameObject Spawn(bool isActive = true)
            {
                for (int i = 0; i < poolList.Count; i++)
                {
                    if (!poolList[i].activeSelf)
                    {
                        poolList[i].SetActive(isActive);
                        return poolList[i];
                    }
                }

                GameObject go = GameObject.Instantiate(m_Obj, m_CachePool, false);
                go.SetActive(isActive);
                poolList.Add(go);

                return go;
            }

            public void UnSpawn(GameObject go)
            {
                if (go != null)
                {
                    go.SetActive(false);
                    go.transform.SetParent(m_Parent, false);
                }
            }

            public void UnSpawnAll()
            {
                for (int i = 0; i < poolList.Count; i++)
                {
                    poolList[i].SetActive(false);
                }
            }

            public void Clear()
            {
                for (int i = poolList.Count - 1; i > 0; i--)
                {
                    GameObject go = poolList[i];

                    if (go != null)
                    {
                        poolList.RemoveAt(i);
                        GameObject.Destroy(go);
                    }
                }
            }

            private GameObject m_Obj = null;
            private Transform m_Parent = null;
            private Transform m_CachePool = null;
            private List<GameObject> m_ListPool = null;
            private string m_Tag = string.Empty;
        }

        private Dictionary<string, Pool> m_DicPool = null;

        protected override void OnAwake()
        {
            m_DicPool = new Dictionary<string, Pool>();
        }

        /// <summary>
        /// 添加一个池
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">预制对象</param>
        /// <param name="prefab">预制数量</param>
        public void AddPool(string tag, GameObject obj, int prefab = 1)
        {
            Pool pool = null;

            if (!m_DicPool.TryGetValue(tag, out pool))
            {
                pool = new Pool(tag, transform, obj);

                for (int i = 0; i < prefab; i++)
                {
                    pool.Spawn(false);
                }

                m_DicPool.Add(tag, pool);
            }
        }

        /// <summary>
        /// 生成一个预制物
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="isActive">活跃状态</param>
        /// <returns></returns>
        public GameObject Spawn(string tag, Transform parent, string layer, bool isActive = true)
        {
            Pool pool = null;

            if (m_DicPool.TryGetValue(tag, out pool))
            {
                GameObject go = pool.Spawn(isActive);
                go.transform.SetParent(parent, false);
                go.SetLayer(layer, true);
                return go;
            }

            return null;
        }

        /// <summary>
        /// 隐藏预制物
        /// </summary>
        /// <param name="tag">标签</param>
        public void UnSpawnAll(string tag)
        {
            Pool pool = null;

            if (m_DicPool.TryGetValue(tag, out pool))
            {
                pool.UnSpawnAll();
            }
        }

        /// <summary>
        /// 隐藏预制物
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">预制物</param>
        public void UnSpawn(string tag, GameObject obj)
        {
            Pool pool = null;

            if (m_DicPool.TryGetValue(tag, out pool))
            {
                pool.UnSpawn(obj);
            }
        }

        /// <summary>
        /// 删除一个池
        /// </summary>
        /// <param name="tag">标签</param>
        public void RemovePool(string tag)
        {
            Pool pool = null;

            if (m_DicPool.TryGetValue(tag, out pool))
            {
                pool.Clear();
                m_DicPool.Remove(tag);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnShutDown()
        {
            List<string> poolNames = new List<string>(m_DicPool.Keys);
            for (int i = poolNames.Count - 1; i > 0; i--)
            {
                Pool pool = m_DicPool[poolNames[i]];
                pool.Clear();
                m_DicPool.Remove(tag);
            }

            m_DicPool.Clear();
        }
    }
}