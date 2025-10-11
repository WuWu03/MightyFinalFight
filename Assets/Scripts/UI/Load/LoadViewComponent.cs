/*
 * @Desc: Load 模块 LoadView 界面数据
 * @Date: 2025-10-11 12:10:01
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class LoadViewComponent : UIBaseComponent
{
	//imgShade,Image
	public Image imgShade { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		imgShade = root.objects[0] as Image;
	}
}