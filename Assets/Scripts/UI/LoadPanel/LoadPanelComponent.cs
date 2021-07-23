/*******************************************************/
/**2021-7-23 11:29**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class LoadPanelComponent : BasePanelComponent
{
	//imgShade,Image
	public Image ImgShade { get; private set; }

	public LoadPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		ImgShade = root.Objects[0] as Image;
	}
}