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
            m_DicPool = new Dictionary<string, Queue<UnityEngine.Object>>();
            m_DicLoadCallback = new Dictionary<string, List<LoadRequest>>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Time.time - m_CollectTimer >= CollectTime)
            {
                m_CollectTimer = Time.time;
                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        public virtual void Get<T>(string assetPath, GameFrameWorkAction<string,UnityEngine.Object, object[]> call, params object[] args) where T:UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.GameFrameworkLog.DebugError("Rescource param is invalid.");
                return;
            }

            Queue<UnityEngine.Object> pool = GetOrCreatePool(assetPath);

            if (pool.Count > 0)
            {
                UnityEngine.Object go = pool.Dequeue();
                call(assetPath, go, args);
                return;
            }

            LoadRequest request = new LoadRequest(assetPath, call, args);

            if (!m_DicLoadCallback.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                listLoadRequest = new List<LoadRequest>() { request };
                m_DicLoadCallback.Add(assetPath, listLoadRequest);
                ResourcesMgr.instance.LoadAssetAsync<T>(assetPath, OnLoaded, true);
            }
            else
            {
                listLoadRequest.Add(request);
            }
        }

        public virtual void Put(string assetPath, UnityEngine.Object go)
        {
            if (string.IsNullOrEmpty(assetPath) || go == null)
            {
                return;
            }

            if(go is GameObject tempGO)
            {
                tempGO.SetActive(false);
                tempGO.transform.SetParent(m_PoolRoot, false);
                tempGO.transform.localPosition = Vector3.zero;
            }

            Queue<UnityEngine.Object> pool = GetOrCreatePool(assetPath);
            pool.Enqueue(go);
        }

        public int GetCount(string assetPath)
        {
            if (m_DicPool.TryGetValue(assetPath, out Queue<UnityEngine.Object> pool))
            {
                return pool.Count;
            }

            return 0;
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
        {
            if (!m_DicLoadCallback.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Debug.GameFrameworkLog.DebugError(StringUtil.FormatDefault("Resource [", assetPath, "] load complete,but the callback is invalid."));
                return;
            }

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                UnityEngine.Object go = obj is GameObject ? UnityEngine.Object.Instantiate(obj) as GameObject : obj;

                if (!listLoadRequest[i].Call(go))
                {
                    Put(assetPath, go);
                }
            }

            m_DicLoadCallback.Remove(assetPath);
        }

        public Queue<UnityEngine.Object> GetPool(string path)
        {
            return this.GetOrCreatePool(path);
        }

        private Queue<UnityEngine.Object> GetOrCreatePool(string path)
        {
            if (!m_DicPool.TryGetValue(path, out Queue<UnityEngine.Object> pool))
            {
                pool = new Queue<UnityEngine.Object>();
                m_DicPool.Add(path, pool);
            }

            return pool;
        }

        protected override void OnShutDown()
        {
            m_DicPool.Clear();
            m_DicLoadCallback.Clear();
        }

        public const int CollectTime = 15;
        private float m_CollectTimer = 0;
        protected Transform m_PoolRoot = null;
        private Dictionary<string, Queue<UnityEngine.Object>> m_DicPool = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadCallback = null;
    }
}