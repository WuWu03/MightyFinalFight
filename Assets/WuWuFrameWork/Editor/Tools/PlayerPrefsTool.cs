using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace WuWuFramework.Editor
{
    public class PlayerPrefsTool : EditorWindow
    {
        private void OnEnable()
        {
            m_ListInputKeys = Serialize.PlayerPrefs.GetSaveKeys();
            m_ListDeleteKeys = new List<string>();
        }

        private void OnDisable()
        {
            m_ListDeleteKeys.Clear();
            m_ListDeleteKeys = null;

            if (m_ListInputKeys == null)
            {
                return;
            }

            m_ListInputKeys.Clear();
            m_ListInputKeys = null;
        }

        private void OnGUI()
        {
            if (m_ListDeleteKeys.Count > 0 && m_ListDeleteKeys != null && m_ListDeleteKeys.Count > 0)
            {
                for (int i = 0; i < m_ListDeleteKeys.Count; i++)
                {
                    m_ListInputKeys.Remove(m_ListDeleteKeys[i]);
                }

                m_ListDeleteKeys.Clear();
            }
   
            if(m_ListInputKeys == null || m_ListInputKeys.Count == 0)
            {
               GUI.Label(new Rect((this.position.width - 120f)/2f, 0, this.position.width, this.position.height), "[PlayerPrefs]数据为空。");
            }
            else
            {
                EditorGUILayout.BeginVertical();
                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

                for (int i = 0; i < m_ListInputKeys.Count; i++)
                {
                    EditorUtil.GUIBoxScope(() =>
                    {
                        EditorGUILayout.BeginHorizontal();
                        string key = m_ListInputKeys[i];
                        string stringValue = PlayerPrefs.GetString(key, string.Empty);
                        string intValue = PlayerPrefs.GetInt(key, 0).ToString();
                        string floatValue = PlayerPrefs.GetFloat(key, 0f).ToString("F2");

                        string content = string.Format("键：{0}    字符串值：{1}    整型值：{2}    浮点值：{3}", key, string.IsNullOrEmpty(stringValue) ? "空" : stringValue, intValue, floatValue);
                        EditorGUILayout.LabelField(content);

                        if (GUILayout.Button("删除", GUILayout.Width(50f)))
                        {
                            Serialize.PlayerPrefs.DeleteKey(m_ListInputKeys[i]);
                            m_ListDeleteKeys.Add(m_ListInputKeys[i]);
                            ShowNotification(new GUIContent("成功删除 PlayerPrefs: " + m_ListInputKeys[i]));
                        }

                        EditorGUILayout.EndHorizontal();
                    });
                }

                EditorGUILayout.EndScrollView();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("清除所有"))
                {
                    Serialize.PlayerPrefs.DeleteAll();
                    m_ListInputKeys.Clear();
                    ShowNotification(new GUIContent("成功清除所有 PlayerPrefs"));
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndVertical();
            }
        }

        private Vector2 m_ScrollPos = Vector2.zero;
        private List<string> m_ListInputKeys = null;
        private List<string> m_ListDeleteKeys = null;
    }
}