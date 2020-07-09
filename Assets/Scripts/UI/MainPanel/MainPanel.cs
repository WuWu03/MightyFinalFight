/*******************************************************/
/**2020-7-9 19:5****************************************/
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
	protected override void OnInit()
	{
		PlayerHpBar = UIRefRoot.Objects[0] as Slider;
		EnemyHpBar = UIRefRoot.Objects[1] as Slider;
		TxtStage = UIRefRoot.Objects[2] as Text;
		TxtPlayerLife = UIRefRoot.Objects[3] as Text;
	}
}