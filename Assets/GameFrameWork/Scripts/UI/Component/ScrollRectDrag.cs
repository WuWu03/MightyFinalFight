using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ScrollRectDrag")]
    public class ScrollRectDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [HeaderAttribute("要拖动的ScrollRect")]
        public ScrollRect[] dragScrolls;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            for (int i = 0; i < dragScrolls.Length; i++)
            {
                dragScrolls[i].OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            for (int i = 0; i < dragScrolls.Length; i++)
            {
                dragScrolls[i].OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragScrolls == null || dragScrolls.Length < 1)
            {
                return;
            }

            for (int i = 0; i < dragScrolls.Length; i++)
            {
                dragScrolls[i].OnEndDrag(eventData);
            }
        }
    }
}
