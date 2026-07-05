/*
 * @Desc: RoundClear 模块 RoundClearView 视图设置
 * @Date: 2026-07-04 17:28:09
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class RoundClearViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "RoundClearView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}