/*******************************************************/
/**2025-08-08 14:43*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class VersionPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "VersionPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override PanelType panelType { get { return PanelType.Normal; } }
	public override PanelLayer panelLayer { get { return PanelLayer.Layer3; } }
	public override PanelCloseMode panelCloseMode { get { return PanelCloseMode.Destroy; } }
}