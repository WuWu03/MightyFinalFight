using System;
using System.IO;
using GameFrameWork.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestItem : ScrollListItem
{
    public TextMeshProUGUI text = null;

    protected override void OnCreate(GameObject go)
    {
        text = go.transform.Find("text").GetComponent<TextMeshProUGUI>();
    }

    public override void OnUpdate()
    {
        text.text = itemIndex.ToString();
    }
}

public class Test2 : MonoBehaviour
{
    public ScrollList view;
    public GameObject go;
    private int count = 100;
    private void Awake()
    {
        Debug.Log(Path.GetExtension(".abs"));
    }

    private void OnScrolled()
    {
        // if (view.normalizedScrollPosition <= 0)
        // {
        //     count += 10;
        //     view.RefreshData(true);
        // }
    }

    private float ItemSize(int t)
    {
        return 50;
    }

    private void Start()
    {
     
       // view.RefreshData();
        //view.SetScrollPositionImmediately(view.scrollSize);
    }

    private int GetDataCount()
    {
        return count;
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