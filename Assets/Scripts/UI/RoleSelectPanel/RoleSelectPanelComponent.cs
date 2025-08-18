/*******************************************************/
/**2025-08-16 13:36*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

	protected override void OnInitComponent(UIRefRoot root)
	{
		roleContent = root.objects[0] as GameObject;
		itemGO = root.objects[1] as GameObject;
		imgSelectRect = root.objects[2] as RectTransform;
		roleContentGroupView = new LayoutGroupView<RoleContentItem>();
	}

	public class RoleContentItem : LayoutGroupViewItem
	{
		public Image imgRoleIcon = null;
		public LanguageText txtName = null;
		public LanguageText txtDesc = null;
		protected override void OnCreate(GameObject go)
		{
			imgRoleIcon = transform.Find("imgRoleIcon").GetComponent<Image>();
			txtName = transform.Find("txtName").GetComponent<LanguageText>();
			txtDesc = transform.Find("txtDesc").GetComponent<LanguageText>();
		}
	}
}