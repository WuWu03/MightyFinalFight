using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
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

            if (!string.IsNullOrEmpty(m_Data.ClassType))
            {
                string[] names = m_IsParent ? m_ParentCompositesNames : m_CompositesNames;
                for (int i = 0; i < names.Length; i++)
                {
                    if(m_Data.ClassType == names[i])
                    {
                        m_CurrSelectComposite = i;
                        break;
                    }
                }
            }

            if(m_Data.PreConditions != null && m_Data.PreConditions.Count > 0)
            {
                for (int i = 0; i < m_Data.PreConditions.Count; i++)
                {
                    if(string.IsNullOrEmpty(m_Data.PreConditions[i].ClassType))
                    {
                        continue;
                    }

                    for (int j = 0; j < m_PreConditionNames.Length; j++)
                    {
                        if(m_Data.PreConditions[i].ClassType == m_PreConditionNames[j])
                        {
                            m_Data.PreConditions[i].SelectIndex = j;
                            break;
                        }
                    }
                }
            }

            m_PreConditionList.elementHeight = 50;
            m_PreConditionList.drawHeaderCallback = (Rect rect) =>
             {
                 GUI.Label(rect, "PreConditions");
             };

            m_PreConditionList.onAddCallback = (ReorderableList list) =>
            {
                m_Data.PreConditions.Add(new BehaviourTreeWindowPreConditon());
                m_Data.WindowRect.height += m_Data.PreConditions.Count > 1 ? 52 : 0;
                list.list = m_Data.PreConditions;
            };

            m_PreConditionList.onRemoveCallback = (ReorderableList list) =>
            {
                m_Data.PreConditions.RemoveAt(list.index);
                m_Data.WindowRect.height -= m_Data.PreConditions.Count > 0 ? 52 : 0;
                list.list = m_Data.PreConditions;
            };

            m_PreConditionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (m_Data.PreConditions.Count < 1) return;
                BehaviourTreeWindowPreConditon preConditon = m_Data.PreConditions[index];
                preConditon.SelectIndex = EditorGUI.Popup(new Rect(rect.x, rect.y + 5, rect.width, 20), preConditon.SelectIndex, m_PreConditionNames);
                preConditon.ClassType = m_PreConditionNames[preConditon.SelectIndex];
                preConditon.Args = EditorGUI.TextField(new Rect(rect.x, rect.y + 25, rect.width, 20), preConditon.Args);
            };

            m_PreConditionList.onReorderCallback = (ReorderableList list) =>
            {
                m_Data.PreConditions = list.list as List<BehaviourTreeWindowPreConditon>;
            };
        }

        public void UpdateData(BehaviourTreeWindowData data, bool isParent, BehaviourTreeWindowNode parent = null)
        {
            m_Data = data;
            m_Parent = parent;
            m_IsParent = isParent;

            if (m_Data != null)
            {
                if (m_PreConditionList == null)
                {
                    m_PreConditionList = new ReorderableList(m_Data.PreConditions, typeof(BehaviourTreeWindowData), true, true, true, true);
                }
                else
                {
                    m_PreConditionList.list = m_Data.PreConditions;
                }

                if (m_Children == null)
                {
                    m_Children = new List<BehaviourTreeWindowNode>();
                }
                else
                {
                    m_Children.Clear();
                }

                string[] assembly = isParent ? m_ParentCompositesNames : m_CompositesNames;

                for (int i = 0; i < assembly.Length; i++)
                {
                    if (assembly[i].Equals(m_Data.ClassType))
                    {
                        m_CurrSelectComposite = i;
                        break;
                    }
                }

                for (int i = 0; i < m_Data.Children.Count; i++)
                {
                    m_Children.Add(new BehaviourTreeWindowNode(m_Data.Children[i],false, this));
                }
            }
            else
            {
                m_Children.Clear();
                m_PreConditionList.list.Clear();
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
            node.m_Data.Id = m_Data.Id * 100 + m_Children.Count + 1;
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
                m_Data.WindowRect = GUI.Window(m_Data.Id, m_Data.WindowRect, DrawNodeWindow, string.Empty);

                for (int i = 0; i < m_Children.Count; i++)
                {
                    m_Children[i].OnGUI(e);
                }

                if (e.keyCode == KeyCode.Return)
                {
                    m_IsChangeName = false;
                }

                DrawCurve();
            }
        }

        public void MouseMove(Vector2 delta)
        {
            m_Data.WindowRect.position += delta;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].MouseMove(delta);
            }
        }

        public void MouseScroll(Vector2 delta)
        {
            //m_Data.WindowRect.width += delta.y;
            //m_Data.WindowRect.height += delta.y;
            //m_Data.WindowRect.position += Vector2.one * delta.y * 2;

            //for (int i = 0; i < m_Children.Count; i++)
            //{
            //    m_Children[i].MouseScroll(delta);
            //}
        }

        public void ChangeName()
        {
            m_IsChangeName = true;
        }

        private void DrawNodeWindow(int id)
        {
            float width = m_Data.WindowRect.width - 20;
            float height = 20;
            float x = 20 / 2;
            float y = 5;

            if (m_IsChangeName)
            {
                m_Data.Name = EditorGUI.TextField(new Rect(x, y, width, height), m_Data.Name);
            }
            else
            {
                string name = m_IsChangeName ? string.Empty : m_Data.Name + (m_IsParent ? "(父节点)" : string.Empty);
                EditorGUI.LabelField(new Rect(x, y, width, height), name);
            }

            EditorGUILayout.Space(25);

            EditorUtility.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("节点类型");
                m_CurrSelectComposite = EditorGUILayout.Popup(m_CurrSelectComposite, m_IsParent ? m_ParentCompositesNames : m_CompositesNames);
                m_Data.ClassType = m_IsParent ? m_ParentCompositesNames[m_CurrSelectComposite] : m_CompositesNames[m_CurrSelectComposite];
                m_Data.Args = EditorGUILayout.TextField(m_Data.Args);
                EditorGUILayout.EndVertical();
            });

            EditorGUILayout.Space(5);
            m_PreConditionList.DoLayoutList();
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
            List<string> assemblyList = new List<string>();
            assemblyList.AddRange(GetAssembly("GameFrameWork.BehaviourTree.PreCondition", "PreCondition"));
            assemblyList.Insert(0, "None");
            m_PreConditionNames = assemblyList.ToArray();
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

        private int m_CurrSelectComposite = 0;
        private bool m_IsParent = false;
        private bool m_IsChangeName = false;

        private ReorderableList m_PreConditionList = null;
        private BehaviourTreeWindowNode m_Parent = null;
        private List<BehaviourTreeWindowNode> m_Children = null;
        private BehaviourTreeWindowData m_Data = null;
    }
}