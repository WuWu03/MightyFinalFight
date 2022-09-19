using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public class LayoutGroupLoopView<T> where T : LayoutGroupViewItem, new()
    {
        public Action<T> onItemUpdateEvent;
        public Action<T, bool> onItemSelectEvent;

        public void Init(GameObject parent, GameObject item, int maxCount, ScrollRect scroll)
        {
            if (scroll == null)
            {
                Log.GameFrameworkLog.LogError("LayoutGroupLoopView initialize failed ScrollRect not found");
                return;
            }

            m_ScrollRect = scroll;
            m_ListItem = new List<T>();
            m_Item = item;
            m_ItemParent = parent;
            m_LayoutGroup = parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
            m_ViewPortSize = scroll.viewport.GetComponent<RectTransform>().sizeDelta;
            m_ContentSizeFitter = m_LayoutGroup.gameObject.GetComponent<ContentSizeFitter>();
            m_LayoutRect = m_LayoutGroup.GetComponent<RectTransform>();

            m_Space = m_LayoutGroup.spacing;

            if (m_ContentSizeFitter != null)
            {
                m_ContentSizeFitter.enabled = false;
            }

            m_LayoutGroup.enabled = false;

            if (m_LayoutGroup is HorizontalLayoutGroup)
            {
                m_PerSize = item.GetComponent<RectTransform>().sizeDelta.x;
                m_LayoutRect.pivot = new Vector2(0, m_LayoutRect.pivot.y);
                m_LayoutRect.anchorMin = new Vector2(0f, m_LayoutRect.anchorMin.y);
                m_LayoutRect.anchorMax = new Vector2(0f, m_LayoutRect.anchorMin.y);
                m_LayoutRect.anchoredPosition = new Vector2(0, m_LayoutRect.anchoredPosition.y);
                m_ShowCount = Mathf.CeilToInt(m_ViewPortSize.x / (m_PerSize + m_Space));
            }
            else
            {
                m_PerSize = item.GetComponent<RectTransform>().sizeDelta.y;
                m_LayoutRect.pivot = new Vector2(m_LayoutRect.pivot.x, 1f);
                m_LayoutRect.anchorMin = new Vector2(m_LayoutRect.anchorMin.x, 1f);
                m_LayoutRect.anchorMax = new Vector2(m_LayoutRect.anchorMax.x, 1f);
                m_LayoutRect.anchoredPosition = new Vector2(m_LayoutRect.anchoredPosition.x, 0);
                m_ShowCount = Mathf.CeilToInt(m_ViewPortSize.y / (m_PerSize + m_Space));
            }

            m_ScrollRect.onValueChanged.AddListener(OnScroll);
            m_Item.SetActive(false);
            Update(maxCount);
        }

        public void Update(int count)
        {
            int diff = m_ShowCount - count;
            float size = count * (m_PerSize + m_Space) - m_Space;

            if (m_LayoutGroup is HorizontalLayoutGroup)
            {
                m_LayoutRect.sizeDelta = new Vector2(size, m_LayoutRect.sizeDelta.y);
                if (diff > 0)
                    m_LayoutRect.anchoredPosition = new Vector2(0, m_LayoutRect.anchoredPosition.y);
            }
            else if (m_LayoutGroup is VerticalLayoutGroup)
            {
                m_LayoutRect.sizeDelta = new Vector2(m_LayoutRect.sizeDelta.x, size);
                if (diff > 0)
                    m_LayoutRect.anchoredPosition = new Vector2(m_LayoutRect.anchoredPosition.x, 0);
            }

            if (diff >= 0)
            {
                for (int i = count; i < m_ListItem.Count; i++)
                {
                    m_ListItem[i].gameObject.SetActive(false);
                }
            }
            else
            {
                diff = m_ShowCount + 1;
                for (int i = m_ListItem.Count; i < diff; i++)
                {
                    GetItem(m_ItemParent.transform, m_Item, m_ListItem.Count);
                }
            }

            m_ContainCount = count;

            for (int i = 0; i < m_ShowCount + 1; i++)
            {
                m_ListItem[i].gameObject.SetActive(true);
                m_ListItem[i].UpdateIndex(m_CurrIndex + i);
                onItemUpdateEvent?.Invoke(m_ListItem[i]);
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

        private void OnScroll(Vector2 arg0)
        {
            float velocity = 0;
            int index = 0;
            int realIndex = 0;
            float anchoredPosition = 0;
            int forwardState = 0;

            if (m_LayoutGroup is HorizontalLayoutGroup)
            {
                velocity = m_ScrollRect.velocity.x;
                anchoredPosition = -m_LayoutRect.anchoredPosition.x;
                forwardState = velocity < 0 ? 1 : -1;
            }
            else if (m_LayoutGroup is VerticalLayoutGroup)
            {
                velocity = m_ScrollRect.velocity.y;
                anchoredPosition = m_LayoutRect.anchoredPosition.y;
                forwardState = velocity > 0 ? 1 : -1;
            }

            if (forwardState == 1)
            {
                index = Mathf.FloorToInt((anchoredPosition - m_PerSize - m_Space) / (m_PerSize + m_Space));
                realIndex = index + m_ShowCount + 1;
            }
            else if (forwardState == -1)
            {
                index = Mathf.FloorToInt(anchoredPosition / (m_PerSize + m_Space));
                realIndex = index;
            }
            else return;

            if (m_CurrIndex == realIndex) return;
            if (realIndex >= m_ContainCount || realIndex < 0) return;

            int itemIndex = index % m_ListItem.Count;

            if (itemIndex >= 0 && itemIndex < m_ListItem.Count)
            {
                m_CurrIndex = realIndex;

                if (forwardState == 1)
                    for (int i = 0; i <= itemIndex; i++)
                    {
                        SetItemPos(m_ListItem[i], m_CurrIndex - itemIndex + i);
                        m_ListItem[i].UpdateIndex(m_CurrIndex - itemIndex + i);
                        onItemUpdateEvent?.Invoke(m_ListItem[i]);
                    }
                else
                    for (int i = m_ListItem.Count - 1; i >= itemIndex; i--)
                    {
                        SetItemPos(m_ListItem[i], m_CurrIndex - itemIndex + i);
                        m_ListItem[i].UpdateIndex(m_CurrIndex - itemIndex + i);
                        onItemUpdateEvent?.Invoke(m_ListItem[i]);
                    }
            }
        }

        private void GetItem(Transform parent, GameObject obj, int index)
        {
            GameObject item = GameObject.Instantiate(obj);
            item.GetOrAddComponent<ScrollRectDrag>().DragScroll = m_ScrollRect;
            item.transform.SetParent(parent, false);
            item.transform.localScale = Vector3.one;
            item.SetActive(false);

            T script = new T();
            script.Create(item, index);

            if (script.selectButton != null)
            {
                script.selectButton.onClick.AddListener(delegate () { SelectItem(index); });
            }

            m_ListItem.Add(script);
            SetItemPos(script, index);
        }

        private void SetItemPos(T itemScript, int index)
        {
            RectTransform itemRect = itemScript.gameObject.GetComponent<RectTransform>();
            if (m_LayoutGroup is HorizontalLayoutGroup)
            {
                itemRect.pivot = new Vector2(0f, 0.5f);
                itemRect.anchorMin = new Vector2(0f, 0.5f);
                itemRect.anchorMax = new Vector2(0f, 0.5f);
                itemRect.anchoredPosition = new Vector2(index * (m_PerSize + m_Space), 0);
            }
            else if (m_LayoutGroup is VerticalLayoutGroup)
            {
                itemRect.pivot = new Vector2(0.5f, 1f);
                itemRect.anchorMin = new Vector2(0.5f, 1f);
                itemRect.anchorMax = new Vector2(0.5f, 1f);
                itemRect.anchoredPosition = new Vector2(0, -index * (m_PerSize + m_Space));
            }
        }

        private float m_PerSize = 0;
        private float m_Space = 0;
        private int m_ShowCount = 1;
        private int m_ContainCount = 0;
        private int m_CurrIndex = 0;
        private int m_CurrSelectIndex = 0;
        private Vector2 m_ViewPortSize = Vector2.zero;
        private HorizontalOrVerticalLayoutGroup m_LayoutGroup = null;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;
        private ContentSizeFitter m_ContentSizeFitter = null;
        private RectTransform m_LayoutRect = null;
        private List<T> m_ListItem = null;
        private ScrollRect m_ScrollRect;
    }
}