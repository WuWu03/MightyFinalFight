using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class RedPointMgr : BaseMgr<RedPointMgr>
    {
        protected override void OnAwake()
        {
            m_ListRedPointTrees = new List<RedPoint>();
        }

        public void Regist(string key, string subKey, string parentKey, RedPointType type)
        {
            RedPoint root = GetRedPointTree(key);

            if (string.IsNullOrEmpty(subKey) && string.IsNullOrEmpty(parentKey))
            {
                if (root == null)
                    root = new RedPoint(key, key, true, type);
                m_ListRedPointTrees.Add(root);
            }
            else
            {
                if (root == null)
                {
                    Log.Debugger.LogError("The red point root [" + key + "] is invalid,please regist first");
                    return;
                }

                RedPoint node = new RedPoint(key, subKey, false, type);
                root.AddChild(node, parentKey);
            }
        }

        public void InitPoint(string key, string subKey, GameFrameWorkAction<RedPointState, int> onShow, Button btn = null)
        {
            RedPoint root = GetRedPointTree(key);

            if (root == null)
            {
                Log.Debugger.LogError("The red point root [" + key + "] is invalid,please regist first");
                return;
            }

            RedPoint node = root.GetChild(subKey);

            if(node == null)
            {
                Log.Debugger.LogError("The red point node [" + subKey + "] is invalid,please regist first");
                return;
            }

            node.Init(onShow, btn);
        }

        public void SetPointState(string key, string subKey, RedPointState state, int data = 0)
        {
            RedPoint root = GetRedPointTree(key);

            if (root == null)
            {
                Log.Debugger.LogError("The red point root [" + key + "] is invalid,please regist first");
                return;
            }

            root.SetTreeState(subKey, state, data);
        }

        public void Remove(string key)
        {
            for (int i = m_ListRedPointTrees.Count - 1; i >= 0; i--)
            {
                if(m_ListRedPointTrees[i].Key.Equals(key))
                {
                    m_ListRedPointTrees[i].Dispose();
                    m_ListRedPointTrees.RemoveAt(i);
                    return;
                }
            }
        }

        public RedPoint GetRedPointTree(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            for (int i = 0; i < m_ListRedPointTrees.Count; i++)
            {
                if (m_ListRedPointTrees[i].Key.Equals(key))
                    return m_ListRedPointTrees[i];
            }

            return null;
        }

        protected override void OnShutDown()
        {
            for (int i = m_ListRedPointTrees.Count - 1; i >= 0; i--)
            {
                m_ListRedPointTrees[i].Dispose();
            }
            m_ListRedPointTrees.Clear();
        }

        private List<RedPoint> m_ListRedPointTrees = null;
    }
}