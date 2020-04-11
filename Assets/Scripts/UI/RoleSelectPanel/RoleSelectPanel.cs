/*******************************************************/
/**2020-4-4 17:26****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FrameWork.UI;

public class RoleSelectPanel : BasePanel
{
	public override string PanelName { get {return "RoleSelectPanel"; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Always; } }
	//ImgSelect,GameObject
	public GameObject ImgSelectGO { get; private set;}
	//TxtRoleName,Text
	public Text TxtRoleName { get; private set;}
	protected override void OnInit()
	{
		ImgSelectGO = UIRefRoot.Objects[0] as GameObject;
		TxtRoleName = UIRefRoot.Objects[1] as Text;
	}
}