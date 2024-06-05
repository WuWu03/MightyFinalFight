/*******************************************************/
/**2024-06-05 11:36*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
using GameFrameWork.Localization;
public class TalkPanelComponent : BasePanelComponent
{
	//bottom/txtContent,Text
	public Text txtContent { get; private set; }
	//bottom/talkSelect,GameObject
	public GameObject talkSelect { get; private set; }
	//bottom/talkSelect/talkSelectItem,GameObject
	public GameObject talkSelectItem { get; private set; }
	public LayoutGroupView<TalkSelectItem> talkSelectGroupView { get; private set; }

	public TalkPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		txtContent = root.objects[0] as Text;
		talkSelect = root.objects[1] as GameObject;
		talkSelectItem = root.objects[2] as GameObject;
		talkSelectGroupView = new LayoutGroupView<TalkSelectItem>();
	}

	public class TalkSelectItem : LayoutGroupViewItem
	{
		public Text txtSelect = null;
		public GameObject selectGO = null;
		protected override void OnCreate(GameObject go)
		{
			txtSelect = transform.Find("txtSelect").GetComponent<Text>();
			selectGO = transform.Find("select").gameObject;
		}
	}
}