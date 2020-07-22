/*******************************************************/
/**2020-7-22 19:39****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FrameWork.UI;

public class MainPanel : BasePanel
{
	public override string PanelName { get { return "MainPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Root; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.MainPanel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Eternal; } }
	//Player/PlayerHpBar,Slider
	public Slider PlayerHpBar { get; private set;}
	//Enemy/EnemyHpBar,Slider
	public Slider EnemyHpBar { get; private set;}
	//State/TxtStage,Text
	public Text TxtStage { get; private set;}
	//PlayerLife/TxtPlayerLife,Text
	public Text TxtPlayerLife { get; private set;}
	//Level/LevelList,GameObject
	public GameObject LevelList { get; private set;}
	//Level/LevelList/Item,GameObject
	public GameObject ItemGO { get; private set;}
	//Exp/TxtExp,Text
	public Text TxtExp { get; private set;}
	public LayoutGroupView<LevelListItem> LevelListGroupView { get; private set;}
	protected override void OnInit()
	{
		PlayerHpBar = UIRefRoot.Objects[0] as Slider;
		EnemyHpBar = UIRefRoot.Objects[1] as Slider;
		TxtStage = UIRefRoot.Objects[2] as Text;
		TxtPlayerLife = UIRefRoot.Objects[3] as Text;
		LevelList = UIRefRoot.Objects[4] as GameObject;
		ItemGO = UIRefRoot.Objects[5] as GameObject;
		TxtExp = UIRefRoot.Objects[11] as Text;
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