using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public abstract class LayoutViewItem<T>
    {
        public int Index { get; set; }
        public virtual MyButton SelectButton { get; }
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }

        protected T Panel { get; private set; }
        public virtual void CreateHandle(T panel)
        {
            Panel = panel;
        }
        public virtual void SelectHandle(bool isSelect) { }
        public abstract void SetData(int index);
    }

    public class LayoutGroupView<T, P> where T : LayoutViewItem<P>, new()
                                   where P : BasePanel, new()
    {
        public GameObject Scroll
        {
            get; private set;
        }

        public ScrollRect ScrollRect
        {
            get; private set;
        }

        private P Panel
        {
            get; set;
        }

        public void Init(P panel, GameObject _scroll, int maxCount = 1, bool isScroll = false)
        {
            Scroll = _scroll;
            m_MaxCount = maxCount;
            ScrollRect = _scroll.GetComponent<ScrollRect>();
            Panel = panel;
            m_ListItem = new List<T>();

            if (isScroll)
            {
                m_Item = _scroll.transform.Find("ViewPort/Item").gameObject;
                m_ItemParent = _scroll.transform.Find("ViewPort/Content").gameObject;

                if (ScrollRect == null)
                {
                    Debug.LogError("沒有挂载ScrollRect");
                    return;
                }
            }
            else
            {
                m_Item = _scroll.transform.Find("Item").gameObject;
                m_ItemParent = _scroll;
            }

            for (int i = 0; i < maxCount; i++)
            {
                GetItem(m_ItemParent.transform, m_Item, i);
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
                    GetItem(m_ItemParent.transform, m_Item, m_ListItem.Count);
                }

                m_MaxCount += diff;
            }

            for (int i = 0; i < m_MaxCount; i++)
            {
                m_ListItem[i].gameObject.SetActive(true);
                m_ListItem[i].SetData(i);
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

        private void GetItem(Transform parent, GameObject obj, int index)
        {
            GameObject item = GameObject.Instantiate(obj);
            item.GetOrAddComponent<ScrollRectDrag>().DragScroll = ScrollRect;
            item.transform.SetParent(parent, false);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.SetActive(false);

            T script = new T();
            script.gameObject = item;
            script.transform = item.transform;
            script.Index = index;
            script.CreateHandle(Panel);

            if (script.SelectButton != null)
            {
                script.SelectButton.onClick.AddListener(delegate () { SelectItem(index); });
            }
            m_ListItem.Add(script);
        }

        public List<T> m_ListItem = null;
        private int m_MaxCount = 1;
        private GameObject m_ItemParent = null;
        private GameObject m_Item = null;
    }
}