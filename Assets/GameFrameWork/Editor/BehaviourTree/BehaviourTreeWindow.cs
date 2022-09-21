using GameFrameWork.BehaviourTree;
using GameFrameWork.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using GameFrameWork.Editor.Config;

namespace GameFrameWork.Editor
{
    public class BehaviourTreeWindow : EditorWindow
    {
        public BehaviourTreeWindow()
        {
            titleContent = new GUIContent(this.GetType().Name);
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            string jsonStr = LitJson.JsonMapper.ToJson(m_BehaviourTreeWindowConfig);
            File.WriteAllText(EditorPathUtil.behaviourTreeWindowDataFullPath, jsonStr);
            m_BehaviourTreeWindowConfig = null;
        }

        private void OnGUI()
        {
            InitConfig();

            if (m_BehaviourTreeWindowConfig != null)
            {
                MainGUI();
            }

            Repaint();
        }

        private void InitConfig()
        {
            if (m_BehaviourTreeWindowConfig != null)
            {
                return;
            }

            if (!Directory.Exists(EditorPathUtil.behaviourTreeWindowConfigFullPath))
            {
                Directory.CreateDirectory(EditorPathUtil.behaviourTreeWindowConfigFullPath);
            }

            if (!File.Exists(EditorPathUtil.behaviourTreeWindowDataFullPath))
            {
                m_BehaviourTreeWindowConfig = new BehaviourTreeWindowConfig();
            }
            else
            {
                string jsonStr = File.ReadAllText(EditorPathUtil.behaviourTreeWindowDataFullPath);
                m_BehaviourTreeWindowConfig = LitJson.JsonMapper.ToObject<BehaviourTreeWindowConfig>(jsonStr);
            }

            m_DicFreeWindowNode = new Dictionary<int, List<BehaviourTreeWindowNode>>();
            m_LeftList = new ReorderableList(m_BehaviourTreeWindowConfig.dataList, typeof(BehaviourTreeWindowData), true, false, false, false);
            m_LeftList.headerHeight = 0;
            m_LeftList.footerHeight = 0;
            m_LeftList.elementHeight = 40;
            m_LeftList.showDefaultBackground = false;
            m_LeftList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                BehaviourTreeWindowData windowData = m_BehaviourTreeWindowConfig.dataList[index];
                m_BehaviourTreeWindowConfig.dataList[index].listRect = new WindowRect(rect.x, rect.y, rect.width, rect.height);

                if (m_LeftOperation == 1 && m_CurrSelect == index)
                    windowData.name = EditorGUI.TextField(new Rect(rect.x, rect.y + 5, rect.width, 15), windowData.name);
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y - 10, rect.width, rect.height), windowData.name);

                if (m_LeftOperation == 2 && m_CurrSelect == index)
                    windowData.id = Convert.ToInt32(EditorGUI.TextField(new Rect(rect.x, rect.y + 22, rect.width, 15), windowData.id.ToString()));
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 10, rect.width, rect.height), windowData.id.ToString());

                EditorGUI.DrawRect(new Rect(rect.x, rect.y + 22, rect.width, 1), Color.gray);
                EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y, rect.width + 25, 1), Color.black);

                if (index > 0)
                    EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y + rect.height, rect.width + 25, 1), Color.black);
            };

            m_LeftList.onSelectCallback = (ReorderableList list) => 
            {
                int oldIndex = m_CurrSelect;
                m_CurrSelect = list.index;

                if (oldIndex != m_CurrSelect)
                {
                    SetRightWindowNode(m_BehaviourTreeWindowConfig.dataList[m_CurrSelect]);
                }

                m_LeftOperation = -1;
            };
        }

        private void MainGUI()
        {
            UnityEngine.Event e = UnityEngine.Event.current;

            m_HorizontalSplitView.BeginSplitView();
            LeftViewGUI(e);
            m_HorizontalSplitView.Split();
            RightViewGUI(e);
            m_HorizontalSplitView.EndSplitView();

            ResetOperation(e);
            SaveConfigGUI();
        }

        private void LeftViewGUI(UnityEngine.Event e)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(0, 0, position.width, 20), GUI.skin.GetStyle("FrameBox"));
            GUILayout.EndArea();
            GUILayout.Space(20);
            m_LeftList.DoLayoutList();
            GUILayout.EndVertical();


            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                bool isClickItem = false;
                for (int i = 0; i < m_BehaviourTreeWindowConfig.dataList.Count; i++)
                {
                    WindowRect rect = m_BehaviourTreeWindowConfig.dataList[i].listRect;
                    Rect listRect = new Rect(rect.x, rect.y, rect.width, rect.height);

                    if (listRect.Contains(e.mousePosition) && i == m_CurrSelect)
                    {
                        isClickItem = true;
                        ShowLeftMenu(0);
                        break;
                    }
                }

                if (!isClickItem)
                {
                    ShowLeftMenu(1);
                }
            }
        }

        private void ShowLeftMenu(int type)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            if (type == 0)
            {
                menu.AddItem(new GUIContent("删除"), false, LeftMenuContextCallback, 0);
                menu.AddItem(new GUIContent("更改名称"), false, LeftMenuContextCallback, 1);
                menu.AddItem(new GUIContent("更改ID"), false, LeftMenuContextCallback, 2);
                menu.AddItem(new GUIContent("添加行为树"), false, LeftMenuContextCallback, 3);
            }
            else if (type == 1)
            {
                menu.AddItem(new GUIContent("添加行为树"), false, LeftMenuContextCallback, 3);
            }
            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        private void LeftMenuContextCallback(object args)
        {
            int operation = (int)args;

            switch(operation)
            {
                case 0:
                    DeleteRootWindowNode();
                    break;
                case 3:
                    BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", m_BehaviourTreeWindowConfig.dataList.Count + 1);
                    m_BehaviourTreeWindowConfig.dataList.Add(data);
                    break;
                default:
                    m_LeftOperation = operation;
                    break;
            }
        }

        private void RightViewGUI(UnityEngine.Event e)
        {
            if (m_BehaviourTreeWindowConfig.dataList == null) return;
            if (m_BehaviourTreeWindowConfig.dataList.Count < 1) return;

            PopMenu(e);
            MouseMove(e);
            MouseScroll(e);

            BeginWindows();

            if (m_RightWindowNode != null)
            {
                m_RightWindowNode.OnGUI(e);
            }

            List<BehaviourTreeWindowNode> list = null;
            if (m_DicFreeWindowNode.TryGetValue(m_CurrSelect, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].OnGUI(e);
                }
            }
            EndWindows();

            if(m_IsDrawTransition)
            {
                EditorUtil.DrawCurve(m_CurrWindowNode.rect, new Rect(e.mousePosition, Vector2.zero), Color.red);
            }
        }


        private Vector2 m_MouseDownPos = Vector2.zero;
        private void MouseMove(UnityEngine.Event e)
        {
            if (e.type == EventType.MouseDrag)
            {
                if (m_RightWindowNode != null && m_MouseDownPos != Vector2.zero && e.alt)
                {
                    m_RightWindowNode.MouseMove(e.mousePosition - m_MouseDownPos);

                    List<BehaviourTreeWindowNode> list = null;

                    if (m_DicFreeWindowNode.TryGetValue(m_CurrSelect, out list))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            list[i].MouseMove(e.mousePosition - m_MouseDownPos);
                        }
                    }
                }

                m_MouseDownPos = e.mousePosition;
            }

            if(e.type == EventType.MouseUp)
            {
                m_MouseDownPos = Vector2.zero;
            }
        }

        private void MouseScroll(UnityEngine.Event e)
        {
            if (e.type == EventType.ScrollWheel)
            {
                if (m_RightWindowNode != null && e.alt)
                {
                    m_RightWindowNode.MouseScroll(e.delta);
                }
            }
        }

        private void PopMenu(UnityEngine.Event e)
        {
            if (e.type != EventType.MouseUp)
            {
                return;
            }

            if (e.button == 0)
            {
                if (m_IsDrawTransition)
                {
                    BehaviourTreeWindowNode node = GetFreeWindowNode(e.mousePosition);
                    if (node == null)
                    {
                        node = GetWindowNode(m_RightWindowNode, e.mousePosition);
                    }

                    if (node != null)
                    {
                        SetFreeNodeParent(node);
                        m_IsDrawTransition = false;
                    }
                }
            }
            else if (e.button == 1)
            {
                if (m_IsDrawTransition)
                {
                    m_IsDrawTransition = false;
                }
                else
                {
                    if (m_CurrSelect < 0 || m_CurrSelect > m_BehaviourTreeWindowConfig.dataList.Count - 1)
                    {
                        return;
                    }

                    m_CurrWindowNode = GetFreeWindowNode(e.mousePosition);
                    m_CurrMousePosition = e.mousePosition;
                    if (m_CurrWindowNode != null)
                    {
                        ShowRightMenu(true);
                    }
                    else
                    {
                        m_CurrWindowNode = GetWindowNode(m_RightWindowNode, e.mousePosition);
                        ShowRightMenu(false);
                    }
                }
            }
        }

        private void ShowRightMenu(bool isFree)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");

            if (m_CurrWindowNode == null)
            {
                menu.AddItem(new GUIContent("增加节点"), false, RightMenuContextCallback, 0);
            }
            else
            {
                menu.AddItem(new GUIContent("更改名称"), false, RightMenuContextCallback, 1);

                if (isFree)
                {
                    menu.AddItem(new GUIContent("关联父节点"), false, RightMenuContextCallback, 2);
                    menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, 3);
                }
                else
                {
                    if (m_CurrWindowNode.parent == null && !m_CurrWindowNode.isParent)
                        menu.AddItem(new GUIContent("关联父节点"), false, RightMenuContextCallback, 2);
                    menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, m_CurrWindowNode.parent == null ? 4 : 5);
                }
            }
            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        private void RightMenuContextCallback(object args)
        {
            int operation = (int)args;

            switch (operation)
            {
                case 0://增加节点
                    AddFreeWindowNode();
                    break;
                case 1://更改名称
                    m_CurrWindowNode.ChangeName();
                    break;
                case 2://关联父节点
                    m_IsDrawTransition = true;
                    break;
                case 3://删除自由节点
                    DeleteFreeWindowNode();
                    break;
                case 4://删除根节点
                    DeleteRootWindowNode();
                    break;
                case 5://删除子节点
                    DeleteChildWindowNode();
                    break;
            }
        }

        private void AddFreeWindowNode()
        {
            List<BehaviourTreeWindowNode> list = null;

            if (!m_DicFreeWindowNode.TryGetValue(m_CurrSelect, out list))
            {
                list = new List<BehaviourTreeWindowNode>();
                m_DicFreeWindowNode.Add(m_CurrSelect, list);
            }

            int id = (m_CurrSelect + 1) * 1000 + list.Count + 1;
            BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", id, m_CurrMousePosition.x, m_CurrMousePosition.y);
            BehaviourTreeWindowNode node = new BehaviourTreeWindowNode(data, false);

            list.Add(node);
        }

        private void DeleteFreeWindowNode()
        {
            List<BehaviourTreeWindowNode> list = null;

            if (m_DicFreeWindowNode.TryGetValue(m_CurrSelect, out list))
            {
                list.Remove(m_CurrWindowNode);
                m_CurrWindowNode = null;
            }
        }

        private void DeleteRootWindowNode()
        {
            m_BehaviourTreeWindowConfig.dataList.RemoveAt(m_CurrSelect);
            if (m_CurrSelect < m_BehaviourTreeWindowConfig.dataList.Count)
            {
                SetRightWindowNode(m_BehaviourTreeWindowConfig.dataList[m_CurrSelect]);
            }
            else
            {
                m_CurrSelect = -1;
                m_LeftList.index = -1;
                SetRightWindowNode(null);
            }
        }

        private void DeleteChildWindowNode()
        {
            m_CurrWindowNode.parent.RemoveChild(m_CurrWindowNode);

            m_CurrWindowNode = null;
        }

        private void SetFreeNodeParent(BehaviourTreeWindowNode parent)
        {
            if (m_CurrWindowNode == null)
            {
                return;
            }

            m_CurrWindowNode.SetParent(parent);
            DeleteFreeWindowNode();
        }

        private BehaviourTreeWindowNode GetWindowNode(BehaviourTreeWindowNode node, Vector2 mousePosition)
        {
            if (node.rect.Contains(mousePosition))
            {
                return node;
            }

            for (int i = 0; i < node.children.Count; i++)
            {
                BehaviourTreeWindowNode ret = GetWindowNode(node.children[i], mousePosition);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        private BehaviourTreeWindowNode GetFreeWindowNode(Vector2 mousePosition)
        {
            List<BehaviourTreeWindowNode> list = null;

            if (m_DicFreeWindowNode.TryGetValue(m_CurrSelect, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].rect.Contains(mousePosition))
                    {
                        return list[i];
                    }
                }
            }

            return null;
        }

        private void ResetOperation(UnityEngine.Event e)
        {
            if (m_LeftOperation != -1)
            {
                if (e.type == EventType.MouseUp)
                    m_LeftOperation = -1;
                if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Return)
                    m_LeftOperation = -1;
            }
        }

        private void SaveConfigGUI()
        {
            if (GUILayout.Button("保存配置"))
            {
                BehaviourTreeConfig config = new BehaviourTreeConfig();
                config.datas = new BehaviourTreeData[m_BehaviourTreeWindowConfig.dataList.Count];

                for (int i = 0; i < m_BehaviourTreeWindowConfig.dataList.Count; i++)
                {
                    config.datas[i] = new BehaviourTreeData();
                }

                for (int i = 0; i < m_BehaviourTreeWindowConfig.dataList.Count; i++)
                {
                    config.datas[i].id = m_BehaviourTreeWindowConfig.dataList[i].id;
                    ExportConfig(config.datas[i], m_BehaviourTreeWindowConfig.dataList[i]);

                }

                string jsonStr = LitJson.JsonMapper.ToJson(config);
                File.WriteAllText(EditorPathUtil.behaviourTreeConfigDataFullPath, jsonStr);

                jsonStr = LitJson.JsonMapper.ToJson(m_BehaviourTreeWindowConfig);
                File.WriteAllText(EditorPathUtil.behaviourTreeWindowDataFullPath, jsonStr);

                ShowNotification(new GUIContent("保存成功"));
            }
        }

        private void ExportConfig(BehaviourTreeData outData, BehaviourTreeWindowData windowData)
        {
            outData.id = windowData.id;
            outData.classType = windowData.classType;
            outData.name = windowData.name;
            outData.args = windowData.args;
            outData.children = new BehaviourTreeData[windowData.children.Count];
            outData.preConditions = new BehaviorTreeBaseData[windowData.preConditions.Count];

            for (int i = 0; i < windowData.preConditions.Count; i++)
            {
                outData.preConditions[i] = new BehaviorTreeBaseData();
            }

            for (int i = 0; i < windowData.preConditions.Count; i++)
            {
                outData.preConditions[i].classType = windowData.preConditions[i].classType;
                outData.preConditions[i].args = windowData.preConditions[i].args;
            }

            for (int i = 0; i < windowData.children.Count; i++)
            {
                outData.children[i] = new BehaviourTreeData();
            }

            for (int i = 0; i < windowData.children.Count; i++)
            {
                ExportConfig(outData.children[i], windowData.children[i]);
            }
        }

        private void SetRightWindowNode(BehaviourTreeWindowData data)
        {
            if (m_RightWindowNode == null)
            {
                m_RightWindowNode = new BehaviourTreeWindowNode(data, true);
            }
            else
            {
                m_RightWindowNode.UpdateData(data, true);
            }
        }

        private bool m_IsDrawTransition = false;
        private Vector2 m_CurrMousePosition = Vector2.zero;
        private BehaviourTreeWindowNode m_RightWindowNode = null;
        private BehaviourTreeWindowNode m_CurrWindowNode = null;
        private Dictionary<int,List<BehaviourTreeWindowNode>> m_DicFreeWindowNode = null;
        private EditorGUISplitView m_HorizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
        private ReorderableList m_LeftList = null;
        private int m_CurrSelect = -1;
        private int m_LeftOperation = -1;
        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
    }
}