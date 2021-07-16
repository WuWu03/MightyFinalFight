/*******************************************************/
/**2021-7-14 15:31**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class RoleSelectPanelComponent : BasePanelComponent
{
	//RoleContent,GameObject
	public GameObject RoleContent { get; private set; }
	//RoleContent/Item,GameObject
	public GameObject ItemGO { get; private set; }
	//ImgSelect,RectTransform
	public RectTransform ImgSelectRect { get; private set; }
	public LayoutGroupView<RoleContentItem> RoleContentGroupView { get; private set; }

	public RoleSelectPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		RoleContent = root.Objects[0] as GameObject;
		ItemGO = root.Objects[1] as GameObject;
		ImgSelectRect = root.Objects[2] as RectTransform;
		RoleContentGroupView = new LayoutGroupView<RoleContentItem>();
	}

	public class RoleContentItem : LayoutGroupViewItem
	{
		public MyButton BtnRoleIcon = null;
		public Text TxtName = null;
		public Text TxtDesc = null;
		protected override void OnCreate(GameObject go)
		{
			BtnRoleIcon = transform.Find("BtnRoleIcon").GetComponent<MyButton>();
			TxtName = transform.Find("TxtName").GetComponent<Text>();
			TxtDesc = transform.Find("TxtDesc").GetComponent<Text>();
		}
	}
}