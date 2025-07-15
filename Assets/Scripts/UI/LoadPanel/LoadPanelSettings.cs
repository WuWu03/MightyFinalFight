/*******************************************************/
/**2025-07-04 21:17*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class LoadPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "LoadPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer8; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Eternal; } }
}