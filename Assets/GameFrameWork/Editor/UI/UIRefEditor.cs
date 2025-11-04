using System.Collections.Generic;
using GameFrameWork.UI;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRef))]
    public class UIRefEditor : UnityEditor.Editor
    {
        private readonly List<string> m_CompNames = new();
        private UIRef m_UIRef;
        
        public void OnEnable()
        {
            m_CompNames.Clear();
            m_UIRef = target as UIRef;
            m_CompNames.Add(nameof(GameObject));

            if (m_UIRef is null)
            {
                throw new GameFrameWorkException("UIRef组件为空");
            }

            Component[] components = m_UIRef.GetComponents<Component>();

            if (components is { Length: > 0 })
            {
                foreach (var component in components)
                {
                    if (component is UIRef) continue;
                    m_CompNames.Add(component.GetType().Name);
                }
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

                string refName = EditorGUILayout.TextField("字段名称", m_UIRef.refName, new GUILayoutOption[0]);

                if (m_UIRef.refName != refName)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                }

                m_UIRef.SetName(refName);
            }

            int currIndex = Mathf.Max(m_CompNames.IndexOf(m_UIRef.componentName), 0);
            int index = EditorGUILayout.Popup("引用的组件", currIndex, m_CompNames.ToArray(), new GUILayoutOption[0]);
            
            if (currIndex != index)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.componentName = m_CompNames[index];
            }

            string desc = EditorGUILayout.TextField("描述", m_UIRef.desc, new GUILayoutOption[0]);
            if (m_UIRef.desc != desc)
            {
                UnityEditor.EditorUtility.SetDirty(m_UIRef);
                m_UIRef.desc = desc;
            }

            if (m_UIRef.IsStaticList() || m_UIRef.IsScrollList())
            {
                SerializedProperty isList = EditorUtil.DrawProperty("列表", serializedObject, "m_IsList", new GUILayoutOption[0]);
                if (m_UIRef.IsList != isList.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsList = isList.boolValue;

                    if (!m_UIRef.IsList)
                    {
                        UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                        for (int i = 1; i < childrenRefs.Length; i++)
                        {
                            UIRef child1 = childrenRefs[i];
                            child1.isListItem = false;
                            UIRef[] childrenRefs2 = child1.GetComponentsInChildren<UIRef>(true);
                            for (int j = 1; j < childrenRefs2.Length; j++)
                            {
                                UIRef child2 = childrenRefs2[i];
                                child2.IsListItemVariable = false;
                            }
                        }
                    }
                }
            }
            else
            {
                m_UIRef.IsList = false;
                UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                for (int i = 1; i < childrenRefs.Length; i++)
                {
                    UIRef child1 = childrenRefs[i];
                    child1.isListItem = false;
                    UIRef[] childrenRefs2 = child1.GetComponentsInChildren<UIRef>(true);
                    for (int j = 1; j < childrenRefs2.Length; j++)
                    {
                        UIRef child2 = childrenRefs2[i];
                        child2.IsListItemVariable = false;
                    }
                }
            }

            if (m_UIRef.IsListItem() && !m_UIRef.IsListItemVariable())
            {
                SerializedProperty isListItem = EditorUtil.DrawProperty("列表格子", serializedObject, "m_IsListItem", new GUILayoutOption[0]);
                if (m_UIRef.isListItem != isListItem.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isListItem = isListItem.boolValue;

                    if (m_UIRef.isListItem)
                    {
                        m_UIRef.gameObject.GetOrAddComponent<UIRefRoot>();
                    }
                    else
                    {
                        UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                        for (int i = 1; i < childrenRefs.Length; i++)
                        {
                            UIRef child1 = childrenRefs[i];
                            child1.IsListItemVariable = false;
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
                m_UIRef.isListItem = false;
                UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                for (int i = 1; i < childrenRefs.Length; i++)
                {
                    UIRef child1 = childrenRefs[i];
                    child1.IsListItemVariable = false;
                }
                if (m_UIRef.gameObject.TryGetComponent<UIRefRoot>(out var uiRefRoot))
                {
                    GameObject.DestroyImmediate(uiRefRoot);
                }
            }

            if (m_UIRef.IsListItemVariable())
            {
                SerializedProperty isListItemVariable = EditorUtil.DrawProperty("列表格子成员", serializedObject, "m_IsListItemVariable", new GUILayoutOption[0]);
                if (m_UIRef.IsListItemVariable != isListItemVariable.boolValue)
                {
                    UnityEditor.EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.IsListItemVariable = isListItemVariable.boolValue;
                }
            }
            else
            {
                m_UIRef.IsListItemVariable = false;
            }

            if (!m_UIRef.IsListItemVariable)
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
    }
}