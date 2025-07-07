/*******************************************************/
/**2025-07-04 21:28*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class MainPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "MainPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Root; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer2; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Eternal; } }
}