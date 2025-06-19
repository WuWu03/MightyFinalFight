using GameFrameWork.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(LanguageText))]
    public class LanguageTextEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            m_LanguageText = (target as LanguageText);
            m_SelectedIndex = -1;

            if (m_LanguageTextKeys == null)
            {
                string languageKeyPath = PlayerPrefs.GetString("unity_editor_language_key_file", string.Empty);

                if (!string.IsNullOrEmpty(languageKeyPath) && System.IO.File.Exists(languageKeyPath))
                {
                    m_LanguageTextKeys = new List<string>();
                    m_LanguageTextKeys.AddRange(System.IO.File.ReadAllLines(languageKeyPath, System.Text.Encoding.UTF8));
                    m_LanguageTextKeys.Sort(StringComparer.Ordinal);
                }
            }
        }

        private void OnDisable()
        {
            m_LanguageText = null;
            m_SelectedIndex = -1;
            m_LanguageTextKeys = null;
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty languageTextKey = serializedObject.FindProperty("languageTextKey");

            if (m_LanguageTextKeys != null && m_LanguageTextKeys.Count > 0)
            {
                GUI.color = m_LanguageTextKeys.Contains(m_LanguageText.languageTextKey) ? Color.green : Color.red;
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("多语言检测文件不存在，请设置多语言检测文件", EditorStyles.boldLabel);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(languageTextKey);

            if (languageTextKey.stringValue != m_LanguageText.languageTextKey)
            {
                EditorUtility.SetDirty(target);

                if (m_LanguageTextKeys != null && m_LanguageTextKeys.Count > 0)
                {
                    m_PopKeys = m_LanguageTextKeys.FindAll(key => key.ToLower().StartsWith(languageTextKey.stringValue.ToLower())).ToArray();
                }

                m_SelectedIndex = -1;
            }

            m_LanguageText.languageTextKey = languageTextKey.stringValue;

            if (m_PopKeys != null && m_PopKeys.Length > 0)
            {
                int select = EditorGUILayout.Popup(m_SelectedIndex, m_PopKeys);
                if (select != m_SelectedIndex)
                {
                    m_SelectedIndex = select;
                    m_LanguageText.languageTextKey = m_PopKeys[select];
                    m_PopKeys = null;
                    m_SelectedIndex = -1;
                }
            }
        }

        private string[] m_PopKeys = null;
        private int m_SelectedIndex = -1;
        private static List<string> m_LanguageTextKeys = null;
        private LanguageText m_LanguageText;
    }
}