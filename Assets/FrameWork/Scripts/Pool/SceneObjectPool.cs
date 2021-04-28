using GameFrameWork.GameEntity;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace GameFrameWork.Pool
{
    public class SceneObjectPool : BaseMgr<SceneObjectPool>
    {    
        private void Awake()
        {
            m_PoolRoot = new GameObject("SceneObjectPool").transform;
            m_PoolRoot.SetParent(transform, false);
            m_PoolRoot.localPosition = new Vector3(9999, 9999, 9999);
            m_ListUsingObj = new List<BaseObject>();
            m_QueueUnUseObj = new Queue<BaseObject>();
        }

        public T Get<T>(string name = null,Transform parent = null) where T : BaseObject
        {
            T obj = null;

            if(m_QueueUnUseObj.Count >0)
            {
                obj = m_QueueUnUseObj.Dequeue() as T;
            }

            if(obj == null)
            {
                obj = new GameObject().GetOrAddComponent<T>();
                DontDestroyOnLoad(obj);
            }

            obj.Init(m_ListUsingObj.Count, name);
            obj.SetParent(parent, false);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
            m_ListUsingObj.Add(obj);
            return obj;
        }

        public void Put(BaseObject obj)
        {
            obj.SetActive(false);
            obj.transform.localPosition = Vector3.zero;
            obj.SetParent(m_PoolRoot, false);    
            m_QueueUnUseObj.Enqueue(obj);
            m_ListUsingObj.Remove(obj);
        }

        public T[] FindObjects<T>(string name = null) where T : BaseObject
        {
            List<T> ret = new List<T>();
            for (int i = 0; i < m_ListUsingObj.Count; i++)
            {
                if(m_ListUsingObj[i] is T)
                {
                    if(string.IsNullOrEmpty(name)|| m_ListUsingObj[i].name.Equals(name))
                    {
                        ret.Add(m_ListUsingObj[i] as T);
                    }
                }              
            }

            return ret.ToArray();
        }

        public T FindObject<T>(int id) where T : BaseObject
        {
            for (int i = 0; i < m_ListUsingObj.Count; i++)
            {
                if(m_ListUsingObj[i] is T && m_ListUsingObj[i].ID.Equals(id))

                {
                    return m_ListUsingObj[i] as T;
                }
            }

            return null;
        }

        protected override void OnShutDown()
        {
            m_ListUsingObj.Clear();
            m_QueueUnUseObj.Clear();
        }

        private Transform m_PoolRoot = null;
        private List<BaseObject> m_ListUsingObj = null;
        private Queue<BaseObject> m_QueueUnUseObj = null;
    }
}