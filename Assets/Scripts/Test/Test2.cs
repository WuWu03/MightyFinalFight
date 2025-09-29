using GameFrameWork.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test2 : MonoBehaviour
{
    public ButtonEx e;
    private void Awake()
    {
        e.onPress.AddListener(OnPress, 0);
        e.onClick.AddListener(OnClick, 1);
        e.onDoubleClick.AddListener(OnDoubleClick, 2);
    }

    private void OnPress(GameObject t1, PointerEventData t2, int arg)
    {
        Debug.Log("OnClick" + arg);
    }

    private void OnClick(GameObject t1, PointerEventData t2, int arg)
    {
        Debug.Log("OnClick" + arg);
    }

    private void OnDoubleClick(GameObject t1, PointerEventData t2, int arg)
    {
        Debug.Log("OnClick" + arg);
    }
}