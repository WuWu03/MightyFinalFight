using FrameWork.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Pool
{
    public abstract class ResPool<T,P> : BaseMgr<P> where T:UnityEngine.Object
                                                    where P:ResPool<T,P>,new()
    {
        private void Awake()
        {
            m_PoolRoot = new GameObject("ResPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(-9999f, -9999f, -9999f);
            m_DicPool = new Dictionary<string, Queue<T>>();
            m_DicLoadCallback = new Dictionary<string, List<Action<T>>>();
        }

        public virtual void Get(string resPath, Action<T> call)
        {
            if (string.IsNullOrEmpty(resPath) || call == null) return;
            Queue<T> pool = this.GetOrCreatePool(resPath);
            if (pool.Count > 0)
            {
                T go = pool.Dequeue();
                call(go);
            }
            else
            {
                List<Action<T>> loadList = null;
                if (!m_DicLoadCallback.TryGetValue(resPath, out loadList))
                {
                    loadList = new List<Action<T>>();
                }

                loadList.Add(call);
                m_DicLoadCallback[resPath] = loadList;
                ResMgr.Ins.LoadAsset(resPath, (UnityEngine.Object obj) =>
                {
                    List<Action<T>> loadListCurr = null;
                    if (m_DicLoadCallback.TryGetValue(resPath, out loadListCurr))
                    {
                        for (int i = 0; i < loadListCurr.Count; i++)
                        {
                            T go = NeedInstantiate ? UnityEngine.Object.Instantiate(obj) as T : obj as T;
                            loadListCurr[i](go);
                        }

                        m_DicLoadCallback.Remove(resPath);
                    }
                }, true, typeof(T));
            }
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

        public override void ShutDown()
        {
            m_DicPool.Clear();
            m_DicLoadCallback.Clear();
        }

        protected abstract bool NeedInstantiate { get; }
        protected Transform m_PoolRoot = null;
        private Dictionary<string, Queue<T>> m_DicPool = null;
        private Dictionary<string, List<Action<T>>> m_DicLoadCallback = null;
    }

    public class GameObjectPool : ResPool<GameObject, GameObjectPool> 
    {
        protected override bool NeedInstantiate { get { return true; } }

        public override void Put(string resPath, GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(m_PoolRoot, false);
            go.transform.localPosition = Vector3.zero;
            base.Put(resPath, go);
        }
    }

    public class AudioClipPool : ResPool<AudioClip, AudioClipPool> 
    {
        protected override bool NeedInstantiate { get { return true; } }
    }

    public class SpritePool : ResPool<Sprite, SpritePool> 
    {
        protected override bool NeedInstantiate { get { return false; } }
    }
}