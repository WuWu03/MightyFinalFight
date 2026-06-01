using System;
using WuWuFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class TestItem : BaseListItem
{
    public TextMeshProUGUI txt;

    protected override void OnCreate(GameObject go)
    {
        txt = go.transform.Find("txt").GetComponent<TextMeshProUGUI>();
    }
}

public class Test2 : MonoBehaviour
{
    public WuWuFramework.UI.ScrollList view;
    public ButtonEx btn;

    private InputAction stickAction;
    private void Awake()
    {
        view.itemUpdateEvent += onRenderItem;
        // btn.onClick.AddListener(delegate(GameObject go)
        // {
        //     this.OnClick(go, 3);
        // });
        //
        // btn.onDoubleClick.AddListener(OnDoubleClick);
        // btn.onPress.AddListener(OnPress);
        //stickAction.starte
        
        //actions.Button
    }

    private void onRenderItem(WuWuFramework.UI.BaseListItem t)
    {
        var item = t as TestItem;
        item.txt.text = item.index.ToString();
    }

    private void Start()
    {      
        view.Init<TestItem>();
        view.SetItemCount(99);
    }

    private void OnPress(GameObject t1)
    {
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