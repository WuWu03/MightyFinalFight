using System.Collections.Generic;
using GameFrameWork.UI;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(UIRef))]
    public class UIRefEditor : UnityEditor.Editor
    {
        private readonly List<string> m_ComponentFullNames = new();
        private readonly List<string> m_ComponentNames = new();
        private UIRef m_UIRef;
        
        public void OnEnable()
        {
            m_ComponentFullNames.Clear();
            m_UIRef = target as UIRef;
            m_ComponentNames.Add(nameof(GameObject));
            m_ComponentFullNames.Add(typeof(GameObject).FullName);
            
            if (m_UIRef is null)
            {
                throw new GameFrameWorkException("UIRef组件为空");
            }

            Component[] components = m_UIRef.GetComponents<Component>();

            if (components is { Length: > 0 })
            {
                foreach (var component in components)
                {
                    if (component is UIRef)
                    {
                        continue;
                    }
                    
                    m_ComponentNames.Add(component.GetType().Name);
                    m_ComponentFullNames.Add(component.GetType().FullName);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.color = Color.green;
            EditorGUILayout.LabelField(m_UIRef.GetName());
            GUI.color = Color.white;
            EditorGUI.BeginChangeCheck();

            SerializedProperty useDefaultName = EditorUtil.DrawProperty("使用默认字段名", serializedObject, "m_UseDefaultName");
            if (m_UIRef.useDefaultName != useDefaultName.boolValue)
            {
                m_UIRef.useDefaultName = useDefaultName.boolValue;
                EditorUtility.SetDirty(m_UIRef);
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

                string refName = EditorGUILayout.TextField("字段名称", m_UIRef.refName);

                if (m_UIRef.refName != refName)
                {
                    EditorUtility.SetDirty(m_UIRef);
                }

                m_UIRef.SetName(refName);
            }

            int currIndex = Mathf.Max(m_ComponentNames.IndexOf(m_UIRef.componentName), 0);
            int index = EditorGUILayout.Popup("引用的组件", currIndex, m_ComponentNames.ToArray());
            
            if (currIndex != index)
            {
                EditorUtility.SetDirty(m_UIRef);
                m_UIRef.componentName = m_ComponentNames[index];
                m_UIRef.componentFullName = m_ComponentFullNames[index];
            }

            string desc = EditorGUILayout.TextField("描述", m_UIRef.desc);
            
            if (m_UIRef.desc != desc)
            {
                EditorUtility.SetDirty(m_UIRef);
                m_UIRef.desc = desc;
            }

            if (m_UIRef.IsStaticList() || m_UIRef.IsScrollList())
            {
                SerializedProperty isList = EditorUtil.DrawProperty("列表", serializedObject, "m_IsList", new GUILayoutOption[0]);
                if (m_UIRef.isList != isList.boolValue)
                {
                    EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isList = isList.boolValue;

                    if (!m_UIRef.isList)
                    {
                        UIRef[] childrenRefs = m_UIRef.GetComponentsInChildren<UIRef>(true);
                        for (int i = 1; i < childrenRefs.Length; i++)
                        {
                            childrenRefs[i].isListItem = false;
                            childrenRefs[i].isListItemVariable = false;
                        }
                    }
                }
            }
            else
            {
                m_UIRef.isList = false;
            }

            if (m_UIRef.IsListItem() && !m_UIRef.IsListItemVariable())
            {
                SerializedProperty isListItem = EditorUtil.DrawProperty("列表格子", serializedObject, "m_IsListItem", new GUILayoutOption[0]);
                if (m_UIRef.isListItem != isListItem.boolValue)
                {
                    EditorUtility.SetDirty(m_UIRef);
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
                            childrenRefs[i].isListItemVariable = false;
                        }

                        if (m_UIRef.gameObject.TryGetComponent<UIRefRoot>(out var uiRefRoot))
                        {
                            DestroyImmediate(uiRefRoot);
                        }
                    }
                }
            }
            else
            {
                m_UIRef.isListItem = false;
                
                if (m_UIRef.gameObject.TryGetComponent<UIRefRoot>(out var uiRefRoot))
                {
                    DestroyImmediate(uiRefRoot);
                }
            }

            if (m_UIRef.IsListItemVariable())
            {
                SerializedProperty isListItemVariable = EditorUtil.DrawProperty("列表格子成员", serializedObject, "m_IsListItemVariable");
                if (m_UIRef.isListItemVariable != isListItemVariable.boolValue)
                {
                    EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isListItemVariable = isListItemVariable.boolValue;
                }
            }
            else
            {
                m_UIRef.isListItemVariable = false;
            }

            if (!m_UIRef.isListItemVariable)
            {
                SerializedProperty isCopyRefStr = EditorUtil.DrawProperty("复制引用", serializedObject, "m_IsCopyRefStr");
                if (m_UIRef.isCopyRefStr != isCopyRefStr.boolValue)
                {
                    EditorUtility.SetDirty(m_UIRef);
                    m_UIRef.isCopyRefStr = isCopyRefStr.boolValue;
                }
            }
            else
            {
                m_UIRef.isCopyRefStr = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}