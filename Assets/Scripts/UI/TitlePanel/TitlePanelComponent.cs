/*******************************************************/
/**2025-07-12 15:16*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class TitlePanelComponent : BasePanelComponent
{
	//imgLogoBG,Image
	public Image imgLogoBG { get; private set; }
	//imgStar,Image
	public Image imgStar { get; private set; }
	//imgRetro,Image
	public Image imgRetro { get; private set; }
	//imgLogo,Image
	public Image imgLogo { get; private set; }
	//txtStart,LanguageText
	public LanguageText txtStart { get; private set; }
	//txtSettings,LanguageText
	public LanguageText txtSettings { get; private set; }
	//txtDeveloper,TextMeshProUGUI
	public TextMeshProUGUI txtDeveloper { get; private set; }
	//imgCapcom,Image
	public Image imgCapcom { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		imgLogoBG = root.objects[0] as Image;
		imgStar = root.objects[1] as Image;
		imgRetro = root.objects[2] as Image;
		imgLogo = root.objects[3] as Image;
		txtStart = root.objects[4] as LanguageText;
		txtSettings = root.objects[5] as LanguageText;
		txtDeveloper = root.objects[6] as TextMeshProUGUI;
		imgCapcom = root.objects[7] as Image;
	}
}