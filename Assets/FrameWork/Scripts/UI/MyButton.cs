using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class MyButton : Selectable, ISubmitHandler
    {
        private void Update()
        {
            if (m_IsPointDown)
            {
                if (Time.unscaledTime - m_CurrDonwTime >= PRESS_TIME)
                {
                    m_IsPress = true;
                    m_IsPointDown = false;
                    m_CurrDonwTime = 0f;
                    onPress?.Invoke();
                }
            }

            if (m_ClickCount > 0)
            {
                if (Time.unscaledTime - m_CurrDonwTime >= DOUBLE_CLICK_TIME)
                {
                    if (m_ClickCount < 2)
                    {
                        onUp?.Invoke();
                        onClick?.Invoke();
                    }
                    m_ClickCount = 0;
                }

                if (m_ClickCount >= 2)
                {
                    onDoubleClick?.Invoke();
                    m_ClickCount = 0;
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            m_IsPointDown = true;
            m_IsPress = false;
            m_CurrDonwTime = Time.unscaledTime;
            onDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            m_IsPointDown = false;

            if (!m_IsPress)
            {
                m_ClickCount++;
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onClick.Invoke();
        }

        public UnityEvent onClick = new UnityEvent();
        public UnityEvent onDoubleClick = new UnityEvent();
        public UnityEvent onPress = new UnityEvent();
        public UnityEvent onDown = new UnityEvent();
        public UnityEvent onUp = new UnityEvent();

        private const float DOUBLE_CLICK_TIME = 0.2f;
        private const float PRESS_TIME = 0.5f;

        private float m_CurrDonwTime = 0f;
        private bool m_IsPointDown = false;
        private bool m_IsPress = false;
        private int m_ClickCount = 0;
    }
}