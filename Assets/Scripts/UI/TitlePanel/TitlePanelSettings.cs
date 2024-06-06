using GameFrameWork.UI;

public class TitlePanelSettings : BasePanelSettings
{
    public override string panelName { get { return "TitlePanel"; } }
    public override float panelUnLoadTime { get { return 0f; } }
    public override UIMgr.Type panelType { get { return UIMgr.Type.Normal; } }
    public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer3; } }
    public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Destroy; } }
}