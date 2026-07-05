/*
 * @Desc: Main 模块 MainView 视图设置
 * @Date: 2026-07-04 18:02:38
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class MainViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "MainView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.MainWindow; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}