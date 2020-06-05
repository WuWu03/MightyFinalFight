/*******************************************************/
/**2020-5-22 11:45****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FrameWork.UI;

public class NewPanel : BasePanel
{
	public override string PanelName { get { return "NewPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Always; } }
	//Image1,Image
	public Image Image1 { get; private set;}
	//List1,RectTransform
	public RectTransform List1 { get; private set;}
	public LayoutGroupLoopView<List1Item> List1GroupView { get; private set;}
	protected override void OnInit()
	{
		Image1 = UIRefRoot.Objects[0] as Image;
		List1 = UIRefRoot.Objects[1] as RectTransform;
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