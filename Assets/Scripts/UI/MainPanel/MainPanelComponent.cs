/*******************************************************/
/**2025-08-18 22:03*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class MainPanelComponent : BasePanelComponent
{
	//player/playerHpBar,Slider
	public Slider playerHpBar { get; private set; }
	//player/playerHpBar/playerHpBarImage,Image
	public Image playerHpBarImage { get; private set; }
	//enemy/enemyHpBar,Slider
	public Slider enemyHpBar { get; private set; }
	//enemy/enemyHpBar/enemyHpBarImage,Image
	public Image enemyHpBarImage { get; private set; }
	//stage/txtStage,TextMeshProUGUI
	public TextMeshProUGUI txtStage { get; private set; }
	//playerLife/txtPlayerLife,TextMeshProUGUI
	public TextMeshProUGUI txtPlayerLife { get; private set; }
	public LayoutGroupView<LevelListItem> levelListGroupView { get; private set; }
	//exp/txtExp,TextMeshProUGUI
	public TextMeshProUGUI txtExp { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		playerHpBar = root.objects[0] as Slider;
		playerHpBarImage = root.objects[1] as Image;
		enemyHpBar = root.objects[2] as Slider;
		enemyHpBarImage = root.objects[3] as Image;
		txtStage = root.objects[4] as TextMeshProUGUI;
		txtPlayerLife = root.objects[5] as TextMeshProUGUI;
		GameObject levelList = root.objects[6] as GameObject;
		GameObject levelListItem = root.objects[7] as GameObject;
		levelListGroupView = new LayoutGroupView<LevelListItem>(levelList,levelListItem);
		txtExp = root.objects[8] as TextMeshProUGUI;
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
			UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
			imgLevel1 = uiRefRoot.objects[0] as Image;
			imgLevel2 = uiRefRoot.objects[1] as Image;
			imgLevel3 = uiRefRoot.objects[2] as Image;
			imgLevel4 = uiRefRoot.objects[3] as Image;
			imgLevel5 = uiRefRoot.objects[4] as Image;
		}
	}
}