using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ButtonEx")]
    public class ButtonEx : UIBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public float pressTime { get; set; }

        public float doubleClickTime { get; set; }

        public GameFrameWorkEvent<GameObject> onClick = new();
        public GameFrameWorkEvent<GameObject> onDoubleClick = new();
        public GameFrameWorkEvent<GameObject> onPress = new();
        public GameFrameWorkEvent<GameObject, PointerEventData> onUp = new();
        public GameFrameWorkEvent<GameObject, PointerEventData> onDown = new();
        
        private float m_CurrDonwTime = 0f;
        private bool m_IsPointDown = false;
        private bool m_IsPress = false;
        private int m_ClickCount = 0;
        
        private const float DoubleClickTime = 0.2f;
        private const float PressTime = 0.5f;
        
        protected override void Awake()
        {
            base.Awake();
            pressTime = PressTime;
            doubleClickTime = DoubleClickTime;
        }

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
            if (m_IsPointDown && Time.unscaledTime - m_CurrDonwTime >= pressTime)
            {
                onPress.Invoke(gameObject);
                m_IsPress = true;
                m_IsPointDown = false;
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                return;
            }

            if (m_ClickCount >= 2)
            {
                onDoubleClick.Invoke(gameObject);
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                return;
            }

            if (m_CurrDonwTime > 0 && Time.unscaledTime - m_CurrDonwTime >= doubleClickTime)
            {
                if (m_ClickCount > 0)
                {
                    onClick.Invoke(gameObject);
                }

                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_IsPress = false;
            m_IsPointDown = true;
            m_CurrDonwTime = Time.unscaledTime;
            onDown?.Invoke(gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_IsPress)
            {
                m_ClickCount++;
            }

            m_IsPress = false;
            m_IsPointDown = false;
            onUp.Invoke(gameObject, eventData);
        }

        public void ResetPressTime()
        {
            pressTime = PressTime;
        }

        public void ResetDoubleClickTime()
        {
            doubleClickTime = DoubleClickTime;
        }
    }
}