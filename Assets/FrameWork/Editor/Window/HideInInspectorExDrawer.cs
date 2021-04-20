using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork
{
    [CustomPropertyDrawer(typeof(HideInInspectorExAttribute))]
    public class HideInInspectorExDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = !(attribute as HideInInspectorExAttribute).Condition;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}