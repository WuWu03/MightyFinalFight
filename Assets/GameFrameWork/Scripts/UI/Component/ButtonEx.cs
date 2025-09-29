using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ButtonEx")]
    public class ButtonEx : UIBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        protected override void OnDestroy()
        {
            base.OnDestroy();
            onClick.RemoveAllListeners();
            onDoubleClick.RemoveAllListeners();
            onPress.RemoveAllListeners();
            onUp.RemoveAllListeners();
            onDown.RemoveAllListeners();
        }

        private void Update()
        {
            if (m_IsPointDown && Time.unscaledTime - m_CurrDonwTime >= PRESS_TIME)
            {
                onPress.Invoke(gameObject, m_OnDownEventData);
                m_IsPress = true;
                m_IsPointDown = false;
                m_CurrDonwTime = 0f;
                m_OnDownEventData = null;
            }

            if (m_ClickCount < 1)
            {
                return;
            }

            if (m_ClickCount >= 2)
            {
                onDoubleClick.Invoke(gameObject, m_OnUpEventData);
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                m_OnUpEventData = null;
            }

            if (Time.unscaledTime - m_CurrDonwTime >= DOUBLE_CLICK_TIME)
            {
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                m_OnUpEventData = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_IsPointDown = true;
            m_IsPress = false;
            m_CurrDonwTime = Time.unscaledTime;
            m_OnDownEventData = eventData;
            onDown?.Invoke(gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onUp.Invoke(gameObject, eventData);
            onClick.Invoke(gameObject, eventData);

            if (!m_IsPress)
            {
                m_ClickCount++;
            }

            m_IsPointDown = false;
            m_OnUpEventData = eventData;
            m_IsPress = false;
        }

        public UIEvent<PointerEventData> onClick = new();
        public UIEvent<PointerEventData> onDoubleClick = new();
        public UIEvent<PointerEventData> onPress = new();
        public UIEvent<PointerEventData> onUp = new();
        public UIEvent<PointerEventData> onDown = new();

        private PointerEventData m_OnUpEventData = null;
        private PointerEventData m_OnDownEventData = null;

        private const float DOUBLE_CLICK_TIME = 0.2f;
        private const float PRESS_TIME = 0.5f;

        private float m_CurrDonwTime = 0f;
        private bool m_IsPointDown = false;
        private bool m_IsPress = false;
        private int m_ClickCount = 0;
    }
}