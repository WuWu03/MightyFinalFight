/*******************************************************/
/**2024-1-16 16:59**************************************/
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
	public Image imgShade { get; private set; }

	public LoadPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		imgShade = root.objects[0] as Image;
	}
}