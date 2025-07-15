/*******************************************************/
/**2025-07-04 21:25*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class HudPanelComponent : BasePanelComponent
{
	//txtDamage,GameObject
	public GameObject txtDamageGO { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtDamageGO = root.objects[0] as GameObject;
	}
}