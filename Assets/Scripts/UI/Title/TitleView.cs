/*
 * @Desc: Title 模块 TitleView 视图
 * @Date: 2026-07-04 17:24:24
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class TitleView : UIBaseView<TitleView, TitleViewPresenter, TitleViewSettings>
{
	//imgLogoBG,ImageEx
	public ImageEx imgLogoBG { get; private set; }
	//imgStar,ImageEx
	public ImageEx imgStar { get; private set; }
	//imgRetro,ImageEx
	public ImageEx imgRetro { get; private set; }
	//imgLogo,ImageEx
	public ImageEx imgLogo { get; private set; }
	//txtStart,LanguageText
	public LanguageText txtStart { get; private set; }
	//txtSettings,LanguageText
	public LanguageText txtSettings { get; private set; }
	//txtDeveloper,TextMeshProUGUI
	public TextMeshProUGUI txtDeveloper { get; private set; }
	//imgCapcom,ImageEx
	public ImageEx imgCapcom { get; private set; }
	//imgIntro1,ImageEx
	public ImageEx imgIntro1 { get; private set; }
	//imgIntro2,ImageEx
	public ImageEx imgIntro2 { get; private set; }
	//txtIntro,LanguageText
	public LanguageText txtIntro { get; private set; }
	//txtIntro,TextMeshProUGUI
	public TextMeshProUGUI txtIntroTmp { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		imgLogoBG = root.objects[0] as ImageEx;
		imgStar = root.objects[1] as ImageEx;
		imgRetro = root.objects[2] as ImageEx;
		imgLogo = root.objects[3] as ImageEx;
		txtStart = root.objects[4] as LanguageText;
		txtSettings = root.objects[5] as LanguageText;
		txtDeveloper = root.objects[6] as TextMeshProUGUI;
		imgCapcom = root.objects[7] as ImageEx;
		imgIntro1 = root.objects[8] as ImageEx;
		imgIntro2 = root.objects[9] as ImageEx;
		txtIntro = root.objects[10] as LanguageText;
		txtIntroTmp = root.objects[11] as TextMeshProUGUI;
	}
}