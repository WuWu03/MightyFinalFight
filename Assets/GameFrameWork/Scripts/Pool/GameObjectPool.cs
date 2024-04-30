using GameFrameWork.Resources;
using GameFrameWork.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class GameObjectPool : BaseMgr<GameObjectPool>
    {
        class Pool
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

            public Pool(string tag, Transform poolRoot, GameObject prefab, bool isFromAsset)
            {
                GameObject root = new GameObject(tag);
                root.transform.SetParent(poolRoot, false);
                m_GORoot = root.transform;
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
                    ResourceUnLoader resourceUnLoader = go.GetOrAddComponent<ResourceUnLoader>();
                    resourceUnLoader.ResetAssetInfo();
                    resourceUnLoader.assetPath = m_Tag;
                }

                m_UsingCount++;
                go.SetActive(isActive);
                return go;
            }

            public void Cache()
            {
                if (m_Prefab == null)
                {
                    return;
                }

                GameObject go = GameObject.Instantiate(m_Prefab, m_GORoot, false);

                if (m_IsFromAsset)
                {
                    ResourceUnLoader resourceUnLoader = go.GetOrAddComponent<ResourceUnLoader>();
                    resourceUnLoader.ResetAssetInfo();
                    resourceUnLoader.assetPath = m_Tag;
                }

                go.SetActive(false);
                m_QueuePool.Enqueue(PoolObjectInfo.Create(go, -1, false, string.Empty));
            }

            public void Put(GameObject go, bool isReleaseImmdiately)
            {
                if (go != null)
                {
                    go.SetActive(false);
                    go.transform.SetParent(m_GORoot, false);
                    go.transform.localPosition = Vector3.zero;
                    m_UsingCount--;
                    m_QueuePool.Enqueue(PoolObjectInfo.Create(go, Time.time, isReleaseImmdiately, string.Empty));
                }
            }

            public void CheckRelease()
            {
                int count = m_QueuePool.Count;

                while (count > 0)
                {
                    count--;

                    PoolObjectInfo info = m_QueuePool.Dequeue();

                    if (info.isReleaseImmediate || (info.releaseTime > 0 && Time.time - info.releaseTime > ConstField.CollectTime))
                    {
                        GameObject.Destroy(info.poolObject);
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
                        ResourceUnLoader[] resourceUnLoaders = (info.poolObject as GameObject).GetComponentsInChildren<ResourceUnLoader>(true);

                        for (int i = 0; i < resourceUnLoaders.Length; i++)
                        {
                            resourceUnLoaders[i].BeforeOnDestroy();
                        }

                        GameObject.Destroy(info.poolObject);
                    }
                }

                m_QueuePool.Clear();
                m_Prefab = null;
                m_Tag = string.Empty;
                m_IsFromAsset = false;
                GameObject.Destroy(m_GORoot.gameObject);
            }

            private GameObject m_Prefab = null;
            private Transform m_GORoot = null;
            private Queue<PoolObjectInfo> m_QueuePool = null;
            private int m_UsingCount = 0;
            private string m_Tag = string.Empty;
            private bool m_IsFromAsset = false;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
           
            m_PoolRoot = new GameObject("GameObjectPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999f, 9999f, 9999f);

            m_DicPool = new Dictionary<string, Pool>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_ListReleasePoolKey = new List<string>();
        }

        /// <summary>
        /// 添加一个池
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">预制对象</param>
        /// <param name="count">预制数量</param>
        public void AddPool(string tag, GameObject obj, int count = 1)
        {
            AddPool(tag, obj, count, false);
        }

        /// <summary>
        /// 删除一个池
        /// </summary>
        public void RemovePool(string tag)
        {
            if (m_DicPool.TryGetValue(tag, out Pool pool))
            {
                pool.Clear();
                m_DicPool.Remove(tag);
            }
        }

        /// <summary>
        /// 生成一个预制物
        /// </summary>
        public GameObject Get(string tag, Transform parent, string layer, bool isActive = true)
        {

            if (m_DicPool.TryGetValue(tag, out Pool pool))
            {
                GameObject go = pool.Get(isActive);

                go.transform.SetParent(parent, false);
                go.SetLayer(layer, true);
                return go;
            }

            return null;
        }

        /// <summary>
        /// 从资源中加载一个物体
        /// </summary>
        public void GetFromAsset(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> call, params object[] args)
        {
            GameObject go = Get(assetPath, null, string.Empty);

            if (go != null)
            {
                call?.Invoke(assetPath, go, args);
            }
            else
            {
                LoadRequest request = LoadRequest.Create();
                request.assetPath = assetPath;
                request.action = call;
                request.args = args;

                if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
                {
                    listLoadRequest = new List<LoadRequest>() { request };
                    m_DicLoadRequests.Add(assetPath, listLoadRequest);
                    ResourcesMgr.instance.LoadAssetAsync<GameObject>(assetPath, OnLoaded);
                }
                else
                {
                    listLoadRequest.Add(request);
                }
            }
        }

        /// <summary>
        /// 回收物体
        /// </summary>
        public void Put(string tag, GameObject go, bool isReleaseImmdiately = false)
        {
            if (m_DicPool.TryGetValue(tag, out Pool pool))
            {
                pool.Put(go, isReleaseImmdiately);
            }
        }

        private void AddPool(string tag, GameObject obj, int prefab, bool isFromAsset)
        {
            if (!m_DicPool.TryGetValue(tag, out Pool pool))
            {
                pool = new Pool(tag, m_PoolRoot, obj, isFromAsset);

                for (int i = 0; i < prefab; i++)
                {
                    pool.Cache();
                }

                m_DicPool.Add(tag, pool);
            }
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
        {
            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Log.LogError(StringUtil.Format("[", assetPath, "] 资源加载完成 , 但回调函数不存在"));
                return;
            }

            AddPool(assetPath, obj as GameObject, 1, true);

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                listLoadRequest[i].Call(Get(assetPath, null, string.Empty));
            }

            m_DicLoadRequests.Remove(assetPath);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            m_ListReleasePoolKey.Clear();

            foreach (KeyValuePair<string, Pool> kvp in m_DicPool)
            {
                kvp.Value.CheckRelease();

                if (kvp.Value.count < 1 && kvp.Value.isFromAsset && kvp.Value.usingCount < 1)
                {
                    m_ListReleasePoolKey.Add(kvp.Key);
                }
            }

            for (int i = 0; i < m_ListReleasePoolKey.Count; i++)
            {
                m_DicPool[m_ListReleasePoolKey[i]].Clear();
                m_DicPool.Remove(m_ListReleasePoolKey[i]);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (KeyValuePair<string, Pool> kvp in m_DicPool)
            {
                kvp.Value.Clear();
            }

            m_DicLoadRequests.Clear();
            m_DicPool.Clear();
        }

        private List<string> m_ListReleasePoolKey = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
        private Transform m_PoolRoot = null;
        private Dictionary<string, Pool> m_DicPool = null;
    }
}