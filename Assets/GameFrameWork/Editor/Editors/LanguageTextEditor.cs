using GameFrameWork.UI;
using System;
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

            if (m_LanguageTextKeys == null)
            {
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Editor/Config/LanguageKeys.txt");

                if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
                {
                    m_LanguageTextKeys = textAsset.text.Split("\r\n", System.StringSplitOptions.RemoveEmptyEntries);
                    Array.Sort(m_LanguageTextKeys, StringComparer.Ordinal);
                }
            }

            if (!string.IsNullOrEmpty(m_LanguageText.languageTextKey))
            {
                m_SelectedIndex = System.Array.IndexOf(m_LanguageTextKeys, m_LanguageText.languageTextKey);
            }
            else
            {
                m_SelectedIndex = -1;
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
            int select = EditorGUILayout.Popup(m_SelectedIndex, m_LanguageTextKeys);

            if(select != m_SelectedIndex)
            {
                m_SelectedIndex = select;
                m_LanguageText.languageTextKey = m_LanguageTextKeys[m_SelectedIndex];
                EditorUtility.SetDirty(m_LanguageText);
            }
        }

        private int m_SelectedIndex = -1;
        private static string[] m_LanguageTextKeys = null;
        private LanguageText m_LanguageText;
    }
}