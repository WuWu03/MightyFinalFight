/*******************************************************/
/**2024-06-06 17:18*************************************/
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
    //txtEnemyDamage,GameObject
    public GameObject txtEnemyDamageGO { get; private set; }
	//txtPlayerDamage,GameObject
	public GameObject txtPlayerDamageGO { get; private set; }

	public HudPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		txtEnemyDamageGO = root.objects[0] as GameObject;
		txtPlayerDamageGO = root.objects[1] as GameObject;
	}
}