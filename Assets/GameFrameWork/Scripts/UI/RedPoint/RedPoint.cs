using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public enum RedPointType
    {
        None,
        Enternal,//一直存在
        Once,//点击一次就消失
    }

    public enum RedPointState
    {
        None,
        Show,
        Hide,
    }

    public class RedPoint
    {
        public string Key
        {
            get
            {
                return m_Key;
            }
        }

        public string SubKey
        {
            get
            {
                return m_SubKey;
            }
        }

        public bool IsRoot
        {
            get
            {
                return m_IsRoot;
            }
        }

        public RedPointType Type
        {
            get
            {
                return m_Type;
            }
        }

        public RedPointState State
        {
            get
            {
                return m_State;
            }
        }

        public int Data
        {
            get
            {
                return m_Data;
            }
        }

        public RedPoint Parent
        {
            get
            {
                return m_Parent;
            }
        }

        public List<RedPoint> Children
        {
            get
            {
                return m_Children;
            }
        }


        public RedPoint(string key, string subKey,bool isRoot, RedPointType type)
        {
            m_Key = key;
            m_SubKey = subKey;
            m_IsRoot = isRoot;
            m_Type = type;
            m_State = RedPointState.Hide;
            m_Data = 0;
            m_Children = new List<RedPoint>();
        }

        public void Init(GameFrameWorkAction<RedPointState, int> onShow, Button btn)
        {
            m_OnShow = onShow;

            if (btn != null)
            {
                m_Btn = btn;
                m_Btn.onClick.AddListener(OnClick);
            }

            m_OnShow?.Invoke(m_State, m_Data);
        }

        public void AddChild(RedPoint node, string parentKey)
        {
            if (m_SubKey.Equals(parentKey))
            {
                node.SetParent(this);
                m_Children.Add(node);
                return;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].AddChild(node, parentKey);
            }
        }

        public RedPoint GetChild(string subKey)
        {
            if(m_SubKey.Equals(subKey))
            {
                return this;
            }

            if(m_Children.Count < 1)
            {
                return null;
            }

            for (int i = 0; i < m_Children.Count; i++)
            {
                RedPoint node = m_Children[i].GetChild(subKey);
                if (node != null) return node;
            }

            return null;
        }

        public void SetParent(RedPoint parent)
        {
            m_Parent = parent;
        }

        public void SetTreeState(string subKey, RedPointState state, int data)
        {
            RedPoint node = GetChild(subKey);
            if (node == null)
            {
                return;
            }

            m_Data = 0;
            node.SetState(subKey, state, data);

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Data += m_Children[i].Data;
            }

            m_OnShow?.Invoke(m_State, m_Data);
        }

        private void SetState(string subKey, RedPointState state, int data)
        {
            m_State = state;

            if (m_SubKey.Equals(subKey))
            {
                m_Data = data;
            }
            else
            {
                for (int i = 0; i < m_Children.Count; i++)
                {
                    if (m_Children[i].State == RedPointState.Show)
                    {
                        m_State = RedPointState.Show;
                        break;
                    }
                }
            }
            
            m_OnShow?.Invoke(m_State, m_Data);

            if (m_Parent != null)
            {
                m_Parent.SetState(subKey, state, data);
            }
        }

        private void OnClick()
        {
            if(m_Type == RedPointType.Once)
            {
                HideChildren();
                SetState(m_SubKey, RedPointState.Hide, m_Data);
            }
        }

        private void HideChildren()
        {
            m_State = RedPointState.Hide;
            m_OnShow?.Invoke(m_State, m_Data);
            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].HideChildren();
            }
        }

        public void Dispose()
        {
            m_Children.Clear();
            m_Children = null;
            m_Parent = null;
            m_Key = null;
            m_SubKey = null;
            m_OnShow = null;
            m_Btn = null;
            m_Type = RedPointType.None;
            m_State = RedPointState.None;
        }

        private string m_Key = string.Empty;
        private string m_SubKey = string.Empty;
        private bool m_IsRoot = false;
        private RedPointType m_Type = RedPointType.None;
        private RedPointState m_State = RedPointState.None;
        private GameFrameWorkAction<RedPointState,int> m_OnShow = null;
        private Button m_Btn;
        private int m_Data = 0;
        private RedPoint m_Parent = null;
        private List<RedPoint> m_Children = null;
    }
}