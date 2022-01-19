using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public static class UIRefUtility
    {
        public static void SetName(this UIRef uiRef, string name)
        {
            UIRefRoot uiRefRoot = uiRef.gameObject.FindComponentInParents<UIRefRoot>();
            if (uiRefRoot == null)
            {
                Debug.LogError("没有 UIRefSetting 组件");
                return;
            }

            List<string> list = new List<string>();
            UIRef[] components = uiRefRoot.GetComponentsInChildren<UIRef>(true);

            int selfIndex = 0;

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.IsCopyRefStr == uiRef.IsCopyRefStr)
                {
                    if (components[i] == uiRef) selfIndex = i;
                    list.Add(component.UseDefaultName ? component.gameObject.name : component.Name);
                }
            }

            uiRef.Name = GetUniqueName(name.Trim(), list,selfIndex);
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

            string text = uiRef.Name;
            if (string.IsNullOrEmpty(text)) return str;

            if (text[0] > 'a' && text[0] < 'z')
            {
                text = (char)(text[0] - ' ') + text.Substring(1);
            }
            return text + str;
        }

        public static string GetUniqueName(string name, IEnumerable<string> array, int selfIndex)
        {
            int index = 0;
            int findIndex = 0;
            string text = name;

            foreach (string current in array)
            {
                if (current == text)
                {
                    if(selfIndex == index)
                    {
                        string nameParam = findIndex == 0 ? string.Empty : findIndex.ToString();
                        text = string.Format("{0}{1}", name, nameParam);
                    }
                    findIndex++;
                }

                index++;
            }

            return text;
        }

        public static bool IsLayoutContent(this UIRef uiRef)
        {
            return uiRef.ComponentName.Contains("LayoutGroup") || uiRef.GetComponent<UnityEngine.UI.LayoutGroup>() != null;
        }

        public static bool IsScrollRect(this UIRef uiRef)
        {
            return uiRef.ComponentName.Contains("ScrollRect") || uiRef.GetComponent<UnityEngine.UI.ScrollRect>() != null;
        }
    }
}