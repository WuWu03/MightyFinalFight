/*
 * @Desc: Talk 模块 TalkView 视图设置
 * @Date: 2026-07-04 18:41:45
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class TalkViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "TalkView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Talk; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Always; } }
}