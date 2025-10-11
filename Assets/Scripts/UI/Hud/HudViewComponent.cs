/*
 * @Desc: Hud 模块 HudView 界面数据
 * @Date: 2025-10-11 12:07:00
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class HudViewComponent : UIBaseComponent
{
	//txtDamage,GameObject
	public GameObject txtDamageGo { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtDamageGo = root.objects[0] as GameObject;
	}
}