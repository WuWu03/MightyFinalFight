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
	public override PanelType panelType { get { return PanelType.Pop; } }
	public override PanelLayer panelLayer { get { return PanelLayer.Layer4; } }
	public override PanelCloseMode panelCloseMode { get { return PanelCloseMode.Destroy; } }
}