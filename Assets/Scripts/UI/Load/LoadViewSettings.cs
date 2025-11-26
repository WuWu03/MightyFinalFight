/*
 * @Desc: Load 模块 LoadView 界面组件
 * @Date: 2025-11-26 16:35:53
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class LoadViewSettings : UIBaseSettings
{
	public override string prefabName { get { return "LoadView.prefab"; } }
	public override float delayDestroyTime { get { return 0f; } }
	public override bool canPopUp { get { return false; } }
	public override UILayer layer { get { return UILayer.Load; } }
	public override UIDestroyMode destroyMode { get { return UIDestroyMode.Eternal; } }
}