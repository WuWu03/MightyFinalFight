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

            m_DicPool = new Dictionary<string, GameObjectPool>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
            m_ListReleasePoolKey = new List<string>();
            m_ListUnloader = new List<GameObjectUnLoader>();
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnShutDown()
        {
            base.OnShutDown();

            foreach (KeyValuePair<string, GameObjectPool> kvp in m_DicPool)
            {
                kvp.Value.Clear();
            }

            m_DicLoadRequests.Clear();
            m_DicPool.Clear();
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
            if (m_DicPool.TryGetValue(tag, out GameObjectPool pool))
            {
                pool.Clear();
                m_DicPool.Remove(tag);
            }
        }

        /// <summary>
        /// 池是否存在
        /// </summary>
        public bool HasPool(string tag)
        {
            return m_DicPool.ContainsKey(tag);
        }

        /// <summary>
        /// 生成一个预制物
        /// </summary>
        public GameObject Get(string tag, Transform parent, string layer, bool isActive = true)
        {
            if (m_DicPool.TryGetValue(tag, out GameObjectPool pool))
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
                    AssetsPool.instance.Get<GameObject>(assetPath, OnLoaded);
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
            if (m_DicPool.TryGetValue(tag, out GameObjectPool pool))
            {
                m_ListUnloader.Clear();
                go.GetComponentsInChildren<GameObjectUnLoader>(true, m_ListUnloader);

                for (int i = 0; i < m_ListUnloader.Count; i++)
                {
                    GameObjectUnLoader gameObjectUnLoader = m_ListUnloader[i];
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

            m_ListReleasePoolKey.Clear();

            foreach (KeyValuePair<string, GameObjectPool> kvp in m_DicPool)
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

        private void AddPool(string tag, GameObject obj, int prefab, bool isFromAsset)
        {
            if (!m_DicPool.TryGetValue(tag, out GameObjectPool pool))
            {
                pool = new GameObjectPool(tag, m_PoolRoot, obj, isFromAsset);

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
                Log.LogError(StringUtil.Append("[", assetPath, "] 资源加载完成 , 但回调函数不存在"));
                return;
            }

            AddPool(assetPath, obj as GameObject, 1, true);

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                listLoadRequest[i].Call(Get(assetPath, null, string.Empty));
                ReferencePool.ReleaseReference(listLoadRequest[i]);
            }

            m_DicLoadRequests.Remove(assetPath);
        }

        private List<string> m_ListReleasePoolKey = null;
        private Transform m_PoolRoot = null;
        private Dictionary<string, GameObjectPool> m_DicPool = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
        private List<GameObjectUnLoader> m_ListUnloader = null;
    }
}