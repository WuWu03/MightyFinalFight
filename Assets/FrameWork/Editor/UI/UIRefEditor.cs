using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIRef))]
public class UIRefEditor : Editor
{
    private UIRef m_Target;
    private string m_PrevName = string.Empty;

    public void OnEnable()
    {
        this.m_Target = (target as UIRef);
        if (string.IsNullOrEmpty(this.m_PrevName))
        {
            this.m_PrevName = this.m_Target.Name;
        }
    }

    public override void OnInspectorGUI()
    {
        GUI.color = Color.green;
        EditorGUILayout.LabelField(this.m_Target.GetName(), new GUILayoutOption[0]);
        GUI.color = Color.white;
        EditorGUI.BeginChangeCheck();
        this.m_Target.UseObjName = EditorGUILayout.Toggle("使用默认字段名", this.m_Target.UseObjName, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            if (this.m_Target.UseObjName)
            {
                this.m_Target.SetObjName(this.m_Target.gameObject.name);
                foreach (UIRef current in UIRefEditor.GetOtherRef(this.m_Target))
                {
                    if (current.UseObjName)
                    {
                        current.UseObjName = false;
                        current.SetName(this.m_Target.gameObject.name);
                        break;
                    }
                }
            }
            else
            {
                this.m_Target.SetName(this.m_Target.gameObject.name);
            }
            EditorUtility.SetDirty(this.m_Target);
        }

        if (!this.m_Target.UseObjName)
        {
            EditorGUI.BeginChangeCheck();
            string name = EditorGUILayout.TextField("字段名称", this.m_Target.Name, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Target.SetName(name);
                EditorUtility.SetDirty(this.m_Target);
            }
        }

        Component[] components = this.m_Target.GetComponents<Component>();
        List<string> list = new List<string>(components.Length);
        list.Add(typeof(GameObject).Name);

        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (!(component is UIRef))
            {
                list.Add(component.GetType().Name);
            }
        }
        EditorGUI.BeginChangeCheck();
        int index = EditorGUILayout.Popup("引用的组件", list.IndexOf(this.m_Target.ComponentName), list.ToArray(), new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            this.m_Target.ComponentName = list[index];
            EditorUtility.SetDirty(this.m_Target);
        }
        EditorGUI.BeginChangeCheck();
        this.m_Target.Desc = EditorGUILayout.TextField("描述", this.m_Target.Desc, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this.m_Target);
        }
        EditorGUI.BeginChangeCheck();
        this.m_Target.OutputClipBoard = EditorGUILayout.Toggle("引用代码输出到剪切板", this.m_Target.OutputClipBoard, new GUILayoutOption[0]);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this.m_Target);
        }
    }

    public static IEnumerable<UIRef> GetOtherRef(UIRef uiref)
    {
        List<UIRef> list = uiref.gameObject.GetComponents<UIRef>().ToList<UIRef>();
        list.Remove(uiref);
        return list;
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
}
