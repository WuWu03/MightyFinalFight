using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public abstract class LayoutLoopViewItem
    {
        public int Index { get; set; }
        public virtual MyButton SelectButton { get; }
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }

        public abstract void CreateHandle();
        public virtual void SelectHandle(bool isSelect) { }
        public abstract void SetData(int index);
    }

    public class LayoutLoopView<T> where T : LayoutLoopViewItem, new()
        //where P : BasePanel
    {
        public GameObject Scroll
        {
            get; private set;
        }

        public ScrollRect ScrollRect
        {
            get; private set;
        }

        public void Init(GameObject _scroll, float perSize, float space)
        {
            Scroll = _scroll;
            ScrollRect = _scroll.GetComponent<ScrollRect>();
            //m_Panel = panel;

            m_PerSize = perSize;
            m_Space = space;
            m_ListItem = new List<T>();
            m_Item = _scroll.transform.Find("ViewPort/Item").gameObject;
            m_ItemParent = _scroll.transform.Find("ViewPort/Content").gameObject;
            m_LayoutGroup = m_ItemParent.GetOrAddComponent<HorizontalOrVerticalLayoutGroup>();
            m_ViewPortSize = _scroll.transform.Find("ViewPort").GetComponent<RectTransform>().sizeDelta;
            m_ContentSizeFitter = m_LayoutGroup.gameObject.GetComponent<ContentSizeFitter>();
            m_LayoutRect = m_LayoutGroup.GetComponent<RectTransform>();

            if (ScrollRect == null)
            {
                Debug.LogError("沒有挂载ScrollRect");
                return;
            }

            if (m_ContentSizeFitter != null)
            {
                m_ContentSizeFitter.enabled = false;
            }

            m_LayoutGroup.enabled = false;

            if (m_LayoutGroup is HorizontalLayoutGroup)
            {
                m_LayoutRect.pivot = new Vector2(0, m_LayoutRect.pivot.y);
                m_LayoutRect.anchorMin = new Vector2(0f, m_LayoutRect.anchorMin.y);
                m_LayoutRect.anchorMax = new Vector2(0f, m_LayoutRect.anchorMin.y);
                m_LayoutRect.anchoredPosition = new Vector2(0, m_LayoutRect.anchoredPosition.y);
                m_ShowCount = Mathf.CeilToInt(m_ViewPortSize.x / (perSize + space));
            }
            else
            {
                m_LayoutRect.pivot = new Vector2(m_LayoutRect.pivot.x, 1f);
                m_LayoutRect.anchorMin = new Vector2(m_LayoutRect.anchorMin.x, 1f);
                m_LayoutRect.anchorMax = new Vector2(m_LayoutRect.anchorMax.x, 1f);
                m_LayoutRect.anchoredPosition = new Vector2(m_LayoutRect.anchoredPosition.x, 0);
                m_ShowCount = Mathf.CeilToInt(m_ViewPortSize.y / (perSize + space));
            }

            ScrollRect.onValueChanged.AddListener(OnScroll);

            for (int i = 0; i < m_ShowCount; i++)
            {
                GetItem(m_ItemParent.transform, m_Item, i);
            }
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
                m_ListItem[i].SetData(m_CurrIndex + i);
            }
        }

        public void SelectItem(int index)
        {
            for (int i = 0; i < m_ListItem.Count; i++)
            {
                m_ListItem[i].SelectHandle(false);
            }

            if (index >= 0 && index < m_ListItem.Count && m_ListItem[index] != null)
            {
                m_ListItem[index].SelectHandle(true);
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
                velocity = ScrollRect.velocity.x;
                anchoredPosition = -m_LayoutRect.anchoredPosition.x;
                forwardState = velocity < 0 ? 1 : -1;// velocity > 0 ? -1 : 0;
            }
            else if (m_LayoutGroup is VerticalLayoutGroup)
            {
                velocity = ScrollRect.velocity.y;
                anchoredPosition = m_LayoutRect.anchoredPosition.y;
                forwardState = velocity > 0 ? 1 : -1;//velocity < 0 ? -1 : 0;
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
                        m_ListItem[i].Index = m_CurrIndex - itemIndex + i;
                        m_ListItem[i].SetData(m_CurrIndex - itemIndex + i);
                    }
                else
                    for (int i = m_ListItem.Count - 1; i >= itemIndex; i--)
                    {
                        SetItemPos(m_ListItem[i], m_CurrIndex - itemIndex + i);
                        m_ListItem[i].Index = m_CurrIndex - itemIndex + i;
                        m_ListItem[i].SetData(m_CurrIndex - itemIndex + i);
                    }
            }
        }

        private void GetItem(Transform parent, GameObject obj, int index)
        {
            GameObject item = GameObject.Instantiate(obj);
            item.GetOrAddComponent<ScrollRectDrag>().DragScroll = ScrollRect;
            item.transform.SetParent(parent, false);
            item.transform.localScale = Vector3.one;
            item.SetActive(false);

            T script = new T();
            script.gameObject = item;
            script.transform = item.transform;
            script.Index = index;
            script.CreateHandle();

            if (script.SelectButton != null)
            {
                script.SelectButton.onClick.AddListener(delegate () { SelectItem(index); });
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
        private Vector2 m_ViewPortSize = Vector2.zero;
        private HorizontalOrVerticalLayoutGroup m_LayoutGroup = null;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;
        private ContentSizeFitter m_ContentSizeFitter = null;
        private RectTransform m_LayoutRect = null;
        private List<T> m_ListItem = null;
        //private P m_Panel;
    }
}