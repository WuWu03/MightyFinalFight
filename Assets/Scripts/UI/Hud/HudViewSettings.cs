/*
 * @Desc: Hud 模块 HudView 界面组件
 * @Date: 2025-10-11 12:07:00
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class HudViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "HudView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Scene; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}