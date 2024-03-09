using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Resources
{
    public class ResourcesPool : BaseMgr<ResourcesPool>
    {
        protected override void OnAwake()
        {
            m_PoolRoot = new GameObject("ResPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(-9999f, -9999f, -9999f);
            m_DicPool = new Dictionary<string, Queue<ResourcePoolInfo>>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_ListMarkResource = new List<ResourceMark>();
            m_RemoveList = new List<string>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            //if (m_CollectTimer != 0 && Time.time - m_CollectTimer < 0.2f)
            //{
            //    return;
            //}

            //m_CollectTimer = Time.time;

            if (m_DicPool == null || m_DicPool.Count < 1)
            {
                return;
            }

            m_ListMarkResource.Clear();
            m_RemoveList.Clear();
            
            foreach(KeyValuePair<string,Queue<ResourcePoolInfo>> kvp in m_DicPool)
            {
                Queue<ResourcePoolInfo> pool = kvp.Value;
                int cout = pool.Count;

                while (cout > 0)
                {
                    cout--;
                    lock (pool)
                    {
                        ResourcePoolInfo resource = pool.Dequeue();

                        if (resource.isReleaseImmediate || Time.time - resource.releaseTime >= COLLECT_TIME)
                        {
                            if (resource.poolObject is GameObject)
                            {
                                ResourceMark[] resourceMarks = (resource.poolObject as GameObject).GetComponentsInChildren<ResourceMark>(true);

                                if(resourceMarks != null && resourceMarks.Length > 0)
                                {
                                    for (int i = 0; i < resourceMarks.Length; i++)
                                    {
                                        if (resourceMarks[i].assetPath != resource.assetPath)
                                        {
                                            resourceMarks[i].transform.SetParent(m_PoolRoot, false);
                                            m_ListMarkResource.Add(resourceMarks[i]);
                                        }
                                    }
                                }
              
                                GameObject.Destroy(resource.poolObject);
                            }

                            ResourcesMgr.instance.UnloadAsset(resource.assetPath, false);
                            ReferencePool.Release(resource);
                            UnityEngine.Resources.UnloadUnusedAssets();
                        }
                        else
                        {
                            pool.Enqueue(resource);
                        }
                    }
                }

                if (pool.Count <= 0)
                {
                    m_RemoveList.Add(kvp.Key);
                }
            }

            for (int i = 0; i < m_RemoveList.Count; i++)
            {
                m_DicPool.Remove(m_RemoveList[i]);
            }

            for (int i = 0; i < m_ListMarkResource.Count; i++)
            {
                Put(m_ListMarkResource[i].assetPath, m_ListMarkResource[i].gameObject);
            }
        }

        public void Get<T>(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> call = null, params object[] args) where T : UnityEngine.Object
        {
            Get(assetPath, call, typeof(T), args);
        }

        public void Get(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> action = null, Type t = null, params object[] args)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Log.LogError("Asset path  is invalid.");
                return;
            }

            Queue<ResourcePoolInfo> pool = GetOrCreatePool(assetPath);

            if (pool.Count > 0)
            {
                lock (pool)
                {
                    ResourcePoolInfo resource = pool.Dequeue();
                    UnityEngine.Object go = resource.poolObject;
                    action(assetPath, go, args);
                    ReferencePool.Release(resource);
                }

                return;
            }

            LoadRequest request = LoadRequest.Create();
            request.assetPath = assetPath;
            request.action = action;
            request.args = args;

            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                listLoadRequest = new List<LoadRequest>() { request };
                m_DicLoadRequests.Add(assetPath, listLoadRequest);
                ResourcesMgr.instance.LoadAssetAsync(assetPath, OnLoaded, t);
            }
            else
            {
                listLoadRequest.Add(request);
            }
        }

        public void Put(string assetPath, UnityEngine.Object go, bool isReleaseImmediate = false)
        {
            if (string.IsNullOrEmpty(assetPath) || go == null)
            {
                return;
            }

            if (go is GameObject tempGO)
            {
                tempGO.SetActive(false);
                tempGO.transform.SetParent(m_PoolRoot, false);
                tempGO.transform.localPosition = Vector3.zero;
            }

            Queue<ResourcePoolInfo> pool = GetOrCreatePool(assetPath);

            lock (pool)
            {
                pool.Enqueue(ResourcePoolInfo.Create(go, Time.time, isReleaseImmediate, assetPath));
            }
        }

        public int GetCount(string assetPath)
        {
            if (m_DicPool.TryGetValue(assetPath, out Queue<ResourcePoolInfo> pool))
            {
                return pool.Count;
            }

            return 0;
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
        {
            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Log.LogError(StringUtil.Format("Resource [", assetPath, "] load complete,but the callback is invalid."));
                return;
            }

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                UnityEngine.Object result = null;

                if (obj is GameObject) 
                {
                    GameObject go = UnityEngine.Object.Instantiate(obj) as GameObject;
                    go.GetOrAddComponent<ResourceMark>().assetPath = assetPath;
                    result = go;
                }
                else
                {
                    result = obj;
                }

                listLoadRequest[i].Call(result);
            }

            m_DicLoadRequests.Remove(assetPath);
        }

        public Queue<ResourcePoolInfo> GetPool(string path)
        {
            return this.GetOrCreatePool(path);
        }

        private Queue<ResourcePoolInfo> GetOrCreatePool(string path)
        {
            if (!m_DicPool.TryGetValue(path, out Queue<ResourcePoolInfo> pool))
            {
                pool = new Queue<ResourcePoolInfo>();
                m_DicPool.Add(path, pool);
            }

            return pool;
        }

        protected override void OnShutDown()
        {
            m_DicPool.Clear();
            m_DicLoadRequests.Clear();
            m_ListMarkResource.Clear();
            m_RemoveList.Clear();

            m_DicPool = null;
            m_DicLoadRequests = null;
            m_ListMarkResource = null;
            m_RemoveList = null;
        }

        public const int COLLECT_TIME = 15;
        private float m_CollectTimer = 0;
        protected Transform m_PoolRoot = null;
        private List<ResourceMark> m_ListMarkResource = null;
        private List<string> m_RemoveList = null;
        private Dictionary<string, Queue<ResourcePoolInfo>> m_DicPool = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
    }
}