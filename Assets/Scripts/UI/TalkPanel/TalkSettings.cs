/*******************************************************/
/**2025-07-04 21:19*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class TalkSettings : UIBaseSettings
{
	public override string name { get { return "TalkPanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.Window2; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Destroy; } }
}