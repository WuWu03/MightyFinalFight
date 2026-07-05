/*
 * @Desc: Title 模块 TitleView 视图设置
 * @Date: 2026-07-04 17:24:24
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class TitleViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "TitleView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Bg; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}