/*
 * @Desc: Talk 模块 TalkView 界面数据
 * @Date: 2025-10-11 12:28:19
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class TalkViewComponent : UIBaseComponent
{
	//bottom/txtContent,LanguageText
	public LanguageText languageContent { get; private set; }
	//bottom/txtContent,TextMeshProUGUI
	public TextMeshProUGUI txtContent { get; private set; }
	public LayoutGroupView<TalkSelectItem> talkSelectGroupView { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		languageContent = root.objects[0] as LanguageText;
		txtContent = root.objects[1] as TextMeshProUGUI;
		GameObject talkSelect = root.objects[2] as GameObject;
		GameObject talkSelectItem = root.objects[3] as GameObject;
		talkSelectGroupView = new LayoutGroupView<TalkSelectItem>(talkSelect,talkSelectItem);
	}

	public class TalkSelectItem : LayoutGroupViewItem
	{
		public LanguageText languageSelect = null;
		public TextMeshProUGUI txtSelect = null;
		public GameObject selectGo = null;
		protected override void OnCreate(GameObject go)
		{
			UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
			languageSelect = uiRefRoot.objects[0] as LanguageText;
			txtSelect = uiRefRoot.objects[1] as TextMeshProUGUI;
			selectGo = uiRefRoot.objects[2] as GameObject;
		}
	}
}