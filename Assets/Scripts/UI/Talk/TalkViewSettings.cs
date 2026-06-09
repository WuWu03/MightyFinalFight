/*
 * @Desc: Talk 模块 TalkView 视图设置
 * @Date: 2026-06-09 15:11:05
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class TalkViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "TalkView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Always; } }
}