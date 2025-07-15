using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ScrollRectPage")]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectPage : MonoBehaviour
    {
        public int pageCount = 1;
        public GameObject[] pageTabs;//页数的标签
        public bool canAutoFlip = false;
        [Min(0f)] public float autoFlipTime = 0f;
        [Range(0f, 0.4f)] public float additiveSensitivity;//翻页灵敏度修正
        [Min(0.1f)] public float moveSpeed;
        private void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
            m_ScrollPos = new float[pageCount];
            
            m_PageIndex = 0;
            m_PageTabIndex = 0;
            m_AutoFlipTimer = Time.unscaledTime;

            for (int i = 0; i < pageCount; i++)
            {
                if (m_ScrollRect.vertical)
                {
                    m_ScrollPos[pageCount - i - 1] = (float)i / (pageCount - 1);
                }
                else
                {
                    m_ScrollPos[i] = (float)i / (pageCount - 1);
                }

            }

            if(pageTabs != null)
            {
                for (int i = 0; i < pageTabs.Length; i++)
                {
                    if (pageTabs[i].GetComponent<Toggle>() != null)
                    {
                        pageTabs[i].GetComponent<Toggle>().isOn = i == m_PageIndex;
                    }
                    else
                    {
                        pageTabs[i].SetActiveSelf(i == m_PageIndex);
                    }
                }
            }

            UIEventListener.Get(gameObject).onBeginDrag.AddListener(OnBeginDrag);
            UIEventListener.Get(gameObject).onEndDrag.AddListener(OnEndDrag);
        }

        private void Update()
        {
            if (m_CanMove)
            {
                float currMoveSpeed = moveSpeed;
                float position;
                float distance;

                if (m_ScrollRect.vertical)
                {
                    position = m_ScrollRect.verticalNormalizedPosition;
                }
                else
                {
                    position = m_ScrollRect.horizontalNormalizedPosition;
                }

                if (m_ScrollPos[m_PageIndex] < position)
                {
                    currMoveSpeed = -moveSpeed;
                }

                if (m_ScrollRect.vertical)
                {
                    m_ScrollRect.verticalNormalizedPosition += currMoveSpeed * Time.deltaTime;
                    distance = Mathf.Abs(m_ScrollRect.verticalNormalizedPosition - m_ScrollPos[m_PageIndex]);
                }
                else
                {
                    m_ScrollRect.horizontalNormalizedPosition += currMoveSpeed * Time.deltaTime;
                    distance = Mathf.Abs(m_ScrollRect.horizontalNormalizedPosition - m_ScrollPos[m_PageIndex]);
                }

                if (distance <= 0.01f)
                {
                    m_CanMove = false;

                    if (m_ScrollRect.vertical)
                    {
                        m_ScrollRect.verticalNormalizedPosition = m_ScrollPos[m_PageIndex];
                    }
                    else
                    {
                        m_ScrollRect.horizontalNormalizedPosition = m_ScrollPos[m_PageIndex];
                    }

                    if(canAutoFlip && m_PageIndex >= pageCount - 1)
                    {
                        //m_ScrollRect.content.GetChild(0).SetAsLastSibling();

                        if (m_ScrollRect.vertical)
                        {
                            m_ScrollRect.verticalNormalizedPosition = m_ScrollPos[m_PageIndex - 1];
                        }
                        else
                        {
                            m_ScrollRect.horizontalNormalizedPosition = m_ScrollPos[m_PageIndex - 1];
                        }

                        m_PageIndex--;
                    }

                    if (pageTabs == null || pageTabs.Length < 1)
                    {
                        return;
                    }

                    GameObject lastTab = pageTabs[m_PageTabIndex];
                    GameObject currTab = pageTabs[m_PageIndex];

                    SetPageTab(lastTab, false);
                    SetPageTab(currTab, true);

                    m_PageTabIndex = m_PageIndex;
                }
            }

            if (canAutoFlip)
            {
                if(Time.unscaledTime - m_AutoFlipTimer > autoFlipTime && !m_CanMove)
                {
                    int pageIndex = m_PageIndex + 1;

                    if(pageIndex >= pageCount)
                    {
                        pageIndex = 0;
                    }

                    m_AutoFlipTimer = Time.unscaledTime;
                    ScrollToPage(pageIndex);
                }
            }
        }

        private void OnBeginDrag(GameObject go, PointerEventData eventData, object arg)
        {
            if (m_ScrollRect.vertical)
            {
                m_OriginalPos = m_ScrollRect.verticalNormalizedPosition;
            }
            else
            {
                m_OriginalPos = m_ScrollRect.horizontalNormalizedPosition;
            }
        }

        private void OnEndDrag(GameObject go, PointerEventData eventData, object arg)
        {
            float posAdditive;
            if (m_ScrollRect.vertical)
            {
                float position = m_ScrollRect.verticalNormalizedPosition;
                posAdditive = position + (position < m_OriginalPos ? -additiveSensitivity : additiveSensitivity);
            }
            else
            {
                float position = m_ScrollRect.horizontalNormalizedPosition;
                posAdditive = position + (position > m_OriginalPos ? additiveSensitivity : -additiveSensitivity);
            }

            float offset = -1f;
            int index = 0;

            for (int i = 0; i < m_ScrollPos.Length; i++)
            {
                float offsetTemp = Mathf.Abs(m_ScrollPos[i] - posAdditive);

                if (offset < 0 || offsetTemp < offset)
                {
                    index = i;
                    offset = offsetTemp;
                }
            }

            ScrollToPage(index);
        }

        private void ScrollToPage(int page)
        {
            m_PageIndex = page;
            m_CanMove = true;
        }

        private void SetPageTab(GameObject page, bool isActive)
        {
            if (page == null)
            {
                return;
            }

            Toggle tabToggle = page.GetComponent<Toggle>();

            if (tabToggle != null)
            {
                tabToggle.isOn = isActive;
            }
            else
            {
                page.SetActiveSelf(isActive);
            }
        }

        private ScrollRect m_ScrollRect = null;
        private float[] m_ScrollPos;
        private bool m_CanMove = false;
        private int m_PageIndex = 0;
        private int m_PageTabIndex = 0;
        private float m_OriginalPos;
        private float m_AutoFlipTimer = -1f;
    }
}