/*
 * @Desc: Load 模块 LoadView 视图设置
 * @Date: 2026-07-04 16:34:06
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class LoadViewSettings : UIBaseViewSettings
{
	public override string prefabName { get { return "LoadView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Load; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}