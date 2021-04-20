/*******************************************************/
/**2021-4-19 14:39**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class NewPanelComponent:BasePanelComponent
{
	//Image1,Image
	public Image Image1 { get; private set;}
	//List1,RectTransform
	public RectTransform List1 { get; private set;}
	//List1/Item,GameObject
	public GameObject ItemGO { get; private set;}
	public LayoutGroupLoopView<List1Item> List1GroupView { get; private set;}

	public NewPanelComponent(UIRefRoot root) : base(root) { }
	protected override void InitComponent(UIRefRoot root)
	{
		Image1 = root.Objects[0] as Image;
		List1 = root.Objects[1] as RectTransform;
		ItemGO = root.Objects[2] as GameObject;
		List1GroupView = new LayoutGroupLoopView<List1Item>();
	}

	public class List1Item : LayoutGroupViewItem
	{
		public Image Icon = null;
		protected override void OnCreate(GameObject go)
		{
			Icon = transform.Find("Icon").GetComponent<Image>();
		}
	}
}