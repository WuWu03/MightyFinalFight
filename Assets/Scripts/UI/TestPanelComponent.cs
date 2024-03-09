/*******************************************************/
/**2024-02-27 09:38*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class TestPanelComponent : BasePanelComponent
{
	//mask,TestMask
	public TestMask mask { get; private set; }
	//txtLeftBottom,Text
	public Text txtLeftBottom { get; private set; }
	//testScroll,ScrollLayoutGroupView
	public ScrollLayoutGroupView testScroll { get; private set; }
	//testScroll/Viewport/Content,GameObject
	public GameObject content { get; private set; }
	//testScroll/Viewport/Content/testItem,GameObject
	public GameObject testItemGO { get; private set; }

	public TestPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		mask = root.objects[0] as TestMask;
		txtLeftBottom = root.objects[1] as Text;
		testScroll = root.objects[2] as ScrollLayoutGroupView;
		content = root.objects[3] as GameObject;
		testItemGO = root.objects[4] as GameObject;
	}

	public class ContentItem : ScrollLayoutGroupViewItem
	{
		public Image bg = null;
		public Text txtName = null;
		public Text txtIndex = null;
		protected override void OnCreate(GameObject go)
		{
			bg = transform.Find("bg").GetComponent<Image>();
			txtName = transform.Find("txtName").GetComponent<Text>();
			txtIndex = transform.Find("txtIndex").GetComponent<Text>();
		}
	}
}