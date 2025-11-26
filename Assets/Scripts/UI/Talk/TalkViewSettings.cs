/*
 * @Desc: Talk 模块 TalkView 界面组件
 * @Date: 2025-11-26 17:08:14
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class TalkViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "TalkView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Always; } }
}