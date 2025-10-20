/*
 * @Desc: Main 模块 MainView 界面组件
 * @Date: 2025-10-20 20:57:12
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class MainViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "MainView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.MainWindow; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}