using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    public sealed class UIEventListener :
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
        [Serializable]
        public class UIEvent<T> where T : BaseEventData
        {
            public UIEvent()
            {
                m_UIEventHandlers = new();
            }

            public void AddListener(GameFrameWorkAction<GameObject, T, object> handle, object arg = null)
            {
                if (m_UIEventHandlers.ContainsKey(handle))
                {
                    Log.LogError("事件已经存在");
                    return;
                }

                m_UIEventHandlers.Add(handle, arg);
            }

            public void RemoveListener(GameFrameWorkAction<GameObject, T, object> handle)
            {
                m_UIEventHandlers.Remove(handle);
            }

            public void RemoveAllListeners()
            {
                m_UIEventHandlers.Clear();
            }

            public void Invoke(GameObject go, T eventData)
            {
                foreach (var handler in m_UIEventHandlers)
                {
                    handler.Key.Invoke(go, eventData, handler.Value);
                }
            }

            private Dictionary<GameFrameWorkAction<GameObject, T, object>, object> m_UIEventHandlers = null;
        }

        public UIEvent<PointerEventData> onEnter = new();
        public UIEvent<PointerEventData> onExit = new();
        public UIEvent<BaseEventData> onSelect = new();
        public UIEvent<BaseEventData> onUpdateSelect = new();
        public UIEvent<BaseEventData> onDeselect = new();
        public UIEvent<PointerEventData> onBeginDrag = new();
        public UIEvent<PointerEventData> onDrag = new();
        public UIEvent<PointerEventData> onEndDrag = new();
        public UIEvent<PointerEventData> onDrop = new();
        public UIEvent<PointerEventData> onScroll = new();
        public UIEvent<AxisEventData> onMove = new();

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
    }
}