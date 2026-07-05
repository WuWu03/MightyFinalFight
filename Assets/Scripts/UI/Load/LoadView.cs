/*
 * @Desc: Load 模块 LoadView 视图
 * @Date: 2026-07-04 16:34:06
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class LoadView : UIBaseView<LoadView, LoadViewPresenter, LoadViewSettings>
{
	//imgShade,ImageEx
	public ImageEx imgShade { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		imgShade = root.objects[0] as ImageEx;
	}
}