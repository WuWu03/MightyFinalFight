using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using GameFrameWork.Utilities;
using GameFrameWork.Serialize;
using UnityEditorInternal;
using System;
using System.Reflection;
using GameFrameWork.BehaviourTree;
using UnityEditor.SceneManagement;

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
            UnityEditor.EditorUtility.SetDirty(m_BehaviourTreeWindowConfig);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        private void OnGUI()
        {
            InitConfig();

            if (m_BehaviourTreeWindowConfig != null)
            {
                CreateBehaviourTreeGUI();
                MainGUI();
            }

            Repaint();
        }

        private void InitConfig()
        {
            if (m_BehaviourTreeWindowConfig != null) return;

            if (!Directory.Exists(PathUtil.BehaviourTreeWindowConfigFullPath))
            {
                Directory.CreateDirectory(PathUtil.BehaviourTreeWindowConfigFullPath);
            }

            if (!File.Exists(PathUtil.BehaviourTreeWindowDataPath))
            {
                EditorUtility.CreateScriptableObject<BehaviourTreeWindowConfig>(PathUtil.BehaviourTreeWindowDataName, PathUtil.BehaviourTreeWindowDataExtend, PathUtil.EdiorConfiglPath);
            }

            m_BehaviourTreeWindowConfig = AssetDatabase.LoadAssetAtPath<BehaviourTreeWindowConfig>(PathUtil.BehaviourTreeWindowDataPath);
            m_WindowConfigSo = new SerializedObject(m_BehaviourTreeWindowConfig);
            m_DicFreeWindowNode = new Dictionary<int, List<BehaviourTreeWindowNode>>();
            m_LeftList = new ReorderableList(m_WindowConfigSo, m_WindowConfigSo.FindProperty("Datas"), true, false, false, false);
            m_LeftList.headerHeight = 0;
            m_LeftList.footerHeight = 0;
            m_LeftList.elementHeight = 40;
            m_LeftList.showDefaultBackground = false;
            m_LeftList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                BehaviourTreeWindowData windowData = m_BehaviourTreeWindowConfig.Datas[index];
                m_BehaviourTreeWindowConfig.Datas[index].ListRect = rect;

                if(m_LeftOperation == 1 && m_CurrSelect == index)
                    windowData.Name = EditorGUI.TextField(new Rect(rect.x, rect.y + 5, rect.width,15), windowData.Name);
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y - 10, rect.width, rect.height), windowData.Name);

                if (m_LeftOperation == 2 && m_CurrSelect == index)
                    windowData.Id = Convert.ToInt32(EditorGUI.TextField(new Rect(rect.x, rect.y + 22, rect.width, 15), windowData.Id.ToString()));
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 10, rect.width, rect.height), windowData.Id.ToString());

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
                    SetRightWindowNode(m_BehaviourTreeWindowConfig.Datas[m_CurrSelect]);
                }

                m_LeftOperation = -1;
            };
        }

        private void CreateBehaviourTreeGUI()
        {
            if (string.IsNullOrEmpty(m_BehaviourTreeWindowConfig.BehaviourConfigPath) || !File.Exists(m_BehaviourTreeWindowConfig.BehaviourConfigPath))
            {
                if (GUILayout.Button("创建行为树"))
                {
                    string selectPath = UnityEditor.EditorUtility.SaveFilePanelInProject("创建新的行为树", "BehaviourTreeData", "asset", "Save BehaviourTreeData as...");
                    if (string.IsNullOrEmpty(selectPath)) return;

                    string path = Path.GetDirectoryName(selectPath) + "/";
                    string name = Path.GetFileNameWithoutExtension(selectPath);
                    string extend = Path.GetExtension(selectPath);
                    m_BehaviourTreeWindowConfig.BehaviourConfigPath = selectPath;
                    EditorUtility.CreateBehaviorConfig(name, extend, path);
                    UnityEditor.EditorUtility.SetDirty(m_BehaviourTreeWindowConfig);
                }
            }
        }

        private void MainGUI()
        {
            if (string.IsNullOrEmpty(m_BehaviourTreeWindowConfig.BehaviourConfigPath) || !File.Exists(m_BehaviourTreeWindowConfig.BehaviourConfigPath))
            {
                return;
            }

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
            m_WindowConfigSo.Update();
            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(0, 0, position.width, 20), GUI.skin.GetStyle("FrameBox"));
            GUILayout.EndArea();
            GUILayout.Space(20);
            m_LeftList.DoLayoutList();
            GUILayout.EndVertical();
            m_WindowConfigSo.ApplyModifiedProperties();

            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                bool isClickItem = false;
                for (int i = 0; i < m_BehaviourTreeWindowConfig.Datas.Count; i++)
                {
                    if (m_BehaviourTreeWindowConfig.Datas[i].ListRect.Contains(e.mousePosition) && i == m_CurrSelect)
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
                    BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", m_BehaviourTreeWindowConfig.Datas.Count + 1);
                    m_BehaviourTreeWindowConfig.Datas.Add(data);
                    break;
                default:
                    m_LeftOperation = operation;
                    break;
            }
        }

        private void RightViewGUI(UnityEngine.Event e)
        {
            if (m_BehaviourTreeWindowConfig.Datas == null) return;
            if (m_BehaviourTreeWindowConfig.Datas.Count < 1) return;

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
                EditorUtility.DrawCurve(m_CurrWindowNode.Rect, new Rect(e.mousePosition, Vector2.zero), Color.red);
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
                    if (m_CurrSelect < 0 || m_CurrSelect > m_BehaviourTreeWindowConfig.Datas.Count - 1) return;
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
                    if (m_CurrWindowNode.Parent == null && !m_CurrWindowNode.IsParent)
                        menu.AddItem(new GUIContent("关联父节点"), false, RightMenuContextCallback, 2);
                    menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, m_CurrWindowNode.Parent == null ? 4 : 5);
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
            m_BehaviourTreeWindowConfig.Datas.RemoveAt(m_CurrSelect);
            if (m_CurrSelect < m_BehaviourTreeWindowConfig.Datas.Count)
                SetRightWindowNode(m_BehaviourTreeWindowConfig.Datas[m_CurrSelect]);
            else
            {
                m_CurrSelect = -1;
                m_LeftList.index = -1;
                SetRightWindowNode(null);
            }
        }

        private void DeleteChildWindowNode()
        {
            m_CurrWindowNode.Parent.RemoveChild(m_CurrWindowNode);

            m_CurrWindowNode = null;
        }

        private void SetFreeNodeParent(BehaviourTreeWindowNode parent)
        {
            if (m_CurrWindowNode == null) return;
            m_CurrWindowNode.SetParent(parent);
            DeleteFreeWindowNode();
        }

        private BehaviourTreeWindowNode GetWindowNode(BehaviourTreeWindowNode node, Vector2 mousePosition)
        {
            if (node.Rect.Contains(mousePosition))
            {
                return node;
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                BehaviourTreeWindowNode ret = GetWindowNode(node.Children[i], mousePosition);
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
                    if (list[i].Rect.Contains(mousePosition))
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
                BehaviourTreeConfig config = AssetDatabase.LoadAssetAtPath<BehaviourTreeConfig>(m_BehaviourTreeWindowConfig.BehaviourConfigPath);
                config.Datas = new List<BehaviourTreeData>();

                for (int i = 0; i < m_BehaviourTreeWindowConfig.Datas.Count; i++)
                {
                    config.Datas.Add(new BehaviourTreeData());
                }

                for (int i = 0; i < m_BehaviourTreeWindowConfig.Datas.Count; i++)
                {
                    config.Datas[i].Id = m_BehaviourTreeWindowConfig.Datas[i].Id;
                    ExportConfig(config.Datas[i], m_BehaviourTreeWindowConfig.Datas[i]);
                }

                UnityEditor.EditorUtility.SetDirty(config);
                ShowNotification(new GUIContent("保存成功"));
            }
        }

        private void ExportConfig(BehaviourTreeData outData, BehaviourTreeWindowData windowData)
        {
            outData.ClassType = windowData.ClassType;
            outData.Name = windowData.Name;
            outData.Args = windowData.Args;
            outData.Childs = new BehaviourTreeData[windowData.Children.Count];
            outData.PreConditions = new BehaviorTreeBaseData[windowData.PreConditions.Count];

            for (int i = 0; i < windowData.PreConditions.Count; i++)
            {
                outData.PreConditions[i] = new BehaviorTreeBaseData();
            }

            for (int i = 0; i < windowData.PreConditions.Count; i++)
            {
                outData.PreConditions[i].ClassType = windowData.PreConditions[i].ClassType;
                outData.PreConditions[i].Args = windowData.PreConditions[i].Args;
            }

            for (int i = 0; i < windowData.Children.Count; i++)
            {
                outData.Childs[i] = new BehaviourTreeData();
            }

            for (int i = 0; i < windowData.Children.Count; i++)
            {
                ExportConfig(outData.Childs[i], windowData.Children[i]);
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
        private SerializedObject m_WindowConfigSo;
        private ReorderableList m_LeftList = null;
        private int m_CurrSelect = -1;
        private int m_LeftOperation = -1;
        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
    }
}