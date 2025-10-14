using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    public class LayoutGroupView<T> where T : LayoutGroupViewItem, new()
    {
        public event GameFrameWorkAction<T> onItemUpdateEvent;
        public event GameFrameWorkAction<T,bool> onItemSelectEvent;
        public event GameFrameWorkAction<T> onItemReleaseEvent;

        public LayoutGroupView(GameObject parent, GameObject item)
        {
            m_Item = item;
            m_ItemParent = parent;
            m_Item.SetActiveSelf(false);
            m_ListItem = new List<T>();
        }

        public void Update(int count)
        {
            for(int i = 0; i < count; i++)
            {
                if(i < m_ListItem.Count)
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

        public T GetItemByIndex(int index)
        {
            if (m_ListItem == null || m_ListItem.Count < 1 || index < 1 || index >= m_ListItem.Count)
            {
                return null;
            }

            return m_ListItem[index];
        }

        public void Release()
        {
            for (int i = 0; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].SetActiveSelf(false);
                onItemReleaseEvent?.Invoke(m_ListItem[i]);
            }
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
            m_ListItem.Add(item);
        }

        public void Clear()
        {

        }

        private List<T> m_ListItem = null;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;

        private int m_CurrSelectIndex = -1;
    }
}