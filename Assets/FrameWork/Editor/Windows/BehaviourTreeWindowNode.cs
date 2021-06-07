using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class BehaviourTreeWindowNode
    {
        public bool IsParent
        {
            get
            {
                return m_IsParent;
            }
        }

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

        
        public BehaviourTreeWindowNode(BehaviourTreeWindowData data,bool isParent,BehaviourTreeWindowNode parent = null)
        {
            InitCompositesName();
            InitPreConditionName();
            UpdateData(data, isParent, parent);
        }

        public void UpdateData(BehaviourTreeWindowData data, bool isParent, BehaviourTreeWindowNode parent = null)
        {
            m_Data = data;
            m_Parent = parent;
            m_IsParent = isParent;

            if (m_Data != null)
            {
                if (m_Children == null)
                    m_Children = new List<BehaviourTreeWindowNode>();
                else
                    m_Children.Clear();

                string[] assembly = isParent ? m_ParentCompositesNames : m_CompositesNames;
                for (int i = 0; i < assembly.Length; i++)
                {
                    if (assembly[i].Equals(m_Data.ClassType))
                    {
                        m_CurrSelect = i;
                        break;
                    }
                }

                for (int i = 0; i < m_Data.Children.Count; i++)
                {
                    m_Children.Add(new BehaviourTreeWindowNode(m_Data.Children[i],false, this));
                }
            }
        }

        public void SetParent(BehaviourTreeWindowNode parent)
        {
            if(m_Parent != null)
            {
                m_Parent.RemoveChild(this);
            }

            parent.AddChild(this);
            m_Parent = parent;
        }

        public void AddChild(BehaviourTreeWindowNode node)
        {
            node.m_Data.ID = m_Data.ID * 100 + m_Children.Count + 1;
            m_Children.Add(node);
            m_Data.Children.Add(node.m_Data);
        }

        public void RemoveChild(BehaviourTreeWindowNode node)
        {
            m_Data.Children.Remove(node.m_Data);
            m_Children.Remove(node);
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

            DrawCurve();
        }

        private void DrawNodeWindow(int id)
        {
            EditorUtility.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("节点类型");
                m_CurrSelect = EditorGUILayout.Popup(m_CurrSelect, m_IsParent ? m_ParentCompositesNames : m_CompositesNames);
                m_Data.ClassType = m_CompositesNames[m_CurrSelect];
                EditorGUILayout.EndVertical();
            });
    
           
            GUI.DragWindow();
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

        private static void InitCompositesName()
        {
            if (m_CompositesNames != null) return;
            m_CompositesNames = GetAssembly("GameFrameWork.BehaviourTree.Composites", "Action");
        }

        private static void InitPreConditionName()
        {
            if (m_PreConditionNames != null) return;
            m_PreConditionNames = GetAssembly("GameFrameWork.BehaviourTree.PreCondition", "PreCondition");
        }

        private static string[] GetAssembly(string typeName,params string[] parttern)
        {
            Assembly assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            Type baseType = assembly.GetType(typeName);
            List<string> list = new List<string>();
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                Type temp = type;
                while (temp.BaseType != null)
                {
                    temp = temp.BaseType;
                    if (temp.Name.Equals(baseType.Name))
                    {
                        bool isParttern = false;

                        for (int i = 0; i < parttern.Length; i++)
                        {
                            if (parttern[i].Equals(type.Name))
                            {
                                isParttern = true;
                                break;
                            }
                        }

                        if (!isParttern)
                        {
                            list.Add(type.Name);
                            break;
                        }
                    }
                }
            }

            return list.ToArray();
        }

        private static string[] m_ParentCompositesNames = new string[] { "Sequence", "LoopSequence", "Selector", "LoopSelector" };
        private static string[] m_CompositesNames = null;
        private static string[] m_PreConditionNames = null;

        private int m_CurrSelect = 0;
        private bool m_IsParent = false;
        private BehaviourTreeWindowNode m_Parent = null;
        private List<BehaviourTreeWindowNode> m_Children = null;
        private BehaviourTreeWindowData m_Data = null;
    }
}