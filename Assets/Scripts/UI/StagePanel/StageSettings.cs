/*******************************************************/
/**2025-08-16 14:22*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class StageSettings : UIBaseSettings
{
	public override string name { get { return "StagePanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.Window1; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Destroy; } }
}