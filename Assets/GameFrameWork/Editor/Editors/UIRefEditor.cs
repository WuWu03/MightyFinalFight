using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRef))]
    public class UIRefEditor :UnityEditor.Editor
    {
        public void OnEnable()
        {
            m_ListCompName.Clear();
            m_UIRef = (target as UIRef);
            Component[] components = m_UIRef.GetComponents<Component>();
            m_ListCompName.Add(typeof(GameObject).Name);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is UIRef) continue;
                m_ListCompName.Add(components[i].GetType().Name);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.color = Color.green;
            EditorGUILayout.LabelField(m_UIRef.GetName(), new GUILayoutOption[0]);
            GUI.color = Color.white;
            EditorGUI.BeginChangeCheck();

            SerializedProperty useDefaultName = EditorUtility.DrawProperty("使用默认字段名", serializedObject, "m_UseDefaultName", new GUILayoutOption[0]);
            if (m_UIRef.UseDefaultName != useDefaultName.boolValue)
            {
                m_UIRef.UseDefaultName = useDefaultName.boolValue;
                UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
            }

            if (useDefaultName.boolValue)
            {
                m_UIRef.SetName(m_UIRef.gameObject.name);
            }
            else
            {
                if (string.IsNullOrEmpty(m_UIRef.Name))
                {
                    m_UIRef.SetName(m_UIRef.gameObject.name);
                }

                string name = EditorGUILayout.TextField("字段名称", m_UIRef.Name, new GUILayoutOption[0]);

                if (m_UIRef.Name != name)
                {
                    UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
                }

                m_UIRef.SetName(name);
            }

            int currIndex = m_ListCompName.IndexOf(m_UIRef.ComponentName);
            if (currIndex < 0) currIndex = 0;
            int index = EditorGUILayout.Popup("引用的组件", currIndex, m_ListCompName.ToArray(), new GUILayoutOption[0]);
            if (currIndex != index)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.ComponentName = m_ListCompName[index];
            }

            string desc = EditorGUILayout.TextField("描述", m_UIRef.Desc, new GUILayoutOption[0]);
            if (m_UIRef.Desc != desc)
            {
                UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
                m_UIRef.Desc = desc;
            }

            if (m_UIRef.IsLayoutContent())
            {
                SerializedProperty isLoopLayout = EditorUtility.DrawProperty("循环列表", serializedObject, "m_IsLoopLayout", new GUILayoutOption[0]);
                if (m_UIRef.IsLoopLayout != isLoopLayout.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsLoopLayout = isLoopLayout.boolValue;
                }
            }
            else m_UIRef.IsLoopLayout = false;

            UIRef parentLayoutRef = m_UIRef.transform.parent == null ? null : m_UIRef.transform.parent.GetComponent<UIRef>();
            if (parentLayoutRef != null && parentLayoutRef.IsLayoutContent())
            {
                SerializedProperty isLayoutItem = EditorUtility.DrawProperty("列表格子", serializedObject, "m_IsLayoutItem", new GUILayoutOption[0]);
                if (m_UIRef.IsLayoutItem != isLayoutItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsLayoutItem = isLayoutItem.boolValue;
                }
            }
            else m_UIRef.IsLayoutItem = false;


            UIRef[] parentLayoutItemRefs = m_UIRef.transform.parent == null ? null : m_UIRef.GetComponentsInParent<UIRef>(true);
            bool isParentLayoutItem = false;
            for (int i = 1; i < parentLayoutItemRefs.Length; i++)
            {
                if (parentLayoutItemRefs[i].IsLayoutItem)
                {
                    isParentLayoutItem = true;
                    break;
                }
            }
            if (isParentLayoutItem)
            {
                SerializedProperty isLayoutItem = EditorUtility.DrawProperty("列表格子成员", serializedObject, "m_IsLayoutItemVariable", new GUILayoutOption[0]);
                if (m_UIRef.IsLayoutItemVariable != isLayoutItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsLayoutItemVariable = isLayoutItem.boolValue && isParentLayoutItem;
                }
            }
            else
            {
                m_UIRef.IsLayoutItemVariable = false;
            }

            if (!m_UIRef.IsLayoutItemVariable)
            {
                SerializedProperty isCopyRefStr = EditorUtility.DrawProperty("引用代码输出到剪切板", serializedObject, "m_IsCopyRefStr", new GUILayoutOption[0]);
                if (m_UIRef.IsCopyRefStr != isCopyRefStr.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsCopyRefStr = isCopyRefStr.boolValue;
                }
            }
            else
            {
                m_UIRef.IsCopyRefStr = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static UIRef[] GetOtherRef(UIRef uiref)
        {
            UIRef[] ret = uiref.gameObject.GetComponents<UIRef>();
            return ret;
        }

        private List<string> m_ListCompName = new List<string>();
        private UIRef m_UIRef;
    }
}