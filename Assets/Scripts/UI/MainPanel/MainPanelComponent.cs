/*******************************************************/
/**2025-06-04 15:32*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class MainPanelComponent : BasePanelComponent
{
	//bg/top/player/playerHpBar,Slider
	public Slider playerHpBar { get; private set; }
	//bg/top/player/playerHpBar/playerHpBarImage,Image
	public Image playerHpBarImage { get; private set; }
	//bg/top/enemy/enemyHpBar,Slider
	public Slider enemyHpBar { get; private set; }
	//bg/top/enemy/enemyHpBar/playerHpBarImage,Image
	public Image playerHpBarImage1 { get; private set; }
	//bg/top/stage/txtStage,Text
	public Text txtStage { get; private set; }
	//bg/top/playerLife/txtPlayerLife,Text
	public Text txtPlayerLife { get; private set; }
	//level/levelList,GameObject
	public GameObject levelList { get; private set; }
	//level/levelList/item,GameObject
	public GameObject itemGO { get; private set; }
	//exp/txtExp,Text
	public Text txtExp { get; private set; }
	public LayoutGroupView<LevelListItem> levelListGroupView { get; private set; }

	public MainPanelComponent(UIRefRoot root) : base(root) { }

	protected override void InitComponent(UIRefRoot root)
	{
		playerHpBar = root.objects[0] as Slider;
		playerHpBarImage = root.objects[1] as Image;
		enemyHpBar = root.objects[2] as Slider;
		playerHpBarImage1 = root.objects[3] as Image;
		txtStage = root.objects[4] as Text;
		txtPlayerLife = root.objects[5] as Text;
		levelList = root.objects[6] as GameObject;
		itemGO = root.objects[7] as GameObject;
		txtExp = root.objects[8] as Text;
		levelListGroupView = new LayoutGroupView<LevelListItem>();
	}

	public class LevelListItem : LayoutGroupViewItem
	{
		public Image imgLevel1 = null;
		public Image imgLevel2 = null;
		public Image imgLevel3 = null;
		public Image imgLevel4 = null;
		public Image imgLevel5 = null;
		protected override void OnCreate(GameObject go)
		{
			imgLevel1 = transform.Find("imgLevel1").GetComponent<Image>();
			imgLevel2 = transform.Find("imgLevel2").GetComponent<Image>();
			imgLevel3 = transform.Find("imgLevel3").GetComponent<Image>();
			imgLevel4 = transform.Find("imgLevel4").GetComponent<Image>();
			imgLevel5 = transform.Find("imgLevel5").GetComponent<Image>();
		}
	}
}