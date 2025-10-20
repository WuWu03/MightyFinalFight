using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class RedDotMgr : GameFrameWorkModule,IRedDotMgr
    {
        private readonly List<RedDot> m_RedDots;
        public RedDotMgr()
        {
            m_RedDots = new List<RedDot>();
        }

        public override void Shutdown()
        {
            for (int i = m_RedDots.Count - 1; i >= 0; i--)
            {
                m_RedDots[i].Dispose();
            }

            m_RedDots.Clear();
        }
        
        public void Add(string key, string subKey, string parentKey, RedPointType type)
        {
            RedDot root = GetRoot(key);

            if (string.IsNullOrEmpty(subKey) || key.Equals(subKey))
            {
                if (root != null)
                {
                    Log.LogError("根节点 [" + key + "] 已经存在 , 请不要重复添加");
                    return;
                }

                root = new RedDot(key, key, true, type);
                m_RedDots.Add(root);
            }
            else
            {
                if (root == null)
                {
                    Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                    return;
                }

                RedDot node = new RedDot(key, subKey, false, type);
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
                for (int i = m_RedDots.Count - 1; i >= 0; i--)
                {
                    if (m_RedDots[i].key.Equals(key))
                    {
                        m_RedDots[i].Dispose();
                        m_RedDots.RemoveAt(i);
                        return;
                    }
                }

                return;
            }

            RedDot root = GetRoot(key);

            if (root == null)
            {
                return;
            }

            root.RemoveChild(subKey);
        }

        public void Init(string key, string subKey, GameFrameWorkAction<RedPointState, int> showEvent, Button btn = null)
        {
            RedDot root = GetRoot(key);

            if (root == null)
            {
                Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                return;
            }

            RedDot node = root.GetChild(subKey);

            if (node == null)
            {
                Log.LogError("节点 [" + subKey + "] 不存在 , 请先添加对应节点");
                return;
            }

            node.Init(showEvent, btn);
        }

        public void SetState(string key, string subKey, RedPointState state, int data = 0)
        {
            RedDot root = GetRoot(key);

            if (root == null)
            {
                Log.LogError("根节点  [" + key + "] 不存在 , 请先添加根节点");
                return;
            }

            root.SetState(subKey, state, data);
        }

        private RedDot GetRoot(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            foreach (var redDot in m_RedDots)
            {
                if (redDot.key.Equals(key))
                {
                    return redDot;
                }
            }

            return null;
        }
    }
}