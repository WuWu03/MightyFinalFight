using GameFrameWork.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class UIListWindow : EditorWindow
    {
        private void OnEnable()
        {
            string uiScenesPath = PathUtil.FormatPath(EditorMgr.GetGameFrameWorkConfig().uiPath, EditorPathUtil.uiScenesPath);
            m_UISceneFiles = GameFrameWork.Utils.FileUtil.GetFiles(uiScenesPath, "*.unity");
        }

        private void OnDisable()
        {
            m_UISceneFiles = null;
        }

        private void OnGUI()
        {
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < m_UISceneFiles.Length; i++)
            {
                string filePath = m_UISceneFiles[i];
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(filePath);

                if (GUILayout.Button("打开"))
                {
                    EditorSceneManager.OpenScene(filePath);
                }
     
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private string[] m_UISceneFiles = null;
    }
}