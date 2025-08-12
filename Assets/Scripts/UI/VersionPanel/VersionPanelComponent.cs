/*******************************************************/
/**2025-08-08 14:43*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class VersionPanelComponent : BasePanelComponent
{
	//txtVersion,LanguageText
	public LanguageText txtVersion { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtVersion = root.objects[0] as LanguageText;
	}
}