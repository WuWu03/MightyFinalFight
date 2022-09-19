/*******************************************************/
/**2022-9-4 16:8**************************************/
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
	//imgLogoBG,Image
	public Image imgLogoBG { get; private set; }
	//imgRetro,Image
	public Image imgRetro { get; private set; }
	//imgStar,Image
	public Image imgStar { get; private set; }
	//imgLogo,Image
	public Image imgLogo { get; private set; }
	//txtStart,Text
	public Text txtStart { get; private set; }
	//txtDeveloper,Text
	public Text txtDeveloper { get; private set; }
	//imgCapcom,Image
	public Image imgCapcom { get; private set; }

	public TitlePanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		imgLogoBG = root.objects[0] as Image;
		imgRetro = root.objects[1] as Image;
		imgStar = root.objects[2] as Image;
		imgLogo = root.objects[3] as Image;
		txtStart = root.objects[4] as Text;
		txtDeveloper = root.objects[5] as Text;
		imgCapcom = root.objects[6] as Image;
	}
}