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
    public class ResPool : BaseMgr<ResPool>
    {
        private void Awake()
        {
            m_PoolRoot = new GameObject("ResPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(-9999f, -9999f, -9999f);
            m_DicPool = new Dictionary<string, Queue<GameObject>>();
            m_DicLoadCallback = new Dictionary<string, List<Action<GameObject>>>();
        }

        public void Get(string resPath, Action<GameObject> call)
        {
            if (string.IsNullOrEmpty(resPath) || call == null) return;
            Queue<GameObject> pool = this.GetOrCreatePool(resPath);
            if (pool.Count > 0)
            {
                GameObject go = pool.Dequeue();
                call(go);
            }
            else
            {
                List<Action<GameObject>> loadList = null;
                if (!m_DicLoadCallback.TryGetValue(resPath, out loadList))
                {
                    loadList = new List<Action<GameObject>>();
                }

                loadList.Add(call);
                m_DicLoadCallback[resPath] = loadList;
                ResMgr.Ins.LoadAsset(resPath, (UnityEngine.Object obj) =>
                {
                    List<Action<GameObject>> loadListCurr = null;
                    if (m_DicLoadCallback.TryGetValue(resPath, out loadListCurr))
                    {
                        for (int i = 0; i < loadListCurr.Count; i++)
                        {
                            GameObject go = GameObject.Instantiate(obj) as GameObject;
                            loadListCurr[i](go);
                        }

                        m_DicLoadCallback.Remove(resPath);
                    }
                });
            }
        }

        public void Put(string resPath, GameObject go)
        {
            if (string.IsNullOrEmpty(resPath) || go == null) return;
            Queue<GameObject> pool = GetOrCreatePool(resPath);
            go.SetActive(false);
            go.transform.SetParent(m_PoolRoot, false);
            go.transform.localPosition = Vector3.zero;
            pool.Enqueue(go);
        }

        public int GetCount(string resPath)
        {
            Queue<GameObject> pool = null;
            if (m_DicPool.TryGetValue(resPath, out pool))
            {
                return pool.Count;
            }
            return 0;
        }

        public Queue<GameObject> GetPool(string path)
        {
            return this.GetOrCreatePool(path);
        }

        private Queue<GameObject> GetOrCreatePool(string path)
        {
            Queue<GameObject> pool = null;
            if (!m_DicPool.TryGetValue(path, out pool))
            {
                pool = new Queue<GameObject>();
                m_DicPool.Add(path, pool);
            }

            return pool;
        }

        private void OnDestroy()
        {
            m_DicPool.Clear();
            m_DicLoadCallback.Clear();
        }

        public override void ShutDown()
        {
            m_DicPool.Clear();
            m_DicLoadCallback.Clear();
        }

        private Transform m_PoolRoot = null;
        private Dictionary<string, Queue<GameObject>> m_DicPool = null;
        private Dictionary<string, List<Action<GameObject>>> m_DicLoadCallback = null;
    }
}