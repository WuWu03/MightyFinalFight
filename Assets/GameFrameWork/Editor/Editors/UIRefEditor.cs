using GameFrameWork.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

            SerializedProperty useDefaultName = EditorUtil.DrawProperty("使用默认字段名", serializedObject, "m_UseDefaultName", new GUILayoutOption[0]);
            if (m_UIRef.useDefaultName != useDefaultName.boolValue)
            {
                m_UIRef.useDefaultName = useDefaultName.boolValue;
                UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
            }

            if (useDefaultName.boolValue)
            {
                m_UIRef.SetName(m_UIRef.gameObject.name);
            }
            else
            {
                if (string.IsNullOrEmpty(m_UIRef.refName))
                {
                    m_UIRef.SetName(m_UIRef.gameObject.name);
                }

                string name = EditorGUILayout.TextField("字段名称", m_UIRef.refName, new GUILayoutOption[0]);

                if (m_UIRef.refName != name)
                {
                    UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
                }

                m_UIRef.SetName(name);
            }

            int currIndex = m_ListCompName.IndexOf(m_UIRef.componentName);
            if (currIndex < 0) currIndex = 0;
            int index = EditorGUILayout.Popup("引用的组件", currIndex, m_ListCompName.ToArray(), new GUILayoutOption[0]);
            if (currIndex != index)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.componentName = m_ListCompName[index];
            }

            string desc = EditorGUILayout.TextField("描述", m_UIRef.desc, new GUILayoutOption[0]);
            if (m_UIRef.desc != desc)
            {
                UnityEditor.EditorUtility.SetDirty(this.m_UIRef);
                m_UIRef.desc = desc;
            }

            if (m_UIRef.IsLayoutContent())
            {
                SerializedProperty isLayout = EditorUtil.DrawProperty("列表", serializedObject, "m_IsLayout", new GUILayoutOption[0]);
                if (m_UIRef.isLayout != isLayout.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayout = isLayout.boolValue;
                }
            }
            else
            {
                m_UIRef.isLayout = false;
            }

            UIRef parentLayoutRef = m_UIRef.transform.parent == null ? null : m_UIRef.transform.parent.GetComponent<UIRef>();
            if (parentLayoutRef != null && parentLayoutRef.IsLayoutContent())
            {
                SerializedProperty isLayoutItem = EditorUtil.DrawProperty("列表格子", serializedObject, "m_IsLayoutItem", new GUILayoutOption[0]);
                if (m_UIRef.isLayoutItem != isLayoutItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayoutItem = isLayoutItem.boolValue;
                }
            }
            else
            {
                m_UIRef.isLayoutItem = false;
            }

            UIRef[] parentLayoutItemRefs = m_UIRef.transform.parent == null ? null : m_UIRef.GetComponentsInParent<UIRef>(true);
            bool isParentLayoutItem = false;
            for (int i = 1; i < parentLayoutItemRefs.Length; i++)
            {
                if (parentLayoutItemRefs[i].isLayoutItem)
                {
                    isParentLayoutItem = true;
                    break;
                }
            }
            if (isParentLayoutItem)
            {
                SerializedProperty isLayoutItem = EditorUtil.DrawProperty("列表格子成员", serializedObject, "m_IsLayoutItemVariable", new GUILayoutOption[0]);
                if (m_UIRef.isLayoutItemVariable != isLayoutItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayoutItemVariable = isLayoutItem.boolValue && isParentLayoutItem;
                }
            }
            else
            {
                m_UIRef.isLayoutItemVariable = false;
            }

            if (m_UIRef.IsScrollRect())
            {
                SerializedProperty isLoopLayout = EditorUtil.DrawProperty("循环滚动", serializedObject, "m_IsScrollLayout", new GUILayoutOption[0]);
                if (m_UIRef.isScrollLayout != isLoopLayout.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isScrollLayout = isLoopLayout.boolValue;
                }

                if (m_UIRef.isScrollLayout && m_UIRef.GetComponent<ScrollLayoutGroupView>() == null)
                {
                    m_UIRef.gameObject.AddComponent<ScrollLayoutGroupView>();
                }
            }
            else
            {
                m_UIRef.isScrollLayout = false;
            }

            if (!m_UIRef.isLayoutItemVariable)
            {
                SerializedProperty isCopyRefStr = EditorUtil.DrawProperty("引用代码输出到剪切板", serializedObject, "m_IsCopyRefStr", new GUILayoutOption[0]);
                if (m_UIRef.isCopyRefStr != isCopyRefStr.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isCopyRefStr = isCopyRefStr.boolValue;
                }
            }
            else
            {
                m_UIRef.isCopyRefStr = false;
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