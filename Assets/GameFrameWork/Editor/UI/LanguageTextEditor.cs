using GameFrameWork.UI;
using GameFrameWork.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(LanguageText))]
    public class LanguageTextEditor : UnityEditor.Editor
    {
        private string[] m_PopKeys;
        private int m_SelectedIndex = -1;
        private static List<string> s_LanguageTextKeys;
        private LanguageText m_LanguageText;
        
        void OnEnable()
        {
            m_LanguageText = (target as LanguageText);
            m_SelectedIndex = -1;

            if (s_LanguageTextKeys == null)
            {
                string languageKeyPath = EditorMgr.GetGameFrameWorkConfig().languageKeyFilePath;
                string languageKeyFullPath = PathUtil.GetAssetFullPath(languageKeyPath);

                if (!string.IsNullOrEmpty(languageKeyPath) && System.IO.File.Exists(languageKeyFullPath))
                {
                    s_LanguageTextKeys = new List<string>();
                    s_LanguageTextKeys.AddRange(System.IO.File.ReadAllLines(languageKeyFullPath, System.Text.Encoding.UTF8));
                    s_LanguageTextKeys.Sort(StringComparer.Ordinal);
                }
            }
        }

        private void OnDisable()
        {
            m_LanguageText = null;
            m_SelectedIndex = -1;
            s_LanguageTextKeys = null;
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty languageTextKey = serializedObject.FindProperty("languageTextKey");

            if (s_LanguageTextKeys is { Count: > 0 })
            {
                if (!string.IsNullOrEmpty(m_LanguageText.languageTextKey))
                {
                    GUI.color = s_LanguageTextKeys.Contains(m_LanguageText.languageTextKey) ? Color.green : Color.red;
                }
                else
                {
                    GUI.color = Color.white;
                }
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

                if (s_LanguageTextKeys is { Count: > 0 })
                {
                    m_PopKeys = s_LanguageTextKeys.FindAll(key => key.ToLower().StartsWith(languageTextKey.stringValue.ToLower())).ToArray();
                }

                m_SelectedIndex = -1;
            }

            m_LanguageText.languageTextKey = languageTextKey.stringValue;

            if (m_PopKeys is { Length: > 0 })
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
    }
}