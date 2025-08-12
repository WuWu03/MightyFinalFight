using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class GameObjectPoolMgr : BaseMgr<GameObjectPoolMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
           
            m_PoolRoot = new GameObject("GameObjectPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999f, 9999f, 9999f);

            m_Pools = new();
            m_LoadRequests = new();
            m_ReleaseKeys = new();
            m_Unloaders = new();
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (KeyValuePair<string, GameObjectPool> kvp in m_Pools)
            {
                kvp.Value.Clear();
            }

            m_ReleaseKeys.Clear();
            m_Pools.Clear();
            m_LoadRequests.Clear();
            m_Unloaders.Clear();
        }

        protected override void OnDestory()
        {
            base.OnDestory();

            m_ReleaseKeys = null;
            m_Pools = null;
            m_LoadRequests = null;
            m_Unloaders = null;
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
        public void GetFromAsset(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction, object arg = null)
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
                    AssetsPool.instance.Get<GameObject>(assetPath, OnLoaded, arg);
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
            if (m_Pools.TryGetValue(tag, out GameObjectPool pool))
            {
                m_Unloaders.Clear();
                go.GetComponentsInChildren<GameObjectUnLoader>(true, m_Unloaders);

                for (int i = 0; i < m_Unloaders.Count; i++)
                {
                    GameObjectUnLoader gameObjectUnLoader = m_Unloaders[i];
                    if (gameObjectUnLoader.gameObject != go && !string.IsNullOrEmpty(gameObjectUnLoader.gameObjectPath))
                    {
                        gameObjectUnLoader.Release();
                    }
                }

                pool.Put(go, isReleaseImmdiately);
            }
        }

        public void CheckRelease()
        {
            base.OnUpdate();

            m_ReleaseKeys.Clear();

            foreach (KeyValuePair<string, GameObjectPool> kvp in m_Pools)
            {
                kvp.Value.CheckRelease();

                if (kvp.Value.count < 1 && kvp.Value.isFromAsset && kvp.Value.usingCount < 1)
                {
                    m_ReleaseKeys.Add(kvp.Key);
                }
            }

            for (int i = 0; i < m_ReleaseKeys.Count; i++)
            {
                m_Pools[m_ReleaseKeys[i]].Clear();
                m_Pools.Remove(m_ReleaseKeys[i]);
            }
        }

        private void AddPool(string tag, GameObject obj, int prefab, bool isFromAsset)
        {
            if (!m_Pools.TryGetValue(tag, out _))
            {
                GameObjectPool pool = new(tag, m_PoolRoot, obj, isFromAsset);

                for (int i = 0; i < prefab; i++)
                {
                    pool.Cache();
                }

                m_Pools.Add(tag, pool);
            }
        }

        private void OnLoaded(string assetPath, UnityEngine.Object obj, object arg)
        {
            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> listLoadRequest))
            {
                Log.LogError(StringUtil.Append("[", assetPath, "] 资源加载完成 , 但回调函数不存在"));
                return;
            }

            AddPool(assetPath, obj as GameObject, 1, true);

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                listLoadRequest[i].Loaded(Get(assetPath, null, string.Empty));
                listLoadRequest[i].Release();
            }

            m_LoadRequests.Remove(assetPath);
        }

        private List<string> m_ReleaseKeys = null;
        private Transform m_PoolRoot = null;
        private Dictionary<string, GameObjectPool> m_Pools = null;
        private Dictionary<string, List<LoadRequest>> m_LoadRequests = null;
        private List<GameObjectUnLoader> m_Unloaders = null;
    }
}