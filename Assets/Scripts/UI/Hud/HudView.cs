/*
 * @Desc: Hud 模块 HudView 视图
 * @Date: 2026-05-22 22:37:38
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class HudView : UIBaseView<HudView, HudViewPresenter, HudViewSettings>
{
	//txtDamage,GameObject
	public GameObject txtDamageGo { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		txtDamageGo = root.objects[0] as GameObject;
	}
}