/*******************************************************/
/**2021-9-7 12:28**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class TitlePanelComponent : BasePanelComponent
{
	//ImgLogoBG,Image
	public Image ImgLogoBG { get; private set; }
	//ImgRetro,Image
	public Image ImgRetro { get; private set; }
	//ImgLogo,Image
	public Image ImgLogo { get; private set; }
	//TxtStart,Text
	public Text TxtStart { get; private set; }
	//TxtDeveloper,Text
	public Text TxtDeveloper { get; private set; }
	//ImgCapcom,Image
	public Image ImgCapcom { get; private set; }

	public TitlePanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		ImgLogoBG = root.Objects[0] as Image;
		ImgRetro = root.Objects[1] as Image;
		ImgLogo = root.Objects[2] as Image;
		TxtStart = root.Objects[3] as Text;
		TxtDeveloper = root.Objects[4] as Text;
		ImgCapcom = root.Objects[5] as Image;
	}
}