using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class LayoutGroupView<T> where T : LayoutGroupViewItem, new()
    {
        public GameFrameWorkAction<T> onItemUpdateEvent;
        public GameFrameWorkAction<T,bool> onItemSelectEvent;
        
        public void Init(GameObject parent,GameObject item, int initCount = 1, ScrollRect scroll = null)
        {
            m_Item = item;
            m_ItemParent = parent;
            m_ScrollRect = scroll;
            m_ListItem = new List<T>();

            for (int i = 0; i < initCount; i++)
            {
                GetItem(i);
            }
        }

        public void Update(int count)
        {
            for(int i = 0; i < count; i++)
            {
                if(i < m_ListItem.Count)
                {
                    m_ListItem[i].gameObject.SetActive(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
                else
                {
                    GetItem(i);
                    m_ListItem[i].gameObject.SetActive(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
            }

            for (int i = count; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].gameObject.SetActive(false);
            }
        }

        public void SelectItem(int index)
        {
            onItemSelectEvent?.Invoke(m_ListItem[m_CurrSelectIndex], false);

            if (index >= 0 && index < m_ListItem.Count && m_ListItem[index] != null)
            {
                m_CurrSelectIndex = index;
                onItemSelectEvent?.Invoke(m_ListItem[index], true);
            }
        }
        
        public T GetItemByIndex(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1) return null;
            if (index >= m_ListItem.Count || index < 1) return null;

            return m_ListItem[index];
        }
            
        private void GetItem(int index)
        {
            GameObject item = GameObject.Instantiate(m_Item);
            item.GetOrAddComponent<ScrollRectDrag>().DragScroll = m_ScrollRect;
            item.transform.SetParent(m_ItemParent.transform, false);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.SetActive(false);

            T script = new T();
            script.Create(item, index);

            if (script.selectButton != null)
            {
                script.selectButton.onClick.AddListener(delegate () { SelectItem(index); });
            }
            m_ListItem.Add(script);
        }

        private List<T> m_ListItem = null;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;
        private ScrollRect m_ScrollRect;

        private int m_CurrSelectIndex = 0;
    }
}