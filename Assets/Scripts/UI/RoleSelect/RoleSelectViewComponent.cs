/*
 * @Desc: RoleSelect 模块 RoleSelectView 界面数据
 * @Date: 2025-10-11 11:06:50
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class RoleSelectViewComponent : UIBaseComponent
{
	public LayoutGroupView<RoleContentItem> roleContentGroupView { get; private set; }
	//imgSelect,RectTransform
	public RectTransform imgSelectRect { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		GameObject roleContent = root.objects[0] as GameObject;
		GameObject item = root.objects[1] as GameObject;
		roleContentGroupView = new LayoutGroupView<RoleContentItem>(roleContent,item);
		imgSelectRect = root.objects[2] as RectTransform;
	}

	public class RoleContentItem : LayoutGroupViewItem
	{
		public Image imgRoleIcon = null;
		public LanguageText txtName = null;
		public LanguageText txtDesc = null;
		protected override void OnCreate(GameObject go)
		{
			UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
			imgRoleIcon = uiRefRoot.objects[0] as Image;
			txtName = uiRefRoot.objects[1] as LanguageText;
			txtDesc = uiRefRoot.objects[2] as LanguageText;
		}
	}
}