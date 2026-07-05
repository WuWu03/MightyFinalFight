/*
 * @Desc: Main 模块 MainView 视图
 * @Date: 2026-07-04 18:02:38
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class MainView : UIBaseView<MainView, MainViewPresenter, MainViewSettings>
{
	//player/playerHpBar,Slider
	public Slider playerHpBar { get; private set; }
	//player/playerHpBar/playerHpBarImage,ImageEx
	public ImageEx playerHpBarImage { get; private set; }
	//enemy/enemyHpBar,Slider
	public Slider enemyHpBar { get; private set; }
	//enemy/enemyHpBar/enemyHpBarImage,ImageEx
	public ImageEx enemyHpBarImage { get; private set; }
	//stage/txtStage,TextMeshProUGUI
	public TextMeshProUGUI txtStage { get; private set; }
	//playerLife/txtPlayerLife,TextMeshProUGUI
	public TextMeshProUGUI txtPlayerLife { get; private set; }
	//level/levelList,StaticList
	public StaticList levelList { get; private set; }
	//exp/txtExp,TextMeshProUGUI
	public TextMeshProUGUI txtExp { get; private set; }

	protected override void OnInitView(UIRefRoot root)
	{
		playerHpBar = root.objects[0] as Slider;
		playerHpBarImage = root.objects[1] as ImageEx;
		enemyHpBar = root.objects[2] as Slider;
		enemyHpBarImage = root.objects[3] as ImageEx;
		txtStage = root.objects[4] as TextMeshProUGUI;
		txtPlayerLife = root.objects[5] as TextMeshProUGUI;
		levelList = root.objects[6] as StaticList;
		levelList?.Init<LevelListItem>();
		txtExp = root.objects[7] as TextMeshProUGUI;
	}

	public class LevelListItem : BaseListItem
	{
		//level/levelList/levelListItem/imgLevel1,GameObject
		public GameObject imgLevel1Go {get; private set;}
		//level/levelList/levelListItem/imgLevel2,GameObject
		public GameObject imgLevel2Go {get; private set;}
		//level/levelList/levelListItem/imgLevel3,GameObject
		public GameObject imgLevel3Go {get; private set;}
		//level/levelList/levelListItem/imgLevel4,GameObject
		public GameObject imgLevel4Go {get; private set;}
		//level/levelList/levelListItem/imgLevel5,GameObject
		public GameObject imgLevel5Go {get; private set;}
		protected override void OnCreate(GameObject go)
		{
			UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
			imgLevel1Go = uiRefRoot.objects[0] as GameObject;
			imgLevel2Go = uiRefRoot.objects[1] as GameObject;
			imgLevel3Go = uiRefRoot.objects[2] as GameObject;
			imgLevel4Go = uiRefRoot.objects[3] as GameObject;
			imgLevel5Go = uiRefRoot.objects[4] as GameObject;
		}
	}
}