using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace WuWuFramework.UI
{
    public class UIEventListener :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
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
        public UnityEvent<PointerEventData> onEnter = new();
        public UnityEvent<PointerEventData> onExit = new();
        public UnityEvent<BaseEventData> onSelect = new();
        public UnityEvent<BaseEventData> onUpdateSelect = new();
        public UnityEvent<BaseEventData> onDeselect = new();
        public UnityEvent<PointerEventData> onBeginDrag = new();
        public UnityEvent<PointerEventData> onDrag = new();
        public UnityEvent<PointerEventData> onEndDrag = new();
        public UnityEvent<PointerEventData> onDrop = new();
        public UnityEvent<PointerEventData> onScroll = new();
        public UnityEvent<AxisEventData> onMove = new();

        public static UIEventListener Get(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            return go.GetOrAddComponent<UIEventListener>();
        }

        private void OnDestroy()
        {
            RemoveAllListeners();
        }

        public void RemoveAllListeners()
        {
            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();
            onSelect.RemoveAllListeners();
            onUpdateSelect.RemoveAllListeners();
            onDeselect.RemoveAllListeners();
            onDrag.RemoveAllListeners();
            onEndDrag.RemoveAllListeners();
            onDrop.RemoveAllListeners();
            onScroll.RemoveAllListeners();
            onMove.RemoveAllListeners();
        }

        public void OnPointerEnter(PointerEventData eventData) { onEnter.Invoke(eventData); }
        public void OnPointerExit(PointerEventData eventData) { onExit.Invoke(eventData); }
        public void OnSelect(BaseEventData eventData) { onSelect.Invoke(eventData); }
        public void OnUpdateSelected(BaseEventData eventData) { onUpdateSelect.Invoke(eventData); }
        public void OnDeselect(BaseEventData eventData) { onDeselect.Invoke(eventData); }
        public void OnBeginDrag(PointerEventData eventData) { onBeginDrag.Invoke(eventData); }
        public void OnDrag(PointerEventData eventData) { onDrag.Invoke(eventData); }
        public void OnEndDrag(PointerEventData eventData) { onEndDrag.Invoke(eventData); }
        public void OnDrop(PointerEventData eventData) { onDrop.Invoke(eventData); }
        public void OnScroll(PointerEventData eventData) { onScroll.Invoke(eventData); }
        public void OnMove(AxisEventData eventData) { onMove.Invoke(eventData); }
    }
}