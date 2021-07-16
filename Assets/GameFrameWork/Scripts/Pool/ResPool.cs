using GameFrameWork.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public abstract class ResPool<T, P> : BaseMgr<P> where T : UnityEngine.Object where P : ResPool<T, P>, new()
    {
        private void Awake()
        {
            m_PoolRoot = new GameObject("Res" + GetType().Name).transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(-9999f, -9999f, -9999f);
            m_DicPool = new Dictionary<string, Queue<T>>();
            m_DicLoadCallback = new Dictionary<string, List<GameFrameWorkAction<T, object[]>>>();
        }

        public virtual void Get(string resPath, GameFrameWorkAction<T, object[]> call, params object[] args)
        {
            if (string.IsNullOrEmpty(resPath) || call == null)
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

            List<GameFrameWorkAction<T, object[]>> loadList = null;

            if (!m_DicLoadCallback.TryGetValue(resPath, out loadList))
            {
                loadList = new List<GameFrameWorkAction<T, object[]>>();
                m_DicLoadCallback.Add(resPath, loadList);
            }

            loadList.Add(call);       
            ResMgr.Ins.LoadAssetAsync(resPath, OnLoaded, true, typeof(T), args);
        }

        public virtual void Put(string resPath, T go)
        {
            if (string.IsNullOrEmpty(resPath) || go == null) return;
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
            List<GameFrameWorkAction<T, object[]>> loadListCurr = null;

            if (!m_DicLoadCallback.TryGetValue(resPath, out loadListCurr))
            {
                Log.GameFrameworkLog.LogError("Resource load complete,but the callback is invalid.");
                return;
            }

            for (int i = 0; i < loadListCurr.Count; i++)
            {
                T go = m_NeedInstantiate ? UnityEngine.Object.Instantiate(obj) as T : obj as T;
                loadListCurr[i](go, args);
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
        protected Transform m_PoolRoot = null;
        private Dictionary<string, Queue<T>> m_DicPool = null;
        private Dictionary<string, List<GameFrameWorkAction<T, object[]>>> m_DicLoadCallback = null;
    }
}