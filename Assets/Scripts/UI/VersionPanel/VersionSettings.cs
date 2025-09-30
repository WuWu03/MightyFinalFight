/*******************************************************/
/**2025-08-08 14:43*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class VersionSettings : UIBaseSettings
{
	public override string name { get { return "VersionPanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.Window1; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Destroy; } }
}