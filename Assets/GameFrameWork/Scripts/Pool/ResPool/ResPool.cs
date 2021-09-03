using GameFrameWork.Resources;
using GameFrameWork.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public abstract class ResPool<T, P> : BaseMgr<P> where T : UnityEngine.Object where P : ResPool<T, P>, new()
    {
        class LoadRequest
        {
            public LoadRequest(GameFrameWorkAction<T, object[]> callback, object[] args)
            {
                m_Callback = callback;
                m_Args = args;
            }

            public void Call(T go)
            {
                m_Callback?.Invoke(go, m_Args);
            }

            private GameFrameWorkAction<T, object[]> m_Callback;
            private object[] m_Args;
        }

        protected override void OnAwake()
        {
            m_PoolRoot = new GameObject("Res" + GetType().Name).transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(-9999f, -9999f, -9999f);
            m_DicPool = new Dictionary<string, Queue<T>>();
            m_DicLoadCallback = new Dictionary<string, List<LoadRequest>>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Time.time - m_CollectTimer >= COLLECT_TIME)
            {
                m_CollectTimer = Time.time;
                UnityEngine.Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        public virtual void Get(string resPath, GameFrameWorkAction<T, object[]> call, params object[] args)
        {
            if (string.IsNullOrEmpty(resPath))
            {
                Log.GameFrameworkLog.LogError("Rescource param is invalid.");
                return;
            }

            Queue<T> pool = GetOrCreatePool(resPath);

            if (pool.Count > 0)
            {
                T go = pool.Dequeue();
                call(go, args);
                return;
            }

            List<LoadRequest> listLoadRequest = null;
            LoadRequest request = new LoadRequest(call, args);

            if (!m_DicLoadCallback.TryGetValue(resPath, out listLoadRequest))
            {
                listLoadRequest = new List<LoadRequest>();
                listLoadRequest.Add(request);
                m_DicLoadCallback.Add(resPath, listLoadRequest);
                ResMgr.Ins.LoadAssetAsync(resPath, OnLoaded, true, typeof(T));
            }
            else
            {
                listLoadRequest.Add(request);
            }
        }

        public virtual void Put(string resPath, T go)
        {
            if (string.IsNullOrEmpty(resPath) || go == null)
            {
                return;
            }

            Queue<T> pool = GetOrCreatePool(resPath);
            pool.Enqueue(go);
        }

        public int GetCount(string resPath)
        {
            Queue<T> pool = null;
            if (m_DicPool.TryGetValue(resPath, out pool))
            {
                return pool.Count;
            }
            return 0;
        }

        private void OnLoaded(string resPath, UnityEngine.Object obj, object[] args)
        {
            List<LoadRequest> listLoadRequest = null;

            if (!m_DicLoadCallback.TryGetValue(resPath, out listLoadRequest))
            {
                Log.GameFrameworkLog.LogError(TextUtil.FormatDefault("Resource [", resPath, "] load complete,but the callback is invalid."));
                return;
            }

            for (int i = 0; i < listLoadRequest.Count; i++)
            {
                T go = m_NeedInstantiate ? UnityEngine.Object.Instantiate(obj) as T : obj as T;
                listLoadRequest[i].Call(go);
            }

            m_DicLoadCallback.Remove(resPath);
        }

        public Queue<T> GetPool(string path)
        {
            return this.GetOrCreatePool(path);
        }

        private Queue<T> GetOrCreatePool(string path)
        {
            Queue<T> pool = null;
            if (!m_DicPool.TryGetValue(path, out pool))
            {
                pool = new Queue<T>();
                m_DicPool.Add(path, pool);
            }

            return pool;
        }

        protected override void OnShutDown()
        {
            m_DicPool.Clear();
            m_DicLoadCallback.Clear();
        }

        protected abstract bool m_NeedInstantiate { get; }

        public const int COLLECT_TIME = 15;
        private float m_CollectTimer = 0;
        protected Transform m_PoolRoot = null;
        private Dictionary<string, Queue<T>> m_DicPool = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadCallback = null;
    }
}