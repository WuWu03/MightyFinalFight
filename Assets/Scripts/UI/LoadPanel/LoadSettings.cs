/*******************************************************/
/**2025-07-16 19:34*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class LoadSettings : UIBaseSettings
{
	public override string name { get { return "LoadPanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.Mask; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Eternal; } }
}