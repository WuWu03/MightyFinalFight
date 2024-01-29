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

        public void Init(GameObject parent, GameObject item, int initCount = 1)
        {
            m_Item = item;
            m_ItemParent = parent;
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
                    m_ListItem[i].SetActive(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
                else
                {
                    GetItem(i);
                    m_ListItem[i].SetActive(true);
                    onItemUpdateEvent?.Invoke(m_ListItem[i]);
                }
            }

            for (int i = count; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].SetActive(false);
            }
        }

        public void SelectItem(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1 || index < 0 || index >= m_ListItem.Count)
            {
                return;
            }

            if (m_CurrSelectIndex > -1 && m_ListItem[m_CurrSelectIndex] != null)
            {
                onItemSelectEvent?.Invoke(m_ListItem[m_CurrSelectIndex], false);
            }

            if (m_ListItem[index] != null)
            {
                m_CurrSelectIndex = index;
                onItemSelectEvent?.Invoke(m_ListItem[index], true);
            }
        }

        public T GetItemByIndex(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1 || index < 1 || index >= m_ListItem.Count)
            {
                return null;
            }

            return m_ListItem[index];
        }
            
        private void GetItem(int index)
        {
            GameObject go = GameObject.Instantiate(m_Item);
            go.transform.SetParent(m_ItemParent.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            T item = new T();
            item.Create(go);
            item.itemIndex = index;

            if (item.selectButton != null)
            {
                item.selectButton.onClick.AddListener(delegate () { SelectItem(index); });
            }

            m_ListItem.Add(item);
        }

        private List<T> m_ListItem = null;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;

        private int m_CurrSelectIndex = -1;
    }
}