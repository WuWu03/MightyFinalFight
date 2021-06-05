using GameFrameWork.Serialize;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class BehaviourTreeWindowNode
    {
        public Rect Rect
        {
            get
            {
                if (m_Data == null)
                    return Rect.zero;

                return m_Data.WindowRect;
            }
        }

        public BehaviourTreeWindowNode Parent
        {
            get
            {
                return m_Parent;
            }
        }

        public List<BehaviourTreeWindowNode> Children
        {
            get
            {
                return m_Children;
            }
        }

        public BehaviourTreeWindowNode(BehaviourTreeWindowData data,BehaviourTreeWindowNode parent = null)
        {
            UpdateData(data, parent);
        }

        public void UpdateData(BehaviourTreeWindowData data,BehaviourTreeWindowNode parent = null)
        {
            m_Data = data;
            m_Parent = parent;
            if (m_Data != null)
            {
                if (m_Children == null)
                    m_Children = new List<BehaviourTreeWindowNode>();
                else
                    m_Children.Clear();

                for (int i = 0; i < m_Data.Children.Count; i++)
                {
                    m_Children.Add(new BehaviourTreeWindowNode(m_Data.Children[i], this));
                }
            }
        }


        public void OnGUI(UnityEngine.Event e)
        {
            if (m_Data != null)
            {
                m_Data.WindowRect = GUI.Window(m_Data.ID, m_Data.WindowRect, DrawNodeWindow, m_Data.Name);

                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].OnGUI(e);
                }
            }
        }

        private void DrawNodeWindow(int id)
        {
            GUI.DragWindow();
            DrawCurve();
        }


        private void DrawCurve()
        {
            if (m_Children != null && m_Children.Count > 0)
            {
                for (int i = 0; i < m_Children.Count; i++)
                {
                    EditorUtility.DrawCurve(Rect, m_Children[i].Rect, Color.yellow);
                }
            }
        }

        private BehaviourTreeWindowNode m_Parent = null;
        private List<BehaviourTreeWindowNode> m_Children = null;
        private BehaviourTreeWindowData m_Data = null;
    }
}