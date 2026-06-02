/*
 * @Desc: Stage 模块 StageView 视图设置
 * @Date: 2026-06-02 23:34:03
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class StageViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "StageView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return true; } }
	public override UILayer layer { get { return UILayer.Window1; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Immediately; } }
}