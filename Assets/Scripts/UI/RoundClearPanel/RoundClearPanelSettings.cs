/*******************************************************/
/**2025-07-12 15:13*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class RoundClearPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "RoundClearPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer4; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Destroy; } }
}