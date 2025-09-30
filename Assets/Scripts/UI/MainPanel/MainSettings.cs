/*******************************************************/
/**2025-08-18 22:03*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork.UI;

public class MainSettings : UIBaseSettings
{
	public override string name { get { return "MainPanel"; } }
	public override float unLoadTime { get { return 0f; } }
	public override UILayer Layer { get { return UILayer.MainWindow; } }
	public override UICloseMode CloseMode { get { return UICloseMode.Eternal; } }
}