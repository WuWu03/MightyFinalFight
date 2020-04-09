using System;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;

public static class UIRefEx
{
    public static void SetName(this UIRef uiRef, string name)
    {
        UIRefRoot uIRefRoot = uiRef.gameObject.FindComponentInParents<UIRefRoot>();
        if (uIRefRoot == null)
        {
            Debug.LogError("没有 mUIRefSetting 组件");
        }
        else
        {
            List<string> list = new List<string>();
            UIRef[] componentsInChildren = uIRefRoot.GetComponentsInChildren<UIRef>(true);
            UIRef[] array = componentsInChildren;
            for (int i = 0; i < array.Length; i++)
            {
                UIRef uIRef = array[i];
                if (!(uIRef == uiRef) && uIRef.OutputClipBoard == uiRef.OutputClipBoard)
                {
                    list.Add(uIRef.UseObjName ? uIRef.gameObject.name : uIRef.Name);
                }
            }
            uiRef.Name = UIRefEditor.GetUniqueName(name.Trim(), list);
        }
    }

    public static string GetName(this UIRef uiRef)
    {
        string str = string.Empty;
        if (string.IsNullOrEmpty(uiRef.ComponentName) || uiRef.ComponentName == typeof(Transform).Name || uiRef.ComponentName == typeof(RectTransform).Name)
        {
            str = "Trans";
        }
        else if (uiRef.ComponentName == typeof(GameObject).Name)
        {
            str = "Obj";
        }
        string text;
        if (uiRef.UseObjName)
        {
            text = uiRef.gameObject.name;
        }
        else
        {
            text = uiRef.Name;
        }

        if (string.IsNullOrEmpty(text))
            return "m" + str;

        text = text.Replace(" ", "_").Replace("(", "_").Replace(")", "_");
        if (text[0] > 'a' && text[0] < 'z')
        {
			text = (char)(text[0] - ' ') + text.Substring(1);
        }
        return "m" + text + str;
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
            if (child != uiRef && child.OutputClipBoard == uiRef.OutputClipBoard && child.UseObjName)
            {
                hashSet.Add(child.gameObject.name);
            }
        }
        uiRef.gameObject.name = UIRefEditor.GetUniqueName(name.Trim(), hashSet).Replace("(", "_").Replace(")", "_");
    }
}
