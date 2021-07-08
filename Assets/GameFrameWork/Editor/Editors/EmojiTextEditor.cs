using GameFrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(EmojiText))]
    public class EmojiTextEditor : UnityEditor.UI.TextEditor
    {
        SerializedProperty m_Text;
        SerializedProperty m_FontData;
        SerializedProperty hrefColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            hrefColor = serializedObject.FindProperty("HrefColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            EditorGUILayout.PropertyField(hrefColor);

            AppearanceControlsGUI();
            RaycastControlsGUI();
            MaskableControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }

    }
}
