using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class UIListWindow : EditorWindow
    {
        private void OnEnable()
        {
            string uiScenePath = EditorPathUtil.GetUIScenesPath();
            m_UISceneFiles = GameFrameWork.Utilities.FileUtil.GetFiles(uiScenePath, "*.unity");
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

                if (GUILayout.Button("´ò¿ª"))
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