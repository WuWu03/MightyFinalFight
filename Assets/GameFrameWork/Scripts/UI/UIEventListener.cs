using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    public sealed class UIEventListener :
    MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerUpHandler,
    ISelectHandler,
    IUpdateSelectedHandler,
    IDeselectHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IScrollHandler,
    IMoveHandler
    {
        public delegate void UIEventHandle<T>(GameObject go, T eventData, object arg) where T : BaseEventData;
        public class UIEvent<T> where T : BaseEventData
        {
            public UIEvent() { }

            public void AddListener(UIEventHandle<T> handle, object arg = null)
            {
                m_UIEventHandle += handle;
                m_Arg = arg;
            }

            public void RemoveListener(UIEventHandle<T> handle)
            {
                m_UIEventHandle -= handle;
            }

            public void RemoveAllListeners()
            {
                m_UIEventHandle -= m_UIEventHandle;
                m_UIEventHandle = null;
            }

            public void Invoke(GameObject go, T eventData)
            {
                m_UIEventHandle?.Invoke(go, eventData, m_Arg);
            }

            private object m_Arg = null;
            private event UIEventHandle<T> m_UIEventHandle = null;
        }

        public UIEvent<PointerEventData> onClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDoubleClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onPress = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onUp = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDown = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onEnter = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onExit = new UIEvent<PointerEventData>();
        public UIEvent<BaseEventData> onSelect = new UIEvent<BaseEventData>();
        public UIEvent<BaseEventData> onUpdateSelect = new UIEvent<BaseEventData>();
        public UIEvent<BaseEventData> onDeselect = new UIEvent<BaseEventData>();
        public UIEvent<PointerEventData> onBeginDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onEndDrag = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDrop = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onScroll = new UIEvent<PointerEventData>();
        public UIEvent<AxisEventData> onMove = new UIEvent<AxisEventData>();

        public static UIEventListener Get(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            return go.GetOrAddComponent<UIEventListener>();
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
                onUp.Invoke(gameObject, m_OnUpEventData);
                onDoubleClick.Invoke(gameObject, m_OnUpEventData);
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                m_OnUpEventData = null;
            }

            if (Time.unscaledTime - m_CurrDonwTime >= DOUBLE_CLICK_TIME)
            {
                onUp.Invoke(gameObject, m_OnUpEventData);
                onClick.Invoke(gameObject, m_OnUpEventData);
                m_ClickCount = 0;
                m_CurrDonwTime = 0f;
                m_OnUpEventData = null;
            }
        }

        private void OnDestroy()
        {
            RemoveAllListeners();
        }

        public void RemoveAllListeners()
        {
            onClick.RemoveAllListeners();
            onDoubleClick.RemoveAllListeners();
            onDown.RemoveAllListeners();
            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();
            onUp.RemoveAllListeners();
            onSelect.RemoveAllListeners();
            onUpdateSelect.RemoveAllListeners();
            onDeselect.RemoveAllListeners();
            onDrag.RemoveAllListeners();
            onEndDrag.RemoveAllListeners();
            onDrop.RemoveAllListeners();
            onScroll.RemoveAllListeners();
            onMove.RemoveAllListeners();
        }

        public void OnPointerClick(PointerEventData eventData)
        {

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
            m_IsPointDown = false;
            m_OnUpEventData = eventData;

            if (!m_IsPress)
            {
                m_ClickCount++;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) { onEnter.Invoke(gameObject, eventData); }
        public void OnPointerExit(PointerEventData eventData) { onExit.Invoke(gameObject, eventData); }
        public void OnSelect(BaseEventData eventData) { onSelect.Invoke(gameObject, eventData); }
        public void OnUpdateSelected(BaseEventData eventData) { onUpdateSelect.Invoke(gameObject, eventData); }
        public void OnDeselect(BaseEventData eventData) { onDeselect.Invoke(gameObject, eventData); }
        public void OnBeginDrag(PointerEventData eventData) { onBeginDrag.Invoke(gameObject, eventData); }
        public void OnDrag(PointerEventData eventData) { onDrag.Invoke(gameObject, eventData); }
        public void OnEndDrag(PointerEventData eventData) { onEndDrag.Invoke(gameObject, eventData); }
        public void OnDrop(PointerEventData eventData) { onDrop.Invoke(gameObject, eventData); }
        public void OnScroll(PointerEventData eventData) { onScroll.Invoke(gameObject, eventData); }
        public void OnMove(AxisEventData eventData) { onMove.Invoke(gameObject, eventData); }

        private const float DOUBLE_CLICK_TIME = 0.2f;
        private const float PRESS_TIME = 0.5f;

        private float m_CurrDonwTime = 0f;
        private bool m_IsPointDown = false;
        private bool m_IsPress = false;
        private int m_ClickCount = 0;
        private PointerEventData m_OnUpEventData = null;
        private PointerEventData m_OnDownEventData = null;
    }
}