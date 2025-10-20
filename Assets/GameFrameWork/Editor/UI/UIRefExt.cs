using GameFrameWork.UI;
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

            List<string> list = new();
            UIRef[] components = uiRefRoot.GetComponentsInChildren<UIRef>(true);
            int selfIndex = 0;

            for (int i = 0; i < components.Length; i++)
            {
                UIRef component = components[i];
                
                if (component.isCopyRefStr == uiRef.isCopyRefStr)
                {
                    if (components[i] == uiRef)
                    {
                        selfIndex = i;
                    }

                    list.Add(component.useDefaultName ? component.gameObject.name : component.refName);
                }
            }

            uiRef.refName = GetUniqueName(name.Trim(), list, selfIndex);
        }

        public static string GetName(this UIRef uiRef, bool isFirstUpper = false)
        {
            string str = string.Empty;

            if (!uiRef.isListItem)
            {
                if (string.IsNullOrEmpty(uiRef.componentName) || uiRef.componentName == nameof(Transform))
                {
                    str = "Trans";
                }
                else if (uiRef.componentName == nameof(RectTransform))
                {
                    str = "Rect";
                }
                else if (uiRef.componentName == nameof(GameObject))
                {
                    str = "Go";
                }
                else if (uiRef.componentName == nameof(StaticList))
                {
                    str = "List";
                }
                else if (uiRef.componentName == nameof(ScrollList))
                {
                    str = "List";
                }
            }

            string refName = uiRef.refName;

            if (string.IsNullOrEmpty(refName))
            {
                return str;
            }

            if (isFirstUpper)
            {
                if (refName[0] > 'a' && refName[0] < 'z')
                {
                    refName = (char)(refName[0] - ' ') + refName.Substring(1);
                }
            }
            else if (refName[0] > 'A' && refName[0] < 'Z')
            {
                refName = (char)(refName[0] + ' ') + refName.Substring(1);
            }

            if (!refName.EndsWith(str))
            {
                refName += str;
            }
            
            return refName;
        }

        private static string GetUniqueName(string name, IEnumerable<string> array, int selfIndex)
        {
            int index = 0;
            int findIndex = 0;
            string text = name;

            foreach (string current in array)
            {
                if (current == text)
                {
                    if (selfIndex == index)
                    {
                        string nameParam = findIndex == 0 ? string.Empty : findIndex.ToString();
                        text = $"{name}{nameParam}";
                    }

                    findIndex++;
                }

                index++;
            }

            return text;
        }

        public static bool IsStaticList(this UIRef uiRef)
        {
            return uiRef.GetComponent<StaticList>() != null;
        }

        public static bool IsScrollList(this UIRef uiRef)
        {
            return uiRef.GetComponent<ScrollList>() != null;
        }

        public static bool IsListItemVariable(this UIRef uiRef)
        {
            Transform parent = uiRef.transform.parent;

            if(parent != null)
            {
                UIRef parentRef = parent.GetComponent<UIRef>();
                return parentRef != null && parentRef.isListItem;
            }

            return false;
        }

        public static bool IsListItem(this UIRef uiRef)
        {
            Transform current = uiRef.transform.parent;

            while (current != null)
            {
                if (current.TryGetComponent(out UIRef component))
                {
                    if (component.IsList)
                    {
                        return true;
                    }
                }

                current = current.parent;
            }

            return false;
        }
    }
}