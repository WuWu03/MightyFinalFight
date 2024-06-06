/*******************************************************/
/**2024-06-05 12:02*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class RoleSelectPanelComponent : BasePanelComponent
{
	//roleContent,GameObject
	public GameObject roleContent { get; private set; }
	//roleContent/item,GameObject
	public GameObject itemGO { get; private set; }
	//imgSelect,RectTransform
	public RectTransform imgSelectRect { get; private set; }
	public LayoutGroupView<RoleContentItem> roleContentGroupView { get; private set; }

	public RoleSelectPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		roleContent = root.objects[0] as GameObject;
		itemGO = root.objects[1] as GameObject;
		imgSelectRect = root.objects[2] as RectTransform;
		roleContentGroupView = new LayoutGroupView<RoleContentItem>();
	}

	public class RoleContentItem : LayoutGroupViewItem
	{
		public ButtonEx btnRoleIcon = null;
		public LanguageText txtName = null;
		public LanguageText txtDesc = null;
		protected override void OnCreate(GameObject go)
		{
			btnRoleIcon = transform.Find("btnRoleIcon").GetComponent<ButtonEx>();
			txtName = transform.Find("txtName").GetComponent<LanguageText>();
			txtDesc = transform.Find("txtDesc").GetComponent<LanguageText>();
		}
	}
}