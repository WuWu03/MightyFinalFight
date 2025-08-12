/*******************************************************/
/**2025-07-16 19:34*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class LoadPanelSettings : BasePanelSettings
{
	public override string panelName { get { return "LoadPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override PanelType panelType { get { return PanelType.Pop; } }
	public override PanelLayer panelLayer { get { return PanelLayer.Layer8; } }
	public override PanelCloseMode panelCloseMode { get { return PanelCloseMode.Eternal; } }
}