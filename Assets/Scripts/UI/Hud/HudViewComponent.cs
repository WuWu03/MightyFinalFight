/*
 * @Desc: Hud 模块 HudView 界面组件
 * @Date: 2025-11-26 16:35:10
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