using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public static class UIRefExt
    {
        public static void SetName(this UIRef uiRef, string name)
        {
            UIRefRoot uiRefRoot = uiRef.gameObject.FindComponentInParents<UIRefRoot>();
            if (uiRefRoot == null)
            {
                UnityEngine.Debug.LogError("没有 UIRefSetting 组件");
                return;
            }

            List<string> list = new List<string>();
            UIRef[] components = uiRefRoot.GetComponentsInChildren<UIRef>(true);

            int selfIndex = 0;

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                if (component.isCopyRefStr == uiRef.isCopyRefStr)
                {
                    if (components[i] == uiRef) selfIndex = i;
                    list.Add(component.useDefaultName ? component.gameObject.name : component.refName);
                }
            }

            uiRef.refName = GetUniqueName(name.Trim(), list,selfIndex);
        }

        public static string GetName(this UIRef uiRef, bool isFirstUpper = false)
        {
            string str = string.Empty;
            if (!uiRef.IsLayoutContent())
            {
                if (string.IsNullOrEmpty(uiRef.componentName) || uiRef.componentName == typeof(Transform).Name)
                {
                    str = "Trans";
                }
                else if (uiRef.componentName == typeof(RectTransform).Name)
                {
                    str = "Rect";
                }
                else if (uiRef.componentName == typeof(GameObject).Name)
                {
                    str = "GO";
                }
            }

            string text = uiRef.refName;

            if (string.IsNullOrEmpty(text))
            {
                return str;
            }

            if (isFirstUpper)
            {
                if (text[0] > 'a' && text[0] < 'z')
                {
                    text = (char)(text[0] - ' ') + text.Substring(1);
                }
            }
            else if (text[0] > 'A' && text[0] < 'Z')
            {
                text = (char)(text[0] + ' ') + text.Substring(1);
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
            return uiRef.componentName.Contains("LayoutGroup") || uiRef.GetComponent<UnityEngine.UI.LayoutGroup>() != null;
        }

        public static bool IsScrollRect(this UIRef uiRef)
        {
            return uiRef.componentName.Contains("ScrollRect") || uiRef.GetComponent<UnityEngine.UI.ScrollRect>() != null;
        }
    }
}