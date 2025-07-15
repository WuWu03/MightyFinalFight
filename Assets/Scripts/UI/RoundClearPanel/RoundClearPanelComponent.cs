/*******************************************************/
/**2025-07-12 15:13*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class RoundClearPanelComponent : BasePanelComponent
{
	//bottom/txtRound,LanguageText
	public LanguageText txtRound { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtRound = root.objects[0] as LanguageText;
	}
}