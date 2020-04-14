using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRef))]
public class UIRefEditor : Editor
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

        SerializedProperty useObjName = FrameWorkEditorMgr.DrawProperty("使用默认字段名", serializedObject, "m_UseObjName", new GUILayoutOption[0]);
        if(m_UIRef.UseObjName != useObjName.boolValue)
        {
            m_UIRef.UseObjName = useObjName.boolValue;
            EditorUtility.SetDirty(this.m_UIRef);
        }

        if (useObjName.boolValue)
        {
            m_UIRef.SetObjName(m_UIRef.gameObject.name);
            foreach (UIRef current in GetOtherRef(m_UIRef))
            {
                if (current == m_UIRef || !current.UseObjName) continue;

                current.UseObjName = false;
                current.SetName(m_UIRef.gameObject.name);
                break;
            }
        }
        else
        {
            if(string.IsNullOrEmpty(m_UIRef.Name))
            {
                m_UIRef.SetName(m_UIRef.gameObject.name);
            }

            string name = EditorGUILayout.TextField("字段名称", m_UIRef.Name, new GUILayoutOption[0]);
            if(m_UIRef.Name != name)
            {
                EditorUtility.SetDirty(this.m_UIRef);
            }
            m_UIRef.SetName(name);
        }
        
        int currIndex = m_ListCompName.IndexOf(m_UIRef.ComponentName);
        if (currIndex < 0) currIndex = 0;
        int index = EditorGUILayout.Popup("引用的组件", currIndex, m_ListCompName.ToArray(), new GUILayoutOption[0]);
        if(currIndex != index)
        {
            EditorUtility.SetDirty(m_UIRef);
            m_UIRef.ComponentName = m_ListCompName[index];
        }

        string desc = EditorGUILayout.TextField("描述", m_UIRef.Desc, new GUILayoutOption[0]);
        if(m_UIRef.Desc!=desc)
        {
            EditorUtility.SetDirty(this.m_UIRef);
            m_UIRef.Desc = desc;
        }

        SerializedProperty isLayoutItem = FrameWorkEditorMgr.DrawProperty("列表格子成员", serializedObject, "m_IsLayoutItem", new GUILayoutOption[0]);
        if(m_UIRef.IsLayoutItem != isLayoutItem.boolValue)
        {
            EditorUtility.SetDirty(m_UIRef);
            m_UIRef.IsLayoutItem = isLayoutItem.boolValue;
        }

        SerializedProperty isCopyRefStr = FrameWorkEditorMgr.DrawProperty("引用代码输出到剪切板", serializedObject, "m_IsCopyRefStr", new GUILayoutOption[0]);
        if(m_UIRef.IsCopyRefStr != isCopyRefStr.boolValue)
        {
            EditorUtility.SetDirty(m_UIRef);
            m_UIRef.IsCopyRefStr = isCopyRefStr.boolValue;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static UIRef[] GetOtherRef(UIRef uiref)
    {
        UIRef[] ret = uiref.gameObject.GetComponents<UIRef>();
        return ret;
    }

    public static string GetUniqueName(string name, IEnumerable<string> array)
    {
        int num = 1;
        string text = name;

        foreach (string current in array)
        {
            if (current == text)
            {
                text = string.Format("{0} {1}", name, num++);
            }
        }

        return text;
    }

    private List<string> m_ListCompName = new List<string>();
    private UIRef m_UIRef;
}
