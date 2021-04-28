/*******************************************************/
/**2021-4-28 17:36**************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
public class MainPanelComponent : BasePanelComponent
{
	//Player/PlayerHpBar,Slider
	public Slider PlayerHpBar { get; private set; }
	//Enemy/EnemyHpBar,Slider
	public Slider EnemyHpBar { get; private set; }
	//State/TxtStage,Text
	public Text TxtStage { get; private set; }
	//PlayerLife/TxtPlayerLife,Text
	public Text TxtPlayerLife { get; private set; }
	//Level/LevelList,GameObject
	public GameObject LevelList { get; private set; }
	//Level/LevelList/Item,GameObject
	public GameObject ItemGO { get; private set; }
	//Exp/TxtExp,Text
	public Text TxtExp { get; private set; }
	public LayoutGroupView<LevelListItem> LevelListGroupView { get; private set; }

	public MainPanelComponent(UIRefRoot root) : base(root) { }
	protected override void InitComponent(UIRefRoot root)
	{
		PlayerHpBar = root.Objects[0] as Slider;
		EnemyHpBar = root.Objects[1] as Slider;
		TxtStage = root.Objects[2] as Text;
		TxtPlayerLife = root.Objects[3] as Text;
		LevelList = root.Objects[4] as GameObject;
		ItemGO = root.Objects[5] as GameObject;
		TxtExp = root.Objects[11] as Text;
		LevelListGroupView = new LayoutGroupView<LevelListItem>();
	}

	public class LevelListItem : LayoutGroupViewItem
	{
		public Image ImgLevel1 = null;
		public Image ImgLevel2 = null;
		public Image ImgLevel3 = null;
		public Image ImgLevel4 = null;
		public Image ImgLevel5 = null;
		protected override void OnCreate(GameObject go)
		{
			ImgLevel1 = transform.Find("ImgLevel1").GetComponent<Image>();
			ImgLevel2 = transform.Find("ImgLevel2").GetComponent<Image>();
			ImgLevel3 = transform.Find("ImgLevel3").GetComponent<Image>();
			ImgLevel4 = transform.Find("ImgLevel4").GetComponent<Image>();
			ImgLevel5 = transform.Find("ImgLevel5").GetComponent<Image>();
		}
	}
}