/*
 * @Desc: RoundClear 模块 RoundClearView 界面组件
 * @Date: 2025-10-11 12:16:07
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