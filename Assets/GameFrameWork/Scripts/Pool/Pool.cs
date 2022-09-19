using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class Pool
    {
        public List<GameObject> poolList
        {
            get
            {
                return m_ListPool;
            }
        }


        public string tag
        {
            get
            {
                return m_Tag;
            }
        }

        public Pool(string tag, Transform parent, GameObject obj)
        {
            GameObject go = new GameObject(tag);
            go.transform.SetParent(parent, false);

            m_CachePool = go.transform;
            m_Obj = obj;
            m_Tag = tag;
            m_Parent = parent;

            m_ListPool = new List<GameObject>();
        }

        public GameObject Spawn(bool isActive = true)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                if (!poolList[i].activeSelf)
                {
                    poolList[i].SetActive(isActive);
                    return poolList[i];
                }
            }

            GameObject go = GameObject.Instantiate(m_Obj, m_CachePool, false);
            go.SetActive(isActive);
            poolList.Add(go);

            return go;
        }

        public void UnSpawn(GameObject go)
        {
            if (go != null)
            {
                go.SetActive(false);
                go.transform.SetParent(m_Parent, false);
            }
        }

        public void UnSpawnAll()
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                poolList[i].SetActive(false);
            }
        }

        public void Clear()
        {
            for (int i = poolList.Count - 1; i > 0; i--)
            {
                GameObject go = poolList[i];

                if (go != null)
                {
                    poolList.RemoveAt(i);
                    GameObject.Destroy(go);
                }
            }
        }

        private GameObject m_Obj = null;
        private Transform m_Parent = null;
        private Transform m_CachePool = null;
        private List<GameObject> m_ListPool = null;
        private string m_Tag = string.Empty;
    }
}