/*******************************************************/
/**2025-06-04 16:43*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class StagePanelComponent : BasePanelComponent
{
	//blue,GameObject
	public GameObject blue { get; private set; }
	//red,GameObject
	public GameObject red { get; private set; }
	//green,GameObject
	public GameObject green { get; private set; }
	//imgMap,GameObject
	public GameObject imgMapGO { get; private set; }
	//heroPos,GameObject
	public GameObject heroPosGO { get; private set; }

	public StagePanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		blue = root.objects[0] as GameObject;
		red = root.objects[1] as GameObject;
		green = root.objects[2] as GameObject;
		imgMapGO = root.objects[3] as GameObject;
		heroPosGO = root.objects[4] as GameObject;
	}
}