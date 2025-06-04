/*******************************************************/
/**2025-06-04 15:31*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class HudPanelComponent : BasePanelComponent
{
	//txtDamage,GameObject
	public GameObject txtDamageGO { get; private set; }

	public HudPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		txtDamageGO = root.objects[0] as GameObject;
	}
}