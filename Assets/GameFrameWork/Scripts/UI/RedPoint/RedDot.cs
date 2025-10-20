using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class RedDot
    {
        private readonly string m_Key;
        private readonly string m_SubKey;
        private readonly bool m_IsRoot;
        private readonly List<RedDot> m_Children;
        private int m_Data = 0;
        private RedPointType m_Type;
        private RedPointState m_State;
        private GameFrameWorkAction<RedPointState, int> m_ShowEvent;
        private Button m_Btn;
        private RedDot m_Parent;
        
        public RedDot(string key, string subKey, bool isRoot, RedPointType type)
        {
            m_Key = key;
            m_SubKey = subKey;
            m_IsRoot = isRoot;
            m_Type = type;
            m_State = RedPointState.Hide;
            m_Data = 0;
            m_Children = new List<RedDot>();
        }
        
        /// <summary>
        /// 主关键字(属于哪一个根节点)
        /// </summary>
        public string key
        {
            get
            {
                return m_Key;
            }
        }

        /// <summary>
        /// 自己的关键字
        /// </summary>
        public string subKey
        {
            get
            {
                return m_SubKey;
            }
        }

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public bool isRoot
        {
            get
            {
                return m_IsRoot;
            }
        }

        /// <summary>
        /// 红点类型
        /// </summary>
        public RedPointType type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public RedPointState state
        {
            get
            {
                return m_State;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        public int data
        {
            get
            {
                return m_Data;
            }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public RedDot parent
        {
            get
            {
                return m_Parent;
            }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<RedDot> children
        {
            get
            {
                return m_Children;
            }
        }
        
        public void Init(GameFrameWorkAction<RedPointState, int> showEvent, Button btn)
        {
            m_ShowEvent = showEvent;

            if (btn is not null)
            {
                m_Btn = btn;
                m_Btn.onClick.AddListener(OnClick);
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        public bool AddChild(RedDot node, string parentKey)
        {
            if (m_SubKey.Equals(parentKey))
            {
                node.SetParent(this);
                m_Children.Add(node);
                return true;
            }

            foreach (var child in m_Children)
            {
                if (child.AddChild(node, parentKey))
                {
                    break;
                }
            }

            return false;
        }

        public RedDot GetChild(string subKey)
        {
            if (m_SubKey.Equals(subKey))
            {
                return this;
            }

            if (m_Children == null)
            {
                return null;
            }

            foreach (var child in m_Children)
            {
                RedDot node = child.GetChild(subKey);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public void RemoveChild(string subKey)
        {
            if (string.IsNullOrEmpty(subKey))
            {
                return;
            }

            if(m_SubKey.Equals(subKey))
            {
                m_Parent.children.Remove(this);
                Dispose();
                return;
            }

            if (m_Children == null)
            {
                return;
            }

            foreach (var child in m_Children)
            {
                child.RemoveChild(subKey);
            }
        }

        public void SetParent(RedDot parent)
        {
            m_Parent = parent;
        }

        public void SetState(string subKey, RedPointState state, int data)
        {
            RedDot node = GetChild(subKey);

            if (node == null)
            {
                return;
            }

            node.SetTreeState(subKey, state, data);
            m_Data = 0;

            foreach (var child in m_Children)
            {
                m_Data += child.m_Data;
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        private void SetTreeState(string subKey, RedPointState state, int data)
        {
            m_State = state;

            if (m_SubKey.Equals(subKey))
            {
                m_Data = data;
            }
            else
            {
                m_Data = 0;

                foreach (var child in m_Children)
                {
                    if (child.state == RedPointState.Show)
                    {
                        m_State = RedPointState.Show;
                        m_Data += child.data;
                    }
                }
            }

            m_Parent?.SetTreeState(subKey, state, data);
            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        private void OnClick()
        {
            if (m_Type == RedPointType.Once)
            {
                HideChildren();
                SetState(m_SubKey, RedPointState.Hide, m_Data);
            }
        }

        private void HideChildren()
        {
            m_State = RedPointState.Hide;
 
            foreach (var child in m_Children)
            {
                child.HideChildren();
            }

            m_ShowEvent?.Invoke(m_State, m_Data);
        }

        public void Dispose()
        {
            foreach (var child in m_Children)
            {
                child.Dispose();
            }

            m_Children.Clear();
            
            if (m_Btn is not null)
            {
                m_Btn.onClick.RemoveListener(OnClick);
            }

            m_Btn = null;
            m_Parent = null;
            m_ShowEvent = null;
            m_Type = RedPointType.None;
            m_State = RedPointState.None;
        }
    }
}