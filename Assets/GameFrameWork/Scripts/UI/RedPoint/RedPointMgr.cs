using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class RedPointMgr : BaseMgr<RedPointMgr>
    {
        protected override void OnAwake()
        {
            m_ListRedPointTrees = new List<RedPoint>();
        }

        public void Add(string key, string subKey, string parentKey, RedPointType type)
        {
            RedPoint root = GetRoot(key);

            if (string.IsNullOrEmpty(subKey) || key.Equals(subKey))
            {
                if (root != null)
                {
                    Log.LogError("根节点 [" + key + "] 已经存在 , 请不要重复添加");
                    return;
                }

                root = new RedPoint(key, key, true, type);
                m_ListRedPointTrees.Add(root);
            }
            else
            {
                if (root == null)
                {
                    Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                    return;
                }

                RedPoint node = new RedPoint(key, subKey, false, type);
                root.AddChild(node, parentKey);
            }
        }

        public void Remove(string key, string subKey)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (string.IsNullOrEmpty(subKey) || key.Equals(subKey))
            {
                for (int i = m_ListRedPointTrees.Count - 1; i >= 0; i--)
                {
                    if (m_ListRedPointTrees[i].key.Equals(key))
                    {
                        m_ListRedPointTrees[i].Dispose();
                        m_ListRedPointTrees.RemoveAt(i);
                        return;
                    }
                }

                return;
            }

            RedPoint root = GetRoot(key);

            if (root == null)
            {
                return;
            }

            root.RemoveChild(subKey);
        }

        public void Init(string key, string subKey, GameFrameWorkAction<RedPointState, int> showEvent, Button btn = null)
        {
            RedPoint root = GetRoot(key);

            if (root == null)
            {
                Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                return;
            }

            RedPoint node = root.GetChild(subKey);

            if (node == null)
            {
                Log.LogError("节点 [" + subKey + "] 不存在 , 请先添加对应节点");
                return;
            }

            node.Init(showEvent, btn);
        }

        public void SetState(string key, string subKey, RedPointState state, int data = 0)
        {
            RedPoint root = GetRoot(key);

            if (root == null)
            {
                Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                return;
            }

            root.SetState(subKey, state, data);
        }

        private RedPoint GetRoot(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            for (int i = 0; i < m_ListRedPointTrees.Count; i++)
            {
                if (m_ListRedPointTrees[i].key.Equals(key))
                {
                    return m_ListRedPointTrees[i];
                }
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