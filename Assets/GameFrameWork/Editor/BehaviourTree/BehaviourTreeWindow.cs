using GameFrameWork.BehaviourTree;
using GameFrameWork.Editor.Config;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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

            GameFrameWork.Utils.FileUtil.VerifyDirectory(EditorPathUtil.editorConfigFullPath);

            if (!File.Exists(EditorPathUtil.behaviourTreeWindowDataFullPath))
            {
                m_BehaviourTreeWindowConfig = new BehaviourTreeWindowConfig();
            }
            else
            {
                string jsonStr = File.ReadAllText(EditorPathUtil.behaviourTreeWindowDataFullPath);
                m_BehaviourTreeWindowConfig = LitJson.JsonMapper.ToObject<BehaviourTreeWindowConfig>(jsonStr);
            }

            m_FreeWindowNodes = new();
            m_LeftList = new(m_BehaviourTreeWindowConfig.dataList, typeof(BehaviourTreeWindowData), true, false, false, false)
            {
                headerHeight = 0,
                footerHeight = 0,
                elementHeight = 40,
                showDefaultBackground = false,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
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
                    {
                        EditorGUI.DrawRect(new Rect(rect.x - 20, rect.y + rect.height, rect.width + 25, 1), Color.black);
                    }
                },

                onSelectCallback = (ReorderableList list) =>
                {
                    int oldIndex = m_CurrSelect;
                    m_CurrSelect = list.index;

                    if (oldIndex != m_CurrSelect)
                    {
                        SetRightWindowNode(m_BehaviourTreeWindowConfig.dataList[m_CurrSelect]);
                    }

                    m_LeftOperation = -1;
                }
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

            switch (operation)
            {
                case 0:
                    DeleteRootWindowNode();
                    break;
                case 3:
                    BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", string.Empty, m_BehaviourTreeWindowConfig.dataList.Count + 1);
                    m_BehaviourTreeWindowConfig.dataList.Add(data);
                    break;
                default:
                    m_LeftOperation = operation;
                    break;
            }
        }

        private void RightViewGUI(UnityEngine.Event e)
        {
            if (m_BehaviourTreeWindowConfig.dataList == null || m_BehaviourTreeWindowConfig.dataList.Count < 1)
            {
                return;
            }

            PopMenu(e);
            MouseMove(e);
            MouseScroll(e);

            BeginWindows();

            if (m_RightWindowNode != null)
            {
                m_RightWindowNode.OnGUI(e);
            }

            if (m_FreeWindowNodes.TryGetValue(m_CurrSelect, out List<BehaviourTreeWindowNode> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].OnGUI(e);
                }
            }

            EndWindows();

            if (m_IsDrawTransition)
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

                    if (m_FreeWindowNodes.TryGetValue(m_CurrSelect, out List<BehaviourTreeWindowNode> list))
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            list[i].MouseMove(e.mousePosition - m_MouseDownPos);
                        }
                    }
                }

                m_MouseDownPos = e.mousePosition;
            }

            if (e.type == EventType.MouseUp)
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
                    float scale = e.delta.y * 0.005f;

                    if (e.delta.y > 0 && m_WindowScale - scale <= 1)
                    {
                        m_RightWindowNode.ResetScale();
                        return;
                    }

                    m_WindowScale -= scale;
                    m_RightWindowNode.MouseScroll(1 - scale);
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
                        SetCurrNodeParent(node);
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
                        ShowRightMenu(true, false);
                    }
                    else
                    {
                        m_CurrWindowNode = GetWindowNode(m_RightWindowNode, e.mousePosition);
                        ShowRightMenu(false, m_CurrWindowNode != null && m_CurrWindowNode.parent == null);
                    }
                }
            }
        }

        private void ShowRightMenu(bool isFree, bool isRoot)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");

            if (m_CurrWindowNode == null)
            {
                string[][] nodePaths = BehaviourTreeUtil.GetNodePaths(false, "增加节点");

                for (int i = 0; i < nodePaths.Length; i++)
                {
                    for (int j = 0; j < nodePaths[i].Length; j++)
                    {
                        menu.AddItem(new GUIContent(nodePaths[i][j]), false, RightMenuContextCallback, 10000 + (i + 1) * 1000 + j + 1);
                    }
                }
            }
            else
            {
                menu.AddItem(new GUIContent("更改名称"), false, RightMenuContextCallback, 3);

                if (!isRoot)
                {
                    menu.AddItem(new GUIContent("关联父节点"), false, RightMenuContextCallback, 4);
                }

                if (isFree)
                {
                    menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, 5);
                }
                else
                {
                    menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, isRoot ? 6 : 7);
                }

                string[][] nodePaths = BehaviourTreeUtil.GetNodePaths(isRoot, "替换节点");

                for (int i = 0; i < nodePaths.Length; i++)
                {
                    for (int j = 0; j < nodePaths[i].Length; j++)
                    {
                        menu.AddItem(new GUIContent(nodePaths[i][j]), false, RightMenuContextCallback, 20000 + (i + 1) * 1000 + j + 1);
                    }
                }
            }
            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        private void RightMenuContextCallback(object args)
        {
            int operation = (int)args;
            int operationSubType = 0;
            int operationIndex = 0;

            if (operation > 10)
            {
                operationIndex = operation % 10 - 1;

                while (operation >= 100)
                {
                    operation /= 10;
                }

                operationSubType = operation % 10 - 1;
                operation /= 10;
            }

            switch (operation)
            {
                case 1:
                    AddFreeWindowNode(BehaviourTreeUtil.GetNodeNames()[operationSubType][operationIndex]);
                    break;
                case 2:
                    ReplaceNodeClassType(BehaviourTreeUtil.GetNodeNames()[operationSubType][operationIndex]);
                    break;
                case 3://更改名称
                    m_CurrWindowNode.ChangeName();
                    break;
                case 4://关联父节点
                    m_IsDrawTransition = true;
                    break;
                case 5://删除自由节点
                    DeleteFreeWindowNode();
                    break;
                case 6://删除根节点
                    DeleteRootWindowNode();
                    break;
                case 7://删除子节点
                    DeleteChildWindowNode();
                    break;
            }
        }

        private void AddFreeWindowNode(string classType)
        {
            if (!m_FreeWindowNodes.TryGetValue(m_CurrSelect, out List<BehaviourTreeWindowNode> list))
            {
                list = new List<BehaviourTreeWindowNode>();
                m_FreeWindowNodes.Add(m_CurrSelect, list);
            }

            int id = (m_CurrSelect + 1) * 1000 + list.Count + 1;
            BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", classType, id, m_CurrMousePosition.x, m_CurrMousePosition.y);
            BehaviourTreeWindowNode node = new BehaviourTreeWindowNode(data, false);

            list.Add(node);
        }

        private void ReplaceNodeClassType(string classType)
        {
            if (m_CurrWindowNode != null)
            {
                m_CurrWindowNode.UpdateClassType(classType);
            }
        }

        private void DeleteFreeWindowNode()
        {
            if (m_FreeWindowNodes.TryGetValue(m_CurrSelect, out List<BehaviourTreeWindowNode> list))
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

        private void SetCurrNodeParent(BehaviourTreeWindowNode parent)
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
            if (m_FreeWindowNodes.TryGetValue(m_CurrSelect, out List<BehaviourTreeWindowNode> list))
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
            if (GUILayout.Button("导出配置"))
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
                string dataPath = PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().configDataPath, PathUtil.behaviourTreeConfigDataName);
                File.WriteAllText(dataPath, jsonStr);

                jsonStr = LitJson.JsonMapper.ToJson(m_BehaviourTreeWindowConfig);
                File.WriteAllText(EditorPathUtil.behaviourTreeWindowDataFullPath, jsonStr);

                ShowNotification(new GUIContent("导出成功"));
            }
        }

        private void ExportConfig(BehaviourTreeData outData, BehaviourTreeWindowData windowData)
        {
            outData.id = windowData.id;
            outData.classType = windowData.classType;
            outData.name = windowData.name;
            outData.args = windowData.args;
            outData.priority = windowData.priority;
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

        private float m_WindowScale = 1;
        private bool m_IsDrawTransition = false;
        private Vector2 m_CurrMousePosition = Vector2.zero;
        private BehaviourTreeWindowNode m_RightWindowNode = null;
        private BehaviourTreeWindowNode m_CurrWindowNode = null;
        private Dictionary<int, List<BehaviourTreeWindowNode>> m_FreeWindowNodes = null;
        private EditorGUISplitView m_HorizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
        private ReorderableList m_LeftList = null;
        private int m_CurrSelect = -1;
        private int m_LeftOperation = -1;
        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
    }
}