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

        BehaviourTreeWindowNode node;

        private void OnEnable()
        {
            //node = new BehaviourTreeWindowNode(1, "test", 0, 0, 100, 60);
            //node2 = new BehaviourTreeNode(2, "test2", 0, 0, 100, 60);
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
                m_BehaviourTreeWindowConfig.WindowDatas[index].Rect = rect;

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
                m_CurrSelect = list.index;
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
                    if (m_BehaviourTreeWindowConfig.WindowDatas[i].Rect.Contains(e.mousePosition) && i == m_CurrSelect)
                    {
                        isClickItem = true;
                        ShowMenu(0);
                        break;
                    }
                }

                if (!isClickItem)
                {
                    ShowMenu(1);
                }
            }
        }

        private void ShowMenu(int type)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            if (type == 0)
            {
                menu.AddItem(new GUIContent("删除"), false, ContextCallback, 0);
                menu.AddItem(new GUIContent("更改名称"), false, ContextCallback, 1);
                menu.AddItem(new GUIContent("更改ID"), false, ContextCallback, 2);
                menu.AddItem(new GUIContent("添加行为树"), false, ContextCallback, 3);
            }
            else if (type == 1)
            {
                menu.AddItem(new GUIContent("添加行为树"), false, ContextCallback, 3);
            }
            menu.AddSeparator("");
            menu.ShowAsContext();
        }

        private void ContextCallback(object args)
        {
            int operation = (int)args;
            if (operation == 0)
                m_BehaviourTreeWindowConfig.WindowDatas.RemoveAt(m_CurrSelect);
            else if(operation == 3)
                m_BehaviourTreeWindowConfig.WindowDatas.Add(new BehaviourTreeWindowData()
                {
                    Name = "未命名",
                    ID = m_BehaviourTreeWindowConfig.WindowDatas.Count + 1,
                });
            else
                m_LeftOperation = operation;
        }

        private void RightViewGUI(UnityEngine.Event e)
        {
            //BeginWindows();
            //node.OnGUI();
            //node2.OnGUI();
            //EndWindows();

            //Vector3 startPos = new Vector3(node.Rect.x + node.Rect.width, node.Rect.y + node.Rect.height / 2, 0);
            //Vector3 endPos = new Vector3(node2.Rect.x, node2.Rect.y + node2.Rect.height / 2, 0);
            //Vector3 startTan = startPos + Vector3.right * 50;
            //Vector3 endTan = endPos + Vector3.left * 50;
            //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 4);
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

        private void CopyChild(List<BehaviourTreeWindowData> src, List<BehaviourTreeWindowData> dest)
        {
            if (src == null || src.Count < 1) return;
            if (dest == null) dest = new List<BehaviourTreeWindowData>();

            for (int i = 0; i < src.Count; i++)
            {
                dest.Add(new BehaviourTreeWindowData()
                {
                    Name = src[i].Name,
                    ClassType = src[i].ClassType,
                    Args = src[i].Args,
                    Rect = src[i].Rect,
                });

                CopyChild(src[i].Childs, dest[i].Childs);
            }
        }

        private EditorGUISplitView m_HorizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
        private SerializedObject m_WindowConfigSo;
        private ReorderableList m_LeftList = null;
        private int m_CurrSelect = -1;
        private int m_LeftOperation = -1;
        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
    }
}