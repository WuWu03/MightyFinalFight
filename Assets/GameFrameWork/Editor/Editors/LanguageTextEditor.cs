using GameFrameWork.UI;
using UnityEditor;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(LanguageText))]
    public class LanguageTextEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            m_LanguageText = (target as LanguageText);
        }

        public override void OnInspectorGUI()
        {
            SerializedProperty languageTextId = serializedObject.FindProperty("languageTextId");
            SerializedProperty languageTextKey = serializedObject.FindProperty("languageTextKey");
            SerializedProperty languageMode = serializedObject.FindProperty("languageMode");

            EditorGUILayout.PropertyField(languageMode);

            if (m_LanguageText.languageMode == LanguageText.LanguageMode.UseKey)
            {
                EditorGUILayout.PropertyField(languageTextKey);
            }
            else
            {
                EditorGUILayout.PropertyField(languageTextId);
            }

            if (m_LanguageText.languageMode != (LanguageText.LanguageMode)languageMode.enumValueIndex
                || m_LanguageText.languageTextId != languageTextId.intValue
                || m_LanguageText.languageTextKey != languageTextKey.stringValue)
            {
                UnityEditor.EditorUtility.SetDirty(target);
            }

            if (m_LanguageText.languageMode == LanguageText.LanguageMode.UseKey)
            {
                m_LanguageText.languageTextId = -1;
                m_LanguageText.languageTextKey = languageTextKey.stringValue;
            }
            else
            {
                m_LanguageText.languageTextKey = string.Empty;
                m_LanguageText.languageTextId = languageTextId.intValue;
            }

            m_LanguageText.languageMode = (LanguageText.LanguageMode)languageMode.enumValueIndex;
        }


        private LanguageText m_LanguageText = null;
    }
}
