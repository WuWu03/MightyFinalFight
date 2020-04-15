/*******************************************************/
/**2020-4-15 16:9****************************************/
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
	protected override void OnInit()
	{
	}
}