using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WuWuFramework.UI
{
    [AddComponentMenu("UI/ScrollRectDrag")]
    public class ScrollRectDrag : MonoBehaviour
    {
        [HeaderAttribute("要拖动的ScrollRect")]
        public ScrollRect[] dragScrolls;

        private void Awake()
        {
            UIEventListener.Get(gameObject).onBeginDrag.AddListener(OnBeginDrag);
            UIEventListener.Get(gameObject).onDrag.AddListener(OnDrag);
            UIEventListener.Get(gameObject).onEndDrag.AddListener(OnEndDrag);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            foreach (var scrollRect in dragScrolls)
            {
                scrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            foreach (var scrollRect in dragScrolls)
            {
                scrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            foreach (var scrollRect in dragScrolls)
            {
                scrollRect.OnEndDrag(eventData);
            }
        }
    }
}
