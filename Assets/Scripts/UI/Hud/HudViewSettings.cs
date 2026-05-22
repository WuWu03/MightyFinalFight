/*
 * @Desc: Hud 模块 HudView 视图设置
 * @Date: 2026-05-22 22:37:38
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class HudViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "HudView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Scene; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}