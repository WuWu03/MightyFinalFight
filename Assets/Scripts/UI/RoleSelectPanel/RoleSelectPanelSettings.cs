/*******************************************************/
/**2025-07-16 19:54*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class RoleSelectPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "RoleSelectPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override PanelType panelType { get { return PanelType.Normal; } }
	public override PanelLayer panelLayer { get { return PanelLayer.Layer3; } }
	public override PanelCloseMode panelCloseMode { get { return PanelCloseMode.Destroy; } }
}