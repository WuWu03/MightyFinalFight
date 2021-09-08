/*******************************************************/
/**2021-9-8 15:44**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class StagePanelComponent : BasePanelComponent
{
	//Blue,GameObject
	public GameObject Blue { get; private set; }
	//Red,GameObject
	public GameObject Red { get; private set; }
	//Green,GameObject
	public GameObject Green { get; private set; }
	//ImgMap,GameObject
	public GameObject ImgMapGO { get; private set; }
	//HeroPos,GameObject
	public GameObject HeroPosGO { get; private set; }

	public StagePanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		Blue = root.Objects[0] as GameObject;
		Red = root.Objects[1] as GameObject;
		Green = root.Objects[2] as GameObject;
		ImgMapGO = root.Objects[3] as GameObject;
		HeroPosGO = root.Objects[4] as GameObject;
	}
}