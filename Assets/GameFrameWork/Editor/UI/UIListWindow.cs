using GameFrameWork.Utils;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using FileUtil = GameFrameWork.Utils.FileUtil;

namespace GameFrameWork.Editor
{
    public class UIListWindow : EditorWindow
    {
        private void OnEnable()
        {
            m_IsCloseWindow = false;
            m_DeleteIndex = -1;
            string uiScenesPath = PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().uiPath, EditorPathUtil.uiScenesPath);
            m_UISceneFiles = GameFrameWork.Utils.FileUtil.GetFiles(uiScenesPath, "*.unity");
        }

        private void OnDisable()
        {
            m_IsCloseWindow = false;
            m_DeleteIndex = -1;
            m_UISceneFiles = null;
        }

        private void OnGUI()
        {
            if (m_IsCloseWindow && UIEditorInit.CanCreateUIScene(m_UIName))
            {
                this.Close();
                UIEditorInit.NewUIScene(m_UIName);
                return;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorUtil.GUIBoxScope(() => 
            {
                string uiName = EditorGUILayout.TextField("名称", m_UIName);
                if (string.IsNullOrEmpty(uiName))
                {
                    uiName = "NewPanel";
                }

                if (uiName != m_UIName)
                {
                    m_UIName = Path.GetFileNameWithoutExtension(uiName);
                }

                if (GUILayout.Button("新建UI"))
                {
                    m_IsCloseWindow = true;
                }
            });
            EditorGUILayout.EndHorizontal();

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            for (int i = 0; i < m_UISceneFiles.Length; i++)
            {
                string filePath = m_UISceneFiles[i];
                string uiName = Path.GetFileNameWithoutExtension(filePath);

                EditorGUILayout.BeginHorizontal();

                EditorUtil.GUIBoxScope(() =>
                {
                    EditorGUILayout.LabelField((i + 1).ToString() + "." + uiName);
                    if (GUILayout.Button("打开"))
                    {
                        EditorSceneManager.OpenScene(filePath);
                    }

                    if (GUILayout.Button("删除"))
                    {
                        if (EditorUtility.DisplayDialog("删除UI", "确定删除 [" + uiName + "] 吗?", "确定", "取消"))
                        {
                            m_DeleteIndex = i;
                        }
                    }
                });

                EditorGUILayout.EndHorizontal();
            }

            if (m_DeleteIndex > -1) 
            {
                string filePath = m_UISceneFiles[m_DeleteIndex];
                FileUtil.DeleteFile(filePath);
                AssetDatabase.Refresh();
                string uiScenesPath = PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().uiPath, EditorPathUtil.uiScenesPath);
                m_UISceneFiles = GameFrameWork.Utils.FileUtil.GetFiles(uiScenesPath, "*.unity");
                m_DeleteIndex = -1;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private bool m_IsCloseWindow = false;
        private string m_UIName = string.Empty;
        private int m_DeleteIndex = -1;
        private Vector2 m_ScrollPos = Vector2.zero;
        private string[] m_UISceneFiles = null;
    }
}