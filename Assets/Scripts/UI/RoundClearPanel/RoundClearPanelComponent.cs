/*******************************************************/
/**2024-06-05 11:36*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
using GameFrameWork.Localization;
public class RoundClearPanelComponent : BasePanelComponent
{
	//bottom/GameObject/txtRound,Text
	public Text txtRound { get; private set; }

	public RoundClearPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		txtRound = root.objects[0] as Text;
	}
}