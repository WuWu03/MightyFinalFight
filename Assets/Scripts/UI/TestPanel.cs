/*******************************************************/
/**2024-02-25 09:24*************************************/
/**Create By WuWu***************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using System;
using GameFrameWork.Resources;

public class TestPanel : BasePanel
{
	public override string panelName { get { return "TestPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer3; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Destroy; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new TestPanelComponent(m_UIRefRoot);
        m_Component.testScroll.itemUpdateEvent += ItemUpdateEvent;
        m_Component.testScroll.getItemSizeEvent += GetItemSizeEvent;
        m_Component.testScroll.getDataCountEvent += GetDataCountEvent;
        m_Component.testScroll.Init<TestPanelComponent.ContentItem>(m_Component.testItemGO);

		Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(m_Component.mask.rectTransform, m_Component.txtLeftBottom.rectTransform);
		m_Component.mask._target = m_Component.txtLeftBottom.rectTransform;
        m_Component.mask._targetBoundsMin = bounds.min;
        m_Component.mask._targetBoundsMax = bounds.max;

		transform.Find("bg").GetComponent<ButtonEx>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
		InnerClose();
    }

	UnityEngine.Object m_Object = null;
    protected override void OnOpen()
	{
		m_Component.testScroll.RefreshData();
        m_Component.txtLeftBottom.text = "左下角测试";
        
        ResourcesPool.instance.Get<GameObject>("ArtResources/Prefabs/CharacterUI/CodyUI", OnLoad);
    }

    private void OnLoad(string t1, UnityEngine.Object t2, object[] t3)
    {
		(t2 as GameObject).transform.SetParent(transform, false);
		m_Object = t2;
    }

    private int GetDataCountEvent()
    {
		return 50;
    }

    private float GetItemSizeEvent(int t)
    {
		return 100f;
    }

    private void ItemUpdateEvent(ScrollLayoutGroupViewItem t)
    {
		TestPanelComponent.ContentItem item = t as TestPanelComponent.ContentItem;
		item.txtIndex.text = t.itemIndex.ToString();
		item.txtName.text = "测试格子+无限滚动列表";
    }

    protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
	}

	protected override void OnDestroy()
	{
	}

	private TestPanelComponent m_Component = null;
}