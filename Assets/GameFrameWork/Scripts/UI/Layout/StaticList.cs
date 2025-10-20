using System;
using System.Collections.Generic;
using GameFrameWork.Event;
using GameFrameWork.Utils;
using UnityEngine;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/StaticList")]
    public class StaticList : MonoBehaviour
    {
        private List<StaticListItem> m_ListItem;
        private GameObject m_ItemParent;
        private GameObject m_Item;
        private int m_CurrSelectIndex = -1;

        public event GameFrameWorkAction<StaticListItem> onItemUpdateEvent;
        public event GameFrameWorkAction<StaticListItem, bool> onItemSelectEvent;
        public event GameFrameWorkAction<StaticListItem> onItemReleaseEvent;
        private Type m_ItemClassType = null;

        public void Init<T>(GameObject parent, GameObject item) where T : StaticListItem, new()
        {
            m_Item = item;
            m_ItemParent = parent;
            m_Item.SetActiveSelf(false);
            m_ItemClassType = typeof(T);
            m_ListItem = new();
        }

        public void RefreshItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (i < m_ListItem.Count)
                {
                    m_ListItem[i].SetActiveSelf(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
                else
                {
                    GetItem(i);
                    m_ListItem[i].SetActiveSelf(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
            }

            for (int i = count; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].SetActiveSelf(false);
            }
        }

        public void SetActive(bool active)
        {
            m_ItemParent.SetActiveSelf(active);
        }

        public void SelectItem(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1 || index < 0 || index >= m_ListItem.Count)
            {
                return;
            }

            if (m_CurrSelectIndex > -1 && m_CurrSelectIndex < m_ListItem.Count && m_ListItem[m_CurrSelectIndex] != null)
            {
                onItemSelectEvent?.Invoke(m_ListItem[m_CurrSelectIndex], false);
            }

            if (m_ListItem[index] != null)
            {
                m_CurrSelectIndex = index;
                onItemSelectEvent?.Invoke(m_ListItem[index], true);
            }
        }

        public StaticListItem GetItemByIndex(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1 || index < 1 || index >= m_ListItem.Count)
            {
                return null;
            }

            return m_ListItem[index];
        }

        public void Release()
        {
            foreach (var item in m_ListItem)
            {
                onItemReleaseEvent?.Invoke(item);
                item.SetActiveSelf(false);
                item.Release();
            }
        }

        private void GetItem(int index)
        {
            GameObject go = Instantiate(m_Item, m_ItemParent.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            StaticListItem item = Activator.CreateInstance(m_ItemClassType) as StaticListItem;

            if (item == null)
            {
                throw new GameFrameWorkException(StringUtil.Append("创建 [", m_ItemClassType.FullName, "] 实例失败"));
            }

            item.Create(go);
            item.itemIndex = index;
            m_ListItem.Add(item);
        }
    }
}