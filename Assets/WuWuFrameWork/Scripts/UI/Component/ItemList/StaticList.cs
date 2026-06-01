using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WuWuFramework.UI
{
    [AddComponentMenu("UI/StaticList")]
    [RequireComponent(typeof(LayoutGroup))]
    public class StaticList : MonoBehaviour
    {
        public event Action<BaseListItem> itemUpdateEvent;
        public event Action<BaseListItem, bool> itemSelectEvent;

        public GameObject prefab;

        private List<BaseListItem> m_ListItem;
        private LayoutGroup m_LayoutGroup;
        private ContentSizeFitter m_ContentSizeFitter;
        private RectTransform m_RectTransform;
        private int m_CurrSelectIndex = -1;
        private Type m_ItemClassType;
        private bool m_IsInit;
        private int m_CurrCount;

        public void Init<T>() where T : BaseListItem, new()
        {
            if (m_IsInit)
            {
                throw new WuWuFrameworkException("不能重复初始化");
            }

            m_LayoutGroup = GetComponent<LayoutGroup>();
            m_ContentSizeFitter = GetComponent<ContentSizeFitter>();
            m_RectTransform = GetComponent<RectTransform>();

            if (m_LayoutGroup is null)
            {
                throw new WuWuFrameworkException("布局组件为空");
            }

            if (prefab is null)
            {
                throw new WuWuFrameworkException("prefab为空");
            }

            prefab.SetActive(false);
            m_ItemClassType = typeof(T);
            m_ListItem = new();
            SetLayoutEnabled(true);
            m_IsInit = true;
        }

        public void SetItemCount(int count)
        {
            if (!m_IsInit)
            {
                throw new WuWuFrameworkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_CurrCount != count)
            {
                SetLayoutEnabled(true);
            }

            for (int i = 0; i < count; i++)
            {
                if (i < m_ListItem.Count)
                {
                    m_ListItem[i].SetActive(true);
                    itemUpdateEvent?.Invoke(m_ListItem[i]);
                }
                else
                {
                    GetItem(i);
                    m_ListItem[i].SetActive(true);
                    itemUpdateEvent?.Invoke(m_ListItem[i]);
                }
            }

            for (int i = count; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].SetActive(false);
            }

            if (m_CurrCount != count)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);
                SetLayoutEnabled(false);
            }

            m_CurrCount = count;
        }

        public void SelectItem(int index)
        {
            if (!m_IsInit)
            {
                throw new WuWuFrameworkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_ListItem == null || m_ListItem.Count < 1 || index < 0 || index >= m_ListItem.Count)
            {
                return;
            }

            if (m_CurrSelectIndex > -1 && m_CurrSelectIndex < m_ListItem.Count && m_ListItem[m_CurrSelectIndex] != null)
            {
                itemSelectEvent?.Invoke(m_ListItem[m_CurrSelectIndex], false);
            }

            if (m_ListItem[index] != null)
            {
                m_CurrSelectIndex = index;
                itemSelectEvent?.Invoke(m_ListItem[index], true);
            }
        }

        public BaseListItem GetItemByIndex(int index)
        {
            if (!m_IsInit)
            {
                throw new WuWuFrameworkException("未初始化，必须先调用Init方法进行初始化");
            }

            if (m_ListItem == null || m_ListItem.Count < 1 || index < 1 || index >= m_ListItem.Count)
            {
                return null;
            }

            return m_ListItem[index];
        }

        private void GetItem(int index)
        {
            GameObject go = Instantiate(prefab, transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            BaseListItem item = Activator.CreateInstance(m_ItemClassType) as BaseListItem;

            if (item == null)
            {
                throw new WuWuFrameworkException("创建 [" + m_ItemClassType.FullName + "] 实例失败");
            }

            item.Create(go);
            item.index = index;
            m_ListItem.Add(item);
        }

        private void SetLayoutEnabled(bool enabled)
        {
            m_LayoutGroup.enabled = enabled;

            if (m_ContentSizeFitter is not null)
            {
                m_ContentSizeFitter.enabled = enabled;
            }
        }
    }
}