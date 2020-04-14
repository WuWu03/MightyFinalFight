using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public abstract class LayoutItem
    {
        public int Index { get; set; }
        public virtual MyButton SelectButton { get; protected set; }
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        public RectTransform rectTransform { get; protected set; }
        public void Create(GameObject go)
        {
            gameObject = go;
            transform = go.transform;
            rectTransform = go.GetComponent<RectTransform>();
            OnCreate(go);
        }

        protected abstract void OnCreate(GameObject go);
    }

    public class LayoutGroupView<T> where T : LayoutItem, new()
    {
        public Action<T> OnItemUpdate;
        public Action<T,bool> OnItemSelect;
        
        public void Init(GameObject parent,GameObject item, int maxCount = 1, ScrollRect scroll = null)
        {
            m_Parent = parent;
            m_MaxCount = maxCount;
            m_Item = item;
            m_ItemParent = parent;
            m_ScrollRect = scroll;
            m_ListItem = new List<T>();

            for (int i = 0; i < maxCount; i++)
            {
                GetItem(i);
            }
        }

        public void Update(int count)
        {
            int diff = m_MaxCount - count;

            if (diff > 0)
            {
                for (int i = count; i < m_ListItem.Count; i++)
                {
                    m_ListItem[i].gameObject.SetActive(false);
                }
            }
            else
            {
                diff = -diff;

                for (int i = 0; i < diff; i++)
                {
                    GetItem(m_ListItem.Count);
                }

                m_MaxCount += diff;
            }

            for (int i = 0; i < count; i++)
            {
                m_ListItem[i].gameObject.SetActive(true);
                OnItemUpdate?.Invoke(m_ListItem[i]);
            }
        }

        public void SelectItem(int index)
        {
            for (int i = 0; i < m_ListItem.Count; i++)
            {
                OnItemSelect?.Invoke(m_ListItem[i], false);
            }

            if (index >= 0 && index < m_ListItem.Count && m_ListItem[index] != null)
            {
                OnItemSelect?.Invoke(m_ListItem[index], true);
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
            script.Index = index;
            script.Create(item);
            OnItemUpdate?.Invoke(script);

            if (script.SelectButton != null)
            {
                script.SelectButton.onClick.AddListener(delegate () { SelectItem(index); });
            }
            m_ListItem.Add(script);
        }

        private List<T> m_ListItem = null;
        private int m_MaxCount = 1;
        private GameObject m_Parent;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;
        private ScrollRect m_ScrollRect;
    }
}