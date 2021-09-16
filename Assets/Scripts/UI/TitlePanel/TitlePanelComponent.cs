/*******************************************************/
/**2021-9-16 18:17**************************************/
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
	//ImgStar,Image
	public Image ImgStar { get; private set; }
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
		ImgStar = root.Objects[2] as Image;
		ImgLogo = root.Objects[3] as Image;
		TxtStart = root.Objects[4] as Text;
		TxtDeveloper = root.Objects[5] as Text;
		ImgCapcom = root.Objects[6] as Image;
	}
}