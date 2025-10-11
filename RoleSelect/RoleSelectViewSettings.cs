/*
@Desc: RoleSelect 模块 RoleSelectView 界面数据* @Date: 2025-10-11 10:34
* @Author: WuWu
* @Note: 工具生成，请勿修改
*/

using GameFrameWork.UI;

public class RoleSelectViewSettings : UIBaseSettings
{
	public override string name { get { return "RoleSelectView"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}