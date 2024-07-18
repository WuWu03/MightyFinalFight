/*******************************************************/
/**2024-06-11 17:03*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class TalkPanelComponent : BasePanelComponent
{
	//bottom/txtContent,LanguageText
	public LanguageText languageContent { get; private set; }
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
		languageContent = root.objects[0] as LanguageText;
		txtContent = root.objects[1] as Text;
		talkSelect = root.objects[2] as GameObject;
		talkSelectItem = root.objects[3] as GameObject;
		talkSelectGroupView = new LayoutGroupView<TalkSelectItem>();
	}

	public class TalkSelectItem : LayoutGroupViewItem
	{
		public LanguageText languageSelect = null;
		public Text txtSelect = null;
		public GameObject selectGO = null;
		protected override void OnCreate(GameObject go)
		{
			languageSelect = transform.Find("txtSelect").GetComponent<LanguageText>();
			txtSelect = transform.Find("txtSelect").GetComponent<Text>();
			selectGO = transform.Find("select").gameObject;
		}
	}
}