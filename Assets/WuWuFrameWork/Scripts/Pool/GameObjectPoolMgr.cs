using System;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Event;
using WuWuFramework.Resources;
using WuWuFramework.Utils;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Pool
{
    public class GameObjectPoolMgr : WuWuFrameworkModule, IGameObjectPoolMgr
    {
        private readonly List<string> m_ReleaseKeys;
        private readonly Dictionary<string, GameObjectPool> m_Pools;
        private readonly Dictionary<string, List<LoadRequest>> m_LoadRequests;
        private readonly List<GameObjectUnLoader> m_GameObjectUnloaders;
        private IResourcePoolMgr m_ResourcePoolMgr;
        private Transform m_PoolRoot;

        public GameObjectPoolMgr()
        {
            m_Pools = new();
            m_LoadRequests = new();
            m_ReleaseKeys = new();
            m_GameObjectUnloaders = new();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Shutdown()
        {
            foreach (KeyValuePair<string, GameObjectPool> kvp in m_Pools)
            {
                kvp.Value.Clear();
            }

            m_ReleaseKeys.Clear();
            m_Pools.Clear();
            m_LoadRequests.Clear();
            m_GameObjectUnloaders.Clear();
        }

        public void SetResourcePoolMgr(IResourcePoolMgr resourcePoolMgr)
        {
            m_ResourcePoolMgr = resourcePoolMgr;
            m_PoolRoot = new GameObject("GameObjectPool").transform;
            m_PoolRoot.SetParent(WuWuFrameworkEntry.gameEntryObj.transform, false);
            m_PoolRoot.localPosition = new Vector3(9999f, 9999f, 9999f);
        }

        /// <summary>
        /// 添加一个池
        /// </summary>
        public void AddPool(string tag, GameObject obj, int count = 1)
        {
            AddPool(tag, obj, count, false);
        }

        /// <summary>
        /// 删除一个池
        /// </summary>
        public void RemovePool(string tag)
        {
            if (m_Pools.TryGetValue(tag, out GameObjectPool pool))
            {
                pool.Clear();
                m_Pools.Remove(tag);
            }
        }

        /// <summary>
        /// 池是否存在
        /// </summary>
        public bool HasPool(string tag)
        {
            return m_Pools.ContainsKey(tag);
        }

        /// <summary>
        /// 生成一个预制物
        /// </summary>
        public GameObject Get(string tag, Transform parent, string layer, bool isActive = true)
        {
            if (m_Pools.TryGetValue(tag, out GameObjectPool pool))
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
        public void GetFromAsset(string assetPath, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null)
        {
            GameObject go = Get(assetPath, null, string.Empty);

            if (go != null)
            {
                loadedAction?.Invoke(assetPath, go, arg);
            }
            else
            {
                LoadRequest request = LoadRequest.Create(assetPath, null, loadedAction, arg);

                if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
                {
                    listLoadRequest = new List<LoadRequest>() { request };
                    m_LoadRequests.Add(assetPath, listLoadRequest);
                    m_ResourcePoolMgr.Get<GameObject>(assetPath, OnLoaded, arg);
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
            if (string.IsNullOrEmpty(tag))
            {
                throw new WuWuFrameworkException("资源路径为空，无法回收资源，检查资源是否加载成功");
            }

            if (m_Pools.TryGetValue(tag, out GameObjectPool pool))
            {
                m_GameObjectUnloaders.Clear();
                go.GetComponentsInChildren<GameObjectUnLoader>(true, m_GameObjectUnloaders);

                foreach (var gameObjectUnLoader in m_GameObjectUnloaders)
                {
                    if (gameObjectUnLoader.gameObject != go && !string.IsNullOrEmpty(gameObjectUnLoader.gameObjectPath))
                    {
                        gameObjectUnLoader.Release(this);
                    }
                }

                pool.Put(go, isReleaseImmdiately);
            }
        }

        public void CheckRelease()
        {
            m_ReleaseKeys.Clear();

            foreach (KeyValuePair<string, GameObjectPool> kvp in m_Pools)
            {
                kvp.Value.CheckRelease();

                if (kvp.Value.count < 1 && kvp.Value.isFromAsset && kvp.Value.usingCount < 1)
                {
                    m_ReleaseKeys.Add(kvp.Key);
                }
            }

            foreach (var releaseKey in m_ReleaseKeys)
            {
                m_Pools[releaseKey].Clear();
                m_Pools.Remove(releaseKey);
            }
        }

        private void AddPool(string tag, GameObject obj, int prefab, bool isFromAsset)
        {
            if (!m_Pools.TryGetValue(tag, out _))
            {
                GameObjectPool pool = new(m_ResourcePoolMgr, tag, m_PoolRoot, obj, isFromAsset);

                for (int i = 0; i < prefab; i++)
                {
                    pool.Cache();
                }

                m_Pools.Add(tag, pool);
            }
        }

        private void OnLoaded(string assetPath, UnityObject obj, object arg)
        {
            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> loadRequests))
            {
                throw new Exception(StringUtil.Append("[", assetPath, "] 资源加载完成 , 但回调函数不存在"));
            }

            AddPool(assetPath, obj as GameObject, 1, true);

            foreach (var loadRequest in loadRequests)
            {
                loadRequest.Loaded(Get(assetPath, null, string.Empty));
                loadRequest.Release();
            }

            m_LoadRequests.Remove(assetPath);
        }
    }
}