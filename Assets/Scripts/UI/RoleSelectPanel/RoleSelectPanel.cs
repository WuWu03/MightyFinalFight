/*******************************************************/
/**2020-4-15 16:0****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FrameWork.UI;

public class RoleSelectPanel : BasePanel
{
	public override string PanelName { get { return "RoleSelectPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.DelayDestroy; } }
	//RoleContent,GameObject
	public GameObject RoleContent { get; private set;}
	//RoleContent/Item,GameObject
	public GameObject ItemGO { get; private set;}
	//ImgSelect,RectTransform
	public RectTransform ImgSelectRect { get; private set;}
	public LayoutGroupView<RoleContentItem> RoleContentGroupView { get; private set;}
	protected override void OnInit()
	{
		RoleContent = UIRefRoot.Objects[0] as GameObject;
		ItemGO = UIRefRoot.Objects[1] as GameObject;
		ImgSelectRect = UIRefRoot.Objects[2] as RectTransform;
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