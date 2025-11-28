/*
 * @Desc: Title 模块 TitleView 界面组件
 * @Date: 2025-11-28 10:22:46
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class TitleViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "TitleView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Bg; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}