/*
 * @Desc: RoleSelect 模块 RoleSelectView 界面组件
 * @Date: 2025-11-26 17:05:58
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class RoleSelectViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "RoleSelectView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}