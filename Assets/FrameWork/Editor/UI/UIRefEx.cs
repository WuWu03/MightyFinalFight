using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

public static class UIRefEx
{
    public static void SetName(this UIRef uiRef, string name)
    {
        UIRefRoot uiRefRoot = uiRef.gameObject.FindComponentInParents<UIRefRoot>();
        if (uiRefRoot == null)
        {
            Debug.LogError("没有 mUIRefSetting 组件");
            return;
        }

        List<string> list = new List<string>();
        UIRef[] components = uiRefRoot.GetComponentsInChildren<UIRef>(true);

        for (int i = 0; i < components.Length; i++)
        {
            UIRef component = components[i];
            if (component != uiRef && component.IsCopyRefStr == uiRef.IsCopyRefStr)
            {
                list.Add(component.UseObjName ? component.gameObject.name : component.Name);
            }
        }

        uiRef.Name = UIRefEditor.GetUniqueName(name.Trim(), list);
    }

    public static string GetName(this UIRef uiRef)
    {
        string str = string.Empty;
        if (!uiRef.IsLayoutContent())
        {
            if (string.IsNullOrEmpty(uiRef.ComponentName) || uiRef.ComponentName == typeof(Transform).Name)
            {
                str = "Trans";
            }
            else if (uiRef.ComponentName == typeof(RectTransform).Name)
            {
                str = "Rect";
            }
            else if (uiRef.ComponentName == typeof(GameObject).Name)
            {
                str = "GO";
            }
        }
        string text;
        if (uiRef.UseObjName) text = uiRef.gameObject.name;
        else text = uiRef.Name;


        if (string.IsNullOrEmpty(text)) return str;

        text = text.Replace(" ", "_").Replace("(", "_").Replace(")", "_");
        if (text[0] > 'a' && text[0] < 'z')
        {
			text = (char)(text[0] - ' ') + text.Substring(1);
        }
        return text + str;
    }

    public static void SetObjName(this UIRef uiRef, string name)
    {
        UIRefRoot uiRefRoot = uiRef.gameObject.FindComponentInParents<UIRefRoot>();//NGUITools.FindInParents<UIRefRoot>(uiRef.gameObject);
        if (uiRefRoot == null)
        {
            Debug.LogError("没有 mUIRefSetting 组件");
            return;
        }

        HashSet<string> hashSet = new HashSet<string>();
        UIRef[] children = uiRefRoot.GetComponentsInChildren<UIRef>(true);
 
        for (int i = 0; i < children.Length; i++)
        {
            UIRef child = children[i];
            if (child != uiRef && child.IsCopyRefStr == uiRef.IsCopyRefStr && child.UseObjName)
            {
                hashSet.Add(child.gameObject.name);
            }
        }
        uiRef.gameObject.name = UIRefEditor.GetUniqueName(name.Trim(), hashSet).Replace("(", "_").Replace(")", "_");
    }

    public static bool IsLayoutContent(this UIRef uiRef)
    {
        return uiRef.ComponentName.Contains("LayoutGroup") ||
               uiRef.GetComponent<UnityEngine.UI.LayoutGroup>() != null;
    }
}
