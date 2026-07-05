/*
 * @Desc: Version 模块 VersionView 视图设置
 * @Date: 2026-07-04 17:28:46
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class VersionViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "VersionView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Bg; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}