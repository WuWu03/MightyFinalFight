/*
 * @Desc: Version 模块 VersionView 界面组件
 * @Date: 2025-10-11 12:36:11
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class VersionViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "VersionView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Bg; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}