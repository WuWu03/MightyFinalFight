/*
 * @Desc: Version 模块 VersionView 视图
 * @Date: 2026-07-04 17:28:46
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class VersionView : UIBaseView<VersionView, VersionViewPresenter, VersionViewSettings>
{
	//txtVersion,LanguageText
	public LanguageText txtVersion { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		txtVersion = root.objects[0] as LanguageText;
	}
}