using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GameFrameWork.Editor
{
    public class PlayerPrefsTool : EditorWindow
    {
        private void OnEnable()
        {
            m_ListInputKeys = new List<string>();
        }

        private void OnDisable()
        {
            m_ListInputKeys.Clear();
            m_InputCount = 0;
            m_ListInputKeys = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            int inputCount = Mathf.Clamp(EditorGUILayout.IntField("输入条目", m_InputCount), 1, 100);

            if (inputCount != m_InputCount)
            {
                m_InputCount = inputCount;
                m_ListInputKeys.Clear();

                for (int i = 0; i < inputCount; i++)
                {
                    m_ListInputKeys.Add(string.Empty);
                }
            }

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            for (int i = 0; i < m_InputCount; i++)
            {
                m_ListInputKeys[i] = EditorGUILayout.TextField((i + 1).ToString(), m_ListInputKeys[i]);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("清除"))
            {
                List<string> invalidKeys = new List<string>();

                for (int i = m_InputCount - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(m_ListInputKeys[i]))
                    {
                        m_ListInputKeys.RemoveAt(i);
                        m_InputCount--;
                        continue;
                    }

                    if (PlayerPrefs.HasKey(m_ListInputKeys[i]))
                    {
                        PlayerPrefs.DeleteKey(m_ListInputKeys[i]);
                        m_ListInputKeys.RemoveAt(i);
                        m_InputCount--;
                    }
                    else
                    {
                        invalidKeys.Add(m_ListInputKeys[i]);
                    }
                }

                if (invalidKeys.Count > 0)
                {
                    invalidKeys.Reverse();
                    string notificationStr = string.Empty;

                    for (int i = 0; i < invalidKeys.Count; i++)
                    {
                        notificationStr += "Key : " + invalidKeys[i] + "\n";
                    }

                    notificationStr += "不存在，请检查key值是否错误";
                    EditorUtility.DisplayDialog("提示", notificationStr, "确定");
                }
                else
                {
                    ShowNotification(new GUIContent("清除成功"));
                }

                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndVertical();
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private List<string> m_ListInputKeys = null;
        private int m_InputCount = 0;
    }
}