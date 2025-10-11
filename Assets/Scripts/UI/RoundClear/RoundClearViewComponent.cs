/*
 * @Desc: RoundClear 模块 RoundClearView 界面数据
 * @Date: 2025-10-11 12:16:07
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class RoundClearViewComponent : UIBaseComponent
{
	//bottom/txtRound,LanguageText
	public LanguageText txtRound { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtRound = root.objects[0] as LanguageText;
	}
}