using GameFrameWork.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{        
    public ScrollList view;
    public ButtonEx btn;
 
    private void Awake()
    {
        btn.onClick.AddListener(delegate(GameObject go)
        {
            this.OnClick(go, 3);
        });
        
        btn.onDoubleClick.AddListener(OnDoubleClick);
        btn.onPress.AddListener(OnPress);
    }
    
    private void OnPress(GameObject t1)
    {
        Button b;
        
        Debug.Log("OnPress");
    }

    private void OnClick(GameObject t1, int arg)
    {
        Debug.Log("OnClick" + arg);
    }

    private void OnDoubleClick(GameObject t1)
    {
        Debug.Log("OnDoubleClick");
    }
}