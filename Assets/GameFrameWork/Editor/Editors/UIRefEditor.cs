using System.Collections.Generic;
using GameFrameWork.UI;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRef))]
    public class UIRefEditor : UnityEditor.Editor
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
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
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
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                }

                m_UIRef.SetName(name);
            }

            int currIndex = Mathf.Max(m_ListCompName.IndexOf(m_UIRef.componentName), 0);
            int index = EditorGUILayout.Popup("引用的组件", currIndex, m_ListCompName.ToArray(), new GUILayoutOption[0]);
            
            if (currIndex != index)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.componentName = m_ListCompName[index];
            }

            string desc = EditorGUILayout.TextField("描述", m_UIRef.desc, new GUILayoutOption[0]);
            if (m_UIRef.desc != desc)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.desc = desc;
            }

            if (m_UIRef.IsLayoutGroupView() || m_UIRef.IsScollLayoutGroupView())
            {
                SerializedProperty isLayout = EditorUtil.DrawProperty("列表", serializedObject, "m_IsLayout", new GUILayoutOption[0]);
                if (m_UIRef.isLayout != isLayout.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayout = isLayout.boolValue;

                    if (!m_UIRef.isLayout)
                    {
                        UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                        for (int i = 1; i < childrenRefs.Length; i++)
                        {
                            UIRef child1 = childrenRefs[i];
                            child1.isLayoutItem = false;
                            UIRef[] childrenRefs2 = child1.GetComponentsInChildren<UIRef>(true);
                            for (int j = 1; j < childrenRefs2.Length; j++)
                            {
                                UIRef child2 = childrenRefs2[i];
                                child2.isLayoutItemVariable = false;
                            }
                        }
                    }
                }
            }
            else
            {
                m_UIRef.isLayout = false;
                UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                for (int i = 1; i < childrenRefs.Length; i++)
                {
                    UIRef child1 = childrenRefs[i];
                    child1.isLayoutItem = false;
                    UIRef[] childrenRefs2 = child1.GetComponentsInChildren<UIRef>(true);
                    for (int j = 1; j < childrenRefs2.Length; j++)
                    {
                        UIRef child2 = childrenRefs2[i];
                        child2.isLayoutItemVariable = false;
                    }
                }
            }

            if (m_UIRef.IsLayoutItem() && !m_UIRef.IsLayoutItemVariable())
            {
                SerializedProperty isLayoutItem = EditorUtil.DrawProperty("列表格子", serializedObject, "m_IsLayoutItem", new GUILayoutOption[0]);
                if (m_UIRef.isLayoutItem != isLayoutItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayoutItem = isLayoutItem.boolValue;

                    if (m_UIRef.isLayoutItem)
                    {
                        m_UIRef.gameObject.GetOrAddComponent<UIRefRoot>();
                    }
                    else
                    {
                        UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                        for (int i = 1; i < childrenRefs.Length; i++)
                        {
                            UIRef child1 = childrenRefs[i];
                            child1.isLayoutItemVariable = false;
                        }

                        if (m_UIRef.gameObject.TryGetComponent<UIRefRoot>(out var uiRefRoot))
                        {
                            GameObject.DestroyImmediate(uiRefRoot);
                        }
                    }
                }
            }
            else
            {
                m_UIRef.isLayoutItem = false;
                UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                for (int i = 1; i < childrenRefs.Length; i++)
                {
                    UIRef child1 = childrenRefs[i];
                    child1.isLayoutItemVariable = false;
                }
                if (m_UIRef.gameObject.TryGetComponent<UIRefRoot>(out var uiRefRoot))
                {
                    GameObject.DestroyImmediate(uiRefRoot);
                }
            }

            if (m_UIRef.IsLayoutItemVariable())
            {
                SerializedProperty isLayoutItemVariable = EditorUtil.DrawProperty("列表格子成员", serializedObject, "m_IsLayoutItemVariable", new GUILayoutOption[0]);
                if (m_UIRef.isLayoutItemVariable != isLayoutItemVariable.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isLayoutItemVariable = isLayoutItemVariable.boolValue;
                }
            }
            else
            {
                m_UIRef.isLayoutItemVariable = false;
            }

            if (!m_UIRef.isLayoutItemVariable)
            {
                SerializedProperty isCopyRefStr = EditorUtil.DrawProperty("复制引用", serializedObject, "m_IsCopyRefStr", new GUILayoutOption[0]);
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