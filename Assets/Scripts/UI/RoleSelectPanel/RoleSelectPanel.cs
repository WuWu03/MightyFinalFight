/*******************************************************/
/**2020-4-4 17:40****************************************/
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
	//_roles,RectTransform
	public RectTransform _rolesRect { get; private set;}
	//_roles/ImgSelect,Image
	public Image ImgSelect { get; private set;}
	protected override void OnInit()
	{
		_rolesRect = UIRefRoot.Objects[0] as RectTransform;
		ImgSelect = UIRefRoot.Objects[1] as Image;
	}
}