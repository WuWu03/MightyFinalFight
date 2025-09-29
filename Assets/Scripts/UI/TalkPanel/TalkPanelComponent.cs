/*******************************************************/
/**2025-07-04 21:19*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/

using TMPro;
using UnityEngine;
using GameFrameWork.UI;

public class TalkPanelComponent : BasePanelComponent
{
	//bottom/txtContent,LanguageText
	public LanguageText languageContent { get; private set; }
	//bottom/txtContent,TextMeshProUGUI
	public TextMeshProUGUI txtContent { get; private set; }
	//bottom/talkSelect,GameObject
	public GameObject talkSelect { get; private set; }
	//bottom/talkSelect/talkSelectItem,GameObject
	public GameObject talkSelectItem { get; private set; }
	public LayoutGroupView<TalkSelectItem> talkSelectGroupView { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		languageContent = root.objects[0] as LanguageText;
		txtContent = root.objects[1] as TextMeshProUGUI;
		talkSelect = root.objects[2] as GameObject;
		talkSelectItem = root.objects[3] as GameObject;
		talkSelectGroupView = new LayoutGroupView<TalkSelectItem>(talkSelect, talkSelectItem);
	}

	public class TalkSelectItem : LayoutGroupViewItem
	{
		public LanguageText languageSelect = null;
		public TextMeshProUGUI txtSelect = null;
		public GameObject selectGO = null;
		protected override void OnCreate(GameObject go)
		{
			languageSelect = transform.Find("txtSelect").GetComponent<LanguageText>();
			txtSelect = transform.Find("txtSelect").GetComponent<TextMeshProUGUI>();
			selectGO = transform.Find("select").gameObject;
		}
	}
}