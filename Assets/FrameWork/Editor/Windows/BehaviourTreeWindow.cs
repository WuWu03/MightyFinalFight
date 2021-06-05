using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using GameFrameWork.Utils;
using GameFrameWork.Serialize;
using UnityEditorInternal;
using System;

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
        }

        private void OnGUI()
        {
            InitConfig();
            CreateBehaviourTreeGUI();
            MainGUI();
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
            m_LeftList = new ReorderableList(m_WindowConfigSo, m_WindowConfigSo.FindProperty("WindowDatas"), true, false, false, false);
            m_LeftList.headerHeight = 0;
            m_LeftList.footerHeight = 0;
            m_LeftList.elementHeight = 40;
            m_LeftList.showDefaultBackground = false;
            m_LeftList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                BehaviourTreeWindowData windowData = m_BehaviourTreeWindowConfig.WindowDatas[index];
                m_BehaviourTreeWindowConfig.WindowDatas[index].ListRect = rect;

                if(m_LeftOperation == 1 && m_CurrSelect == index)
                    windowData.Name = EditorGUI.TextField(new Rect(rect.x, rect.y + 5, rect.width,15), windowData.Name);
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y - 10, rect.width, rect.height), windowData.Name);

                if (m_LeftOperation == 2 && m_CurrSelect == index)
                    windowData.ID = Convert.ToInt32(EditorGUI.TextField(new Rect(rect.x, rect.y + 22, rect.width, 15), windowData.ID.ToString()));
                else
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 10, rect.width, rect.height), windowData.ID.ToString());

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
                    SetRightWindowNode(m_BehaviourTreeWindowConfig.WindowDatas[m_CurrSelect]);
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
                return;
            
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
                for (int i = 0; i < m_BehaviourTreeWindowConfig.WindowDatas.Count; i++)
                {
                    if (m_BehaviourTreeWindowConfig.WindowDatas[i].ListRect.Contains(e.mousePosition) && i == m_CurrSelect)
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
                    m_BehaviourTreeWindowConfig.WindowDatas.RemoveAt(m_CurrSelect);
                    if (m_CurrSelect < m_BehaviourTreeWindowConfig.WindowDatas.Count)
                        SetRightWindowNode(m_BehaviourTreeWindowConfig.WindowDatas[m_CurrSelect]);
                    else
                        SetRightWindowNode(null);
                    break;
                case 3:
                    BehaviourTreeWindowData data = new BehaviourTreeWindowData("未命名", m_BehaviourTreeWindowConfig.WindowDatas.Count + 1);
                    m_BehaviourTreeWindowConfig.WindowDatas.Add(data);
                    break;
                default:
                    m_LeftOperation = operation;
                    break;
            }
        }

        private void RightViewGUI(UnityEngine.Event e)
        {
            if (m_BehaviourTreeWindowConfig.WindowDatas == null) return;
            if (m_BehaviourTreeWindowConfig.WindowDatas.Count < 1) return;

            BeginWindows();
            if (m_RightWindowNode != null)
            {
                m_RightWindowNode.OnGUI(e);
            }
            EndWindows();

            if (m_CurrSelect < 0 || m_CurrSelect > m_BehaviourTreeWindowConfig.WindowDatas.Count - 1) return;

            if (e.button == 1 && e.type == EventType.MouseUp)
            {
                bool isOnWindowNode = IsOnWindowNode(m_BehaviourTreeWindowConfig.WindowDatas[m_CurrSelect], e.mousePosition);
                ShowRightMenu(isOnWindowNode ? 1 : 0);
            }
        }

        private bool IsOnWindowNode(BehaviourTreeWindowData data,Vector2 mousePosition)
        {
            if (data.WindowRect.Contains(mousePosition))
            {
                return true;
            }

            for (int i = 0; i < data.Children.Count; i++)
            {
                if (IsOnWindowNode(data.Children[i], mousePosition))
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowRightMenu(int type)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            if (type == 0)
            {
                menu.AddItem(new GUIContent("增加节点"), false, RightMenuContextCallback, 0);
            }
            else if (type == 1)
            {
                menu.AddItem(new GUIContent("关联父节点"), false, RightMenuContextCallback, 1);
                menu.AddItem(new GUIContent("删除节点"), false, RightMenuContextCallback, 2);
            }
            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        private void RightMenuContextCallback(object args)
        {
            int operation = (int)args;

            switch (operation)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
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

            }
        }

        private void SetRightWindowNode(BehaviourTreeWindowData data)
        {
            if (m_RightWindowNode == null)
            {
                m_RightWindowNode = new BehaviourTreeWindowNode(data);
            }
            else
            {
                m_RightWindowNode.UpdateData(data);
            }
        }

        private BehaviourTreeWindowNode m_RightWindowNode = null;
        private EditorGUISplitView m_HorizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
        private SerializedObject m_WindowConfigSo;
        private ReorderableList m_LeftList = null;
        private int m_CurrSelect = -1;
        private int m_LeftOperation = -1;
        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
    }
}