/*******************************************************/
/**2025-08-16 14:21*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class MainPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "MainPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override PanelType panelType { get { return PanelType.Root; } }
	public override PanelLayer panelLayer { get { return PanelLayer.Layer2; } }
	public override PanelCloseMode panelCloseMode { get { return PanelCloseMode.Eternal; } }
}