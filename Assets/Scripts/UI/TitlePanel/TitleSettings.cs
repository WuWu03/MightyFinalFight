/*******************************************************/
/**2025-08-21 17:16*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class TitleSettings : UIBaseSettings
{
	public override string name { get { return "TitlePanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.Window1; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Destroy; } }
}