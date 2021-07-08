using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ScrollRectDrag")]
    public class ScrollRectDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [HeaderAttribute("要拖动的ScrollRect")]
        public ScrollRect DragScroll;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DragScroll != null)
            {
                DragScroll.OnBeginDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragScroll != null)
            {
                DragScroll.OnEndDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (DragScroll != null)
            {
                DragScroll.OnDrag(eventData);
            }
        }
    }
}
