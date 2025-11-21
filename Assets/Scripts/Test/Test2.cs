using System;
using GameFrameWork.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TestItem : GameFrameWork.UI.ScrollListItem
{
    public TextMeshProUGUI txt;

    protected override void OnCreate(GameObject go)
    {
        txt = go.transform.Find("txt").GetComponent<TextMeshProUGUI>();
    }
}

public class Test2 : MonoBehaviour
{
    public GameFrameWork.UI.ScrollList view;
    public ButtonEx btn;

    private void Awake()
    {
        view.renderItemEvent += onRenderItem;
        // btn.onClick.AddListener(delegate(GameObject go)
        // {
        //     this.OnClick(go, 3);
        // });
        //
        // btn.onDoubleClick.AddListener(OnDoubleClick);
        // btn.onPress.AddListener(OnPress);
    }

    private void onRenderItem(GameFrameWork.UI.ScrollListItem t)
    {
        var item = t as TestItem;
        item.txt.text = item.itemIndex.ToString();
    }

    private void Start()
    {      
        view.Init<TestItem>();
        view.SetItemCount(99);
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