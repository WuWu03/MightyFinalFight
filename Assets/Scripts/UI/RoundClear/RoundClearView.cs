/*
 * @Desc: RoundClear 模块 RoundClearView 视图
 * @Date: 2026-07-04 17:28:09
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class RoundClearView : UIBaseView<RoundClearView, RoundClearViewPresenter, RoundClearViewSettings>
{
	//bottom/txtRound,LanguageText
	public LanguageText txtRound { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		txtRound = root.objects[0] as LanguageText;
	}
}