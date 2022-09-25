using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using GameFrameWork.Editor.Config;

namespace GameFrameWork.Editor
{
    public class BehaviourTreeWindowNode
    {
        public bool isParent
        {
            get
            {
                return m_IsParent;
            }
        }

        public Rect rect
        {
            get
            {
                if (m_Data == null)
                {
                    return Rect.zero;
                }

                return m_WindowRect;
            }
        }

        public BehaviourTreeWindowNode parent
        {
            get
            {
                return m_Parent;
            }
        }

        public List<BehaviourTreeWindowNode> children
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

            if (!string.IsNullOrEmpty(m_Data.classType))
            {
                string[] names = m_IsParent ? m_ParentCompositesNames : m_CompositesNames;
                for (int i = 0; i < names.Length; i++)
                {
                    if(m_Data.classType == names[i])
                    {
                        m_CurrSelectComposite = i;
                        break;
                    }
                }
            }

            if(m_Data.preConditions != null && m_Data.preConditions.Count > 0)
            {
                for (int i = 0; i < m_Data.preConditions.Count; i++)
                {
                    if(string.IsNullOrEmpty(m_Data.preConditions[i].classType))
                    {
                        continue;
                    }

                    for (int j = 0; j < m_PreConditionNames.Length; j++)
                    {
                        if(m_Data.preConditions[i].classType == m_PreConditionNames[j])
                        {
                            m_Data.preConditions[i].selectIndex = j;
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
                m_Data.preConditions.Add(new BehaviourTreeWindowPreConditon());
                m_WindowRect.height += m_Data.preConditions.Count > 1 ? 52 : 0;
                m_Data.windowRect.height = m_WindowRect.height;
                list.list = m_Data.preConditions;
            };

            m_PreConditionList.onRemoveCallback = (ReorderableList list) =>
            {
                m_Data.preConditions.RemoveAt(list.index);
                m_WindowRect.height -= m_Data.preConditions.Count > 0 ? 52 : 0;
                m_Data.windowRect.height = m_WindowRect.height;
                list.list = m_Data.preConditions;
            };

            m_PreConditionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (m_Data.preConditions.Count < 1)
                {
                    return;
                }

                BehaviourTreeWindowPreConditon preConditon = m_Data.preConditions[index];
                preConditon.selectIndex = EditorGUI.Popup(new Rect(rect.x, rect.y + 5, rect.width, 20), preConditon.selectIndex, m_PreConditionNames);
                preConditon.classType = m_PreConditionNames[preConditon.selectIndex];
                preConditon.args = EditorGUI.TextField(new Rect(rect.x, rect.y + 25, rect.width, 20), preConditon.args);
            };

            m_PreConditionList.onReorderCallback = (ReorderableList list) =>
            {
                m_Data.preConditions = list.list as List<BehaviourTreeWindowPreConditon>;
            };
        }

        public void UpdateData(BehaviourTreeWindowData data, bool isParent, BehaviourTreeWindowNode parent = null)
        {
            m_Data = data;
            m_Parent = parent;
            m_IsParent = isParent;
            m_WindowRect = new Rect(data.windowRect.x, data.windowRect.y, data.windowRect.width, data.windowRect.height);

            if (m_Data != null)
            {
                if (m_PreConditionList == null)
                {
                    m_PreConditionList = new ReorderableList(m_Data.preConditions, typeof(BehaviourTreeWindowData), true, true, true, true);
                }
                else
                {
                    m_PreConditionList.list = m_Data.preConditions;
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
                    if (assembly[i].Equals(m_Data.classType))
                    {
                        m_CurrSelectComposite = i;
                        break;
                    }
                }

                for (int i = 0; i < m_Data.children.Count; i++)
                {
                    m_Children.Add(new BehaviourTreeWindowNode(m_Data.children[i],false, this));
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
            node.m_Data.id = m_Data.id * 100 + m_Children.Count + 1;
            m_Children.Add(node);
            m_Data.children.Add(node.m_Data);
        }

        public void RemoveChild(BehaviourTreeWindowNode node)
        {
            m_Data.children.Remove(node.m_Data);
            m_Children.Remove(node);
        }

        public void OnGUI(UnityEngine.Event e)
        {
            if (m_Data != null)
            {
                m_WindowRect = GUI.Window(m_Data.id, m_WindowRect, DrawNodeWindow, string.Empty);

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
            m_WindowRect.position += delta;
            m_Data.windowRect.x = m_WindowRect.position.x;
            m_Data.windowRect.y = m_WindowRect.position.y;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].MouseMove(delta);
            }
        }

        public void MouseScroll(float scale)
        {
            m_WindowRect.width *= scale;
            m_WindowRect.height *= scale;
            m_WindowRect.position *= scale;
            m_Data.windowRect.x = m_WindowRect.position.x;
            m_Data.windowRect.y = m_WindowRect.position.y;

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].MouseScroll(scale);
            }
        }

        public void ResetScale()
        {
            m_WindowRect = new Rect(m_Data.windowRect.x, m_Data.windowRect.y, m_Data.windowRect.width, m_Data.windowRect.height);

            for (int i = 0; i < m_Children.Count; i++)
            {
                m_Children[i].ResetScale();
            }
        }

        public void ChangeName()
        {
            m_IsChangeName = true;
        }

        private void DrawNodeWindow(int id)
        {
            float width = m_WindowRect.width - 20;
            float height = 20;
            float x = 20 / 2;
            float y = 5;

            if (m_IsChangeName)
            {
                m_Data.name = EditorGUI.TextField(new Rect(x, y, width, height), m_Data.name);
            }
            else
            {
                string name = m_IsChangeName ? string.Empty : m_Data.name + (m_IsParent ? "(父节点)" : string.Empty);
                EditorGUI.LabelField(new Rect(x, y, width, height), name);
            }

            EditorGUILayout.Space(25);

            EditorUtil.GUIBoxScope(() =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("节点类型");
                m_CurrSelectComposite = EditorGUILayout.Popup(m_CurrSelectComposite, m_IsParent ? m_ParentCompositesNames : m_CompositesNames);
                m_Data.classType = m_IsParent ? m_ParentCompositesNames[m_CurrSelectComposite] : m_CompositesNames[m_CurrSelectComposite];
                m_Data.args = EditorGUILayout.TextField(m_Data.args);
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
                    EditorUtil.DrawCurve(rect, m_Children[i].rect, Color.yellow);
                }
            }
        }

        private static void InitCompositesName()
        {
            if (m_CompositesNames != null)
            {
                return;
            }

            m_CompositesNames = GetAssembly("GameFrameWork.BehaviourTree.Composites", "Action");
        }

        private static void InitPreConditionName()
        {
            if (m_PreConditionNames != null)
            {
                return;
            }

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
        private Rect m_WindowRect;
    }
}